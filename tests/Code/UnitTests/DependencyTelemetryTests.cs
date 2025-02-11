// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using Azure.Monitor.Telemetry.Tests;

/// <summary>
/// Tests for <see cref="DependencyTelemetry"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class DependencyTelemetryTests
{
	[TestMethod]
	public void Constructor_ShouldInitializeCorrectly()
	{
		// arrange
		var data = "data";
		var duration = TimeSpan.Zero;
		var id = Guid.NewGuid().ToString();
		var measurements = new List<KeyValuePair<String, Double>>() { new("m", 0) };
		var name = "test";
		var operation = new TelemetryOperation();
		var properties = new List<KeyValuePair<String, String>>() { new("a", "b") };
		var resultCode = "OK";
		var success = true;
		var tags = new List<KeyValuePair<String, String>>() { new(TelemetryTagKey.CloudRole, "role") };
		var target = "target";
		var time = DateTime.UtcNow;
		var type = DependencyType.AI;

		// act
		var telemetry = new DependencyTelemetry(time, id, name)
		{
			Data = data,
			Duration = duration,
			Measurements = measurements,
			Operation = operation,
			Properties = properties,
			ResultCode = resultCode,
			Success = success,
			Tags = tags,
			Target = target,
			Type = type
		};

		// assert
		Assert.AreEqual(data, telemetry.Data, nameof(DependencyTelemetry.Data));

		Assert.AreEqual(duration, telemetry.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(DependencyTelemetry.Id));

		Assert.AreEqual(name, telemetry.Name, nameof(DependencyTelemetry.Name));

		AssertHelpers.AreEqual(operation, telemetry.Operation);

		Assert.AreEqual(resultCode, telemetry.ResultCode, nameof(DependencyTelemetry.ResultCode));

		Assert.AreEqual(success, telemetry.Success, nameof(DependencyTelemetry.Success));

		Assert.AreEqual(tags, telemetry.Tags, nameof(DependencyTelemetry.Tags));

		Assert.AreEqual(target, telemetry.Target, nameof(DependencyTelemetry.Target));

		Assert.AreEqual(time, telemetry.Time, nameof(DependencyTelemetry.Time));

		Assert.AreEqual(type, telemetry.Type, nameof(DependencyTelemetry.Type));
	}
}
