** PLEASE NOTE THAT ALL README FILES ARE FOR CONTEXT ONLY AS WELL AS THE MASTER PROMPT USED TO SUMMARIZE TO PROVIDE THE MARKER WITH A QUICK OVERVIEW OF WHAT IS IN THE EVIDENCE FILES. IT IS NOT FOR MARKING AND THE MARKER CAN VERIFY WITH THE MASTER PROMPT AS WELL THAT THE CONTENT IN OUR EXCEL FOR A QUICK OVERVIEW. THE EXCEL IS OUR SOURCE OF TRUTH AND WAS DONE WITHOUT THE ASSISTANCE OF AI. THE PROMPT IS THE ONLY THING TAT IS USED TO SUMMARIZE FOR CONTEXT ONLY.

This folder contains the evidence for all tests run along with a README file for each category of tests in a summary format as a support. These README files is not our actual test outcomes, this is only for reference and should not be used for marking, this is the master prompt used to summarize for context only. We have an official Excel File with our tests, the executions and the outcomes that must be used for marking. **


<img width="786" height="177" alt="image" src="https://github.com/user-attachments/assets/75a85f91-7314-446c-9679-bca574ef7fb5" />


CV Analysis & Results — Test Execution Summary
Project: Level Up — Career Readiness Mobile Application
Module: 07 — CV Analysis and Results
Test date: 29 October 2026
Tester: Michelle van der Merwe
Test cases: CVAN-001 to CVAN-011
Test methods: Manual UI, Manual UI + API, and API testing
Test environments: Device; browser (n8n logs)
1. Testing Objective
Evaluate CV upload and analysis, result presentation and consistency, persistence, document ownership protection, OCR extraction, error recovery, navigation behaviour, zero-value rendering, and confidentiality of CV data in the Level Up application and its backend/n8n integration.
2. Test Execution Overview
Metric	Recorded result
Total test cases	11
Passed on first attempt	10
Passed after fix	1
Outstanding recorded failures	0
Final recorded pass rate	100%
Critical-priority tests (P0)	8
High-priority tests (P1)	3


The original execution log records all 11 cases as ultimately passed, including CVAN-005 (DCVAN-005), which passed after a backend fix. A recorded pass is not, by itself, confirmation of every aspect of an expected outcome; the follow-up section distinguishes the documented observation from checks that may still require explicit evidence.
3. Test Case Register
Test ID	Priority	Category	Test scenario	Recorded result	Evidence
CVAN-001	P0	Functional	Analyse an uploaded CV	Pass	CVAN-001 evidence (PDF)
CVAN-002	P0	Functional	Display all CV result fields	Pass	CVAN-002 evidence (PDF)
CVAN-003	P0	Functional	CV result values match the API	Pass	CVAN-003 evidence (PNG)
CVAN-004	P1	Functional	Retrieve the latest analysis after refresh	Pass	CVAN-004 evidence (PDF)
CVAN-005	P0	Edge Case	CV result belongs only to the current user	Pass after Fix	CVAN-005 evidence (PDF)
CVAN-006	P0	Edge Case	Handle CV with insufficient readable text	Pass	CVAN-006 evidence (PDF)
CVAN-007	P0	Recovery	AI workflow failure does not save a false result	Pass	CVAN-007 evidence (PNG)
CVAN-008	P0	Recovery	Retry analysis after failure	Pass	CVAN-008 evidence (PDF)
CVAN-009	P1	Edge Case	Navigate away during analysis	Pass	CVAN-009 evidence (PDF)
CVAN-010	P1	Edge Case	Zero scores render correctly	Pass	CVAN-010 evidence (PDF)
CVAN-011	P0	Security	Raw CV evidence is not exposed unnecessarily	Pass	CVAN-011 evidence (PNG)


