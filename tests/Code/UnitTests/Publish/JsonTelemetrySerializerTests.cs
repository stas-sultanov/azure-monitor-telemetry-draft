﻿// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.UnitTests;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Azure.Monitor.Telemetry.Publish;
using Azure.Monitor.Telemetry.Tests;

/// <summary>
/// Tests for <see cref="JsonTelemetrySerializer"/> class.
/// </summary>
[TestCategory("UnitTests")]
[TestClass]
public class JsonTelemetrySerializerTests
{
	#region Types

	private sealed class UnknownTelemetry(DateTime time) : Telemetry
	{
		public TelemetryOperation Operation { get; set; } = null;
		public PropertyList Properties { get; set; } = null;
		public TagList Tags { get; set; } = null;
		public DateTime Time { get; init; } = time;
	}

	#endregion

	#region Fields

	private static readonly TagList publisherTags =
	[
		new(TelemetryTagKey.InternalSdkVersion, "test"),
	];

	private static readonly TagList trackerTags =
	[
		new(TelemetryTagKey.CloudRole, "TestMachine"),
	];

	private static readonly String instrumentationKey = Guid.NewGuid().ToString();
	private readonly TelemetryFactory telemetryFactory = new();

	#endregion

	#region Methods: Tests

	[TestMethod]
	public void Method_Serialize_AvailabilityTelemetry()
	{
		// arrange
		var expectedName = @"AppAvailabilityResults";
		var expectedType = @"AvailabilityData";

		var telemetry = telemetryFactory.Create_AvailabilityTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var duration = GetDuration(jsonElement);

		var id = GetId(jsonElement);

		var measurements = GetMeasurements(jsonElement);

		var message = GetMessage(jsonElement);

		var name = GetName(jsonElement);

		var runLocation = GetRunLocation(jsonElement);

		var success = GetSuccess(jsonElement);

		AssertHelpers.PropertiesAreEqual(telemetry, duration, id, measurements, message, name, runLocation, success);
	}

	[TestMethod]
	public void Method_Serialize_DependencyTelemetry()
	{
		// arrange
		var expectedName = @"AppDependencies";
		var expectedType = @"RemoteDependencyData";

		var telemetry = telemetryFactory.Create_DependencyTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var data = GetData(jsonElement);

		var duration = GetDuration(jsonElement);

		var id = GetId(jsonElement);

		var measurements = GetMeasurements(jsonElement);

		var name = GetName(jsonElement);

		var resultCode = GetResultCode(jsonElement);

		var success = GetSuccess(jsonElement);

		var target = GetTarget(jsonElement);

		var type = GetType(jsonElement);

		AssertHelpers.PropertiesAreEqual(telemetry, data, duration, id, measurements, name, resultCode, success, target, type);
	}

	[TestMethod]
	public void Method_Serialize_EventTelemetry()
	{
		// arrange
		var expectedName = @"AppEvents";
		var expectedType = @"EventData";

		var telemetry = telemetryFactory.Create_EventTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var measurements = GetMeasurements(jsonElement);

		var name = GetName(jsonElement);

		AssertHelpers.PropertiesAreEqual(telemetry, measurements, name);
	}

	[TestMethod]
	public void Method_Serialize_ExceptionTelemetry()
	{
		// arrange
		var expectedName = @"AppExceptions";
		var expectedType = @"ExceptionData";

		var telemetry = telemetryFactory.Create_ExceptionTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var converter = new JsonStringEnumConverter();

		var options = new JsonSerializerOptions();

		options.Converters.Add(converter);

		DeserializeAndAssert(jsonElement, @"severityLevel", telemetry.SeverityLevel.Value, options);
	}

	[TestMethod]
	public void Method_Serialize_MetricTelemetry()
	{
		// arrange
		var expectedName = @"AppMetrics";
		var expectedType = @"MetricData";

		var aggregation = new MetricValueAggregation()
		{
			Count = 3,
			Min = 1,
			Max = 3
		};
		var telemetry = telemetryFactory.Create_MetricTelemetry_Max("tests", 6, aggregation);

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var metricJsonElement = jsonElement.GetProperty(@"metrics")[0];

		var count = metricJsonElement.GetProperty(@"count").Deserialize<Int32>();

		var max = metricJsonElement.GetProperty(@"max").Deserialize<Double>();

		var min = metricJsonElement.GetProperty(@"min").Deserialize<Double>();

		var name = GetName(metricJsonElement);

		var @namespace = metricJsonElement.GetProperty(@"ns").Deserialize<String>();

		var value = metricJsonElement.GetProperty(@"value").Deserialize<Double>();

		AssertHelpers.PropertiesAreEqual(telemetry, name, @namespace, value, new MetricValueAggregation() { Count = count, Max = max, Min = min });
	}

