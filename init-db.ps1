<#
.SYNOPSIS
    Initializes the VolunteerHub Access database — creates all 5 tables (idempotent)
    and seeds the SuperAdmin account on first run.
    Safe to re-run: each CREATE TABLE is skipped if the table already exists.

.USAGE
    .\init-db.ps1
#>

$ErrorActionPreference = "Stop"

$dbPath      = "$PSScriptRoot\VolunteerHub\App_Data\VolunteerHub.accdb"
$unsafeDll   = "$PSScriptRoot\packages\System.Runtime.CompilerServices.Unsafe.4.5.3\lib\net461\System.Runtime.CompilerServices.Unsafe.dll"
$memoryDll   = "$PSScriptRoot\packages\System.Memory.4.5.4\lib\net461\System.Memory.dll"
$bcryptDll   = "$PSScriptRoot\packages\BCrypt.Net-Next.4.0.3\lib\net472\BCrypt.Net-Next.dll"

# ── Load BCrypt with its dependencies (order matters) ───────────────────────
Add-Type -Path $unsafeDll  -ErrorAction SilentlyContinue
Add-Type -Path $memoryDll  -ErrorAction SilentlyContinue
Add-Type -Path $bcryptDll

# ── Helpers ───────────────────────────────────────────────────────────────────
$connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;"

function NewConn {
    $c = New-Object System.Data.OleDb.OleDbConnection($connStr)
    $c.Open()
    return $c
}

function Exec($conn, $sql) {
    $cmd = New-Object System.Data.OleDb.OleDbCommand($sql, $conn)
    try   { $cmd.ExecuteNonQuery() | Out-Null }
    finally { $cmd.Dispose() }
}

function TableExists($conn, $name) {
    try {
        $cmd = New-Object System.Data.OleDb.OleDbCommand("SELECT TOP 1 * FROM [$name]", $conn)
        try   { $r = $cmd.ExecuteReader(); $r.Close(); return $true }
        finally { $cmd.Dispose() }
    } catch { return $false }
}

Write-Host "Connecting to: $dbPath" -ForegroundColor Cyan

# ── AppUser ──────────────────────────────────────────────────────────────────
$conn = NewConn
if (TableExists $conn "AppUser") {
    Write-Host "AppUser          — already exists, skipped." -ForegroundColor Yellow
    $conn.Close(); $conn.Dispose()
} else {
    Exec $conn @"
CREATE TABLE AppUser (
    Id                     COUNTER CONSTRAINT PK_AppUser PRIMARY KEY,
    FirstName              TEXT(100) NOT NULL,
    LastName               TEXT(100) NOT NULL,
    Email                  TEXT(255) NOT NULL,
    PasswordHash           TEXT(255) NOT NULL,
    IsMale                 YESNO NOT NULL,
    DateOfBirth            DATETIME,
    Phone                  TEXT(20),
    Address                MEMO,
    ImageProfilePath       TEXT(255),
    Role                   TEXT(20) NOT NULL,
    WorkspaceId            LONG,
    IsActive               YESNO NOT NULL,
    CreatedAt              DATETIME NOT NULL,
    LastLoginAt            DATETIME,
    RememberMeToken        TEXT(100),
    RememberMeTokenExpiry  DATETIME
)
"@
    $conn.Close(); $conn.Dispose()
    # Fresh connection required — Access OleDB does not expose new tables on existing connection
    $conn = NewConn
    Exec $conn "CREATE UNIQUE INDEX IX_AppUser_Email ON AppUser (Email)"
    Exec $conn "CREATE INDEX IX_AppUser_WorkspaceId ON AppUser (WorkspaceId)"
    Exec $conn "CREATE INDEX IX_AppUser_Token ON AppUser (RememberMeToken)"
    $conn.Close(); $conn.Dispose()
    Write-Host "AppUser          — created." -ForegroundColor Green
}

# ── Workspaces ───────────────────────────────────────────────────────────────
$conn = NewConn
if (TableExists $conn "Workspaces") {
    Write-Host "Workspaces       — already exists, skipped." -ForegroundColor Yellow
    $conn.Close(); $conn.Dispose()
} else {
    Exec $conn @"
CREATE TABLE Workspaces (
    Id        COUNTER CONSTRAINT PK_Workspaces PRIMARY KEY,
    Name      TEXT(200) NOT NULL,
    Code      TEXT(50)  NOT NULL,
    LogoPath  TEXT(255),
    IsActive  YESNO     NOT NULL,
    CreatedAt DATETIME  NOT NULL
)
"@
    $conn.Close(); $conn.Dispose()
    $conn = NewConn
    Exec $conn "CREATE UNIQUE INDEX IX_Workspaces_Code ON Workspaces (Code)"
    $conn.Close(); $conn.Dispose()
    Write-Host "Workspaces       — created." -ForegroundColor Green
}