4. Detailed Test Results
CVAN-001 — Analyse an uploaded CV
- Priority: P0
- Test type: Functional
- Execution method: Manual UI
- Preconditions: Valid uploaded CV and target career where required
- Test steps: Start analysis and wait for completion.
- Expected result: Progress is shown and one analysis result is stored for the owned CV.
- Observed result: File selected and uploaded, analysis in progress with n8n workflow running.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-001 evidence (PDF)
CVAN-002 — Display all CV result fields
- Priority: P0
- Test type: Functional
- Execution method: Manual UI
- Preconditions: Successful analysis
- Test steps: Open the CV report.
- Expected result: Quality, ATS and formatting scores plus strengths, weaknesses and recommendations display correctly.
- Observed result: CV and transcript upload and analysis run directly after one another, once both has run you are able to view the report that includes both analyses. This feature is fully functional and passes test.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-002 evidence (PDF)
CVAN-003 — CV result values match the API
- Priority: P0
- Test type: Functional
- Execution method: Manual UI + API
- Preconditions: Successful analysis
- Test steps: Compare visible values with n8n workflow.
- Expected result: Every displayed metric and timestamp matches the persisted analysis.
- Observed result: Based on n8n workflow executoins you can see the timestamp is correct and analyses passes.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-003 evidence (PNG)
CVAN-004 — Retrieve the latest analysis after refresh
- Priority: P1
- Test type: Functional
- Execution method: Manual UI
- Preconditions: Completed CV analysis
- Test steps: Refresh or sign out and sign back in, then open results.
- Expected result: The latest saved analysis loads without rerunning the workflow.
- Observed result: Signed-out and logged back in, no need to rerun the upload and analysis workflow, results are still there and still the same.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-004 evidence (PDF)
CVAN-005 — CV result belongs only to the current user
- Priority: P0
- Test type: Edge Case
- Execution method: Manual UI + API
- Preconditions: Two authenticated users
- Test steps: Analyse as user A and attempt to retrieve it as user B.
- Expected result: User B cannot access user A's CV analysis.
- Observed result: CV upload of another user and the backend and workflow did not pick up that the CV and the user are two different users due to redaction of sensitive information before sending to LLM in n8n workflow.
- Recorded status: Pass after Fix
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-005 evidence (PDF)
- Defect reference: DCVAN-005
- Fix/retest notes: After fix has been applied to backend, a file that belongs to another user was uploaded and the workflow sent it back with a message telling the user that the file does not match the logged in user.
CVAN-006 — Handle CV with insufficient readable text
- Priority: P0
- Test type: Edge Case
- Execution method: Manual UI
- Preconditions: Image-only or nearly empty CV
- Test steps: Start analysis.
- Expected result: The Adobe OCR API is used to extract text from document even if it is image-only or nearly empty CV and provides text to n8n workflow for analysis.
- Observed result: The Adobe OCR API is fully functional as all text is extracted and sent to n8n workflow and LLM for CV analysis.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-006 evidence (PDF)
CVAN-007 — AI workflow failure does not save a false result
- Priority: P0
- Test type: Recovery
- Execution method: Manual UI
- Preconditions: Analysis provider is unavailable or returns failure
- Test steps: Start analysis.
- Expected result: The operation fails clearly, no successful analysis is persisted and Retry is available.
- Observed result: Operation failed and there was no persisted result returned, afterwards a retry was available.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-007 evidence (PNG)
CVAN-008 — Retry analysis after failure
- Priority: P0
- Test type: Recovery
- Execution method: Manual UI
- Preconditions: Previous analysis attempt failed
- Test steps: Restore the provider and choose Retry.
- Expected result: The existing uploaded CV is reused and a successful result is saved once.
- Observed result: Provider error when trying to upload selected CV document, operation retry and document analyzed successfully.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-008 evidence (PDF)
CVAN-009 — Navigate away during analysis
- Priority: P1
- Test type: Edge Case
- Execution method: Manual UI
- Preconditions: Analysis is running
- Test steps: Open another screen and return.
- Expected result: The app does not crash or start duplicate analysis and presents the correct final state.
- Observed result: CV upload started and navigated away from the screen to the home screen, went back to the cv analysis and it was still in progress.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-009 evidence (PDF)
CVAN-010 — Zero scores render correctly
- Priority: P1
- Test type: Edge Case
- Execution method: Manual UI
- Preconditions: Test response contains a valid zero score
- Test steps: Open the CV report.
- Expected result: Zero is shown as 0 rather than blank, missing or an error.
- Observed result: Values have been removed from the original transcript to check for blank or zero score error, document evaluated successfully without an error.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Device
- Evidence: CVAN-010 evidence (PDF)
CVAN-011 — Raw CV evidence is not exposed unnecessarily
- Priority: P0
- Test type: Security
- Execution method: API
- Preconditions: Completed analysis
- Test steps: Inspect result screens, network errors and visible logs.
- Expected result: Only intended summary data is shown and raw document content is not leaked.
- Observed result: As part of the backend process personal data is redacted and not shared with n8n workflow sent to the LLM for analyses. Data that is sensitve is secure.
- Recorded status: Pass
- Tested by/on: Michelle van der Merwe — 29/10/2026
- Environment: Browser (n8n Logs)
- Evidence: CVAN-011 evidence (PNG)
5. Defect and Resolution Summary
DCVAN-005 — CV ownership mismatch
Associated test: CVAN-005
Priority: P0
Final recorded result: Pass after Fix

