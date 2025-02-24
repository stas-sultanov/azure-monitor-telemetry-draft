﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

/// <summary>
/// Represents printf-style trace statements that are text searched.
/// </summary>
/// <param name="operation">The distributed operation context.</param>
/// <param name="time">The UTC timestamp when the trace has occurred.</param>
/// <param name="message">The message.</param>
/// <param name="severityLevel">The severity level.</param>
public sealed class TraceTelemetry
(
	OperationContext operation,
	DateTime time,
	String message,
	SeverityLevel severityLevel
)
	: Telemetry
{
	#region Properties

	/// <summary>
	/// The message.
	/// </summary>
	/// <remarks>Maximum length: 32768 characters.</remarks>
	public String Message { get; } = message;

	/// <inheritdoc/>
	public OperationContext Operation { get; } = operation;

	/// <inheritdoc/>
	public KeyValuePair<String, String>[]? Properties { get; init; }

	/// <summary>
	/// The severity level.
	/// </summary>
	public SeverityLevel SeverityLevel { get; } = severityLevel;

	/// <inheritdoc/>
	public KeyValuePair<String, String>[]? Tags { get; init; }

	/// <summary>
	/// The UTC timestamp when the trace has occurred.
	/// </summary>
	public DateTime Time { get; } = time;

	#endregion
}
