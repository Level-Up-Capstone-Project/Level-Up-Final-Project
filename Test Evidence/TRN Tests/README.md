<img width="798" height="373" alt="image" src="https://github.com/user-attachments/assets/c7e62c85-d23d-446e-bc5b-9cb962977f91" />

Transcript Upload & Analysis — Test Execution Summary
Project: Level Up — Career Readiness Mobile Application
Module: 08 — Transcript Upload and Analysis
Test execution date: 29 October 2026
Tester: Michelle van der Merwe
Test IDs: TRN-001 to TRN-012
Test environment: Device; manual UI testing and selected API checks (specific device model not recorded)
Evidence repository: TRN Test Evidence
1. Testing Objective
The objective of this test cycle was to assess the Level Up application's transcript upload and AI-assisted analysis workflow. Testing covered valid PDF processing, result presentation, marks and distinction calculations, separation from CV records, unsupported or malformed files, incomplete or invalid provider data, error recovery, timeout and cancellation, and user-specific access controls.
2. Test Execution Overview
Metric	Recorded result
Total test cases	12
Passed on first attempt	11
Passed after fix and retest	1
Outstanding failures recorded	0
Final recorded pass rate	100% (12/12)
Priority P0 (critical)	9
Priority P1 (high)	3
Functional tests	5
Edge-case tests	4
Recovery tests	2
Security tests	1
Documented defect	DTRN-012 (reported fixed)


Interpretation: The figures above reflect the test sheet's reported statuses. They do not independently prove that every expected assertion was checked; follow-up verification items are documented below.
3. Detailed Test Results
TRN-001 — Upload and Analyse a Valid Transcript PDF
- Priority / Category / Method: P0 · Functional · Manual UI
- Precondition: Authenticated user with a valid PDF transcript.
- Steps: Select the transcript and allow analysis to complete.
- Expected: Progress milestones are displayed, followed by one transcript analysis report.
- Actual: The transcript was selected and upload progress showed the milestones reached.
- Recorded status: Pass (first attempt).
- Review note: The written observation confirms upload progress; verify that exactly one completed report was returned.
- Evidence: TRN-001 Test
TRN-002 — Display Transcript Summary and Metadata
- Priority / Category / Method: P0 · Functional · Manual UI
- Precondition: Successful transcript analysis.
- Steps: Allow the uploaded transcript to be analysed.
- Expected: A visible completion message confirms that analysis has finished.
- Actual: Transcript upload and analysis completed, with a visual confirmation presented to the user.
- Recorded status: Pass (first attempt).
- Evidence: TRN-002 Test
TRN-003 — Display Subject Marks and Average Accurately
- Priority / Category / Method: P0 · Functional · Manual UI + API
- Precondition: Transcript with known marks.
- Steps: Compare the analysis report with the source document and API results.
- Expected: Each mark, including any zero marks, and the calculated average match the returned analysis.
- Actual: The report displayed analysed marks, and the tester counted distinctions and checked the other marks mentioned.
- Recorded status: Pass (first attempt).
- Review note: The observation does not explicitly document an independent average calculation, zero-mark verification or API-to-source comparison. Record these checks separately where applicable.
- Evidence: TRN-003 Test
TRN-004 — Count Distinctions at 75% and Above
- Priority / Category / Method: P1 · Functional · Manual UI
- Precondition: Transcript with marks around the 75% threshold.
- Steps: Open the report and count the highlighted distinctions.
- Expected: All marks of 75% or higher contribute to the distinction total; other marks are also addressed.
- Actual: The tester counted the distinctions and confirmed that other marks appeared in the analysis.
- Recorded status: Pass (first attempt).
- Review note: Preserve a boundary case showing how a mark of exactly 75% is counted.
- Evidence: TRN-004 Test
TRN-005 — Transcript Analysis Does Not Create or Replace a CV
- Priority / Category / Method: P0 · Functional · Manual UI + API
- Precondition: An existing CV record and a valid transcript.
- Steps: Analyse the transcript, then retrieve the user's current CV.
- Expected: The existing CV remains unchanged.
- Actual: The transcript workflow provided suggestions to update the CV, rather than directly changing it. The user remained responsible for editing and reuploading the CV.
- Recorded status: Pass (first attempt).
- Review note: The description supports the intended workflow separation, but retrieving and comparing the CV record before and after analysis would more directly verify the assertion.
- Evidence: TRN-005 Test
TRN-006 — Reject Unsupported Transcript Type
- Priority / Category / Method: P0 · Edge Case · Manual UI
- Precondition: Non-PDF document.
- Steps: Attempt to upload a DOCX file.
- Expected: The application rejects the unsupported file and indicates the required PDF format.
- Actual: The DOCX file was rejected. The upload interface's supported-formats text was corrected to remove DOCX.
- Recorded status: Pass (first attempt; UI text fix documented).
- Evidence: TRN-006 Test
TRN-007 — Reject Empty or Corrupted Transcript
- Priority / Category / Method: P0 · Edge Case · Manual UI
- Precondition: Empty or malformed PDF.
- Steps: Attempt transcript analysis.
- Expected: The app displays a validation error and does not store a successful report.
- Actual: An empty PDF was submitted, and the workflow reported that it did not contain enough readable text.
- Recorded status: Pass (first attempt).
- Review note: Empty-PDF rejection is observed. A corrupted-PDF example and a check that no successful report was saved are not explicitly documented.
- Evidence: TRN-007 Test
TRN-008 — Handle Missing Optional Analysis Fields
- Priority / Category / Method: P1 · Edge Case · Manual UI
- Precondition: Provider response omits optional metadata.
- Steps: Complete analysis and open the results.
- Expected: Available fields display without blank-card crashes or fabricated values.
- Actual: The tester reported that available results matched the transcript and that no invented values or blank-card crashes occurred.
- Recorded status: Pass (first attempt).
- Review note: Verify using a response that actually omits optional fields; the observation alone does not establish that the omission was exercised.
- Evidence: TRN-008 Test
TRN-009 — Omit Invalid Provider Scores
- Priority / Category / Method: P1 · Edge Case · Manual UI
- Precondition: Provider response contains invalid score values.
- Steps: Complete analysis and inspect the report.
- Expected: Invalid scores are not presented as valid marks, while the rest of the report stays usable.
- Actual: Some score positions were replaced by special characters (@, !, &); the remaining available results still loaded.
- Recorded status: Pass (first attempt).
- Review note: The described output confirms resilience but does not clearly establish whether invalid values were omitted rather than displayed. Confirm how these characters appeared in the final report.
- Evidence: TRN-009 Test
TRN-010 — Provider Failure Does Not Persist a Report
- Priority / Category / Method: P0 · Recovery · Manual UI
- Precondition: Transcript provider/workflow returns an error.
- Steps: Trigger an analysis error and reload the current transcript analysis.
- Expected: No false successful report is persisted and the user sees a recoverable error.
- Actual: Upload encountered an error, after which the tester retried and achieved a successful result.
- Recorded status: Pass (first attempt).
- Review note: Successful retry is established; the observation does not state whether storage was inspected for a false report after the failed attempt.
- Evidence: TRN-010 Test
TRN-011 — Distinguish Timeout from Cancellation
- Priority / Category / Method: P0 · Recovery · Manual UI
- Precondition: Slow provider response.
- Steps: Allow one request to time out, then cancel a separate upload request.
- Expected: The two conditions result in appropriate, distinct outcomes without false completion.
- Actual: The first request timed out and displayed an error; a separate upload was cancelled.
- Recorded status: Pass (first attempt).
- Review note: Record the exact error and cancellation messages to document the distinction explicitly.
- Evidence: TRN-011 Test
TRN-012 — Transcript Results Belong Only to the Current User
- Priority / Category / Method: P0 · Security · Manual UI + API
- Precondition: Two authenticated users.
- Steps: Analyse a transcript as user A, then attempt to retrieve it as user B.
- Expected: User B cannot access user A's transcript analysis.
- Initial finding: The backend and workflow initially failed to identify a mismatch between the logged-in user and an uploaded transcript from another user. The tester attributed the mismatch to the redaction of identifying information before the document was sent to the LLM in the n8n workflow.
- Fix / retest: After a backend fix, uploading a transcript belonging to another user produced a message stating that the document did not match the logged-in user.
- Recorded status: Pass after Fix.
- Defect ID: DTRN-012.
- Review note: The documented retest demonstrates improved ownership validation at upload. A separate authorization check is still advisable to establish that user B cannot retrieve user A's previously stored analysis using an API request or manipulated record identifier.
- Evidence: DTRN-012 Test
4. Defect and Retest Summary
Defect ID	Related test	Recorded issue	Fix / retest outcome	Final status
DTRN-012	TRN-012	Ownership mismatch initially not detected when another user's transcript was uploaded	Backend change added a rejection message for a transcript that does not match the logged-in user	Pass after Fix


