﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

/// <summary>
/// Represents telemetry of an event that occurred in an application.
/// </summary>
/// <remarks>
/// Typically, it is a user interaction such as a button click or an order checkout.
/// It can also be an application lifecycle event like initialization or a configuration update.
/// Semantically, events might or might not be correlated to requests.
/// If used properly, event telemetry is more important than requests or traces.
/// </remarks>
/// <param name="time">The UTC timestamp when the event has occurred.</param>
/// <param name="name">The name.</param>
public sealed class EventTelemetry
(
	DateTime time,
	String name
)
	: Telemetry
{
	#region Properties

	/// <summary>
	/// A collection of measurements.
	/// </summary>
	/// <remarks>
	/// Maximum key length: 150 characters.
	/// Is null by default.
	/// </remarks>
	public MeasurementList Measurements { get; init; } = null;

	/// <summary>
	/// The name.
	/// </summary>
	/// <remarks>Maximum length: 512 characters.</remarks>
	public String Name { get; } = name;

	/// <inheritdoc/>
	public TelemetryOperation Operation { get; init; } = null;

	/// <inheritdoc/>
	public PropertyList Properties { get; init; } = null;

	/// <inheritdoc/>
	public TagList Tags { get; init; } = null;

	/// <summary>
	/// The UTC timestamp when the event has occurred.
	/// </summary>
	public DateTime Time { get; } = time;

	#endregion
}
