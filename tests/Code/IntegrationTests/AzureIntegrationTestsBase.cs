// Created by Stas Sultanov.
// Copyright © Stas Sultanov.

namespace Azure.Monitor.Telemetry.IntegrationTests;

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;

using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Telemetry;
using Azure.Monitor.Telemetry.Publish;

using Microsoft.VisualStudio.TestTools.UnitTesting;

public abstract class AzureIntegrationTestsBase : IDisposable
{
	#region Fields

	private static readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly HttpClient httpClient;

	#endregion

	#region Properties

	protected TestContext TestContext { get; }

	protected TelemetryTracker TelemetryTracker { get; }

	protected DefaultAzureCredential TokenCredential { get; }

	#endregion

	#region Constructors

	/// <summary>
	/// Initialize instance.
	/// </summary>
	/// <param name="testContext">Test context.</param>
	/// <param name="configKeyPrefixList">The list of configuration key prefixes</param>
	public AzureIntegrationTestsBase
	(
		TestContext testContext,
		KeyValuePair<String, String>[] trackerTags,
		params Tuple<String, Boolean, KeyValuePair<String, String>[]>[] configList
	)
	{
		TestContext = testContext;

		// create token credential
		TokenCredential = new DefaultAzureCredential(false);
		//(
		//	// to authenticate developer within the IDE
		//	new VisualStudioCredential(),
		//	// to authenticate developer within the IDE
		//	new VisualStudioCodeCredential(),
		//	// to authenticate workflow within the agent
		//	new AzureCliCredential()
		//);

		httpClient = new HttpClient();

		var telemetryPublishers = new List<TelemetryPublisher>();

		foreach (var config in configList)
		{
			var ingestionEndpoint = new Uri(TestContext.Properties[config.Item1 + "IngestionEndpoint"].ToString());
			var instrumentationKey = new Guid(TestContext.Properties[config.Item1 + "InstrumentationKey"].ToString());
			var publisherTags = config.Item3;

			TelemetryPublisher publisher;

			if (!config.Item2)
			{
				publisher = new HttpTelemetryPublisher(httpClient, ingestionEndpoint, instrumentationKey, tags: publisherTags);
			}
			else
			{
				async Task<BearerToken> getAccessToken(CancellationToken cancellationToken)
				{
					var tokenRequestContext = new TokenRequestContext(HttpTelemetryPublisher.AuthorizationScopes);

					var token = await TokenCredential.GetTokenAsync(tokenRequestContext, cancellationToken);

					var result = new BearerToken(token.Token, token.ExpiresOn);

					return result;
				}

				publisher = new HttpTelemetryPublisher(httpClient, ingestionEndpoint, instrumentationKey, getAccessToken, publisherTags);
			}

			telemetryPublishers.Add(publisher);
		}

		KeyValuePair<String, String>[] extraTrackerTags =
		[
			new (TelemetryTagKey.CloudRole, "Test Agent"),
			new (TelemetryTagKey.CloudRoleInstance, Environment.MachineName)
		];

		TelemetryTracker = new TelemetryTracker([.. extraTrackerTags, .. trackerTags], [.. telemetryPublishers])
		{
			Operation = new OperationContext()
			{
				Name = $"TEST #{DateTime.UtcNow:yyMMddHHmm}",
				Id = ActivityTraceId.CreateRandom().ToString()
			}
		};
	}

	#endregion

	#region Methods: Implementation of IDisposable

	/// <inheritdoc/>
	public virtual void Dispose()
	{
		httpClient.Dispose();

		GC.SuppressFinalize(this);
	}

	#endregion

	#region Methods

	protected static void AssertStandardSuccess(TelemetryPublishResult[] results)
	{
		// check results count
		Assert.AreEqual(1, results.Length, "Results count not 1");

		Assert.IsTrue(results[0] is HttpTelemetryPublishResult);

		// get first
		var result = (HttpTelemetryPublishResult)results[0];

		// check success
		Assert.IsTrue(result.Success, result.Response);

		// check status code
		Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

		// deserialize response
		var response = JsonSerializer.Deserialize<HttpTelemetryPublishResponse>(result.Response, jsonSerializerOptions);

		// check not null
		if (response == null)
		{
			Assert.Fail("Track response can not be deserialized.");

			return;
		}

		Assert.AreEqual(result.Count, response.ItemsAccepted);

		Assert.AreEqual(result.Count, response.ItemsReceived);

		Assert.AreEqual(0, response.Errors.Length);
	}

	#endregion
}