Issue observed: When a CV belonging to another user was uploaded, the backend/workflow initially failed to detect the ownership mismatch. The test notes associate this with personal information being redacted before the document was sent to the n8n LLM workflow.

Reported correction: Following a backend fix, the tester uploaded a CV belonging to another user again. The workflow returned a message stating that the document did not match the logged-in user.

Scope clarification: This retest documents a rejected mismatched upload. The stated acceptance condition also requires preventing user B from retrieving user A’s stored CV analysis; that cross-account retrieval restriction should be verified separately with two authenticated sessions or API requests.

Evidence: CVAN-005 evidence (PDF)
6. Findings and Recommended Verification
Test	Evidence or scope consideration	Recommended follow-up
CVAN-001	The observed note confirms selection, upload and workflow progress; it does not explicitly confirm a single persisted analysis for the owned CV.	Retrieve the completed report through the API and confirm exactly one saved result.
CVAN-003	The observation mentions correct n8n execution time and a successful analysis, but not a field-by-field comparison of displayed metrics with persisted API values.	Compare each result metric and timestamp against the API/persisted record.
CVAN-005	The reported backend fix rejects a CV associated with another user, whereas the original expected result concerns unauthorized retrieval of an existing CV analysis.	Authenticate as two users and attempt direct cross-user access to the saved result.
CVAN-006	OCR extraction is reported as operational; the test should explicitly cover an image-only or nearly empty CV and verify the extracted text passed into analysis.	Preserve a sanitized source-to-OCR-to-workflow comparison for a challenging PDF.
CVAN-008	Successful retry was observed; the note does not establish that the existing uploaded file was reused or the result was stored only once.	Verify the upload identifier remains unchanged and exactly one successful analysis is persisted.
CVAN-009	Returning to an in-progress analysis was observed; the final completed state and absence of duplicate jobs were not expressly documented.	Wait for completion after navigation and inspect execution count and result state.
CVAN-010	The observation describes removing values from a transcript, while the scenario requires a valid zero score in the CV report. Empty/missing values and the number 0 are not equivalent.	Use a CV-analysis API response with an explicit zero metric and verify the UI renders 0.
CVAN-011	The notes describe redaction before the n8n/LLM stage. This alone does not demonstrate that raw CV contents are absent from every result screen, error response, or visible log.	Inspect UI, API responses, logs, and error paths using sanitized test data.


These recommendations do not alter the statuses documented in the supplied test register. They identify where stronger evidence would make the report more defensible.
7. Overall Evaluation
The recorded tests indicate that the CV analysis functionality successfully supports upload and processing, combined CV/transcript reporting, retrieval after sign-out, OCR extraction through the Adobe OCR API, recovery from provider failures, and in-progress navigation. The logs also record redaction of personal information before sending content to the AI workflow.

A significant access/ownership validation issue, tracked as DCVAN-005, was corrected and retested. The supplied record documents the successful rejection of a user-mismatched CV after this fix.

The evidence supports a 100% final recorded pass rate (11/11), subject to the verification items above. This is a summary of the supplied test execution record, not an independent audit of the application, backend, stored results, or linked screenshots.
8. Conclusion
The Level Up CV Analysis and Results module achieved 11 recorded passes, comprising 10 first-attempt passes and 1 pass after a defect fix on 29 October 2026. The module demonstrated the expected behaviour in the executed functional and recovery scenarios, while recording an important ownership-validation correction.

Before using the tests as comprehensive evidence of security and boundary-case correctness, the team should confirm API metric equality, zero-score rendering, cross-account retrieval authorization, and protection against raw CV disclosure on all relevant surfaces.
