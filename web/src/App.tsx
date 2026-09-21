import { FormEvent, useState } from "react";
import { Link, Navigate, Route, Routes, useNavigate, useParams, useSearchParams } from "react-router-dom";
import "./station.css";
import { STAFF } from "./data";
import { addPatient, closeInbox, currentUser, loadStore, signIn, signOut, startEncounter } from "./store";
import { ageYears, fullName, type Encounter, type Staff } from "./types";

function session(): Staff | null {
  return currentUser();
}

function Shell({ children }: { children: React.ReactNode }) {
  const user = session();
  const navigate = useNavigate();
  const [q, setQ] = useState("");
  if (!user) return <Navigate to="/login" replace />;
  return (
    <>
      <header className="topbar">
        <Link className="brand" to="/">Aetheris</Link>
        <span className="tenant">Harborview General</span>
        <form className="find" onSubmit={(e) => { e.preventDefault(); navigate(`/patients?q=${encodeURIComponent(q)}`); }}>
          <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Find patient — name or MRN" />
        </form>
        <nav className="topnav">
          <Link to="/">Station</Link>
          <Link to="/schedule">Schedule</Link>
          <Link to="/inbox">Inbox</Link>
          <Link to="/patients">MPI</Link>
        </nav>
        <div className="who">
          <strong>{user.name}</strong>
          <span>{user.role}</span>
          <a href="/login" onClick={(e) => { e.preventDefault(); signOut(); navigate("/login"); }}>Sign out</a>
        </div>
      </header>
      <div className="demo-banner">Demonstration HIS on Vercel. Fictional patients in this browser only. Not a certified EHR.</div>
      <main className="shell">{children}</main>
    </>
  );
}

function Login() {
  const navigate = useNavigate();
  const [email, setEmail] = useState(STAFF[0].email);
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  function onSubmit(e: FormEvent) {
    e.preventDefault();
    const user = signIn(email, password);
    if (!user) { setError("Unknown staff account or wrong password."); return; }
    navigate("/");
  }
  return (
    <div className="login">
      <h1>Aetheris Station</h1>
      <p className="lede">Harborview General — browser demo for Vercel</p>
      {error && <p className="err">{error}</p>}
      <form onSubmit={onSubmit}>
        <label>Staff email</label>
        <input value={email} onChange={(e) => setEmail(e.target.value)} type="email" />
        <label>Password</label>
        <input value={password} onChange={(e) => setPassword(e.target.value)} type="password" />
        <button className="btn" type="submit">Open station</button>
      </form>
      <p className="lede" style={{ marginTop: 16 }}>
        Maya Rao, MD / Clinician#2026<br />James Okoro, RN / Nurse#2026<br />Priya Shah / Registrar#2026
      </p>
    </div>
  );
}

