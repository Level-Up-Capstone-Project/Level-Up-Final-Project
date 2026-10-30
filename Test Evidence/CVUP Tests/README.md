** PLEASE NOTE THAT ALL README FILES ARE FOR CONTEXT ONLY AS WELL AS THE MASTER PROMPT USED TO SUMMARIZE TO PROVIDE THE MARKER WITH A QUICK OVERVIEW OF WHAT IS IN THE EVIDENCE FILES. IT IS NOT FOR MARKING AND THE MARKER CAN VERIFY WITH THE MASTER PROMPT AS WELL THAT THE CONTENT IN OUR EXCEL FOR A QUICK OVERVIEW. THE EXCEL IS OUR SOURCE OF TRUTH AND WAS DONE WITHOUT THE ASSISTANCE OF AI. THE PROMPT IS THE ONLY THING TAT IS USED TO SUMMARIZE FOR CONTEXT ONLY.

This folder contains the evidence for all tests run along with a README file for each category of tests in a summary format as a support. These README files is not our actual test outcomes, this is only for reference and should not be used for marking, this is the master prompt used to summarize for context only. We have an official Excel File with our tests, the executions and the outcomes that must be used for marking. **


<img width="792" height="282" alt="image" src="https://github.com/user-attachments/assets/69628c54-ee5f-47ce-ad03-0d692a86d9a9" />


Level Up — Software Test Execution Summary Report
Module: 06 — CV Upload and Storage  |  Execution date: 29 October 2026  |  Tester: Michelle van der Merwe

Reporting basis: 13 supplied CVUP test-case records (CVUP-001–CVUP-013). This is a module-specific summary, not an assessment of the entire Level Up application. All outcomes below are based on recorded test entries; the linked evidence files were not independently opened or validated.
1. Executive Summary
All 13 of 13 supplied CV upload and storage tests were marked Pass (100.00% pass rate among completed tests). The cases exercise valid PDF upload, file-type and size validation, malformed and empty-file handling, unauthenticated API access, file selection behaviour, duplicate prevention, dashboard score visibility, and storage recovery. There are no recorded failed, blocked, or unexecuted tests and no linked defect IDs. Nevertheless, recorded narratives do not establish every expected backend effect—for example CV record creation, absence of partial storage, request cancellation, and recovery consistency. Thus module release recommendation: Conditionally Ready, pending targeted verification; this does not constitute platform-wide production approval.
2. Test Execution Overview
Metric	Count	Percentage
Total Test Cases	13	100.00%
Passed	13	100.00% of all cases
Failed	0	0.00% of all cases
Blocked	0	0.00% of all cases
Not Executed	0	0.00% of all cases
In Progress	0	0.00% of all cases
Other / Unclear	0	0.00% of all cases
Execution Rate	13 / 13	100.00%
Pass Rate (Completed Tests)	13 / 13	100.00%
Failure Rate (Completed Tests)	0 / 13	0.00%
Blocked Rate	0 / 13	0.00%


Formulas: Execution rate = (Passed + Failed + Blocked + In Progress) / 13; pass and failure rates use the 13 completed cases (Passed + Failed); blocked rate uses all 13 cases. Percentage values are rounded to two decimals. These are recorded status metrics, not independent validations of all assertions.
3. Testing Scope and Coverage
Section	Total	Passed	Failed	Blocked	Not Executed	Pass Rate (Completed)	Observations / Outstanding Defects / Next Actions
06 — CV Upload and Storage	13	13	0	0	0	100.00%	All documented cases passed; no defect IDs reported. Confirm database/storage assertions, independent corruption case, cancellation behaviour, and recovery checks.


