This folder contains the evidence for all tests run along with a README file for each category of tests in a summary format as a support. These README files is not our actual test outcomes, this is only for reference and should not be used for marking, this is the master prompt used to summarize for context only. We have an official Excel File with our tests, the executions and the outcomes that must be used for marking. 

MASTER PROMPT — SOFTWARE TEST EXECUTION SUMMARY

ROLE

Act as a Senior Software Quality Assurance (QA) Engineer, Test Analyst and Technical Documentation Specialist.

Your responsibility is to analyse the provided software testing records and produce a comprehensive, accurate, professional Test Execution Summary Report in Markdown (.md) format.

The application under test is Level Up — Career Readiness Platform, a career development application supporting user profiles, CV and academic transcript analysis, skills gap assessments, personalised learning pathways, AI career coaching, career readiness dashboards, career assessments and employment opportunity recommendations.

Your report must be suitable for academic assessment, software engineering project documentation, GitHub repositories, stakeholder reviews and development team handover.

1. INPUT DATA STRUCTURE

The supplied testing dataset contains the following columns:

Field| Description
Test ID| Unique identifier for each test case
Section| Application module or functional area
Priority| Critical, High, Medium or Low
Testing Category| Functional, Integration, Security, UI/UX, Performance, etc.
Method| Testing approach or technique used
Test Case| Description of the functionality being tested
Preconditions| Requirements or conditions before execution
Steps / Input| Actions performed and input data
Expected Result| Intended application behaviour
Status| Recorded execution status
Actual Result| Observed application behaviour
Tester| Person responsible for the test
Test Date| Date the test was executed
Device / Browser| Environment used during execution
Evidence File / Link| Supporting screenshots, recordings, logs or links
Defect ID| Linked defect identifier
Retest Notes| Observations from subsequent testing

Analyse all supplied records, including incomplete, failed, blocked and unexecuted cases.

2. ANALYSIS REQUIREMENTS

2.1 Data validation

Before summarising the dataset:

- Identify the total number of test case records.
- Check for duplicate Test IDs.
- Identify missing or inconsistent field values.
- Detect contradictory statuses and actual results.
- Identify missing test evidence and defect references.
- Highlight any records requiring clarification.
- Do not invent missing data.
- Preserve the original Test IDs and Defect IDs.
- Distinguish explicitly recorded information from inferred observations.

2.2 Execution status analysis

Classify test cases into the following categories:

- Passed — Recorded as successful and consistent with the actual result.
- Failed — Recorded as failed or explicitly showing unmet expected behaviour.
- Blocked — Execution could not proceed because of a dependency or issue.
- Not Executed — Test has not been performed.
- In Progress — Execution has started but is incomplete.
- Other / Unclear — Status cannot be mapped confidently.

Normalise equivalent status labels where appropriate, but explain any inconsistencies.

Do not automatically treat blank statuses as Passed.

Calculate:

- Total test cases
- Total passed
- Total failed
- Total blocked
- Total not executed
- Total in progress
- Total other / unclear
- Execution percentage
- Pass percentage
- Failure percentage
- Blocked percentage

Use these formulas:

Execution Rate = (Passed + Failed + Blocked + In Progress) / Total Test Cases × 100

Pass Rate (Completed Tests) = Passed / (Passed + Failed) × 100

Failure Rate (Completed Tests) = Failed / (Passed + Failed) × 100

Blocked Rate = Blocked / Total Test Cases × 100

State the denominators clearly. If a denominator is zero, report N/A rather than inventing a percentage.

Round percentages to two decimal places.

2.3 Functional coverage analysis

Group test cases according to the supplied Section field.

For every section, provide:

- Total test cases
- Passed
- Failed
- Blocked
- Not executed
- Pass rate
- Significant observations
- Outstanding defects
- Recommended next actions

Identify application modules that are fully tested, partially tested, untested or affected by failures.

