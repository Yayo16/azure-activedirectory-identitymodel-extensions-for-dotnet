﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Logging.Tests;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect.Configuration;
using Microsoft.IdentityModel.TestUtils;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Xunit;

namespace Microsoft.IdentityModel.Protocols.OpenIdConnect.Tests
{
    public class ConfigurationManagerTelemetryTests
    {
        readonly List<Metric> ExportedItems;
        readonly MeterProvider MeterProvider;

        public ConfigurationManagerTelemetryTests()
        {
            ExportedItems = new List<Metric>();

            MeterProvider = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault())
                .AddMeter(IdentityModelTelemetry.MeterName)
                .AddInMemoryExporter(ExportedItems, (options) =>
                {
                    options.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions
                    {
                        ExportIntervalMilliseconds = 10,
                    };
                })
                .Build();
        }

        [Fact]
        public async Task RequestRefresh_ExpectedTagsExist()
        {
            // arrange
            var testTelemetryClient = new MockTelemetryInstrumentation();
            // mock up retirever to return something quickly 
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                OpenIdConfigData.AccountsGoogle,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(),
                new OpenIdConnectConfigurationValidator())
            {
                _telemetryClient = testTelemetryClient
            };

            // act
            configurationManager.RequestRefresh();
            //// Wait for UpdateCurrentConfiguration to complete
            while (TestUtilities.GetField(configurationManager, "_currentConfiguration") == null)
            {
                await Task.Delay(100);
            }

            // assert
            var expectedCounterTagList = new Dictionary<string, object>
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, TelemetryConstants.Direct },
            };

            var expectedHistogramTagList = new Dictionary<string, object>
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer }
            };

            Assert.Equal(expectedCounterTagList, testTelemetryClient.ExportedItems);
            Assert.Equal(expectedHistogramTagList, testTelemetryClient.ExportedHistogramItems);
        }

        [Theory, MemberData(nameof(GetConfiguration_ExpectedTagList_TheoryData), DisableDiscoveryEnumeration = true)]
        public async Task GetConfigurationAsync_ExpectedTagsExist(ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration> theoryData)
        {
            var testTelemetryClient = new MockTelemetryInstrumentation();

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                theoryData.MetadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                theoryData.DocumentRetriever,
                theoryData.ConfigurationValidator)
            {
                _telemetryClient = testTelemetryClient
            };

            try
            {
                await configurationManager.GetConfigurationAsync();
                if (theoryData.SyncAfter != null)
                {
                    testTelemetryClient.ClearExportedItems();
                    TestUtilities.SetField(configurationManager, "_syncAfter", theoryData.SyncAfter);
                    await configurationManager.GetConfigurationAsync();
                }

            }
            catch (Exception)
            {
                // Ignore exceptions
            }

            Assert.Equal(theoryData.ExpectedTagList, testTelemetryClient.ExportedItems);
        }

        public static TheoryData<ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>> GetConfiguration_ExpectedTagList_TheoryData()
        {
            return new TheoryData<ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>>
            {
                new ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>("Success-retrieve from endpoint")
                {
                    MetadataAddress = OpenIdConfigData.AccountsGoogle,
                    ConfigurationValidator = new OpenIdConnectConfigurationValidator(),
                    ExpectedTagList = new Dictionary<string, object>
                    {
                        { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                        { TelemetryConstants.OperationStatusTag, TelemetryConstants.FirstRefresh },
                    }
                },
                new ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>("Failure-invalid metadata address")
                {
                    MetadataAddress = OpenIdConfigData.HttpsBadUri,
                    ConfigurationValidator = new OpenIdConnectConfigurationValidator(),
                    ExpectedTagList = new Dictionary<string, object>
                    {
                        { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                        { TelemetryConstants.OperationStatusTag, TelemetryConstants.FirstRefresh },
                        { TelemetryConstants.ExceptionTypeTag, new IOException().GetType().ToString() },
                    }
                },
                new ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>("Failure-invalid config")
                {
                    MetadataAddress = OpenIdConfigData.JsonFile,
                    DocumentRetriever = new FileDocumentRetriever(),
                    // The config being loaded has two keys; require three to force invalidity
                    ConfigurationValidator = new OpenIdConnectConfigurationValidator() { MinimumNumberOfKeys = 3 },
                    ExpectedTagList = new Dictionary<string, object>
                    {
                        { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                        { TelemetryConstants.OperationStatusTag, TelemetryConstants.FirstRefresh },
                        { TelemetryConstants.ExceptionTypeTag, new InvalidConfigurationException().GetType().ToString() },
                    }
                },
                new ConfigurationManagerTelemetryTheoryData<OpenIdConnectConfiguration>("Success-refresh")
                {
                    MetadataAddress = OpenIdConfigData.AADCommonUrl,
                    ConfigurationValidator = new OpenIdConnectConfigurationValidator(),
                    SyncAfter = DateTime.UtcNow - TimeSpan.FromDays(2),
                    ExpectedTagList = new Dictionary<string, object>
                    {
                        { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                        { TelemetryConstants.OperationStatusTag, TelemetryConstants.Automatic },
                    }
                },
            };
        }
    }

    public class ConfigurationManagerTelemetryTheoryData<T> : TheoryDataBase where T : class
    {
        public ConfigurationManagerTelemetryTheoryData(string testId) : base(testId) { }

        public string MetadataAddress { get; set; }

        public IDocumentRetriever DocumentRetriever { get; set; } = new HttpDocumentRetriever();

        public IConfigurationValidator<T> ConfigurationValidator { get; set; }

        public DateTimeOffset? SyncAfter { get; set; } = null;

        public Dictionary<string, object> ExpectedTagList { get; set; }
    }
}
