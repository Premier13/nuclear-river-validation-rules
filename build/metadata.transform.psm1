Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

$DomainNames = @{
	'Cyprus' = 'com.cy'
	'Czech' = 'cz'
	'Emirates' = 'ae'
	'Russia' = 'ru'
	'Ukraine' = 'ua'
	'Kazakhstan' = 'kz'
	'Kyrgyzstan' = 'kg'
	'Uzbekistan' = 'uz'
	'Azerbaijan' = 'az'
}

$DBSuffixes = @{
	'Cyprus' = 'CY'
	'Czech' = 'CZ'
	'Emirates' = 'AE'
	'Russia' = 'RU'
	'Ukraine' = 'UA'
	'Kazakhstan' = 'KZ'
	'Kyrgyzstan' = 'KG'
	'Uzbekistan' = 'UZ'
	'Azerbaijan' = 'AZ'
}

function Get-DbEnv($Context){
	switch($Context.EnvType){
		'Business' {
			return $Context.EnvType
		}
		'Edu' {
			return $Context.EnvType
		}
		default {
			return $null
		}
	}
}

function Get-DBHostMetadata($Context){
	switch($Context.EnvType){
		'Test' {
			$dbHost = 'uk-erm-sql02'
		}
		'Business' {
			$dbHost = 'uk-erm-edu03'
		}
		'Edu' {
			$dbHost = 'uk-erm-edu03'
		}
		'Production' {
			$dbHost = 'uk-sql20\erm'
		}
		'Load' {
			$dbHost = 'uk-test-sql01\MSSQL2017'
		}
		'Appveyor' {
			$dbHost = '(local)\SQL2016'
		}
	}

	return @{ 'DBHost' = $dbHost }
}

function Get-KafkaMetadata($Context){

	$kafkaGroupId = "erm_vr_$($Context.EnvType.ToLowerInvariant())"
	if ($Context['Index']){
		$kafkaGroupId += "_$($Context.Index)"
	}

	$metadata = @{
		'KafkaGroupId' = $kafkaGroupId
	}

	switch($Context.EnvType){
		{$_ -in ('Production', 'Load')} {
			$metadata += @{
				 'AmsFactsTopic' = 'ams_okapi_prod.am.validity'
				 'RulesetFactsTopic' = 'casino_staging_flowRulesets_compacted'
			 }
		}
		'Test' {
			$metadata += @{
				'AmsFactsTopic' = 'ams_okapi_staging.am.validity'
				'RulesetFactsTopic' = 'casino_staging_flowRulesets_compacted'
			}
		}
		'Business' {
			$metadata += @{
				'AmsFactsTopic' = "ams_okapi_business$($Context['Index']).am.validity"
				'RulesetFactsTopic' = 'erm_business01_flowRulesets'
			}
		}
		'Edu' {
			$metadata += @{
				'AmsFactsTopic' = "ams_okapi_edu$($Context['Index']).am.validity"
				'RulesetFactsTopic' = 'casino_staging_flowRulesets_compacted'
			}
		}
		default {
			return @{}
		}
	}

	return $metadata;
}

function Get-XdtMetadata($Context){
	return @('environments\Common\Erm.Release.config', "environments\Erm.$($Context.EnvType).config")
}

function Get-RegexMetadata($Context){

	$regex = @{ '{EntryPoint}' = $Context['EntryPoint'] }

	if ($Context['Index']){
		$regex += @{ '{EnvNum}' = $Context['Index'] }
	}
	if ($Context['EnvType']){
		$regex += @{ '{DbEnv}' = (Get-DbEnv $Context) }
		$regex += @{ '{EnvType}' = $Context['EnvType'] }
	}

	$serviceBusMetadata = (Get-ServiceBusMetadata $Context)['ServiceBus']
	if ($serviceBusMetadata.Count -ne 0){
		if ($serviceBusMetadata['CreateTopics']){
			foreach($metadata in $serviceBusMetadata.CreateTopics.GetEnumerator()){
				$regex["{$($metadata.Key)}"] = $metadata.Value.Name
			}
		}

		if ($serviceBusMetadata['CreateSubscriptions']){
			foreach($metadata in $serviceBusMetadata.CreateSubscriptions.GetEnumerator()){
				$regex["{$($metadata.Key)}"] = $metadata.Value.Name
			}
		}
	}

	$keyValuePairs = @{}
	$keyValuePairs += Get-DBHostMetadata $Context
	$keyValuePairs += Get-KafkaMetadata $Context

	foreach($keyValuePair in $keyValuePairs.GetEnumerator()){
		$regex["{$($keyValuePair.Key)}"] = $keyValuePair.Value
	}

	return $regex
}

function Get-TransformMetadata ($Context) {

	return @{
		'Transform' = @{
			'Xdt' = Get-XdtMetadata $Context
			'Regex' = Get-RegexMetadata $Context
		}
	}
}

Export-ModuleMember -Function Get-TransformMetadata -Variable DomainNames
