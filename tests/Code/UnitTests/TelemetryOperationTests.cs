// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

/// <summary>
/// Tests for <see cref="TelemetryOperation"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public sealed class TelemetryOperationTests
{
	[TestMethod]
	public void Constructor_ShouldInitializeProperties()
	{
		// arrange
		var id = "testId";
		var name = "testName";
		var parentId = "testParentId";
		var syntheticSource = "testSyntheticSource";

		// act
		var telemetryOperation = new TelemetryOperation
		{
			Id = id,
			Name = name,
			ParentId = parentId,
			SyntheticSource = syntheticSource
		};

		// assert
		Assert.AreEqual(id, telemetryOperation.Id, nameof(TelemetryOperation.Id));

		Assert.AreEqual(name, telemetryOperation.Name, nameof(TelemetryOperation.Id));

		Assert.AreEqual(parentId, telemetryOperation.ParentId, nameof(TelemetryOperation.ParentId));

		Assert.AreEqual(syntheticSource, telemetryOperation.SyntheticSource, nameof(TelemetryOperation.SyntheticSource));
	}
}