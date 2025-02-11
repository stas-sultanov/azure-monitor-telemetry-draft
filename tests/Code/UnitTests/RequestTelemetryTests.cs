// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using Azure.Monitor.Telemetry.Tests;

/// <summary>
/// Tests for <see cref="RequestTelemetry"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class RequestTelemetryTests
{
	[TestMethod]
	public void Constructor_ShouldInitializeCorrectly()
	{
		// arrange
		var id = "test-id";
		var name = "Test Request";
		var operation = new TelemetryOperation();
		var responseCode = "200";
		var time = DateTime.UtcNow;
		var url = new Uri("http://example.com");

		// act
		var requestTelemetry = new RequestTelemetry(time, id, url, responseCode)
		{
			Name = name,
			Operation = operation
		};

		// assert
		Assert.AreEqual(id, requestTelemetry.Id, nameof(RequestTelemetry.Id));

		Assert.AreEqual(name, requestTelemetry.Name, nameof(RequestTelemetry.Name));

		AssertHelpers.AreEqual(operation, requestTelemetry.Operation);

		Assert.AreEqual(responseCode, requestTelemetry.ResponseCode, nameof(RequestTelemetry.ResponseCode));

		Assert.AreEqual(time, requestTelemetry.Time, nameof(RequestTelemetry.Time));

		Assert.AreEqual(url, requestTelemetry.Url, nameof(RequestTelemetry.Url));
	}
}