Coverage classification: The documented test set is fully executed. Full functional coverage of the module is not demonstrated: unsupported scenarios such as simultaneous uploads, permissions isolation across users, network interruption, and download/access control are not represented.
4. Testing Category and Method Summary
Test categories
Recorded category	Tests	Passed	Failed	Observation
Functional	3	3	0	Valid upload, DOCX rejection, and selection replacement reported.
Edge Case	6	6	0	Input validation and navigation tested; CVUP-006 needs stronger corruption-specific evidence.
Security	2	2	0	Renamed file and missing token rejected; downstream persistence checks not shown.
Bug/Fix	1	1	0	No duplicate documents reported after repeated actions.
Recovery	1	1	0	Storage-related retry reported successful.


Recorded methods
Method	Tests	Passed	Failed
Manual UI	10	10	0
Manual UI + API	2	2	0
API	1	1	0


No explicit automated regression, load, performance, cross-platform, or penetration testing is represented in this subset. This does not establish whether those methods were used elsewhere.
5. Priority-Based Test Results
Recorded priority	Total	Passed	Failed	Blocked	Outstanding
P0 (mapped to Critical for this report)	10	10	0	0	Verify persistence, malicious-input rejection, storage recovery, and duplicate-prevention assertions.
P1 (mapped to High for this report)	3	3	0	0	Confirm previous upload cancellation and Home score association.
P2 / Medium	0	0	0	0	Not represented.
P3 / Low	0	0	0	0	Not represented.


Priority mapping is interpretive: Source records use P0 and P1, not explicit severity levels. No defect severity is inferred. There are no recorded critical/high-priority failures, security failures, or integration failures; unverified assertions remain residual risks.
6. Failed Test Case Summary
Test ID	Section	Priority	Test Case	Expected Result	Actual Result	Defect ID
None	—	—	No test recorded as failed.	—	—	—


Clarifications, not confirmed failures: CVUP-006 primarily describes a file-opening corruption error, rather than conclusively observing the app rejecting a corrupted native PDF. CVUP-001, CVUP-009, CVUP-010, CVUP-012, and CVUP-013 describe successful user-facing outcomes but do not independently document every expected storage or request-lifecycle assertion.
7. Defect Summary
Defect ID	Linked Test IDs	Section	Recorded Failure	Evidence	Retest Status	Outstanding Action
None recorded	None	06 — CV Upload and Storage	No failed outcomes documented	Evidence URLs listed for all tests	All records say first-attempt pass; no separately identified defect retests	Do not open or close defects without further confirmation.


The N/A value in all 13 Defect ID cells is treated as no linked defect, not as proof that no technical defects exist. No failed test lacks a defect reference because no failed tests are recorded.
8. Blocked and Unexecuted Tests
There are 0 blocked and 0 not executed cases in the supplied records. No dependency prevented completion as recorded. Additional unlisted tests should not be presumed completed.
9. Device, Browser and Environment Coverage
Device / Browser entry	Tests	Passed	Failed	Interpretation
Device	13	13	0	Generic device label only; OS, model, browser, version, and network characteristics not provided.