# ── Projects ─────────────────────────────────────────────────────────────────
$conn = NewConn
if (TableExists $conn "Projects") {
    Write-Host "Projects         — already exists, skipped." -ForegroundColor Yellow
    $conn.Close(); $conn.Dispose()
} else {
    Exec $conn @"
CREATE TABLE Projects (
    Id            COUNTER   CONSTRAINT PK_Projects PRIMARY KEY,
    WorkspaceId   LONG      NOT NULL,
    Title         TEXT(200) NOT NULL,
    Description   MEMO,
    Location      TEXT(200),
    StartDate     DATETIME  NOT NULL,
    EndDate       DATETIME  NOT NULL,
    MaxVolunteers LONG,
    HoursRequired DOUBLE,
    CreatedAt     DATETIME  NOT NULL
)
"@
    $conn.Close(); $conn.Dispose()
    $conn = NewConn
    Exec $conn "CREATE INDEX IX_Projects_WorkspaceId ON Projects (WorkspaceId)"
    $conn.Close(); $conn.Dispose()
    Write-Host "Projects         — created." -ForegroundColor Green
}

# ── VolunteerProject ─────────────────────────────────────────────────────────
$conn = NewConn
if (TableExists $conn "VolunteerProject") {
    Write-Host "VolunteerProject — already exists, skipped." -ForegroundColor Yellow
    $conn.Close(); $conn.Dispose()
} else {
    Exec $conn @"
CREATE TABLE VolunteerProject (
    Id         COUNTER  CONSTRAINT PK_VolunteerProject PRIMARY KEY,
    UserId     LONG     NOT NULL,
    ProjectId  LONG     NOT NULL,
    EnrolledAt DATETIME NOT NULL
)
"@
    $conn.Close(); $conn.Dispose()
    $conn = NewConn
    Exec $conn "CREATE INDEX IX_VP_UserId ON VolunteerProject (UserId)"
    Exec $conn "CREATE INDEX IX_VP_ProjectId ON VolunteerProject (ProjectId)"
    $conn.Close(); $conn.Dispose()
    Write-Host "VolunteerProject — created." -ForegroundColor Green
}

# ── Events ───────────────────────────────────────────────────────────────────
$conn = NewConn
if (TableExists $conn "Events") {
    Write-Host "Events           — already exists, skipped." -ForegroundColor Yellow
    $conn.Close(); $conn.Dispose()
} else {
    Exec $conn @"
CREATE TABLE Events (
    Id          COUNTER  CONSTRAINT PK_Events PRIMARY KEY,
    UserId      LONG     NOT NULL,
    ProjectId   LONG     NOT NULL,
    EventDate   DATETIME NOT NULL,
    StartTime   TEXT(10),
    EndTime     TEXT(10),
    HoursLogged DOUBLE   NOT NULL,
    Notes       MEMO,
    LoggedAt    DATETIME NOT NULL
)
"@
    $conn.Close(); $conn.Dispose()
    $conn = NewConn
    Exec $conn "CREATE INDEX IX_Events_UserId ON Events (UserId)"
    Exec $conn "CREATE INDEX IX_Events_ProjectId ON Events (ProjectId)"
    $conn.Close(); $conn.Dispose()
    Write-Host "Events           — created." -ForegroundColor Green
}

# ── Seed SuperAdmin ──────────────────────────────────────────────────────────
$conn = NewConn
$countCmd = New-Object System.Data.OleDb.OleDbCommand("SELECT COUNT(*) FROM AppUser WHERE PasswordHash IS NOT NULL AND PasswordHash <> ''", $conn)
try   { $userCount = [int]$countCmd.ExecuteScalar() }
finally { $countCmd.Dispose() }

if ($userCount -gt 0) {
    Write-Host "SuperAdmin seed  — already exists, skipped." -ForegroundColor Yellow
} else {
    $hash = [BCrypt.Net.BCrypt]::HashPassword("Admin@1234", 12)
    $insCmd = New-Object System.Data.OleDb.OleDbCommand(
        "INSERT INTO AppUser (FirstName, LastName, Email, PasswordHash, IsMale, Role, IsActive, CreatedAt) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
        $conn)
    try {
        $p1 = $insCmd.Parameters.Add("@fn",  [System.Data.OleDb.OleDbType]::VarChar);    $p1.Value = "Super"
        $p2 = $insCmd.Parameters.Add("@ln",  [System.Data.OleDb.OleDbType]::VarChar);    $p2.Value = "Admin"
        $p3 = $insCmd.Parameters.Add("@em",  [System.Data.OleDb.OleDbType]::VarChar);    $p3.Value = "admin@volunteerhub.local"
        $p4 = $insCmd.Parameters.Add("@ph",  [System.Data.OleDb.OleDbType]::VarChar);    $p4.Value = $hash
        $p5 = $insCmd.Parameters.Add("@gen", [System.Data.OleDb.OleDbType]::Boolean);    $p5.Value = $true
        $p6 = $insCmd.Parameters.Add("@rl",  [System.Data.OleDb.OleDbType]::VarChar);    $p6.Value = "SuperAdmin"
        $p7 = $insCmd.Parameters.Add("@ac",  [System.Data.OleDb.OleDbType]::Boolean);    $p7.Value = $true
        $p8 = $insCmd.Parameters.Add("@ca",  [System.Data.OleDb.OleDbType]::DBDate);     $p8.Value = [datetime]::UtcNow
        $insCmd.ExecuteNonQuery() | Out-Null
        Write-Host "SuperAdmin seed  — inserted (admin@volunteerhub.local / Admin@1234)." -ForegroundColor Green
    } finally { $insCmd.Dispose() }
}
$conn.Close(); $conn.Dispose()

Write-Host "`nDatabase initialization complete." -ForegroundColor Cyan
