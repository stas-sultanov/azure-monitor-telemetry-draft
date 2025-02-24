﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Publish;

using System;
using System.Runtime.CompilerServices;

using Azure.Monitor.Telemetry;

/// <summary>
/// Provides extension methods.
/// </summary>
public static class Extensions
{
	#region Methods

	/// <summary>
	/// Tracks an instance of <see cref="HttpTelemetryPublishResult"/> as dependency telemetry.
	/// </summary>
	/// <param name="telemetryTracker">The telemetry tracker instance.</param>
	/// <param name="id">The unique identifier for the decency call.</param>
	/// <param name="publishResult">The result of the publish operation.</param>
	/// <param name="measurements">The measurements associated with the telemetry. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void TrackDependency
	(
		this TelemetryTracker telemetryTracker,
		String id,
		HttpTelemetryPublishResult publishResult,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		List<KeyValuePair<String, Double>> adjustedMeasurements = measurements == null
			? new()
			: new(measurements);

		adjustedMeasurements.Add(new KeyValuePair<String, Double>(nameof(HttpTelemetryPublishResult.Count), publishResult.Count));

		var name = String.Concat("POST ", publishResult.Url.AbsolutePath);

		var telemetry = new DependencyTelemetry(telemetryTracker.Operation, publishResult.Time, id, name)
		{
			Data = publishResult.Url.ToString(),
			Duration = publishResult.Duration,
			Measurements = [.. adjustedMeasurements],
			Properties = properties,
			ResultCode = publishResult.StatusCode.ToString(),
			Success = publishResult.Success,
			Target = publishResult.Url.Host,
			Tags = tags,
			Type = DependencyType.AzureMonitor
		};

		telemetryTracker.Add(telemetry);
	}

	#endregion
}
