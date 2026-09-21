export type Role = "Attending" | "Nurse" | "Registrar";

export type Staff = {
  email: string;
  name: string;
  role: Role;
  password: string;
};

export type Patient = {
  id: string;
  mrn: string;
  givenName: string;
  familyName: string;
  dob: string;
  sex: string;
  phone?: string;
};

export type Allergy = { patientId: string; substance: string; reaction: string; criticality: "High" | "Low" };
export type Problem = { patientId: string; display: string; status: string };
export type Medication = { patientId: string; name: string; sig: string };
export type Encounter = {
  id: string;
  patientId: string;
  type: "Outpatient" | "Inpatient" | "Emergency";
  status: string;
  startedAt: string;
  location: string;
  clinician: string;
  chiefComplaint?: string;
};
export type Appointment = {
  id: string;
  patientId: string;
  startsAt: string;
  clinic: string;
  provider: string;
  reason: string;
  status: string;
};
export type InboxItem = {
  id: string;
  patientId?: string;
  type: "Result" | "Refill" | "PatientMessage" | "StaffTask";
  title: string;
  body: string;
  status: "Open" | "Done";
  assignedRole: Role;
};

export type Store = {
  patients: Patient[];
  allergies: Allergy[];
  problems: Problem[];
  medications: Medication[];
  encounters: Encounter[];
  appointments: Appointment[];
  inbox: InboxItem[];
};

export function fullName(p: Patient) {
  return `${p.familyName}, ${p.givenName}`;
}

export function ageYears(dob: string) {
  const d = new Date(dob);
  const today = new Date();
  let age = today.getFullYear() - d.getFullYear();
  const m = today.getMonth() - d.getMonth();
  if (m < 0 || (m === 0 && today.getDate() < d.getDate())) age -= 1;
  return age;
}
