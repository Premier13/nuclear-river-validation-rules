Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

# TODO: QueueDeploy-ServiceBus
Task Deploy-ServiceBus -Precondition { $Metadata['UpdateSchemas'] } {
	if ($Metadata['ValidationRules.Replication.Host']){
		# fixme: обращение к $Properties.Tenants выглядит не совсем уместным, возможно етсь более корректное решение.
		foreach ($tenant in $Properties.Tenants.Split(',')){
			Deploy-ServiceBus 'ValidationRules.Replication.Host' $tenant
		}
	}
}

function Deploy-ServiceBus ($entryPointName, $tenant){

	$serviceBusMetadata = $Metadata[$entryPointName]['ServiceBus']
	if (!$serviceBusMetadata){
		Write-Host "Skip Deploy-ServiceBus"
		return
	}

	if ($serviceBusMetadata['CreateTopics']){
		foreach($createTopicsMetadata in $serviceBusMetadata.CreateTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $createTopicsMetadata.ConnectionStringNameProvider $tenant
			Create-Topic $connectionString $createTopicsMetadata.Name $createTopicsMetadata.Properties
		}
	}
	if ($serviceBusMetadata['DeleteTopics']){
		foreach($deleteTopicsMetadata in $serviceBusMetadata.DeleteTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $deleteTopicsMetadata.ConnectionStringNameProvider $tenant
			Delete-Topic $connectionString $deleteTopicsMetadata.Name
		}
	}

	if ($serviceBusMetadata['CreateSubscriptions']){
		foreach($createSubscriptionsMetadata in $serviceBusMetadata.CreateSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $createSubscriptionsMetadata.ConnectionStringNameProvider $tenant
			if ($Metadata['UpdateSchemas']){
				Delete-Subscription $connectionString $createSubscriptionsMetadata.TopicName $createSubscriptionsMetadata.Name
				Write-Host "Delete-Subscription:", $tenant, $createSubscriptionsMetadata.TopicName, $createSubscriptionsMetadata.Name
			}
			Create-Subscription $connectionString $createSubscriptionsMetadata.TopicName $createSubscriptionsMetadata.Name $createSubscriptionsMetadata.Properties
			Write-Host "Create-Subscription:", $tenant, $createSubscriptionsMetadata.TopicName, $createSubscriptionsMetadata.Name
		}
	}
	if ($serviceBusMetadata['DeleteSubscriptions']){
		foreach($deleteSubscriptionsMetadata in $serviceBusMetadata.DeleteSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $deleteSubscriptionsMetadata.ConnectionStringNameProvider $tenant
			Delete-Subscription $connectionString $deleteSubscriptionsMetadata.TopicName $deleteSubscriptionsMetadata.Name
			Write-Host "Delete-Subscription:", $deleteSubscriptionsMetadata.TopicName, $deleteSubscriptionsMetadata.Name
		}
	}
}

function Get-EntryPointConnectionString ($entryPointName, $connectionStringNameProvider, $tenant) {
	$artifactDir = Get-Artifacts $entryPointName

	$configItem = @(Get-ChildItem $artifactDir -Filter '*.exe.config')
	if ($configItem.Length -eq 0){
		throw "Can't find *.exe.config file in $artifactDir"
	}
	if ($configItem.Length -gt 1){
		throw "Find more than one *.exe.config file in $artifactDir"
	}

	[xml]$config = Get-Content $configItem.FullName
	$connectionStringName = $connectionStringNameProvider.Invoke($tenant)
	$connectionString = Get-ConnectionString $config $connectionStringName
	return $connectionString
}
