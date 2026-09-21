# Aetheris

Multi-tenant hospital information system (HIS / HMS) built as SaaS.

**Stack:** .NET 8, ASP.NET Core Razor Pages, EF Core SQLite, vertical slices by screen.
**Repo:** https://github.com/gundam-labs/aetheris

Demonstration product. Not a certified EHR. Do not store live patient records.

## What we borrowed (workflows, not a copy)

Popular EHRs share the same clinical desktop. Epic calls the shell Hyperspace; Oracle Health calls the chart PowerChart. Aetheris uses its own names for the same jobs:

| Job in a hospital | Typical EHR surface | Aetheris |
| --- | --- | --- |
| Clinician desktop | Hyperspace / PowerChart | Station |
| Identify the person | Prelude / registration | Find patient (MPI) |
| The record | Chart Review / Snapshot | Chart |
| Clinic day | Cadence | Schedule |
| Results, refills, messages | In Basket / Message Center | Inbox |
| Admit / stay / discharge | ADT / Grand Central | Encounter on the chart |
| Orders, pharmacy, lab, billing | CPOE / Willow / Beaker / Resolute | not built yet |

No Epic or Oracle trademarks on the product. No live PHI.

## Run

```
cd src/Aetheris.Web
dotnet run
```

Open http://localhost:5088

| Staff | Password | Role |
| --- | --- | --- |
| maya.rao@harborview.demo | Clinician#2026 | Attending |
| james.okoro@harborview.demo | Nurse#2026 | Nurse |
| priya.shah@harborview.demo | Registrar#2026 | Registrar |

Tenant: Harborview General. SQLite file `aetheris.db` is created on first run.

## Layout

```
src/Aetheris.Web/
  Domain/           entities + MRN
  Data/             EF context + demo seed
  Tenancy/          current user from cookie
  Pages/Account     login
  Pages/Patients    MPI search + register
  Pages/Chart       snapshot, problems, meds, allergies, start visit
  Pages/Schedule    today's board
  Pages/Inbox       results / refills / portal messages
```

## Next slices

Orders (lab + meds), results filing, ADT transfer, bed board, billing.
