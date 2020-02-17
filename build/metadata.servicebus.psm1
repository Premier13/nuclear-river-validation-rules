Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

$topicProperties = @{
	'Properties' = @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
		'RequiresDuplicateDetection' = $true
	}
}

$subscriptionProperties = @{
	'Properties' = @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
		'LockDuration' = New-TimeSpan -Minutes 5
	}
}

function Get-ServiceBusMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-TopicsMetadata $Context

	return @{ 'ServiceBus' = $metadata}
}

function Get-TopicsMetadata ($Context) {

	$metadata = @{}

	switch ($Context.EntryPoint) {

		'ValidationRules.Replication.Host' {

			$metadata = @{
				'CreateTopics' = @{

					'ErmFactsTopic' = @{
						'Name' = 'topic.performedoperations'
						'ConnectionStringNameProvider' = {param($tenant) "ServiceBus.$tenant" }
					} + $topicProperties

				}

				'CreateSubscriptions' = @{

					'ErmEventsFlowSubscription' = @{
						'TopicName' = 'topic.performedoperations'
						'Name' = '213A17BF-2945-4F98-B02F-62235C0A107E'
						'ConnectionStringNameProvider' = {param($tenant) "ServiceBus.$tenant" }
					} + $subscriptionProperties

				}

				'DeleteTopics' = @{}
				'DeleteSubscriptions' = @{}
			}
		}
	}

	return $metadata
}

Export-ModuleMember -Function Get-ServiceBusMetadata
