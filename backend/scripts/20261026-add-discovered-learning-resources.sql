START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    ALTER TABLE learning_resources ADD canonical_url character varying(500);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    ALTER TABLE learning_resources ADD provider_key character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    ALTER TABLE learning_resources ADD provider_resource_id character varying(200);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000001';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://exercism.org/tracks/csharp', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000002';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://www.postgresql.org/docs/current/tutorial-sql.html', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000003';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://sqlbolt.com/', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000004';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Overview', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000005';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000006';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://dev.java/learn', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000007';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://exercism.org/tracks/java', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000008';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://docs.python.org/3/tutorial', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000009';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://exercism.org/tracks/python', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000010';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://www.postgresql.org/docs/current/tutorial.html', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000011';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://pgexercises.com/', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000012';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://react.dev/learn', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000013';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://react.dev/learn/tutorial-tic-tac-toe', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000014';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000015';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Structuring_content/Test_your_skills/HTML_text_basics', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000016';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/Styling_basics', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000017';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Learn_web_development/Core/CSS_layout', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000018';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000019';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://javascript.info/', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000020';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://www.typescriptlang.org/docs/handbook/intro.html', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000021';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://www.typescriptlang.org/play', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000022';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://git-scm.com/book/en/v2', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000023';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://github.com/skills/introduction-to-github', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000024';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/dotnet/core/get-started', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000025';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/paths/build-dotnet-applications-csharp', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000026';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://nodejs.org/learn/getting-started/introduction-to-nodejs', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000027';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://nodejs.org/api/test.html', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000028';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://docs.docker.com/get-started', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000029';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://docs.docker.com/get-started/workshop', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000030';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/paths/microsoft-azure-fundamentals-describe-cloud-concepts', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000031';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/azure', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000032';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000033';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://jestjs.io/docs/getting-started', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000034';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://linuxjourney.com/', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000035';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://overthewire.org/wargames/bandit', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000036';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://docs.github.com/en/actions/get-started/quickstart', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000037';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://github.com/skills/hello-github-actions', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000038';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://support.microsoft.com/en-us/excel', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000039';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://support.microsoft.com/en-us/office/overview-of-formulas-in-excel-ecfdc708-9162-49e8-b993-c311f47ca173', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000040';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/powerplatform/power-bi', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000041';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/power-bi/create-reports/desktop-excel-stunning-report', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000042';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://learn.microsoft.com/en-us/training/paths/visualize-data-power-bi', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000043';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources SET canonical_url = 'https://www.tableau.com/learn/training', provider_key = NULL, provider_resource_id = NULL
    WHERE resource_id = '40000000-0000-0000-0000-000000000044';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    UPDATE learning_resources
    SET canonical_url = CASE
        WHEN split_part(url, '#', 1) ~ '^https://[^/]+/$'
            THEN split_part(url, '#', 1)
        ELSE regexp_replace(split_part(url, '#', 1), '/+$', '')
    END
    WHERE canonical_url IS NULL OR btrim(canonical_url) = '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    ALTER TABLE learning_resources ALTER COLUMN canonical_url SET NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    CREATE UNIQUE INDEX learning_resources_skill_canonical_url_key ON learning_resources (skill_id, canonical_url);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    CREATE UNIQUE INDEX learning_resources_skill_provider_resource_key ON learning_resources (skill_id, provider_key, provider_resource_id) WHERE provider_key IS NOT NULL AND provider_resource_id IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    ALTER TABLE learning_resources ADD CONSTRAINT learning_resources_provider_identity_check CHECK (provider_resource_id IS NULL OR provider_key IS NOT NULL);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261026065033_AddDiscoveredLearningResources') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261026065033_AddDiscoveredLearningResources', '8.0.31');
    END IF;
END $EF$;
COMMIT;

