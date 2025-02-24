﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry;

using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

/// <summary>
/// Providing functionality to collect and publish telemetry items.
/// </summary>
/// <remarks>
/// Provides thread-safe collection of telemetry items and supports batch publishing through configured telemetry publishers.
/// Allows specifying common tags that will be attached to each telemetry item during publish operation.
/// </remarks>
public sealed class TelemetryTracker
{
	#region Fields

	private static readonly TelemetryPublishResult[] emptySuccess = [];
	private readonly ConcurrentQueue<Telemetry> items;
	private readonly KeyValuePair<String, String>[]? tags;
	private readonly AsyncLocal<OperationContext> operation;
	private readonly OperationContext rootOperation;
	private readonly TelemetryPublisher[] telemetryPublishers;

	#endregion

	#region Constructor

	/// <summary>
	/// Initializes a new instance of the <see cref="TelemetryTracker"/> class.
	/// </summary>
	/// <param name="telemetryPublisher">A telemetry publisher to publish the telemetry data.</param>
	/// <param name="rootOperation">Root destributed operation.</param>
	/// <param name="tags">An array of tags to attach to each telemetry item. Is optional.</param>
	/// <exception cref="ArgumentNullException">If <paramref name="telemetryPublisher"/> is null.</exception>
	/// <exception cref="ArgumentException">If <paramref name="tags"/> contains an item which key or value is null or whitespace.</exception>
	public TelemetryTracker
	(
		TelemetryPublisher telemetryPublisher,
		OperationContext? rootOperation = null,
		KeyValuePair<String, String>[]? tags = null
	)
		: this([telemetryPublisher], rootOperation, tags)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TelemetryTracker"/> class.
	/// </summary>
	/// <param name="telemetryPublishers">An array of telemetry publishers to publish the telemetry data.</param>
	/// <param name="rootOperation">Root destributed operation.</param>
	/// <param name="tags">An array of tags to attach to each telemetry item. Is optional.</param>
	public TelemetryTracker
	(
		TelemetryPublisher[] telemetryPublishers,
		OperationContext? rootOperation = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		this.telemetryPublishers = telemetryPublishers; 

		this.rootOperation = rootOperation ?? new OperationContext();

		this.tags = tags;

		items = new();

		operation = new()
		{
			Value = this.rootOperation
		};
	}

	#endregion

	#region Properties

