@"
USE StayWizeDb;

INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
SELECT NEWID(), 'Admin', 'ADMIN', NEWID()
WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN');

INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FirstName, LastName)
SELECT NEWID(), 'nahuel@staywize.com', 'NAHUEL@STAYWIZE.COM', 'nahuel@staywize.com', 'NAHUEL@STAYWIZE.COM', 1, 'AQAAAAIAAYagAAAAELSPtqbSNqMFiSsJebO9r3Y5PQ0dTjB5JVmFhYNVjDyPOJ0vcL9VxYAT6RWVQ2BSXA==', NEWID(), NEWID(), 0, 0, 0, 0, 'Nahuel', 'Krauss'
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = 'NAHUEL@STAYWIZE.COM');

INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.NormalizedEmail = 'NAHUEL@STAYWIZE.COM'
AND r.NormalizedName = 'ADMIN';
"@ | Out-File -FilePath "C:\Users\nahue\source\repos\StayWize\seed.sql" -Encoding ASCII