	[TestMethod]
	public void Method_Serialize_PageViewTelemetry()
	{
		// arrange
		var expectedName = @"AppPageViews";
		var expectedType = @"PageViewData";

		var telemetry = telemetryFactory.Create_PageViewTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"url", telemetry.Url);
	}

	[TestMethod]
	public void Method_Serialize_RequestTelemetry()
	{
		// arrange
		var expectedName = @"AppRequests";
		var expectedType = @"RequestData";

		var telemetry = telemetryFactory.Create_RequestTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);

		DeserializeAndAssert(jsonElement, @"responseCode", telemetry.ResponseCode);

		DeserializeAndAssert(jsonElement, @"url", telemetry.Url);
	}

	[TestMethod]
	public void Method_Serialize_TraceTelemetry()
	{
		// arrange
		var expectedName = @"AppTraces";
		var expectedType = @"MessageData";

		var telemetry = telemetryFactory.Create_TraceTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry, trackerTags, publisherTags);

		// assert
		var expectedTags = trackerTags.Union(publisherTags).Union(telemetry.Tags).ToArray();

		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, expectedTags, expectedType);

		var converter = new JsonStringEnumConverter();

		var options = new JsonSerializerOptions();

		options.Converters.Add(converter);

		DeserializeAndAssert(jsonElement, @"severityLevel", telemetry.SeverityLevel, options);
	}

	[TestMethod]
	public void Method_Serialize_UnknownTelemetry()
	{
		// arrange
		var instrumentationKey = Guid.NewGuid().ToString();
		var telemetry = new UnknownTelemetry(DateTime.UtcNow);

		// act
		var memoryStream = new MemoryStream();

		using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 32768, true))
		{
			JsonTelemetrySerializer.Serialize(streamWriter, instrumentationKey, telemetry, null, null);
		}

		// assert
		Assert.AreEqual(3, memoryStream.Position);
	}

	#endregion

	#region Methods: Helpers

	private static JsonElement SerializeAndDeserialize
	(
		String instrumentationKey,
		Telemetry telemetry,
		TagList trackerTags,
		TagList publisherTags
	)
	{
		var memoryStream = new MemoryStream();

		using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 32768, true))
		{
			JsonTelemetrySerializer.Serialize(streamWriter, instrumentationKey, telemetry, trackerTags, publisherTags);
		}

		memoryStream.Position = 0;

		using var streamReader = new StreamReader(memoryStream);

		var streamAsString = streamReader.ReadToEnd();

		var result = JsonSerializer.Deserialize<JsonDocument>(streamAsString);

		return result.RootElement;
	}

	private static JsonElement DeserializeAndAssertBase
	(
		JsonElement jsonElement,
		String expectedName,
		DateTime expectedTime,
		String expectedInstrumentationKey,
		TagList expectedTags,
		String expectedBaseType
	)
	{
		DeserializeAndAssert(jsonElement, "name", expectedName);

		DeserializeAndAssert(jsonElement, "time", expectedTime);

		DeserializeAndAssert(jsonElement, "iKey", expectedInstrumentationKey);

		var actualTags = jsonElement.GetProperty(@"tags").Deserialize<Dictionary<String, String>>();

		foreach (var expectedTag in expectedTags)
		{
			if (!actualTags.TryGetValue(expectedTag.Key, out var actualValue))
			{
				Assert.Fail("tags does not contain key: {0}", expectedTag.Key);
			}

			Assert.AreEqual(expectedTag.Value, actualValue, $"tag {expectedTag.Key} value");
		}

		var childElement = jsonElement.GetProperty(@"data");

		DeserializeAndAssert(childElement, "baseType", expectedBaseType);

		var dataElement = childElement.GetProperty(@"baseData");

		return dataElement;
	}

	private static void DeserializeAndAssert<ElementType>
	(
		JsonElement jsonElement,
		String propertyName,
		ElementType expectedValue,
		JsonSerializerOptions options = null
	)
	{
		var actualValue = jsonElement.GetProperty(propertyName).Deserialize<ElementType>(options);

		Assert.AreEqual(expectedValue, actualValue, propertyName);
	}

	private static String GetData(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"data").Deserialize<String>();
	}

	private static TimeSpan GetDuration(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"duration").Deserialize<TimeSpan>();
	}

	private static String GetId(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"id").Deserialize<String>();
	}

	private static String GetMessage(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"message").Deserialize<String>();
	}

	private static MeasurementList GetMeasurements(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"measurements").Deserialize<Dictionary<String, Double>>().ToList();
	}

	private static String GetName(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"name").Deserialize<String>();
	}

	private static String GetResultCode(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"resultCode").Deserialize<String>();
	}

	private static String GetRunLocation(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"runLocation").Deserialize<String>();
	}

	private static Boolean GetSuccess(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"success").Deserialize<Boolean>();
	}

	private static String GetTarget(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"target").Deserialize<String>();
	}

	private static String GetType(JsonElement jsonElement)
	{
		return jsonElement.GetProperty(@"type").Deserialize<String>();
	}

	#endregion
}