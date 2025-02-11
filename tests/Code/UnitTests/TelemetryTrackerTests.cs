// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Azure.Monitor.Telemetry;
using Azure.Monitor.Telemetry.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for <see cref="TelemetryTracker"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class TelemetryTrackerTests
{
	#region Fields

	private readonly IReadOnlyList<KeyValuePair<String, Double>> measurements = [new("m", 0)];
	private readonly TelemetryOperation operation = new() { Id = new Guid().ToString("N"), Name = "Test" };
	private readonly IReadOnlyList<KeyValuePair<String, String>> properties = [new("a", "b")];
	private readonly IReadOnlyList<KeyValuePair<String, String>> tags = [new(TelemetryTagKey.CloudRole, "role")];

	#endregion

	#region Tests: Constructors

	[TestMethod]
	public void Constructor_ShouldInitializeProperties()
	{
		// arrange
		var operationId = Guid.NewGuid().ToString("N");
		var operation = new TelemetryOperation
		{
			Id = operationId
		};
		var tags = new List<KeyValuePair<String, String>>
		{
			new(TelemetryTagKey.CloudRole, "tester")
		};
		var telemetryPublisher = new HttpTelemetryPublisherMock();

		// act
		_ = new TelemetryTracker(tags, telemetryPublisher)
		{
			Operation = operation
		};
	}

	#endregion

	#region Tests: Method PublishAsync

	[TestMethod]
	public async Task Method_PublishAsync_ShouldReturnEmptySuccess_WhenNoItems()
	{
		// arrange
		var telemetryTracker = new TelemetryTracker();

		// act
		var result = await telemetryTracker.PublishAsync();

		// assert
		Assert.AreEqual(0, result.Length);
	}

	#endregion

	#region Tests: Method Track

	[TestMethod]
	public void Method_Add_ShouldNotAddIfNull()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher);

		// act
		telemetryTracker.Add(null);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.Count;

		// assert
		Assert.AreEqual(0, actualResult);
	}

	[TestMethod]
	public void Method_Add_ShouldEnqueueTelemetryItem()
	{
		// arrange
		var operationId = Guid.NewGuid().ToString("N");
		var operation = new TelemetryOperation
		{
			Id = operationId
		};
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
		{
			Operation = operation
		};
		var telemetry = new TraceTelemetry(DateTime.UtcNow, "test", SeverityLevel.Information)
		{
			Operation = operation
		};

		// act
		telemetryTracker.Add(telemetry);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as TraceTelemetry;

		// assert
		Assert.AreEqual(telemetry, actualResult);
	}

	#endregion

	[TestMethod]
	public void Method_TrackDependency_With_HttpRequest()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
		{
			Operation = operation
		};
		var id = "test-id";
		var time = DateTime.UtcNow;
		var httpMethod = HttpMethod.Post;
		var uri = new Uri("http://example.com");
		var statusCode = HttpStatusCode.OK;
		var duration = TimeSpan.FromSeconds(1);

		// act
		telemetryTracker.TrackDependency(time, id, httpMethod, uri, statusCode, duration);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as DependencyTelemetry;

		// assert
		Assert.AreEqual(uri.ToString(), actualResult.Data, nameof(DependencyTelemetry.Data));

		Assert.AreEqual(duration, actualResult.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(id, actualResult.Id, nameof(DependencyTelemetry.Id));

		Assert.AreEqual($"{httpMethod.Method} {uri.AbsolutePath}", actualResult.Name, nameof(DependencyTelemetry.Name));

		Assert.AreEqual(operation, actualResult.Operation, nameof(DependencyTelemetry.Operation));

		Assert.AreEqual(statusCode.ToString(), actualResult.ResultCode, nameof(DependencyTelemetry.ResultCode));

		Assert.IsTrue(actualResult.Success, nameof(DependencyTelemetry.Success));

		Assert.AreEqual(uri.Host, actualResult.Target, nameof(DependencyTelemetry.Target));

		Assert.AreEqual(time, actualResult.Time, nameof(DependencyTelemetry.Time));

		Assert.AreEqual(DependencyType.HTTP, actualResult.Type, nameof(DependencyTelemetry.Type));
	}

	[TestMethod]
	public void Method_TrackDependencyInProc()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
		{
			Operation = operation
		};
		var id = "test-id";
		var name = "name";
		var typeName = "Service";
		var time = DateTime.UtcNow;
		var duration = TimeSpan.FromSeconds(1);
		var success = true;

		// act
		telemetryTracker.TrackDependencyInProc(time, id, name, success, duration, typeName);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as DependencyTelemetry;

		// assert
		Assert.AreEqual(duration, actualResult.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(id, actualResult.Id, nameof(DependencyTelemetry.Id));

		Assert.AreEqual(name, actualResult.Name, nameof(DependencyTelemetry.Name));

		Assert.IsTrue(actualResult.Success, nameof(DependencyTelemetry.Success));

		Assert.AreEqual(time, actualResult.Time, nameof(DependencyTelemetry.Time));

		Assert.AreEqual(DependencyType.InProc + " | " + typeName, actualResult.Type, nameof(DependencyTelemetry.Type));
	}

	[TestMethod]
	public void Method_TrackEvent()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var name = "test";
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
		{
			Operation = operation
		};

		// act
		telemetryTracker.TrackEvent(name, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as EventTelemetry;

		// assert
		Assert.AreEqual(name, actualResult.Name, nameof(ExceptionTelemetry.Exception));

		Assert.AreEqual(operation, actualResult.Operation, nameof(RequestTelemetry.Operation));
	}

	[TestMethod]
	public void Method_TrackException()
	{
		// arrange
		var telemetryPublisher = new HttpTelemetryPublisherMock();
		var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
		{
			Operation = operation
		};
		var exception = new Exception("Test exception");
		var severityLevel = SeverityLevel.Error;

		// act
		telemetryTracker.TrackException(exception, severityLevel, measurements, properties, tags);
		telemetryTracker.PublishAsync().Wait();
		var actualResult = telemetryPublisher.Buffer.First() as ExceptionTelemetry;

		// assert
		Assert.AreEqual(exception, actualResult.Exception, nameof(ExceptionTelemetry.Exception));

		Assert.AreEqual(operation, actualResult.Operation, nameof(ExceptionTelemetry.Operation));

		Assert.AreEqual(severityLevel, actualResult.SeverityLevel, nameof(ExceptionTelemetry.SeverityLevel));
	}

	//[TestMethod]
	//public void Method_TrackRequestBegin()
	//{
	//	// arrange
	//	var telemetryTracker = new TelemetryTracker([])
	//	{
	//		Operation = operation
	//	};
	//	var id = "test-id";
	//	var url = new Uri("http://example.com");
	//	var name = "Test Request";

	//	// act
	//	var actualResult = telemetryTracker.TrackRequestBegin(id, url, name);

	//	// assert
	//	Assert.AreEqual(id, actualResult.Id, nameof(RequestTelemetry.Id));

	//	Assert.AreEqual(name, actualResult.Name, nameof(RequestTelemetry.Name));

	//	Assert.AreEqual(operation, actualResult.Operation, nameof(RequestTelemetry.Operation));

	//	Assert.AreEqual(url, actualResult.Url, nameof(RequestTelemetry.Url));
	//}

	//[TestMethod]
	//public void Method_TrackRequestEnd()
	//{
	//	// arrange
	//	var telemetryPublisher = new HttpTelemetryPublisherMock();
	//	var telemetryTracker = new TelemetryTracker([], telemetryPublisher)
	//	{
	//		Operation = operation
	//	};
	//	var id = "test-id";
	//	var requestTelemetry = new RequestTelemetry(DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(1)), id, new Uri("http://example.com"))
	//	{
	//		Operation = operation
	//	};
	//	var success = true;
	//	var responseCode = "200";

	//	// act
	//	telemetryTracker.TrackRequestEnd(requestTelemetry, success, responseCode);
	//	telemetryTracker.PublishAsync().Wait();
	//	var actualResult = telemetryPublisher.Buffer.First() as RequestTelemetry;

	//	// assert
	//	Assert.IsTrue(requestTelemetry.Duration > TimeSpan.Zero, nameof(RequestTelemetry.Duration));

	//	Assert.AreEqual(responseCode, actualResult.ResponseCode, nameof(RequestTelemetry.ResponseCode));

	//	Assert.AreEqual(success, actualResult.Success, nameof(RequestTelemetry.Success));
	//}
}