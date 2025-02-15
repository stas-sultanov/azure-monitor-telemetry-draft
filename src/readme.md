# Azure Monitor Telemetry

Azure Monitor Telemetry is a .NET library for collecting and publishing telemetry data to Azure Monitor service.
This library helps you monitor the performance and usage of your applications by tracking dependencies, exceptions, requests, and traces.

## Getting Started

### Installation

To install the Azure Monitor Telemetry library, use the following command:

```sh
dotnet add package Stas.Azure.Monitor.Telemetry
```

### Get an Ingestion Endpoint and an Instrumentation Key

To use the library you will need to provide it with an Ingestion Endpoint and an Instrumentation Key which can be obtained from the properties of the Azure resource of [Microsoft.Insights/components]() type aka Application Insights. 

### Initialize a TelemetryTracker

The `TelemetryTracker` is the primary root object for the library. Almost all functionality around telemetry sending is located on this object.
You must initialize an instance of this object and populate it with Ingestion Endpoint Uri and Instrumentation Key to identify your application.

#### Without Entra based Authentication

```C#
using Azure.Monitor.Telemetry;
using Azure.Monitor.Telemetry.Senders.HttpV2;

// create HTTP Client for telemetry sender
using var telemetrySenderHttpClient = new HttpClient();

// create telemetry sender
var httpTelemetrySender = new HttpTelemetrySender
(
	telemetrySenderHttpClient,
	new Uri("INSERT INGESTION ENDPOINT HERE"),
	new Guid("INSERT INSTRUMENTATION KEY HERE")
);

// create telemetry tracker
var telemetryTracker = new TelemetryTracker(telemetrySenders: httpTelemetrySender);

```

#### With Entra based Authentication

```C#
using Azure.Monitor.Telemetry;
using Azure.Monitor.Telemetry.Senders.HttpV2;

// create HTTP Client for telemetry sender
using var telemetrySenderHttpClient = new HttpClient();

// create token source
var tokenCredential = new DefaultAzureCredential();

// create telemetry sender
var httpTelemetrySender = new HttpTelemetrySender
(
	telemetrySenderHttpClient,
	new Uri("INSERT INGESTION ENDPOINT HERE"),
	new Guid("INSERT INSTRUMENTATION KEY HERE"),
	async (cancellationToken) =>
	{
		var tokenRequestContext = new TokenRequestContext(HttpTelemetrySender.AuthorizationScopes);

		var token = await tokenCredential.GetTokenAsync(tokenRequestContext, cancellationToken);

		return new BearerToken(token.Token, token.ExpiresOn);
	}
);

// create telemetry tracker
var telemetryTracker = new TelemetryTracker(tags, httpTelemetrySender);

```

### Use the TelemetryTracker to track telemetry

This library does not provide any automatic telemetry collection or any automatic meta-data properties.
You can populate common context by using `tags` argument of the `TelemetryTracker` constructor which will be automatically attached to each telemetry item sent. You can also attach additional property data to each telemetry item sent by using `Telemetry.Tags` property. The `TelemetryClient` exposes a method Add that adds telemetry information into the processing queue.

```C#
``` 

### Ensure you don't lose telemetry

This library makes use instance of `ConcurrentQueue` to collect and send telemetry data.
As a result, if the process is terminated suddenly, you could lose telemetry that is stored in the queue.
It is recommended to track the closing of your process and call the `TelemetryTracker.PublishAsync()` method to ensure no telemetry is lost.
