# This script sets the environment variables containing secrets for the development environment. The environment variables below are needed to
# have a working JWT authentication and e-mail notifications in development environment.
# Usage:
# In an admin PowerShell window, navigate to the folder of the this script, then load it with the command: . ".\setsecrets.ps1"
# Then issue this command: Set-Secret-EnvVars -jwtSecret <JWT_secret> -jwtValidInSecs <JWT_lifespan_in_seconds> -smtpUsername <SMTP_username> -smtpPassword <SMTP_password>

function Set-Secret-EnvVars
{
	param([string]$jwtSecret, [System.UInt32]$jwtValidInSecs, [string]$smtpUsername, [string]$smtpPassword)

	Write-Host "New environment variable values will be:"
	Write-Host "JWTCONFIG__SECRET: $jwtSecret"
	Write-Host "JWTCONFIG__VALIDINSECONDS: $jwtValidInSecs"
	Write-Host "EMAILCONFIG__SMTPUSERNAME: $smtpUsername"
	Write-Host "EMAILCONFIG__SMTPPASSWORD: $smtpPassword"

	Write-Host "Original environment variable values:"
	Display-EnvVars

	Write-Host "Setting environment variables to new values..."
	try
	{
		[System.Environment]::SetEnvironmentVariable("JWTCONFIG__SECRET", $jwtSecret, "Machine")
		[System.Environment]::SetEnvironmentVariable("JWTCONFIG__VALIDINSECONDS", $jwtValidInSecs, "Machine")
		[System.Environment]::SetEnvironmentVariable("EMAILCONFIG__SMTPUSERNAME", $smtpUsername, "Machine")
		[System.Environment]::SetEnvironmentVariable("EMAILCONFIG__SMTPPASSWORD", $smtpPassword, "Machine")
	}
	catch [System.Security.SecurityException]
	{
		Write-Host "Required permission lacks to set the environment variables. Check if you have started PowerShell as admin and try again."
		return
	}
	catch [System.Exception]
	{
		Write-Host "Something went wrong during setting the environment variables. Exception message: $($_.Exception.Message)"
		return
	}

	Write-Host "Done! Start a new PowerShell window to use the new environment variable values!"
}

function Display-EnvVars
{
	Write-Host "JWTCONFIG__SECRET: $env:JWTCONFIG__SECRET"
	Write-Host "JWTCONFIG__VALIDINSECONDS: $env:JWTCONFIG__VALIDINSECONDS"
	Write-Host "EMAILCONFIG__SMTPUSERNAME: $env:EMAILCONFIG__SMTPUSERNAME"
	Write-Host "EMAILCONFIG__SMTPPASSWORD: $env:EMAILCONFIG__SMTPPASSWORD"
}