Do not claim complete functional coverage solely because all documented tests passed.

2.4 Priority and risk analysis

Group results by Critical, High, Medium and Low priority.

Identify:

- Failed critical-priority test cases
- Failed high-priority test cases
- Blocked high-risk functionality
- Security-related failures
- Integration failures
- Data integrity concerns
- User experience issues
- Deployment or operational risks

Distinguish test priority from defect severity. Do not assume a High-priority test represents a High-severity defect.

Explain the potential impact of each significant issue using the recorded evidence.

2.5 Testing category and method analysis

Summarise the testing performed by category and method.

Identify:

- Categories with sufficient documented execution
- Categories with recorded failures
- Categories with limited coverage
- Testing methods used
- Testing methods not represented in the data
- Opportunities to improve testing depth

Do not assume an absent category was never tested outside the provided dataset.

2.6 Expected versus actual results

For failed or inconsistent test cases, compare:

1. Test ID
2. Test Case
3. Expected Result
4. Actual Result
5. Observed deviation
6. Potential user or system impact
7. Defect ID
8. Recommended next action

Clearly distinguish confirmed defects from possible defects requiring investigation.

Do not invent technical root causes.

2.7 Defect analysis

Extract all referenced Defect IDs.

For every unique defect, include:

- Defect ID
- Related Test IDs
- Affected application section
- Recorded failure
- Actual behaviour
- Supporting evidence
- Retest status or notes
- Outstanding action

Do not treat multiple test cases referencing one Defect ID as multiple unique defects.

Identify failed tests without linked defects.

2.8 Environment and device coverage

Analyse the Device / Browser column.

Provide:

- Platforms used
- Browsers tested
- Devices tested
- Number of tests per environment
- Failures per environment
- Any evident platform-specific problems
- Environments not represented in the provided test records

Avoid interpreting missing environment information as evidence of compatibility.

2.9 Test evidence and traceability

Evaluate the availability of supporting evidence.

Identify:

- Test cases with evidence links or files
- Test cases without recorded evidence
- Failed tests without evidence
- Broken or inaccessible links, only if access has actually been attempted
- Incomplete retest records
- Traceability between tests, defects and retesting activities

Do not claim that evidence has been reviewed if only its filename or URL is available.

2.10 Retesting and regression analysis

Use the Retest Notes column to identify:

- Tests marked for retesting
- Tests successfully retested
- Tests that remain unsuccessful
- Tests requiring regression testing
- Defects with unresolved retest outcomes

Do not mark a defect as resolved without supporting evidence.

3. REQUIRED MARKDOWN REPORT STRUCTURE

Generate the report using the following structure and headings.

Level Up — Software Test Execution Summary Report

1. Executive Summary

Provide a concise overview of the testing results, overall quality observations, significant failures, unresolved issues and current release-readiness position.

2. Test Execution Overview

Provide a Markdown table containing:

Metric| Count| Percentage
Total Test Cases| | 100%
Passed| | 
Failed| | 
Blocked| | 
Not Executed| | 
In Progress| | 
Other / Unclear| | 
Execution Rate| | 
Pass Rate (Completed Tests)| | 

Follow the table with an interpretation of the results.

3. Testing Scope and Coverage

Summarise the sections and functionality represented in the dataset.

Provide a section-by-section coverage table.

4. Testing Category and Method Summary

Provide separate category and method summaries, including test quantities, execution outcomes and significant observations.

5. Priority-Based Test Results

Provide a table containing:

Priority| Total| Passed| Failed| Blocked| Outstanding

Explain the most important priority-related risks.

6. Failed Test Case Summary

Provide a table containing:

Test ID| Section| Priority| Test Case| Expected Result| Actual Result| Defect ID

Summarise the most significant failures and their observed impact.

7. Defect Summary

Provide a consolidated defect table with linked test cases, issue descriptions, available evidence and resolution or retest status.

Highlight unresolved defects and missing defect references.