No identifiable Android/iOS model, browser, operating-system version, emulator, or physical-device breakdown is recorded. Cross-device and cross-browser compatibility therefore remain not demonstrated.
10. Test Evidence and Traceability
Evidence links are supplied for 13/13 (100.00%) records: 12 PDFs and 1 PNG (CVUP-009). 0 records lack an evidence URL. These links identify claimed supporting files but their contents and accessibility have not been independently checked. No Defect IDs were linked, and all retest notes say N/A / passed on first attempt. File-by-file traceability is retained in the appendix.
11. Retesting and Regression Testing
The retest field reports first-attempt success for all 13 records; no separately dated post-fix retest is documented. CVUP-013 records a successful retry following storage unavailability, which is a recovery scenario, not evidence of a documented defect fix. Recommended regression focus: authentic versus renamed PDFs (CVUP-001, CVUP-004, CVUP-006), size and empty-file limits (CVUP-005, CVUP-007), authentication (CVUP-009), replacement and repeated taps (CVUP-010, CVUP-012), and storage recovery (CVUP-013).
12. Key Findings
12.1 Successful Functionality
The records report successful PDF upload (CVUP-001), unsupported format rejection (CVUP-002–CVUP-004), empty and oversized file handling (CVUP-005, CVUP-007), displayed Home score (CVUP-008), missing-token rejection (CVUP-009), picker cancellation (CVUP-011), duplicate prevention (CVUP-012), and recovery retry (CVUP-013).
12.2 Identified Failures
No failures are recorded. CVUP-006 is a documentation/evidence limitation rather than a confirmed application defect.
12.3 High-Risk Areas
Storage integrity and user ownership (CVUP-001, CVUP-009, CVUP-013), upload race/cancellation behaviour (CVUP-010, CVUP-012), server-side content validation (CVUP-004, CVUP-006), and persistence of computed scores (CVUP-008) warrant additional verification. These are potential risks; no actual failures in these areas have been established.
12.4 Testing Gaps
The records lack named device/OS/browser data, explicit performance measurements, concurrent-user scenarios, verification of backend storage/DB state, and automated regression results. File signature and storage behaviours would benefit from reproducible API assertions.
12.5 Data Quality Issues
All Test IDs are unique and all source rows include statuses, test dates, testers, device entries, and evidence links. The generic Device label is underspecified; N/A defect IDs and retest entries are consistent. CVUP-006 overlaps the renamed-file method used in CVUP-004 and does not clearly establish an independent corruption test. CVUP-007 says 5mb, while the expected service limit is 5 MiB; precise binary threshold test values are not shown. No contradictory status/actual-result pair establishes a confirmed failure.
13. Recommendations and Corrective Actions
Priority	Recommended Action	Related Test IDs	Expected Outcome
P0	Verify server/database/storage state before and after successful, unauthorized, rejected, and interrupted uploads.	CVUP-001, CVUP-004, CVUP-005, CVUP-007, CVUP-009, CVUP-013	Confirm exactly one owned CV record on success and no persistence or orphaned data on rejection.
P0	Re-run corruption testing with a genuinely damaged PDF and capture upload/API response.	CVUP-006	Establish independent corrupt-file rejection and usable error feedback.
P0	Exercise precise boundary sizes at 5 MiB and just above, documenting request and storage results.	CVUP-007	Verify threshold enforcement and no partial writes.
P1	Capture request cancellation / concurrency traces when replacing a file or repeatedly tapping upload.	CVUP-010, CVUP-012	Confirm old request cancellation/idempotency and final file ownership.
P1	Verify the displayed score is tied to the newly stored CV and current user.	CVUP-008	Demonstrate correct end-to-end record-to-dashboard association.
P1	Document exact physical device, OS, app version, API environment, network conditions, and any browser used.	CVUP-001–013	Provide reproducible and comparable environment coverage.
P1	Check all evidence links and annotate screenshots/logs with expected-versus-actual assertions.	CVUP-001–013	Strengthen auditability and traceability.
P2	Add automated API regression and targeted network/storage recovery tests.	CVUP-004, CVUP-009, CVUP-013	Repeat core security and resilience checks consistently.


14. Overall Quality Assessment
Quality dimension	Assessment	Basis
Functional reliability	Partially Supported	13 recorded passes, but persistence and cancellation assertions are not fully documented.
Integration stability	Partially Supported	UI-to-score and API workflows are represented; backend verification is limited.
Security testing evidence	Partially Supported	Renamed-content and unauthenticated upload cases passed; authorization and stored-record checks are not fully traced.
Usability testing evidence	Supported (limited scope)	Immediate format/empty-file feedback and safe picker cancellation are described.
Performance testing evidence	Not Demonstrated	No latency, throughput, or stress metrics.
Platform compatibility evidence	Not Demonstrated	All entries simply state Device.
Test completeness	Partially Supported	All 13 planned records executed; overall module scenario coverage not proven.
Release readiness	Partially Supported	No recorded failures, with targeted evidence gaps before acceptance.


