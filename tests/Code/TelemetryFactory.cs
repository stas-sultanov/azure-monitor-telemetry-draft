﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.Tests;

using System;
using System.Diagnostics;
using System.Globalization;

using Azure.Monitor.Telemetry.Types;

/// <summary>
/// Provides a set of method to create types that implement <see cref="Telemetry"/> for test purposes.
/// </summary>
internal sealed class TelemetryFactory
{
	#region Properties

	public KeyValuePair<String, Double>[] Measurements { get; set; }
	public TelemetryOperation Operation { get; }
	public KeyValuePair<String, String>[] Properties { get; set; }
	public KeyValuePair<String, String>[] Tags { get; set; }

	#endregion

	#region Constructors

	internal TelemetryFactory
	(
		String operationName
	)
	{
		Measurements = [new("m", 0), new("n", 1.5)];

		Operation = new TelemetryOperation
		{
			Id = GetOperationId(),
			Name = operationName
		};

		Properties = [new("key", "value")];

		Tags = [new(TelemetryTagKey.CloudRole, Environment.MachineName), new(TelemetryTagKey.CloudRoleInstance, Environment.ProcessId.ToString(CultureInfo.InvariantCulture))];
	}

	#endregion

	#region Methods: Helpers

	internal static String GetOperationId()
	{
		return ActivityTraceId.CreateRandom().ToString();
	}

	internal static String GetActivityId()
	{
		return ActivitySpanId.CreateRandom().ToString();
	}

	internal static TimeSpan GetRandomDuration
	(
		Int32 millisecondsMin,
		Int32 millisecondsMax
	)
	{
		var milliseconds = Random.Shared.Next(millisecondsMin, millisecondsMax);

		return TimeSpan.FromMilliseconds(milliseconds);
	}

	internal static void Simulate_ExceptionThrow
	(
		String? param1
	)
	{
		throw new ArgumentNullException(nameof(param1), "L1");
	}

	internal static void Simulate_ExceptionThrow_WithInnerException
	(
		String? paramL2
	)
	{
		try
		{
			Simulate_ExceptionThrow(paramL2);
		}
		catch (Exception exception)
		{
			throw new Exception("L2", exception);
		}
	}

	#endregion

	#region Methods: Create