8. Blocked and Unexecuted Tests

Explain which tests could not be completed, the recorded reasons and the impact on testing confidence.

9. Device, Browser and Environment Coverage

Provide an environment coverage table and observations about platform-specific results.

10. Test Evidence and Traceability

Summarise the quality and completeness of testing evidence and defect traceability.

11. Retesting and Regression Testing

Document retesting activity, outstanding retests and suggested regression-testing priorities.

12. Key Findings

Organise findings under:

12.1 Successful Functionality

12.2 Identified Failures

12.3 High-Risk Areas

12.4 Testing Gaps

12.5 Data Quality Issues

Each finding must reference relevant Test IDs or supporting records wherever possible.

13. Recommendations and Corrective Actions

Provide an actionable table:

Priority| Recommended Action| Related Test IDs| Expected Outcome

Prioritise confirmed failures, missing evidence, unresolved defects, blocked tests and important coverage gaps.

14. Overall Quality Assessment

Evaluate:

- Functional reliability
- Integration stability
- Security testing evidence
- Usability testing evidence
- Performance testing evidence
- Platform compatibility evidence
- Test completeness
- Release readiness

Use Supported, Partially Supported, Not Demonstrated or Significant Concerns where appropriate.

These assessments must reflect the available test evidence rather than assumptions.

15. Release Readiness and Conclusion

Provide one of the following recommendations:

- Ready for Release
- Conditionally Ready
- Not Ready
- Insufficient Evidence

Explain the recommendation with reference to recorded test outcomes, outstanding defects, blocked tests and coverage gaps.

Do not declare the application production-ready merely because a high percentage of tests passed.

16. Appendix — Test Case Traceability

Include a concise Markdown table containing:

Test ID| Section| Priority| Status| Tester| Test Date| Device / Browser| Evidence| Defect ID| Retest Notes

Preserve meaningful references to the original test records.

If the dataset is too large for a complete appendix, state the limitation and retain all failed, blocked and critical-priority cases in the visible report.

4. OUTPUT REQUIREMENTS

Strictly follow these formatting and quality requirements:

1. Return only valid GitHub-Flavoured Markdown.
2. Begin the report with a level-one Markdown heading.
3. Use descriptive headings and properly aligned Markdown tables.
4. Ensure every reported count can be traced to the supplied data.
5. Do not fabricate test results, dates, defects, testers or evidence.
6. Use "Not Provided" for missing information when necessary.
7. Separate recorded facts from analysis and recommendations.
8. Use professional, objective software engineering language.
9. Avoid unnecessary repetition or excessive descriptions of individual passing tests.
10. Prioritise failures, risks, quality concerns and actionable improvements.
11. Refer to specific Test IDs for significant findings.
12. Maintain consistent status terminology throughout the report.
13. Ensure that every table is syntactically valid Markdown.
14. Make the report readable in GitHub, VS Code and Markdown document viewers.
15. Do not wrap the entire report inside a Markdown code fence.
16. Do not create unsupported claims about features or technical architecture.
17. If the input is incomplete, summarise the available records and explicitly explain the limitations.
18. If any required calculation cannot be performed reliably, show "N/A" and explain why.

5. TEST DATA

Analyse the following test execution records:

[PASTE COMPLETE TESTING TABLE, CSV, JSON OR EXTRACTED SPREADSHEET DATA HERE]

6. FINAL INSTRUCTION

Produce the complete Level Up Software Test Execution Summary Report in ".md" format.

Your primary objective is to transform raw testing records into a clear, evidence-based, decision-ready report that communicates:

- What was tested
- How testing was performed
- Which tests passed or failed
- What remains incomplete
- Which defects and risks remain outstanding
- What corrective actions should be prioritised
- Whether the recorded evidence supports release readiness

Do not simply reproduce the original test spreadsheet. Analyse it, consolidate findings, calculate meaningful metrics and provide actionable conclusions.
