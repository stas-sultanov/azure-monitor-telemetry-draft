﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using System;
using System.Net.Http;

using Azure.Monitor.Telemetry.Dependency;
using Azure.Monitor.Telemetry.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for <see cref="TelemetryTrackedHttpClientHandler"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class TelemetryTrackedHttpClientHandlerTests
{
	[TestMethod]
	public void Constructor_ThrowsException_WhenParameterIsNull()
	{
		// arrange
		static String getId()
		{
			return Guid.NewGuid().ToString();
		}

		var telemetryTracker = new TelemetryTracker([]);

		// act and assert
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => _ = new TelemetryTrackedHttpClientHandler(null, getId), "telemetryTracker");

		_ = Assert.ThrowsExactly<ArgumentNullException>(() => _ = new TelemetryTrackedHttpClientHandler(telemetryTracker, null));

		using var x = new TelemetryTrackedHttpClientHandler(telemetryTracker, getId);
	}

	[TestMethod]
	public async Task SendAsync_TracksTelemetry()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([telemetryPublisher], []);
		var handler = new TelemetryTrackedHttpClientHandler(telemetryTracker, () => "test-id");
		var httpClient = new HttpClient(handler);
		var request = new HttpRequestMessage(HttpMethod.Get, "https://google.com");

		// act
		_ = await httpClient.SendAsync(request, CancellationToken.None);

		_ = await telemetryTracker.PublishAsync(CancellationToken.None);

		// assert
		Assert.AreEqual(1, telemetryPublisher.Buffer.Count, "Items Count");

		var telemetry = telemetryPublisher.Buffer[0];

		Assert.IsInstanceOfType<DependencyTelemetry>(telemetry);
	}
}