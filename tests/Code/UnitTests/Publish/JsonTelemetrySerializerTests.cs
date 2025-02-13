// Created by Stas Sultanov.
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
		public IReadOnlyList<KeyValuePair<String, String>> Properties { get; set; } = null;
		public IReadOnlyList<KeyValuePair<String, String>> Tags { get; set; } = null;
		public DateTime Time { get; init; } = time;
	}

	#endregion

	private static readonly String instrumentationKey = Guid.NewGuid().ToString();
	private readonly TelemetryFactory telemetryFactory = new();

	#region Methods: tests

	[TestMethod]
	public void Method_Serialize_AvailabilityTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppAvailabilityResults";
		var expectedType = @"AvailabilityData";

		var telemetry = telemetryFactory.Create_AvailabilityTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);

		DeserializeAndAssert(jsonElement, @"runLocation", telemetry.RunLocation);

		DeserializeAndAssert(jsonElement, @"message", telemetry.Message);
	}

	[TestMethod]
	public void Method_Serialize_AvailabilityTelemetry_Min()
	{
		// arrange
		var expectedName = @"AppAvailabilityResults";
		var expectedType = @"AvailabilityData";

		var telemetry = telemetryFactory.Create_AvailabilityTelemetry_Min();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);

		DeserializeAndAssert(jsonElement, @"message", telemetry.Message);
	}

	[TestMethod]
	public void Method_Serialize_DependencyTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppDependencies";
		var expectedType = @"RemoteDependencyData";

		var telemetry = telemetryFactory.Create_DependencyTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"data", telemetry.Data);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);

		DeserializeAndAssert(jsonElement, @"resultCode", telemetry.ResultCode);

		DeserializeAndAssert(jsonElement, @"type", telemetry.Type);

		DeserializeAndAssert(jsonElement, @"target", telemetry.Target);
	}

	[TestMethod]
	public void Method_Serialize_DependencyTelemetry_Min()
	{
		// arrange
		var expectedName = @"AppDependencies";
		var expectedType = @"RemoteDependencyData";

		var telemetry = telemetryFactory.Create_DependencyTelemetry_Min();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);
	}

	[TestMethod]
	public void Method_Serialize_EventTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppEvents";
		var expectedType = @"EventData";

		var telemetry = telemetryFactory.Create_EventTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		var converter = new JsonStringEnumConverter();

		var options = new JsonSerializerOptions();

		options.Converters.Add(converter);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name, options);
	}

	[TestMethod]
	public void Method_Serialize_ExceptionTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppExceptions";
		var expectedType = @"ExceptionData";

		var telemetry = telemetryFactory.Create_ExceptionTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		var converter = new JsonStringEnumConverter();

		var options = new JsonSerializerOptions();

		options.Converters.Add(converter);

		DeserializeAndAssert(jsonElement, @"severityLevel", telemetry.SeverityLevel.Value, options);
	}

	[TestMethod]
	public void Method_Serialize_MetricTelemetry_Max()
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
		telemetryFactory.Value = 6;
		var telemetry = telemetryFactory.Create_MetricTelemetry_Max(aggregation);

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		var metricJsonElements = jsonElement.GetProperty("metrics").Deserialize<JsonElement[]>();

		var metricJsonElement = metricJsonElements[0];

		DeserializeAndAssert(metricJsonElement, @"count", telemetry.ValueAggregation.Count);

		DeserializeAndAssert(metricJsonElement, @"max", telemetry.ValueAggregation.Max);

		DeserializeAndAssert(metricJsonElement, @"min", telemetry.ValueAggregation.Min);

		DeserializeAndAssert(metricJsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(metricJsonElement, @"value", telemetry.Value);
	}

	[TestMethod]
	public void Method_Serialize_PageViewTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppPageViews";
		var expectedType = @"PageViewData";

		var telemetry = telemetryFactory.Create_PageViewTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"url", telemetry.Url);
	}

	[TestMethod]
	public void Method_Serialize_RequestTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppRequests";
		var expectedType = @"RequestData";

		var telemetry = telemetryFactory.Create_RequestTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

		DeserializeAndAssert(jsonElement, @"id", telemetry.Id);

		DeserializeAndAssert(jsonElement, @"name", telemetry.Name);

		DeserializeAndAssert(jsonElement, @"duration", telemetry.Duration);

		DeserializeAndAssert(jsonElement, @"success", telemetry.Success);

		DeserializeAndAssert(jsonElement, @"responseCode", telemetry.ResponseCode);

		DeserializeAndAssert(jsonElement, @"url", telemetry.Url);
	}

	[TestMethod]
	public void Method_Serialize_TraceTelemetry_Max()
	{
		// arrange
		var expectedName = @"AppTraces";
		var expectedType = @"MessageData";

		var telemetry = telemetryFactory.Create_TraceTelemetry_Max();

		// act
		var rootElement = SerializeAndDeserialize(instrumentationKey, telemetry);

		// assert
		var jsonElement = DeserializeAndAssertBase(rootElement, expectedName, telemetry.Time, instrumentationKey, [], expectedType);

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

	#region Methods: helpers

	private static JsonElement SerializeAndDeserialize
	(
		String instrumentationKey,
		Telemetry telemetry,
		TagList trackerTags = null,
		TagList publisherTags = null
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
		KeyValuePair<String, String>[] expectedTags,
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

	#endregion
}