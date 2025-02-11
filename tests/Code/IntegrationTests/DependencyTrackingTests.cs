// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.IntegrationTests;

using System.Diagnostics;

using Azure.Core.Pipeline;
using Azure.Monitor.Telemetry.Dependency;
using Azure.Storage.Queues;

[TestClass]
public sealed class DependencyTrackingTests : AzureIntegrationTestsBase
{
	private const String QueueName = "test";

	#region Data

	private static readonly Random random = new(DateTime.UtcNow.Millisecond);

	private HttpClientTransport queueClientHttpClientTransport;

	private QueueClient queueClient;

	#endregion

	[TestInitialize]
	public override void Initialize()
	{
		base.Initialize();

		var handler = new TelemetryTrackedHttpClientHandler(TelemetryTracker, () => ActivitySpanId.CreateRandom().ToString());

		queueClientHttpClientTransport = new HttpClientTransport(handler);

		var queueServiceUri = new Uri(TestContext.Properties[@"Azure.Queue.Default.ServiceUri"].ToString());

		var queueClientOptions = new QueueClientOptions()
		{
			MessageEncoding = QueueMessageEncoding.Base64,
			Transport = queueClientHttpClientTransport
		};

		var queueService = new QueueServiceClient(queueServiceUri, tokenCredential, queueClientOptions);

		queueClient = queueService.GetQueueClient(QueueName);
	}

	#region Tests

	//[TestMethod]
	//public async Task AzureQueue_Success()
	//{
	//	var cancellationToken = TestContext.CancellationTokenSource.Token;

	//	var request = TelemetryTracker.TrackRequestBegin(ActivityTraceId.CreateRandom().ToString(), new Uri($"tst:{nameof(DependencyTrackingTests)}"));

	//	_ = await queueClient.SendMessageAsync("begin", cancellationToken);

	//	await Task.Delay(random.Next(300));

	//	_ = await queueClient.SendMessageAsync("end", cancellationToken);

	//	TelemetryTracker.TrackRequestEnd(request, true, "1");

	//	var result = await TelemetryTracker.PublishAsync(cancellationToken);

	//	AssertStandardSuccess(result);
	//}

	#endregion
}
