﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

/// <summary>
/// Represents the aggregation of metric values within a sample set.
/// </summary>
public sealed class MetricValueAggregation
{
	/// <summary>
	/// The number of values in the sample set
	/// </summary>
	public Int32 Count { get; set; }

	/// <summary>
	/// The min value of the metric across the sample set.
	/// </summary>
	public Double Min { get; set; }

	/// <summary>
	/// The max value of the metric across the sample set.
	/// </summary>
	public Double Max { get; set; }
}
