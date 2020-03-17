Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

Task QueueBuild-Migrations  {
	if ($Metadata['ValidationRules.Migrator']){
		$projectFileName = Get-ProjectFileName 'src' 'ValidationRules.Migrator'
		QueueBuild-AppPackage $projectFileName 'ValidationRules.Migrator'
	}
}

Task Run-Migrations -Precondition { $Metadata['ValidationRules.Migrator'] }{
	$dbHost = $Metadata['ValidationRules.Migrator']['Transform']['Regex']['{DBHost}']
	$catalog = Get-DatabaseName $Metadata['ValidationRules.Migrator']['Transform']['Regex']
	$connectionString = "Data Source=$($dbHost);Initial Catalog=$($catalog);Application Name=ValidationRules.Migrator;Integrated Security=True;"
	Run-Migrations $connectionString
}

function Get-DatabaseName($metadata) {
	$envNum = $metadata["{EnvNum}"]
	switch($metadata['{EnvType}']) {
		'Test' {
			return "ValidationRulesRU$($envNum)"
		}
		'Load' {
			return "ValidationRulesRU"
		}
		'Edu' {
			return "ValidationRulesEduRU"
		}
		'Business' {
			return "ValidationRulesBusinessRU$($envNum)"
		}
		'Production' {
			return "ValidationRules"
		}
	}
}

function Run-Migrations ($connectionString){
	$artifactName = Get-Artifacts 'ValidationRules.Migrator'
	$exePath = Join-Path $artifactName 'ValidationRules.Migrator.exe'

	Write-Host 'Invoke migrator with' $connectionString
	& $exePath $connectionString | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}
