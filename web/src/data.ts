import type { Appointment, InboxItem, Store, Staff } from "./types";

export const STAFF: Staff[] = [
  { email: "maya.rao@harborview.demo", name: "Maya Rao, MD", role: "Attending", password: "Clinician#2026" },
  { email: "james.okoro@harborview.demo", name: "James Okoro, RN", role: "Nurse", password: "Nurse#2026" },
  { email: "priya.shah@harborview.demo", name: "Priya Shah", role: "Registrar", password: "Registrar#2026" },
];

const today = new Date();
today.setHours(0, 0, 0, 0);
const at = (h: number, m = 0) => {
  const d = new Date(today);
  d.setHours(h, m, 0, 0);
  return d.toISOString();
};

export const seed: Store = {
  patients: [
    { id: "p1", mrn: "HV-100001", givenName: "Elena", familyName: "Vasquez", dob: "1978-04-12", sex: "F", phone: "+1-555-0101" },
    { id: "p2", mrn: "HV-100002", givenName: "Samuel", familyName: "Okonkwo", dob: "1959-11-03", sex: "M" },
    { id: "p3", mrn: "HV-100003", givenName: "Aisha", familyName: "Mensah", dob: "2020-06-18", sex: "F" },
    { id: "p4", mrn: "HV-100004", givenName: "Robert", familyName: "Chen", dob: "1955-01-29", sex: "M" },
    { id: "p5", mrn: "HV-100005", givenName: "Fatima", familyName: "Al-Najjar", dob: "1992-08-07", sex: "F" },
    { id: "p6", mrn: "HV-100006", givenName: "David", familyName: "Park", dob: "1997-02-14", sex: "M" },
    { id: "p7", mrn: "HV-100007", givenName: "Grace", familyName: "Nwosu", dob: "1971-09-22", sex: "F" },
    { id: "p8", mrn: "HV-100008", givenName: "Tomas", familyName: "Alvarez", dob: "1944-12-01", sex: "M" },
  ],
  allergies: [
    { patientId: "p1", substance: "Lisinopril", reaction: "Angioedema", criticality: "High" },
    { patientId: "p4", substance: "Penicillin", reaction: "Rash", criticality: "Low" },
  ],
  problems: [
    { patientId: "p1", display: "Asthma", status: "Active" },
    { patientId: "p2", display: "Type 2 diabetes mellitus", status: "Active" },
    { patientId: "p2", display: "Essential hypertension", status: "Active" },
    { patientId: "p4", display: "Heart failure with reduced EF", status: "Active" },
    { patientId: "p5", display: "Pregnancy, 28 weeks", status: "Active" },
    { patientId: "p7", display: "Breast cancer, in remission", status: "Active" },
    { patientId: "p8", display: "Community-acquired pneumonia", status: "Active" },
  ],
  medications: [
    { patientId: "p1", name: "Budesonide-formoterol inhaler", sig: "2 puffs BID" },
    { patientId: "p2", name: "Metformin 1000 mg", sig: "1 tab BID with food" },
    { patientId: "p2", name: "Amlodipine 10 mg", sig: "1 tab daily" },
    { patientId: "p4", name: "Warfarin 5 mg", sig: "as directed by INR" },
    { patientId: "p4", name: "Furosemide 40 mg", sig: "1 tab daily" },
  ],
  encounters: [
    {
      id: "e1",
      patientId: "p6",
      type: "Emergency",
      status: "Open",
      startedAt: new Date(Date.now() - 2 * 3600_000).toISOString(),
      location: "ED Bay 4",
      clinician: "James Okoro, RN",
      chiefComplaint: "Ankle injury after football",
    },
    {
      id: "e2",
      patientId: "p8",
      type: "Inpatient",
      status: "Open",
      startedAt: new Date(Date.now() - 3 * 86400_000).toISOString(),
      location: "Ward 3B / Bed 12",
      clinician: "Maya Rao, MD",
      chiefComplaint: "Fever and productive cough",
    },
  ],
  appointments: [
    appt("p1", 9, 0, "Primary Care 2", "Asthma review"),
    appt("p2", 9, 20, "Primary Care 2", "Diabetes follow-up"),
    appt("p5", 10, 0, "Women's Health", "Antenatal visit"),
    appt("p7", 11, 0, "Oncology clinic", "Surveillance"),
    appt("p3", 14, 0, "Paediatrics", "Well child"),
  ],
  inbox: [
    item("i1", "p2", "Result", "HbA1c 8.4% — review", "Drawn 18 Sep 2026. Prior 7.9%.", "Attending"),
    item("i2", "p4", "Result", "INR 3.6 — high", "Warfarin patient. Hold and call.", "Attending"),
    item("i3", "p1", "Refill", "Refill: budesonide-formoterol", "Patient requested 90-day refill via portal.", "Attending"),
    item("i4", "p5", "PatientMessage", "Portal: reduced fetal movement?", "Message from patient this morning.", "Nurse"),
  ],
};

function appt(patientId: string, h: number, m: number, clinic: string, reason: string): Appointment {
  return {
    id: `a-${patientId}-${h}${m}`,
    patientId,
    startsAt: at(h, m),
    clinic,
    provider: "Maya Rao, MD",
    reason,
    status: "Booked",
  };
}

function item(
  id: string,
  patientId: string,
  type: InboxItem["type"],
  title: string,
  body: string,
  assignedRole: InboxItem["assignedRole"],
): InboxItem {
  return { id, patientId, type, title, body, status: "Open", assignedRole };
}
