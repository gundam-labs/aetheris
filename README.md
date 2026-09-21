# Aetheris

Multi-tenant hospital information system (HIS / HMS) built as SaaS.

**Stack:** .NET 8, ASP.NET Core, vertical slices, one hospital = one tenant.

This is a demonstration product. Not a certified EHR. Do not store live patient records.

## Category name

Hospitals buy this class of software as:

- **HIS** — Hospital Information System (clinical + admin on one platform)
- **HMS** — Hospital Management System (ops, beds, billing, pharmacy)
- **EHR / EMR** — the clinical chart inside the HIS

**Aetheris** is the product name. The category is HIS.

## How we build

Organic, slice by slice. No big-bang modules.

1. Tenant + identity (a hospital can sign in)
2. Master Patient Index (register / find a patient)
3. Encounter / visit
4. Appointments and beds
5. Orders (lab, pharmacy) later
6. Billing later

Vertical slice = feature folder with command, query, endpoint, and tests together.

## Repo

Private: https://github.com/gundam-labs/aetheris

Owner account: `gundam-labs`.

Do not mix this brand with Meridian Trust, BadAss Bank, or Dhanvi Dual unless asked.