A supported-file-types label was also corrected during TRN-006, removing DOCX from a list of acceptable formats. No defect ID was recorded for this UI text change.
5. Recommended Follow-Up Verification
The following are coverage and evidence recommendations, not additional recorded failures:
1. TRN-012 — Cross-user access: Attempt to fetch another authenticated user's previously saved transcript report using that report's ID; verify that access is denied by the backend independently of the document ownership check at upload.
2. TRN-003 / TRN-004 — Numerical accuracy: Capture expected marks, an independently calculated average, zero-mark behaviour, and boundary marks of 74%, 75%, and 76%.
3. TRN-008 / TRN-009 — Response validation: Test controlled provider responses with omitted optional fields and explicitly invalid score strings; confirm that incorrect values do not appear as valid marks.
4. TRN-007 / TRN-010 — Persistence: Confirm that failed or empty analyses do not create a successful stored report, including after a refresh.
5. TRN-001 / TRN-005 — State consistency: Verify that one successful upload yields exactly one report and that an existing CV record remains unchanged.
6. TRN-011 — Recovery messaging: Capture distinct visible timeout and user-cancellation states.
6. Overall Evaluation
The recorded test cycle indicates that the Level Up transcript analysis module supports the main user journey: selecting a PDF transcript, displaying upload milestones, performing AI-assisted analysis and presenting academic results. The observations also show PDF-only file rejection, empty-document error handling, recovery from a workflow failure, and a corrected user-ownership validation flow.

Eleven cases were recorded as passing on the first attempt; one security case (TRN-012) passed after a documented backend fix and retest. The final recorded result is 12 of 12 passing, with no outstanding failures listed in the provided test log.

Some case descriptions do not independently demonstrate all of their expected outcomes. The follow-up checks above should be completed before making broad claims about numerical accuracy, data persistence, missing-field handling, or full cross-user access isolation.
7. Conclusion
The transcript upload and analysis component demonstrated positive functional, recovery and security test results in this cycle. A material ownership-validation defect was identified, addressed in the backend, and reported as passing on retest. The available evidence supports continued integration testing, with focused checks recommended for score correctness, malformed provider data, saved-report behaviour and user-specific authorization.

Final recorded outcome: 12/12 Pass (11 first attempt; 1 after fix).
Date: 29 October 2026
Tester: Michelle van der Merwe
Recorded open defects: None listed.

