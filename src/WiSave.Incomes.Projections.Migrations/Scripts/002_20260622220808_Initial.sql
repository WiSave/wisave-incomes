CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                                                        "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
    );

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260622220808_Initial') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'projections') THEN
CREATE SCHEMA projections;
END IF;
END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260622220808_Initial') THEN
CREATE TABLE projections.categories (
                                        "Id" uuid NOT NULL,
                                        "UserId" uuid NOT NULL,
                                        "Name" character varying(200) NOT NULL,
                                        "SortOrder" integer NOT NULL DEFAULT 0,
                                        subcategories jsonb NOT NULL,
                                        CONSTRAINT "PK_categories" PRIMARY KEY ("Id")
);
END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260622220808_Initial') THEN
CREATE INDEX "IX_categories_UserId" ON projections.categories ("UserId");
END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260622220808_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260622220808_Initial', '10.0.9');
END IF;
END $EF$;
COMMIT;

