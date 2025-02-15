// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

public sealed class MetricValueAggregation
{
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
