﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Tests;

using System;

/// <summary>
/// Provides helper methods for asserting.
/// </summary>
internal static class AssertHelpers
{
	#region Static: Fields

	private static readonly KeyValuePairEqualityComparer<String, Double> measurementComparer = new(StringComparer.InvariantCulture, EqualityComparer<Double>.Default);

	private static readonly KeyValuePairEqualityComparer<String, String> propertyComparer = new(StringComparer.InvariantCulture, StringComparer.InvariantCulture);

	#endregion

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
	public static void AreEqual(OperationContext expected, OperationContext actual)
	{
		// check if both params are referencing to the same object
		if (ReferenceEquals(expected, actual))
		{
			return;
		}

		Assert.AreEqual(expected.Id, actual.Id, nameof(OperationContext.Id));

		Assert.AreEqual(expected.Name, actual.Name, nameof(OperationContext.Name));

		Assert.AreEqual(expected.ParentId, actual.ParentId, nameof(OperationContext.ParentId));

		Assert.AreEqual(expected.SyntheticSource, actual.SyntheticSource, nameof(OperationContext.SyntheticSource));
	}

	#endregion

	#region Methods: Properties Are Equal

	/// <summary>
	/// Tests whether data within the instance of <see cref="OperationContext"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		OperationContext actual,
		String id,
		String name,
		String parentId,
		String syntheticSource
	)
	{
		Assert.AreEqual(id, actual.Id, nameof(OperationContext.Id));

		Assert.AreEqual(name, actual.Name, nameof(OperationContext.Name));

		Assert.AreEqual(parentId, actual.ParentId, nameof(OperationContext.ParentId));

		Assert.AreEqual(syntheticSource, actual.SyntheticSource, nameof(OperationContext.SyntheticSource));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="Telemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		Telemetry telemetry,
		OperationContext operation,
		KeyValuePair<String, String>[] properties,
		KeyValuePair<String, String>[] tags,
		DateTime? time = null
	)
	{
		AreEqual(operation, telemetry.Operation);

		CollectionAssert.AreEquivalent(properties, telemetry.Properties, propertyComparer, nameof(Telemetry.Properties));

		if (time.HasValue)
		{
			Assert.AreEqual(time, telemetry.Time, nameof(Telemetry.Time));
		}
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="AvailabilityTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		AvailabilityTelemetry telemetry,
		TimeSpan duration,
		String? id,
		KeyValuePair<String, Double>[] measurements,
		String? message,
		String? name,
		String? runLocation,
		Boolean success
	)
	{
		Assert.AreEqual(duration, telemetry.Duration, nameof(AvailabilityTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(AvailabilityTelemetry.Id));

		CollectionAssert.AreEquivalent(measurements, telemetry.Measurements, measurementComparer, nameof(AvailabilityTelemetry.Measurements));

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
		String? data,
		TimeSpan duration,
		String? id,
		KeyValuePair<String, Double>[] measurements,
		String? name,
		String? resultCode,
		Boolean success,
		String? target,
		String? type
	)
	{
		Assert.AreEqual(data, telemetry.Data, nameof(DependencyTelemetry.Data));

		Assert.AreEqual(duration, telemetry.Duration, nameof(DependencyTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(DependencyTelemetry.Id));

		CollectionAssert.AreEquivalent(measurements, telemetry.Measurements, measurementComparer, nameof(DependencyTelemetry.Measurements));

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
		KeyValuePair<String, Double>[] measurements,
		String? name
	)
	{
		CollectionAssert.AreEquivalent(measurements, telemetry.Measurements, measurementComparer, nameof(EventTelemetry.Measurements));

		Assert.AreEqual(name, telemetry.Name, nameof(EventTelemetry.Name));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="ExceptionTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		ExceptionTelemetry telemetry,
		Exception exception,
		KeyValuePair<String, Double>[] measurements,
		SeverityLevel? severityLevel
	)
	{
		Assert.AreEqual(exception, telemetry.Exception, nameof(ExceptionTelemetry.Exception));

		CollectionAssert.AreEquivalent(measurements, telemetry.Measurements, measurementComparer, nameof(ExceptionTelemetry.Measurements));

		Assert.AreEqual(severityLevel, telemetry.SeverityLevel, nameof(ExceptionTelemetry.SeverityLevel));
	}

	/// <summary>
	/// Tests whether data within instance of <see cref="MetricTelemetry"/> is equal to the expected values.
	/// </summary>
	public static void PropertiesAreEqual
	(
		MetricTelemetry telemetry,
		String? name,
		String? @namespace,
		Double value,
		MetricValueAggregation? valueAggregation = null
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
		KeyValuePair<String, Double>[] measurements,
		String name,
		String responseCode,
		Boolean success,
		Uri url
	)
	{
		Assert.AreEqual(duration, telemetry.Duration, nameof(RequestTelemetry.Duration));

		Assert.AreEqual(id, telemetry.Id, nameof(RequestTelemetry.Id));

		CollectionAssert.AreEquivalent(measurements, telemetry.Measurements, measurementComparer, nameof(RequestTelemetry.Measurements));

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
