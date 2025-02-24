// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.IntegrationTests;

using System.Diagnostics;

using Azure.Core.Pipeline;
using Azure.Monitor.Telemetry.Dependency;
using Azure.Storage.Queues;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// The goals of this test:
/// - publish telemetry data into two instances of AppInsights; one with auth, one without auth.
/// - test dependency tracking with <see cref="TelemetryTrackedHttpClientHandler"/>.
/// </summary>
[TestCategory("IntegrationTests")]
[TestClass]
public sealed class DependencyTrackingTests : AzureIntegrationTestsBase
{
	private const String QueueName = "commands";

	#region Data

	private static readonly Random random = new(DateTime.UtcNow.Millisecond);

	private readonly HttpClientTransport queueClientHttpClientTransport;

	private readonly QueueClient queueClient;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="DependencyTrackingTests"/> class.
	/// </summary>
	/// <param name="testContext">The test context.</param>
	public DependencyTrackingTests(TestContext testContext)
		: base
		(
			testContext,
			[],
			[
				Tuple.Create(@"Azure.Monitor.AuthOn.", true, Array.Empty<KeyValuePair<String, String>>()),
				Tuple.Create(@"Azure.Monitor.AuthOff.", false, Array.Empty<KeyValuePair<String, String>>())
			]
		)
	{
		var handler = new TelemetryTrackedHttpClientHandler(TelemetryTracker, () => ActivitySpanId.CreateRandom().ToString());

		queueClientHttpClientTransport = new HttpClientTransport(handler);

		var queueServiceUriParamName = @"Azure.Queue.Default.ServiceUri";
		var queueServiceUriParam = TestContext.Properties[queueServiceUriParamName]?.ToString() ?? throw new ArgumentException($"Parameter {queueServiceUriParamName} has not been provided.");
		var queueServiceUri = new Uri(queueServiceUriParam);

		var queueClientOptions = new QueueClientOptions()
		{
			MessageEncoding = QueueMessageEncoding.Base64,
			Transport = queueClientHttpClientTransport
		};

		var queueService = new QueueServiceClient(queueServiceUri, TokenCredential, queueClientOptions);

		queueClient = queueService.GetQueueClient(QueueName);
	}

	#endregion

	#region Methods: Tests

	[TestMethod]
	public async Task AzureQueue_Success()
	{
		// prepare
		var startTime = DateTime.UtcNow;

		var requestId = ActivityTraceId.CreateRandom().ToString();

		var requestOperation = TelemetryTracker.Operation;

		//TelemetryTracker.Operation = requestOperation with(ParentId = requestId);

		var cancellationToken = TestContext.CancellationTokenSource.Token;

		// execute
		_ = await queueClient.SendMessageAsync("begin", cancellationToken);

		await Task.Delay(random.Next(300));

		_ = await queueClient.SendMessageAsync("end", cancellationToken);

		var requestTelemetry = new RequestTelemetry(requestOperation, startTime, requestId, new Uri($"tst:{nameof(DependencyTrackingTests)}"), "OK")
		{
			Duration = DateTime.UtcNow - startTime,
			Name = nameof(AzureQueue_Success),
			Success = true
		};

		TelemetryTracker.Add(requestTelemetry);

		var result = await TelemetryTracker.PublishAsync(cancellationToken);

		// assert
		AssertStandardSuccess(result);
	}

	#endregion

	#region Methods: Implementation of IDisposable

	/// <inheritdoc/>
	public override void Dispose()
	{
		queueClientHttpClientTransport.Dispose();

		base.Dispose();
	}

	#endregion
}
