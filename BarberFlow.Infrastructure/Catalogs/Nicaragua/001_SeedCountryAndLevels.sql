-- All Nicaragua catalog parameters: country, administrative levels and area types.
SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRANSACTION;

DECLARE @CountryId INT;

SELECT @CountryId = Id
FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
WHERE Code = N'NIC';

IF @CountryId IS NULL
BEGIN
    INSERT INTO dbo.Countries (Code, Name)
    VALUES (N'NIC', N'Nicaragua');

    SET @CountryId = CONVERT(INT, SCOPE_IDENTITY());
END;

DECLARE @Levels TABLE
(
    [Level] INT NOT NULL,
    DisplayName NVARCHAR(80) NOT NULL,
    PRIMARY KEY ([Level], DisplayName)
);

INSERT INTO @Levels ([Level], DisplayName)
VALUES
    (1, N'Departamento'),
    (1, N'Region'),
    (2, N'Municipio'),
    (3, N'Barrio'),
    (3, N'Reparto'),
    (3, N'Comarca'),
    (3, N'Comunidad'),
    (3, N'Localidad'),
    (3, N'Caserio'),
    (3, N'Sector'),
    (3, N'Zona'),
    (3, N'Microregion');

INSERT INTO dbo.CountryAdministrativeLevels (CountryId, [Level], DisplayName)
SELECT @CountryId, source.[Level], source.DisplayName
FROM @Levels AS source
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.CountryAdministrativeLevels AS target WITH (UPDLOCK, HOLDLOCK)
    WHERE target.CountryId = @CountryId
      AND target.[Level] = source.[Level]
      AND target.DisplayName = source.DisplayName
);

DECLARE @AreaTypes TABLE
(
    AdministrativeLevelName NVARCHAR(80) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(80) NOT NULL,
    PRIMARY KEY (AdministrativeLevelName, Code)
);

INSERT INTO @AreaTypes (AdministrativeLevelName, Code, Name)
VALUES
    (N'Departamento', N'DEPARTMENT', N'Departamento'),
    (N'Region', N'REGION', N'Region'),
    (N'Municipio', N'MUNICIPALITY', N'Municipio'),
    (N'Barrio', N'NEIGHBORHOOD', N'Barrio'),
    (N'Reparto', N'SUBDIVISION', N'Reparto'),
    (N'Comarca', N'RURAL_DISTRICT', N'Comarca'),
    (N'Comunidad', N'COMMUNITY', N'Comunidad'),
    (N'Localidad', N'LOCALITY', N'Localidad'),
    (N'Caserio', N'HAMLET', N'Caserio'),
    (N'Sector', N'SECTOR', N'Sector'),
    (N'Zona', N'ZONE', N'Zona'),
    (N'Microregion', N'MICROREGION', N'Microregion');

INSERT INTO dbo.AdministrativeAreaTypes (CountryAdministrativeLevelId, Code, Name)
SELECT administrativeLevel.Id, source.Code, source.Name
FROM @AreaTypes AS source
INNER JOIN dbo.CountryAdministrativeLevels AS administrativeLevel
    ON administrativeLevel.CountryId = @CountryId
   AND administrativeLevel.DisplayName = source.AdministrativeLevelName
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.AdministrativeAreaTypes AS target WITH (UPDLOCK, HOLDLOCK)
    WHERE target.CountryAdministrativeLevelId = administrativeLevel.Id
      AND target.Code = source.Code
);

COMMIT TRANSACTION;