function Station() {
  const store = loadStore();
  const open = store.inbox.filter((i) => i.status === "Open").length;
  const start = new Date(); start.setHours(0, 0, 0, 0);
  const end = new Date(start); end.setDate(end.getDate() + 1);
  const today = store.appointments.filter((a) => { const t = +new Date(a.startsAt); return t >= +start && t < +end; });
  const next = [...store.appointments].filter((a) => +new Date(a.startsAt) >= Date.now()).sort((a, b) => a.startsAt.localeCompare(b.startsAt)).slice(0, 5);
  const board = store.encounters.filter((e) => e.status === "Open");
  return (
    <>
      <h1>Station</h1>
      <p className="lede">Clinician desktop. Data lives in this browser until the .NET API is hosted.</p>
      <div className="cards">
        <Link className="card" to="/inbox"><h2>Inbox</h2><div className="meta">{open} open items</div></Link>
        <Link className="card" to="/schedule"><h2>Schedule</h2><div className="meta">{today.length} visits today</div></Link>
        <Link className="card" to="/patients"><h2>Find patient</h2><div className="meta">Master Patient Index</div></Link>
        <div className="card"><h2>Open encounters</h2><div className="meta">{board.length} ED / inpatient</div></div>
      </div>
      <div className="grid">
        <div>
          <h2>Up next</h2>
          <table className="data">
            <thead><tr><th>Time</th><th>Patient</th><th>Clinic</th><th>Reason</th></tr></thead>
            <tbody>
              {next.map((a) => {
                const p = store.patients.find((x) => x.id === a.patientId);
                return (
                  <tr key={a.id}>
                    <td>{new Date(a.startsAt).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</td>
                    <td>{p ? <Link to={`/chart/${p.id}`}>{fullName(p)}</Link> : "—"}</td>
                    <td>{a.clinic}</td>
                    <td>{a.reason}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
        <div>
          <h2>Unit / ED board</h2>
          {board.map((e) => {
            const p = store.patients.find((x) => x.id === e.patientId);
            return (
              <p key={e.id}>
                <span className={`pill ${e.type === "Emergency" ? "ed" : "ip"}`}>{e.type}</span>{" "}
                {p ? <Link to={`/chart/${p.id}`}>{fullName(p)}</Link> : "—"} · {e.location}
              </p>
            );
          })}
        </div>
      </div>
    </>
  );
}

function Patients() {
  const [params] = useSearchParams();
  const q = (params.get("q") ?? "").trim().toLowerCase();
  const store = loadStore();
  const results = store.patients.filter((p) => !q || p.mrn.toLowerCase().includes(q) || p.familyName.toLowerCase().includes(q) || p.givenName.toLowerCase().includes(q));
  return (
    <>
      <h1>Find patient</h1>
      <p className="lede">Search first. Register only after you have looked.</p>
      <form className="row" method="get" action="/patients">
        <input name="q" defaultValue={params.get("q") ?? ""} placeholder="Vasquez or HV-100001" style={{ maxWidth: 320 }} />
        <button className="btn" type="submit">Search</button>
        <Link className="btn secondary" to="/patients/register">Register new</Link>
      </form>
      <table className="data">
        <thead><tr><th>MRN</th><th>Name</th><th>DOB</th><th>Sex</th><th></th></tr></thead>
        <tbody>
          {results.map((p) => (
            <tr key={p.id}>
              <td>{p.mrn}</td><td>{fullName(p)}</td><td>{p.dob}</td><td>{p.sex}</td>
              <td><Link to={`/chart/${p.id}`}>Open chart</Link></td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

function Register() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  function onSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const form = new FormData(e.currentTarget);
    const givenName = String(form.get("givenName") ?? "").trim();
    const familyName = String(form.get("familyName") ?? "").trim();
    if (!givenName || !familyName) { setError("Given name and family name are required."); return; }
    const p = addPatient(loadStore(), { givenName, familyName, dob: String(form.get("dob") || "1980-01-01"), sex: String(form.get("sex") || "F"), phone: String(form.get("phone") || "") });
    navigate(`/chart/${p.id}`);
  }
  return (
    <>
      <h1>Register patient</h1>
      <p className="lede">Creates an MRN in this browser. Later this posts to the .NET API.</p>
      {error && <p className="err">{error}</p>}
      <form onSubmit={onSubmit} style={{ maxWidth: 420 }}>
        <label>Given name</label><input name="givenName" />
        <label>Family name</label><input name="familyName" />
        <label>Date of birth</label><input name="dob" type="date" defaultValue="1980-01-01" />
        <label>Sex</label><select name="sex"><option>F</option><option>M</option><option>X</option></select>
        <label>Phone</label><input name="phone" />
        <button className="btn" type="submit">Create chart</button>
      </form>
    </>
  );
}

function Chart() {
  const { id } = useParams();
  const user = session()!;
  const [, bump] = useState(0);
  const store = loadStore();
  const p = store.patients.find((x) => x.id === id);
  if (!p) return <p>Patient not found.</p>;
  const allergies = store.allergies.filter((a) => a.patientId === p.id);
  const problems = store.problems.filter((a) => a.patientId === p.id);
  const meds = store.medications.filter((a) => a.patientId === p.id);
  const encounters = store.encounters.filter((a) => a.patientId === p.id).sort((a, b) => b.startedAt.localeCompare(a.startedAt));
  function onStart(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const form = new FormData(e.currentTarget);
    startEncounter(store, p.id, (form.get("type") as Encounter["type"]) || "Outpatient", String(form.get("location") || "Clinic A"), String(form.get("cc") || ""), user.name);
    bump((n) => n + 1);
  }
  const high = allergies.some((a) => a.criticality === "High");
  return (
    <>
      <div className="banner">
        <div><div className="name">{fullName(p)}</div><div className="meta">{p.sex} · {ageYears(p.dob)} y · DOB {p.dob}</div></div>
        <div>MRN {p.mrn}</div>
        <div>{high ? <span className="crit">ALLERGY {allergies.map((a) => a.substance).join(", ")}</span> : allergies.length ? `Allergies: ${allergies.map((a) => a.substance).join(", ")}` : "NKDA"}</div>
      </div>
      <div className="row">
        <section className="panel"><h3>Problem list</h3>{problems.map((pr) => <div key={pr.display}>{pr.display} <span className="pill">{pr.status}</span></div>)}{!problems.length && <div className="meta">None recorded</div>}</section>
        <section className="panel"><h3>Medications</h3>{meds.map((m) => <div key={m.name}><strong>{m.name}</strong> — {m.sig}</div>)}{!meds.length && <div className="meta">None recorded</div>}</section>
        <section className="panel"><h3>Allergies</h3>{allergies.map((a) => <div key={a.substance} className={a.criticality === "High" ? "crit" : ""}>{a.substance} — {a.reaction}</div>)}{!allergies.length && <div className="meta">NKDA</div>}</section>
      </div>
      <div className="row" style={{ marginTop: 16 }}>
        <section className="panel">
          <h3>Encounters</h3>
          <table className="data">
            <thead><tr><th>When</th><th>Type</th><th>Where</th><th>Complaint</th><th>Status</th></tr></thead>
            <tbody>
              {encounters.map((e) => (
                <tr key={e.id}><td>{new Date(e.startedAt).toLocaleString()}</td><td>{e.type}</td><td>{e.location}</td><td>{e.chiefComplaint}</td><td>{e.status}</td></tr>
              ))}
            </tbody>
          </table>
        </section>
        <section className="panel">
          <h3>Start encounter</h3>
          <form onSubmit={onStart}>
            <label>Type</label>
            <select name="type"><option>Outpatient</option><option>Inpatient</option><option>Emergency</option></select>
            <label>Location</label><input name="location" defaultValue="Clinic A" />
            <label>Chief complaint</label><input name="cc" />
            <button className="btn" type="submit">Open visit</button>
          </form>
        </section>
      </div>
    </>
  );
}

function SchedulePage() {
  const store = loadStore();
  const start = new Date(); start.setHours(0, 0, 0, 0);
  const end = new Date(start); end.setDate(end.getDate() + 1);
  const day = store.appointments.filter((a) => { const t = +new Date(a.startsAt); return t >= +start && t < +end; }).sort((a, b) => a.startsAt.localeCompare(b.startsAt));
  return (
    <>
      <h1>Schedule</h1>
      <p className="lede">Today's clinic board.</p>
      <table className="data">
        <thead><tr><th>Time</th><th>Patient</th><th>MRN</th><th>Clinic</th><th>Reason</th><th>Status</th></tr></thead>
        <tbody>
          {day.map((a) => {
            const p = store.patients.find((x) => x.id === a.patientId);
            return (
              <tr key={a.id}>
                <td>{new Date(a.startsAt).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</td>
                <td>{p ? <Link to={`/chart/${p.id}`}>{fullName(p)}</Link> : "—"}</td>
                <td>{p?.mrn}</td><td>{a.clinic}</td><td>{a.reason}</td><td>{a.status}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </>
  );
}

function Inbox() {
  const [, bump] = useState(0);
  const store = loadStore();
  const items = store.inbox.filter((i) => i.status === "Open");
  return (
    <>
      <h1>Inbox</h1>
      <p className="lede">Results, refills, portal messages.</p>
      <table className="data">
        <thead><tr><th>Type</th><th>Item</th><th>Patient</th><th>For</th><th></th></tr></thead>
        <tbody>
          {items.map((i) => {
            const p = store.patients.find((x) => x.id === i.patientId);
            return (
              <tr key={i.id}>
                <td><span className="pill">{i.type}</span></td>
                <td><strong>{i.title}</strong><div className="meta">{i.body}</div></td>
                <td>{p ? <Link to={`/chart/${p.id}`}>{fullName(p)}</Link> : "—"}</td>
                <td>{i.assignedRole}</td>
                <td><button className="btn secondary" onClick={() => { closeInbox(store, i.id); bump((n) => n + 1); }}>Done</button></td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </>
  );
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/" element={<Shell><Station /></Shell>} />
      <Route path="/patients" element={<Shell><Patients /></Shell>} />
      <Route path="/patients/register" element={<Shell><Register /></Shell>} />
      <Route path="/chart/:id" element={<Shell><Chart /></Shell>} />
      <Route path="/schedule" element={<Shell><SchedulePage /></Shell>} />
      <Route path="/inbox" element={<Shell><Inbox /></Shell>} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