15. Release Readiness and Conclusion
Recommendation: Conditionally Ready — CV Upload and Storage module only. The supplied suite reports 13 passes, no failures, no blocked cases, and a complete set of evidence URLs. Before formal acceptance, independently verify storage and record ownership, malformed-file rejection (particularly CVUP-006), cancellation/duplication mechanics, and service-failure recovery. This module-only recommendation cannot establish overall Level Up production readiness without results from other modules, environments, security, performance, and integration testing.
16. Appendix — Test Case Traceability
Test ID	Section	Priority	Status	Tester	Test Date	Device / Browser	Evidence	Defect ID	Retest Notes
CVUP-001	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-001 evidence	N/A	N/A — passed on first attempt
CVUP-002	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-002 evidence	N/A	N/A — passed on first attempt
CVUP-003	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-003 evidence	N/A	N/A — passed on first attempt
CVUP-004	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-004 evidence	N/A	N/A — passed on first attempt
CVUP-005	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-005 evidence	N/A	N/A — passed on first attempt
CVUP-006	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-006 evidence	N/A	N/A — passed on first attempt
CVUP-007	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-007 evidence	N/A	N/A — passed on first attempt
CVUP-008	06 — CV Upload and Storage	P1	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-008 evidence	N/A	N/A — passed on first attempt
CVUP-009	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-009 evidence	N/A	N/A — passed on first attempt
CVUP-010	06 — CV Upload and Storage	P1	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-010 evidence	N/A	N/A — passed on first attempt
CVUP-011	06 — CV Upload and Storage	P1	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-011 evidence	N/A	N/A — passed on first attempt
CVUP-012	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-012 evidence	N/A	N/A — passed on first attempt
CVUP-013	06 — CV Upload and Storage	P0	Pass	Michelle van der Merwe	29/10/2026	Device	CVUP-013 evidence	N/A	N/A — passed on first attempt


Appendix notes — Test scenario and observed result
Test ID	Recorded category	Method	Test case	Condensed actual-result observation
CVUP-001	Functional	Manual UI	Upload valid PDF CV	PDF selected and uploaded; CV record creation was not independently described.
CVUP-002	Functional	Manual UI	Reject DOCX CV	DOCX unsupported; PDF only.
CVUP-003	Edge Case	Manual UI	Reject unsupported file type	Transcript screenshot rejected with immediate file-type mismatch feedback.
CVUP-004	Security	Manual UI + API	Reject renamed fake PDF	Renamed DOCX triggered error indicating content was not PDF.
CVUP-005	Edge Case	Manual UI	Reject empty file	Zero-byte file produced an empty-file error.
CVUP-006	Edge Case	Manual UI	Reject corrupted document	Renamed DOCX could not be opened; record does not clearly demonstrate application-side upload rejection of an independently corrupted PDF.
CVUP-007	Edge Case	Manual UI + API	Reject file over 5 MiB	Over-limit file reportedly rejected automatically.
CVUP-008	Edge Case	Manual UI	Show CV score on Home	Valid PDF uploaded; analysed score reported visible on Home.
CVUP-009	Security	API	Reject unauthenticated upload	API returned 401 Unauthorized; absence of stored record was not separately documented.
CVUP-010	Functional	Manual UI	Replace selected file during upload	Alternate file selection was successful; cancellation of first request was not separately confirmed.
CVUP-011	Edge Case	Manual UI	Cancel file picker	Picker cancellation left screen usable without error.
CVUP-012	Bug/Fix	Manual UI	Prevent duplicate uploads	Repeated actions reportedly did not create multiple documents.
CVUP-013	Recovery	Manual UI	Recover after storage failure	Retry succeeded after storage-unavailable incident; no-false-success and storage-cleanup checks not documented.


Evidence handling note: URLs are reproduced from the supplied records; no claim is made that their linked contents were inspected. Commentary on insufficient assertions is an analyst observation, not an alteration of the source Pass statuses.
