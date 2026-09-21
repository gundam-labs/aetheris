import { seed, STAFF } from "./data";
import type { Encounter, Patient, Staff, Store } from "./types";

const KEY = "aetheris.station.v1";
const USER = "aetheris.user.v1";

export function loadStore(): Store {
  try {
    const raw = localStorage.getItem(KEY);
    if (raw) return JSON.parse(raw) as Store;
  } catch {
    /* first visit */
  }
  return structuredClone(seed);
}

export function saveStore(store: Store) {
  localStorage.setItem(KEY, JSON.stringify(store));
}

export function currentUser(): Staff | null {
  try {
    const raw = sessionStorage.getItem(USER);
    return raw ? (JSON.parse(raw) as Staff) : null;
  } catch {
    return null;
  }
}

export function signIn(email: string, password: string): Staff | null {
  const staff = STAFF.find((s) => s.email === email && s.password === password);
  if (!staff) return null;
  sessionStorage.setItem(USER, JSON.stringify(staff));
  return staff;
}

export function signOut() {
  sessionStorage.removeItem(USER);
}

export function nextMrn(store: Store) {
  const nums = store.patients.map((p) => Number(p.mrn.split("-")[1] ?? "100000"));
  const n = Math.max(100000, ...nums) + 1;
  return `HV-${String(n).padStart(6, "0")}`;
}

export function addPatient(store: Store, input: Omit<Patient, "id" | "mrn">): Patient {
  const patient: Patient = { ...input, id: crypto.randomUUID(), mrn: nextMrn(store) };
  store.patients.push(patient);
  saveStore(store);
  return patient;
}

export function startEncounter(
  store: Store,
  patientId: string,
  type: Encounter["type"],
  location: string,
  chiefComplaint: string,
  clinician: string,
) {
  store.encounters.unshift({
    id: crypto.randomUUID(),
    patientId,
    type,
    status: "Open",
    startedAt: new Date().toISOString(),
    location,
    clinician,
    chiefComplaint,
  });
  saveStore(store);
}

export function closeInbox(store: Store, id: string) {
  const item = store.inbox.find((x) => x.id === id);
  if (item) item.status = "Done";
  saveStore(store);
}
