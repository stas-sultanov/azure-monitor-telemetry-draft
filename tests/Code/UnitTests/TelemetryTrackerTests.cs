// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Azure.Monitor.Telemetry;
using Azure.Monitor.Telemetry.Mocks;
using Azure.Monitor.Telemetry.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for <see cref="TelemetryTracker"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class TelemetryTrackerTests
{
	#region Fields

	private readonly KeyValuePair<String, Double> [] measurements = [new("m", 0)];
	private readonly OperationContext operation = new(new Guid().ToString("N"), "Test");
	private readonly KeyValuePair<String, String> [] properties = [new("a", "b")];
	private readonly KeyValuePair<String, String> [] tags = [new(TelemetryTagKey.CloudRole, "role")];

	#endregion

	#region Methos: Tests Constructors

	[TestMethod]
	public void Constructor()
	{
		// arrange
		var operationId = Guid.NewGuid().ToString("N");
		var operation = new OperationContext(operationId);
		var tags = new KeyValuePair<String, String> []
		{
			new(TelemetryTagKey.CloudRole, "tester")
		};
		var telemetryPublisher = new HttpTelemetryPublisherMock();

		// act
		_ = new TelemetryTracker(telemetryPublisher, operation, tags);
	}

	#endregion

	#region Tests: Method PublishAsync

	[TestMethod]
	public async Task Method_PublishAsync_ShouldReturnEmptySuccess_WhenNoItems()
	{
		// arrange
		var telemetryTracker = new TelemetryTracker([]);

		// act
		var result = await telemetryTracker.PublishAsync();

		// assert
		Assert.AreEqual(0, result.Length);
	}

	#endregion

	#region Tests: Method Add

	[TestMethod]
	public void Method_Add_ShouldEnqueueTelemetryItem()
	{
		// arrange
		var operationId = Guid.NewGuid().ToString("N");
		var operation = new OperationContext(operationId);
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var telemetry = new TraceTelemetry(operation, DateTime.UtcNow, "test", SeverityLevel.Information);

		// act
		telemetryTracker.Add(telemetry);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as TraceTelemetry;

		// assert
		Assert.AreEqual(telemetry, actualResult);
	}

	#endregion

	#region Tests: Method Track

	[TestMethod]
	public void Method_TrackAvailability()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var id = "test-id";
		var name = "name";
		var message = "ok";
		var time = DateTime.UtcNow;
		var duration = TimeSpan.FromSeconds(1);
		var success = true;
		var runLocation = "test-server";

		// act
		telemetryTracker.TrackAvailability(time, id, name, message, duration, success, runLocation, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as AvailabilityTelemetry;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, duration, id, measurements, message, name, runLocation, success);
	}

	[TestMethod]
	public void Method_TrackDependency_With_HttpRequest()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var id = "test-id";
		var time = DateTime.UtcNow;
		var httpMethod = HttpMethod.Post;
		var uri = new Uri("http://example.com");
		var statusCode = HttpStatusCode.OK;
		var duration = TimeSpan.FromSeconds(1);

		// act
		telemetryTracker.TrackDependency(time, id, httpMethod, uri, statusCode, duration, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as DependencyTelemetry;

		// assert
		var data = uri.ToString();
		var name = $"{httpMethod.Method} {uri.AbsolutePath}";
		var resultCode = statusCode.ToString();

		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, data, duration, id, measurements, name, resultCode, true, uri.Host, DependencyType.HTTP);
	}

	[TestMethod]
	public void Method_TrackDependencyInProc()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var id = "test-id";
		var name = "name";
		var typeName = "Service";
		var time = DateTime.UtcNow;
		var duration = TimeSpan.FromSeconds(1);
		var success = true;

		// act
		telemetryTracker.TrackDependencyInProc(time, id, name, success, duration, typeName, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as DependencyTelemetry;
		var type = DependencyType.InProc + " | " + typeName;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, null, duration, id, measurements, name, null, true, null, type);
	}

	[TestMethod]
	public void Method_TrackEvent()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var name = "test";
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);

		// act
		telemetryTracker.TrackEvent(name, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as EventTelemetry;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, measurements, name);
	}

	[TestMethod]
	public void Method_TrackException()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var exception = new Exception("Test exception");
		var severityLevel = SeverityLevel.Error;

		// act
		telemetryTracker.TrackException(exception, severityLevel, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as ExceptionTelemetry;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, exception, measurements, severityLevel);
	}

	[TestMethod]
	public void Method_TrackMetric()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);
		var name = "test";
		var @namespace = "tests";
		var value = 6;
		var valueAggregation = new MetricValueAggregation
		{
			Count = 3,
			Max = 3,
			Min = 1,
		};

		// act
		telemetryTracker.TrackMetric(@namespace, name, value, valueAggregation, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as MetricTelemetry;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, name, @namespace, value, valueAggregation);
	}

	[TestMethod]
	public void Method_TrackTrace()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var message = "test";
		var severityLevel = SeverityLevel.Information;
		var telemetryTracker = new TelemetryTracker(telemetryPublisher, operation);

		// act
		telemetryTracker.TrackTrace(message, severityLevel, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as TraceTelemetry;

		// assert
		Assert.IsNotNull(actualResult);

		AssertHelpers.PropertiesAreEqual(actualResult, operation, properties, tags);

		AssertHelpers.PropertiesAreEqual(actualResult, message, severityLevel);
	}

	#endregion
}