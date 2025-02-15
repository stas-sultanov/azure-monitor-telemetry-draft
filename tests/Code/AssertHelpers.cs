﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Tests;

using System;

/// <summary>
/// Provides helper methods for asserting.
/// </summary>
internal static class AssertHelpers
{
	#region Methods: AreEqual

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
		// check if both params are not referencing to the same object
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

	#endregion

	#region Methods: Properties Are Equal

	/// <summary>
	/// Tests whether data within the instance of <see cref="TelemetryOperation"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		TelemetryOperation actual,
		String id,
		String name,
		String parentId,
		String syntheticSource
	)
	{
		Assert.AreEqual(id, actual.Id, nameof(TelemetryOperation.Id));

		Assert.AreEqual(name, actual.Name, nameof(TelemetryOperation.Name));

		Assert.AreEqual(parentId, actual.ParentId, nameof(TelemetryOperation.ParentId));

		Assert.AreEqual(syntheticSource, actual.SyntheticSource, nameof(TelemetryOperation.SyntheticSource));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="Telemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		Telemetry actual,
		TelemetryOperation operation,
		PropertyList properties,
		TagList tags,
		DateTime? time = null
	)
	{
		AreEqual(operation, actual.Operation);

		if (time.HasValue)
		{
			Assert.AreEqual(time, actual.Time, nameof(Telemetry.Time));
		}
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="AvailabilityTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		AvailabilityTelemetry telemetry,
		TimeSpan duration,
		String id,
		MeasurementList measurements,
		String message,
		String name,
		String runLocation,
		Boolean success
	)
	{
		Assert.AreEqual(duration, telemetry.Duration, nameof(AvailabilityTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(AvailabilityTelemetry.Id));

		// Assert.AreEqual(measurements, telemetry.Measurements, nameof(ExceptionTelemetry.Measurements));

		Assert.AreEqual(message, telemetry.Message, nameof(AvailabilityTelemetry.Message));

		Assert.AreEqual(name, telemetry.Name, nameof(AvailabilityTelemetry.Name));

		Assert.AreEqual(runLocation, telemetry.RunLocation, nameof(AvailabilityTelemetry.RunLocation));

		Assert.AreEqual(success, telemetry.Success, nameof(AvailabilityTelemetry.Success));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="DependencyTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		DependencyTelemetry telemetry,
		String data,
		TimeSpan duration,
		String id,
		MeasurementList measurements,
		String name,
		String resultCode,
		Boolean success,
		String target,
		String type
	)
	{
		Assert.AreEqual(data, telemetry.Data, nameof(DependencyTelemetry.Data));

		Assert.AreEqual(duration, telemetry.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(DependencyTelemetry.Id));

		// Assert.AreEqual(measurements, telemetry.Measurements, nameof(DependencyTelemetry.Measurements));

		Assert.AreEqual(name, telemetry.Name, nameof(DependencyTelemetry.Name));

		Assert.AreEqual(resultCode, telemetry.ResultCode, nameof(DependencyTelemetry.ResultCode));

		Assert.AreEqual(success, telemetry.Success, nameof(DependencyTelemetry.Success));

		Assert.AreEqual(target, telemetry.Target, nameof(DependencyTelemetry.Target));

		Assert.AreEqual(type, telemetry.Type, nameof(DependencyTelemetry.Type));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="EventTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		EventTelemetry telemetry,
		MeasurementList measurements,
		String name
	)
	{
		// Assert.AreEqual(measurements, telemetry.Measurements, nameof(EventTelemetry.Measurements));

		Assert.AreEqual(name, telemetry.Name, nameof(EventTelemetry.Name));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="ExceptionTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		ExceptionTelemetry telemetry,
		Exception exception,
		MeasurementList measurements,
		SeverityLevel? severityLevel
	)
	{
		Assert.AreEqual(exception, telemetry.Exception, nameof(ExceptionTelemetry.Exception));

		Assert.AreEqual(measurements, telemetry.Measurements, nameof(EventTelemetry.Measurements));

		Assert.AreEqual(severityLevel, telemetry.SeverityLevel, nameof(ExceptionTelemetry.SeverityLevel));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="MetricTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		MetricTelemetry telemetry,
		String name,
		String @namespace,
		Double value,
		MetricValueAggregation valueAggregation = null
	)
	{
		Assert.AreEqual(name, telemetry.Name, nameof(MetricTelemetry.Name));

		Assert.AreEqual(@namespace, telemetry.Namespace, nameof(MetricTelemetry.Namespace));

		Assert.AreEqual(value, telemetry.Value, nameof(MetricTelemetry.Value));

		if (valueAggregation != null)
		{
			Assert.IsNotNull(telemetry.ValueAggregation, nameof(MetricTelemetry.ValueAggregation));

			Assert.AreEqual(valueAggregation.Count, telemetry.ValueAggregation.Count, nameof(MetricValueAggregation.Count));

			Assert.AreEqual(valueAggregation.Max, telemetry.ValueAggregation.Max, nameof(MetricValueAggregation.Max));

			Assert.AreEqual(valueAggregation.Min, telemetry.ValueAggregation.Min, nameof(MetricValueAggregation.Min));
		}
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="RequestTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		RequestTelemetry telemetry,
		TimeSpan duration,
		String id,
		MeasurementList measurements,
		String name,
		String responseCode,
		Boolean success,
		Uri url
	)
	{
		Assert.AreEqual(duration, telemetry.Duration, nameof(RequestTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(RequestTelemetry.Id));

		Assert.AreEqual(measurements, telemetry.Measurements, nameof(RequestTelemetry.Measurements));

		Assert.AreEqual(name, telemetry.Name, nameof(RequestTelemetry.Name));

		Assert.AreEqual(responseCode, telemetry.ResponseCode, nameof(RequestTelemetry.ResponseCode));

		Assert.AreEqual(success, telemetry.Success, nameof(RequestTelemetry.Success));

		Assert.AreEqual(url, telemetry.Url, nameof(RequestTelemetry.Url));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="TraceTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		TraceTelemetry telemetry,
		String message,
		SeverityLevel severityLevel
	)
	{
		Assert.AreEqual(message, telemetry.Message, nameof(TraceTelemetry.Message));

		Assert.AreEqual(severityLevel, telemetry.SeverityLevel, nameof(TraceTelemetry.SeverityLevel));
	}

	#endregion
}
