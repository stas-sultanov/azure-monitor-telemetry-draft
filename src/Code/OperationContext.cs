// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

/// <summary>
/// Represents a distributed operation containing information about operation hierarchy and synthetic sources.
/// </summary>
/// <remarks>
/// This type is used to track and correlate telemetry data across different operations and their relationships.
/// </remarks>
/// <param name="id">The topmost operation identifier.</param>
/// <param name="name">The topmost operation name.</param>
/// <param name="parentId">The parent operation identifier.</param>
/// <param name="syntheticSource">The synthetic source.</param>
public sealed class OperationContext
(
	String? id = null,
	String? name = null,
	String? parentId = null,
	String? syntheticSource = null
)
{
	#region Properties

	/// <summary>The topmost operation identifier.</summary>
	public String? Id { get; } = id;

	/// <summary>The topmost operation name.</summary>
	public String? Name { get; } = name;

	/// <summary>The parent operation identifier.</summary>
	public String? ParentId { get; } = parentId;

	/// <summary>The synthetic source.</summary>
	public String? SyntheticSource { get; } = syntheticSource;

	#endregion
}
