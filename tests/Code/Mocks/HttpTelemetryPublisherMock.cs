﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Mocks;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Azure.Monitor.Telemetry.Publish;

internal sealed class HttpTelemetryPublisherMock : TelemetryPublisher
{
	#region fields

	public const String MockValidIngestEndpoint = @"https://dc.in.applicationinsights.azure.com/";

	public static readonly Uri MockValidIngestEndpointUri = new(MockValidIngestEndpoint);

	#endregion

	public Queue<Telemetry> Buffer { get; } = [];

	/// <inheritdoc/>
	public Task<TelemetryPublishResult> PublishAsync
	(
		IReadOnlyList<Telemetry> telemetryItems,
		IReadOnlyList<KeyValuePair<String, String>>? tags,
		CancellationToken cancellationToken
	)
	{
		var time = DateTime.UtcNow;

		foreach (var item in telemetryItems)
		{
			Buffer.Enqueue(item);
		}

		var publishResult = (TelemetryPublishResult) new HttpTelemetryPublishResult
		{
			Count = telemetryItems.Count,
			Duration = DateTime.UtcNow.Subtract(time),
			Response = "OK",
			StatusCode = HttpStatusCode.OK,
			Success = true,
			Time = time,
			Url = MockValidIngestEndpointUri
		};

		var result = Task.FromResult(publishResult);

		return result;
	}
}
