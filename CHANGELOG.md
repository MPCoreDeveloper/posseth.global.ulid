# Changelog

Alle significante wijzigingen aan dit project worden in dit bestand bijgehouden.

De opmaak is gebaseerd op [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
en dit project volgt [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-08-25

### Added

- **Native T-SQL variant** (zonder CLR): `src\posseth.global.ulid.mssql\TSQL\Ulid.TSQL.sql`
  met de functies `dbo.GenerateUlid`, `dbo.IsValidUlid`, `dbo.UlidToTimestamp` en
  `dbo.ExtractDateFromUlid`.
- `NewUlid(long)` valideert nu de 48-bit timestamp-grens en gooit
  `ArgumentOutOfRangeException` voor waarden boven `2^48 - 1`.
- De Base32-decoder wijst nu ook waarden af die de 128-bit ULID-range overschrijden
  (eerste karakter boven `7`), conform de ULID-specificatie.

### Changed

- **BREAKING**: de Crockford Base32-encoding is gecorrigeerd conform de officiële
  [ULID-specificatie](https://github.com/ulid/spec). De 128-bit waarde wordt nu gecodeerd met
  het eerste karakter dat 3 bits draagt (2 leidende nulbits) in plaats van een 5-bit uitgelijnde
  bitstroom.
- ULIDs die door deze library worden gegenereerd zijn nu **interchangeable** met alle andere
  standards-compliant ULID-implementaties (C#, JavaScript, Go, Python, ...).
- Versie opgehoogd naar **2.0.0**.

### Fixed

- Timestamp-extractie (`ToDateTime`, `ToEpoch`, `GetTimestampFromUlid`,
  `dbo.ExtractDateFromUlid`) levert nu het correcte standaard-tijdstip op voor ULIDs van andere
  implementaties.

### Deprecated / Removed

- ULIDs die met versie **1.x** zijn gegenereerd zijn **niet** uitwisselbaar met 2.x.
  Behandel 1.x-ULIDs als legacy-data en genereer ze opnieuw.

## [1.x] en eerder

Voor versie 1.x was er geen changelog. Let op: ULIDs gegenereerd met 1.x gebruikten een
non-compliant Base32-encoding en zijn niet uitwisselbaar met 2.x.
