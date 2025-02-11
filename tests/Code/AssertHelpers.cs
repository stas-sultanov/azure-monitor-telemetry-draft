// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Tests;

internal class AssertHelpers
{
	/// <summary>
	/// Tests whether the specified values are equal and throws an exception if the two values are not equal.
	/// </summary>
	/// <param name="expected">
	/// The first value to compare. This is the value the tests expects.
	/// </param>
	/// <param name="actual">
	/// The second value to compare. This is the value produced by the code under test.
	/// </param>
	/// <exception cref="AssertFailedException">
	/// Thrown if <paramref name="expected"/> is not equal to <paramref name="actual"/>.
	/// </exception>
	public static void AreEqual(TelemetryOperation expected, TelemetryOperation actual)
	{
		// check if both params are not referncing to the same object
		if (ReferenceEquals(expected, actual))
		{
			return;
		}

		Assert.AreEqual(expected.Id, actual.Id, nameof(TelemetryOperation.Id));

		Assert.AreEqual(expected.Name, actual.Name, nameof(TelemetryOperation.Name));

		Assert.AreEqual(expected.ParentId, actual.ParentId, nameof(TelemetryOperation.ParentId));

		Assert.AreEqual(expected.SyntheticSource, actual.SyntheticSource, nameof(TelemetryOperation.SyntheticSource));
	}

	/// <summary>
	/// Tests whether the specified values are equal and throws an exception if the two values are not equal.
	/// </summary>
	/// <param name="expected">
	/// The first value to compare. This is the value the tests expects.
	/// </param>
	/// <param name="actual">
	/// The second value to compare. This is the value produced by the code under test.
	/// </param>
	/// <exception cref="AssertFailedException">
	/// Thrown if <paramref name="expected"/> is not equal to <paramref name="actual"/>.
	/// </exception>
	private static void AreEqual(Telemetry expected, Telemetry actual)
	{
		AreEqual(expected.Operation, actual.Operation);

		// CollectionAssert.AreEquivalent(expected.Tags, actual.Tags);

		Assert.AreEqual(expected.Time, actual.Time, nameof(Telemetry.Time));
	}

	/// <summary>
	/// Tests whether the specified values are equal and throws an exception if the two values are not equal.
	/// </summary>
	/// <param name="expected">
	/// The first value to compare. This is the value the tests expects.
	/// </param>
	/// <param name="actual">
	/// The second value to compare. This is the value produced by the code under test.
	/// </param>
	/// <exception cref="AssertFailedException">
	/// Thrown if <paramref name="expected"/> is not equal to <paramref name="actual"/>.
	/// </exception>
	public static void AreEqual(DependencyTelemetry expected, DependencyTelemetry actual)
	{
		// check if both params are not referencing to the same object
		if (ReferenceEquals(expected, actual))
		{
			return;
		}

		AreEqual((Telemetry) expected, actual);

		Assert.AreEqual(expected.Data, actual.Data, nameof(DependencyTelemetry.Data));

		Assert.AreEqual(expected.Duration, actual.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(expected.Id, actual.Id, nameof(DependencyTelemetry.Id));

		Assert.AreEqual(expected.Name, actual.Name, nameof(DependencyTelemetry.Name));

		Assert.AreEqual(expected.ResultCode, actual.ResultCode, nameof(DependencyTelemetry.ResultCode));

		Assert.AreEqual(expected.Success, actual.Success, nameof(DependencyTelemetry.Success));

		Assert.AreEqual(expected.Type, actual.Type, nameof(DependencyTelemetry.Type));

		Assert.AreEqual(expected.Target, actual.Target, nameof(DependencyTelemetry.Target));
	}
}
