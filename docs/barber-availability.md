# Barber availability

## Responsibility model

- The branch schedule remains the habitual schedule for every collaborator.
  A collaborator without an individual approved schedule inherits it automatically.
- A Barber can register a short break or a punctual absence directly. A break
  cannot overlap an existing appointment. An absence blocks future bookings
  immediately; it never cancels or reassigns existing appointments.
- A Barber requests permanent weekly changes and vacations. Those records are
  pending until an Owner approves or rejects them.
- Before approving a requested schedule or vacation, the Owner is told if it
  conflicts with scheduled appointments. The decision is rejected until those
  appointments have been managed through the operating flow.
- The Owner can review pending requests and the appointments affected by an
  approved punctual absence from the team availability view.

## Availability rules

- Weekly schedules use ISO weekdays: 1 (Monday) through 7 (Sunday).
- An individual week can contain up to four non-overlapping periods per day.
  A day without periods is a day off; a gap between periods is a break.
- A complete service must fit inside both branch opening hours and the
  collaborator's approved individual availability. It also cannot overlap a
  break, an approved absence/vacation, or a scheduled appointment.
- Full-day ranges include both dates for the user and are stored as a
  half-open UTC interval by the backend. Weekly rules are interpreted in the
  branch time zone.
- Calendar, slots, appointment creation and rescheduling share the same
  availability calculation. Booking and availability mutations use a SQL
  Server application lock per linked professional to prevent race conditions.

## API routes

All routes require the active profile shown in the prefix and enforce that
profile's relationship to the collaborator.

### Barber

- `GET /api/barber/collaborators/{collaboratorId}/working-hours`
- `POST /api/barber/collaborators/{collaboratorId}/working-hours/requests`
- `DELETE /api/barber/collaborators/{collaboratorId}/working-hours/requests/{requestId}`
- `GET /api/barber/collaborators/{collaboratorId}/time-off?year={year}&month={month}`
- `POST /api/barber/collaborators/{collaboratorId}/time-off`
- `DELETE /api/barber/collaborators/{collaboratorId}/time-off/{timeOffId}`

Types for time off: `1` absence, `2` vacation, `3` break. Vacations begin in
`Pending`; the other two begin approved after validation.

### Owner

- `GET /api/owner/availability?collaboratorId={collaboratorId}&year={year}&month={month}`
- `POST /api/owner/collaborators/{collaboratorId}/working-hours/requests/{requestId}/approve`
- `POST /api/owner/collaborators/{collaboratorId}/working-hours/requests/{requestId}/reject`
- `POST /api/owner/collaborators/{collaboratorId}/time-off/{timeOffId}/approve`
- `POST /api/owner/collaborators/{collaboratorId}/time-off/{timeOffId}/reject`

## Persistence and verification

`AddCollaboratorAvailability` adds individual working hours, working periods
and dated blocks. `AddAvailabilityApprovalWorkflow` adds the request/review
workflow, audit fields and schedule request periods. `RemoveTimeOffStatusDefault`
ensures every domain operation supplies its state explicitly.

Run:

```text
dotnet test BarberFlow.Tests/BarberFlow.Tests.csproj -c Release
flutter test test/barber_availability_test.dart
```

The optional SQL Server test requires `BARBERFLOW_TEST_CONNECTION` pointing to
a migrated development database. It uses a transaction and rolls back all of
its fixture data. Never point it to production.
