param
(
    [Parameter(Mandatory=$true)]
    [string]$artifactPath = $null,
    [Parameter(Mandatory=$true)]
    [string]$databaseServer = $null,
    [Parameter(Mandatory=$true)]
    [string]$databaseName = $null,
    [Parameter(Mandatory=$true)]
	[string]$serviceUsername = $null,
    [Parameter(Mandatory=$true)]
	[string]$servicePassword = $null,
    [Parameter(Mandatory=$true)]
	[string]$rootUrl = $null
)

# Setup the script
$ErrorActionPreference = "Stop"

# Stop service
$serviceName = "siftvoting"
if (Get-Service $serviceName -ErrorAction SilentlyContinue) {
if ((Get-Service $serviceName).Status -eq "Running") {
  Stop-Service $serviceName
  Write-Verbose "Stopping $serviceName"
} else {
  Write-Verbose "$serviceName found, but it is not running."
}
} else {
Write-Host "$serviceName not found"
}
  
# Ensure we have suitable credentials
Write-Verbose ("Starting deployment as " + [Security.Principal.WindowsIdentity]::GetCurrent().Name)
If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Error "Account does not have admin credentials"
    Break
}

# First we backup the database
if ((Invoke-SqlCmd -Query ("SELECT DB_ID('" + $databaseName +"')") -ServerInstance $databaseServer)[0].GetType() -ne [System.DBNull])
{
    $backupFilename = $databaseName + "_release_backup_" + (get-date -format "yyyy_MM_dd_HHmmss") + ".bak"
    Write-Verbose ("Backing up " + $databaseName + " on " + $databaseServer + " to " + $backupFilename + "...")
    Backup-SqlDatabase -Database $databaseName -BackupFile $backupFilename -ServerInstance $databaseServer
}
else
{
    Write-Verbose ("Database " + $databaseName + " on " + $databaseServer + " does not exist, skipping backup...")
}
Write-Verbose("Skipping database backup")

# Now we apply deltas
Write-Verbose "Applying database deltas..."
& (Join-Path $artifactPath "Database\DatabaseDeltaApplicator\DatabaseDeltaApplicator.exe") $databaseServer $databaseName "Main" (Join-Path $artifactPath "Database")
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Failed to apply deltas, terminating release"
}

# Construct connection string
$connectionString = ("Server=" + $databaseServer + "; Database=" + $databaseName + "; Trusted_Connection=True; Application Name=SIFT Voting API");

# Install Voting API
$msiFile = (Get-Item (Join-Path $artifactPath "*.msi"))[0].FullName
$connectionString = $connectionString
$msiLogFile = $msiFile + ".log"
Write-Verbose ("Executing installation of " + $msiFile + " and logging to " + $msiLogFile + "...")
$processStartInfo = new-object System.Diagnostics.ProcessStartInfo "msiexec.exe"
$processStartInfo.Arguments = ("/i `"" + $msiFile + "`" /lv* `"" + $msiLogFile + "`" /qn SERVICE_USERNAME=`"" + $serviceUsername + "`" SERVICE_PASSWORD=`"" + $servicePassword + "`" CONFIG_CONNECTION_STRING=`"" + $connectionString + "`" CONFIG_SWAGGER_OVERRIDE_ROOT_URL=`"" + $rootUrl + "`"")
$processStartInfo.Verb = "runas"
$process = [System.Diagnostics.Process]::Start($processStartInfo)
$process.WaitForExit()
if ($process.ExitCode -ne 0)
{
    Write-Error "Failed to install service, terminating release"
}

# We're done
Write-Verbose "Release was successful"