	/// <summary>
	/// Creates instance of <see cref="AvailabilityTelemetry"/> with full load.
	/// </summary>
	public AvailabilityTelemetry Create_AvailabilityTelemetry_Max
	(
		String name,
		String message = @"Passed"
	)
	{
		var id = GetActivityId();

		var duration = GetRandomDuration(100, 2000);

		var result = new AvailabilityTelemetry
		{
			Duration = duration,
			Id = id,
			Measurements = Measurements,
			Message = message,
			Name = name,
			Operation = Operation,
			Properties = Properties,
			RunLocation = "Earth",
			Success = true,
			Tags = Tags,
			Time = DateTime.UtcNow
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="AvailabilityTelemetry"/> with minimum load.
	/// </summary>
	public AvailabilityTelemetry Create_AvailabilityTelemetry_Min
	(
		String name,
		String message = @"Passed"
	)
	{
		var id = GetActivityId();

		var result = new AvailabilityTelemetry
		{
			Duration = TimeSpan.Zero,
			Id = id,
			Message = message,
			Name = name,
			Operation = Operation,
			Success = true,
			Time = DateTime.UtcNow
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="DependencyTelemetry"/> with full load.
	/// </summary>
	public DependencyTelemetry Create_DependencyTelemetry_Max
	(
		String name,
		Uri url
	)
	{
		var id = GetActivityId();

		var type = url.DetectDependencyTypeFromHttp();

		var duration = GetRandomDuration(200, 800);

		var result = new DependencyTelemetry
		{
			Data = "data",
			Duration = duration,
			Id = id,
			Measurements = Measurements,
			Name = name,
			Operation = Operation,
			Properties = Properties,
			ResultCode = "401",
			Success = false,
			Tags = Tags,
			Target = "target",
			Time = DateTime.UtcNow,
			Type = type
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="DependencyTelemetry"/> with minimum load.
	/// </summary>
	public DependencyTelemetry Create_DependencyTelemetry_Min
	(
		String name
	)
	{
		var id = GetActivityId();

		var result = new DependencyTelemetry
		{
			Operation = Operation,
			Time = DateTime.UtcNow,
			Id = id,
			Name = name
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="EventTelemetry"/> with full load.
	/// </summary>
	public EventTelemetry Create_EventTelemetry_Max
	(
		String name
	)
	{
		var result = new EventTelemetry
		{
			Measurements = Measurements,
			Name = name,
			Operation = Operation,
			Properties = Properties,
			Tags = Tags,
			Time = DateTime.UtcNow
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="EventTelemetry"/> with minimum load.
	/// </summary>
	public EventTelemetry Create_EventTelemetry_Min
	(
		String name
	)
	{
		var result = new EventTelemetry
		{
			Name = name,
			Operation = Operation,
			Time = DateTime.UtcNow,
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="ExceptionTelemetry"/> with full load.
	/// </summary>
	public ExceptionTelemetry Create_ExceptionTelemetry_Max()
	{
		try
		{
			Simulate_ExceptionThrow_WithInnerException(null);

			throw new Exception();
		}
		catch (Exception exception)
		{
			var exceptions = exception.Convert();

			var result = new ExceptionTelemetry
			{
				Exceptions = exceptions,
				Measurements = Measurements,
				Operation = Operation,
				Properties = Properties,
				SeverityLevel = SeverityLevel.Critical,
				Tags = Tags,
				Time = DateTime.UtcNow
			};

			return result;
		}
	}

	/// <summary>
	/// Creates instance of <see cref="ExceptionTelemetry"/> with minimum load.
	/// </summary>
	public ExceptionTelemetry Create_ExceptionTelemetry_Min()
	{
		try
		{
			Simulate_ExceptionThrow(null);

			throw new Exception();
		}
		catch (Exception exception)
		{
			var exceptions = exception.Convert();

			var result = new ExceptionTelemetry
			{
				Exceptions = exceptions,
				Operation = Operation,
				Time = DateTime.UtcNow
			};

			return result;
		}
	}

	/// <summary>
	/// Creates instance of <see cref="MetricTelemetry"/> with full load.
	/// </summary>
	public MetricTelemetry Create_MetricTelemetry_Max
	(
		String @namespace,
		String name,
		Double value,
		MetricValueAggregation aggregation
	)
	{
		var result = new MetricTelemetry
		{
			Operation = Operation,
			Time = DateTime.UtcNow,
			Namespace = @namespace,
			Name = name,
			Value = value,
			ValueAggregation = aggregation,
			Properties = Properties,
			Tags = Tags
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="MetricTelemetry"/> with minimum load.
	/// </summary>
	public MetricTelemetry Create_MetricTelemetry_Min
	(
		String @namespace,
		String name,
		Double value
	)
	{
		var result = new MetricTelemetry
		{
			Name = name,
			Namespace = @namespace,
			Operation = Operation,
			Time = DateTime.UtcNow,
			Value = value
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="PageViewTelemetry"/> with full load.
	/// </summary>
	public PageViewTelemetry Create_PageViewTelemetry_Max
	(
		String name,
		Uri url
	)
	{
		var id = GetActivityId();

		var duration = GetRandomDuration(300, 700);

		var result = new PageViewTelemetry
		{
			Measurements = Measurements,
			Duration = duration,
			Id = id,
			Name = name,
			Operation = Operation,
			Properties = Properties,
			Tags = Tags,
			Time = DateTime.UtcNow,
			Url = url
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="PageViewTelemetry"/> with minimum load.
	/// </summary>
	public PageViewTelemetry Create_PageViewTelemetry_Min
	(
		String name
	)
	{
		var id = GetActivityId();

		var result = new PageViewTelemetry
		{
			Id = id,
			Name = name,
			Operation = Operation,
			Time = DateTime.UtcNow
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="RequestTelemetry"/> with full load.
	/// </summary>
	public RequestTelemetry Create_RequestTelemetry_Max
	(
		String name,
		Uri url
	)
	{
		var id = GetActivityId();

		var result = new RequestTelemetry
		{
			Duration = TimeSpan.FromSeconds(1),
			Id = id,
			Measurements = Measurements,
			Name = name,
			Operation = Operation,
			Properties = Properties,
			ResponseCode = "200",
			Success = true,
			Tags = Tags,
			Time = DateTime.UtcNow,
			Url = url
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="RequestTelemetry"/> with minimum load.
	/// </summary>
	public RequestTelemetry Create_RequestTelemetry_Min(Uri url)
	{
		var id = GetActivityId();

		var result = new RequestTelemetry
		{
			Operation = Operation,
			Time = DateTime.UtcNow,
			Id = id,
			Url = url,
			ResponseCode = "1"
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="TraceTelemetry"/> with full load.
	/// </summary>
	public TraceTelemetry Create_TraceTelemetry_Max(String message)
	{
		var result = new TraceTelemetry
		{
			Message = message,
			Operation = Operation,
			Properties = Properties,
			SeverityLevel = SeverityLevel.Information,
			Tags = Tags,
			Time = DateTime.UtcNow
		};

		return result;
	}

	/// <summary>
	/// Creates instance of <see cref="TraceTelemetry"/> with minimum load.
	/// </summary>
	public TraceTelemetry Create_TraceTelemetry_Min(String message)
	{
		var result = new TraceTelemetry
		{
			Message = message,
			Operation = Operation,
			SeverityLevel = SeverityLevel.Verbose,
			Time = DateTime.UtcNow
		};

		return result;
	}

	#endregion
}