	/// <summary>
	/// An asynchronous local storage for the current telemetry operation.
	/// </summary>
	public OperationContext Operation
	{
		get => operation.Value ?? rootOperation;

		private set => operation.Value = value;
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds a telemetry item to the tracking queue.
	/// </summary>
	/// <param name="telemetry">The telemetry item to add.</param>
	public void Add(Telemetry telemetry)
	{
		items.Enqueue(telemetry);
	}

	/// <summary>
	/// Publishes all telemetry items in the queue using all configured telemetry publishers.
	/// </summary>
	/// <param name="cancellationToken">Optional token to cancel the operation.</param>
	/// <returns>An array of <see cref="TelemetryPublishResult"/> indicating the status for each configured publisher.</returns>
	/// <remarks>
	/// If the queue is empty, returns an empty success result array.
	/// The method processes all items in the queue and publishes them in parallel to all configured publishers.
	/// </remarks>
	public async Task<TelemetryPublishResult[]> PublishAsync
	(
		CancellationToken cancellationToken = default
	)
	{
		// check if there are any items to publish
		if (items.IsEmpty)
		{
			return emptySuccess;
		}

		// create a list to store telemetry items
		// List.Capacity do not use ConcurrentQueue.Count because it takes time to calculate and it may change 
		var telemetryList = new List<Telemetry>();

		// dequeue all items from the queue
		while (items.TryDequeue(out var telemetry))
		{
			telemetryList.Add(telemetry);
		}

		// create a list to store publish results
		var resultList = new Task<TelemetryPublishResult>[telemetryPublishers.Length];

		// publish telemetry items to all configured senders
		for (var publisherIndex = 0; publisherIndex < telemetryPublishers.Length; publisherIndex++)
		{
			var sender = telemetryPublishers[publisherIndex];

			resultList[publisherIndex] = sender.PublishAsync(telemetryList, tags, cancellationToken);
		}

		// wait for all publishers to complete
		var result = await Task.WhenAll(resultList);

		return result;
	}

	#endregion

	#region Methods: Track

	/// <summary>
	/// Tracks availability by creating an instance of <see cref="AvailabilityTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="time">The time when the dependency call was initiated.</param>
	/// <param name="id">The unique identifier for the dependency call.</param>
	/// <param name="name">The name of the telemetry instance.</param>
	/// <param name="message">The message associated with the telemetry instance.</param>
	/// <param name="duration">The duration of the dependency call.</param>
	/// <param name="success">A boolean indicating whether the dependency call was successful.</param>
	/// <param name="runLocation">The location where the telemetry was run. This parameter is optional.</param>
	/// <param name="measurements">The measurements associated with the telemetry. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackAvailability
	(
		DateTime time,
		String id,
		String name,
		String message,
		TimeSpan duration,
		Boolean success,
		String? runLocation = null,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var telemetry = new AvailabilityTelemetry(Operation, time, id, name, message)
		{
			Duration = duration,
			Measurements = measurements,
			Properties = properties,
			RunLocation = runLocation,
			Success = success,
			Tags = tags
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks dependency by creating an instance of <see cref="DependencyTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="time">The time when the dependency call was initiated.</param>
	/// <param name="id">The unique identifier for the dependency call.</param>
	/// <param name="httpMethod">The HTTP method used for the dependency call.</param>
	/// <param name="uri">The URI of the dependency call.</param>
	/// <param name="statusCode">The HTTP status code returned by the dependency call.</param>
	/// <param name="duration">The duration of the dependency call.</param>
	/// <param name="measurements">The measurements associated with the telemetry. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackDependency
	(
		DateTime time,
		String id,
		HttpMethod httpMethod,
		Uri uri,
		HttpStatusCode statusCode,
		TimeSpan duration,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var name = String.Concat(httpMethod.Method, " ", uri.AbsolutePath);

		var success = (Int32)statusCode is >= 200 and < 300;

		var telemetry = new DependencyTelemetry(Operation, time, id, name)
		{
			Duration = duration,
			Measurements = measurements,
			Properties = properties,
			ResultCode = statusCode.ToString(),
			Success = success,
			Tags = tags,
			Target = uri.Host,
			Type = DependencyType.DetectTypeFromHttp(uri),
			Data = uri.ToString()
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks InProc dependency by creating an instance of <see cref="DependencyTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="time">The time at which the dependency call was made.</param>
	/// <param name="id">The unique identifier for the dependency call.</param>
	/// <param name="name">The name of the dependency.</param>
	/// <param name="success">A boolean indicating whether the dependency call was successful.</param>
	/// <param name="duration">The duration of the dependency call.</param>
	/// <param name="typeName">The type name of the dependency. Defaults to null.</param>
	/// <param name="measurements">A list of measurements associated with the dependency call. Defaults to null.</param>
	/// <param name="properties">A list of properties associated with the dependency call. Defaults to null.</param>
	/// <param name="tags">A list of tags associated with the dependency call. Defaults to null.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackDependencyInProc
	(
		DateTime time,
		String id,
		String name,
		Boolean success,
		TimeSpan duration,
		String? typeName = null,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var type = String.IsNullOrWhiteSpace(typeName) ? DependencyType.InProc : DependencyType.InProc + " | " + typeName;

		var telemetry = new DependencyTelemetry(Operation, time, id, name)
		{
			Duration = duration,
			Measurements = measurements,
			Properties = properties,
			Success = success,
			Tags = tags,
			Type = type
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks event by creating an instance of <see cref="EventTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="name">The name of the event to be tracked.</param>
	/// <param name="measurements">The measurements associated with the telemetry. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackEvent
	(
		String name,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var time = DateTime.UtcNow;

		var telemetry = new EventTelemetry(Operation, time, name)
		{
			Measurements = measurements,
			Properties = properties,
			Tags = tags
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks exception by creating an instance of <see cref="ExceptionTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="exception">The exception to be tracked.</param>
	/// <param name="measurements">The measurements associated with the telemetry. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="severityLevel">The severity level of the exception. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackException
	(
		Exception exception,
		SeverityLevel? severityLevel = null,
		KeyValuePair<String, Double>[]? measurements = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var time = DateTime.UtcNow;

		var telemetry = new ExceptionTelemetry(Operation, time, exception)
		{
			Measurements = measurements,
			Properties = properties,
			SeverityLevel = severityLevel,
			Tags = tags
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks metric by creating an instance of <see cref="MetricTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="namespace">The namespace of the metric to be tracked.</param>
	/// <param name="name">The name of the metric to be tracked.</param>
	/// <param name="value">The value of the metric to be tracked.</param>
	/// <param name="valueAggregation">The aggregation type of the metric. This parameter is optional.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackMetric
	(
		String @namespace,
		String name,
		Double value,
		MetricValueAggregation? valueAggregation = null,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var time = DateTime.UtcNow;

		var telemetry = new MetricTelemetry(Operation, time, @namespace, name, value, valueAggregation)
		{
			Properties = properties,
			Tags = tags
		};

		Add(telemetry);
	}

	/// <summary>
	/// Tracks trace by creating an instance of <see cref="TraceTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OperationContext TrackOperationBegin
	(
		String id,
		String? name = null,
		String? parentId = null,
		String? syntheticSource = null
	)
	{
		// save current operation
		var result = Operation;

		// replace with new
		Operation = new OperationContext(id, name, parentId, syntheticSource);

		return result;
	}

	/// <summary>
	/// Tracks trace by creating an instance of <see cref="TraceTelemetry"/> and calling the <see cref="Add(Telemetry)"/> method.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="severityLevel">The severity level.</param>
	/// <param name="properties">The properties associated with the telemetry. This parameter is optional.</param>
	/// <param name="tags">The tags associated with the telemetry. This parameter is optional.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void TrackTrace
	(
		String message,
		SeverityLevel severityLevel,
		KeyValuePair<String, String>[]? properties = null,
		KeyValuePair<String, String>[]? tags = null
	)
	{
		var time = DateTime.UtcNow;

		var telemetry = new TraceTelemetry(Operation, time, message, severityLevel)
		{
			Properties = properties,
			Tags = tags
		};

		Add(telemetry);
	}

	#endregion
}
