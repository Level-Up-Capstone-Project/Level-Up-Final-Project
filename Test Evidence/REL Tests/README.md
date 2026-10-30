# Section 19 - Performance, offline use and recovery

Executed locally on 2026-10-30 using Microsoft Edge, the production frontend build, the local backend, and the automated frontend regression suite.

| Test ID | Status | Evidence |
| --- | --- | --- |
| REL-001 | Pass | Edge application screenshot and performance console |
| REL-002 | Blocked | Public API timings captured; authenticated dashboard/profile/roadmap screen timing still required |
| REL-003 | Blocked | Requires a configured live AI operation and visible progress capture |
| REL-004 | Blocked | Requires an authenticated test account and Edge offline-mode capture |
| REL-005 | Pass | Automated upload failure/manual retry test |
| REL-006 | Pass | Automated AI-analysis retry test |
| REL-007 | Blocked | Cancellation coverage exists; slow-network cross-screen navigation still requires direct execution |
| REL-008 | Pass | Automated signed-in refresh/session migration test |
| REL-009 | Pass | Automated invalid JSON and empty error-response tests |
| REL-010 | Blocked | Requires an authenticated session and controlled backend restart |

The blocked-test evidence states exactly what must be supplied before each test can be completed.
