﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Logging.Tests;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.TestUtils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Validators;
using Xunit;

namespace System.IdentityModel.Tokens.Jwt.Tests
{
    public class JwtSecurityTokenHandlerTelemetryTests
    {
        [Fact]
        public void ValidateToken_ExpectedTagsExist()
        {
            var invalidIssuerConfig = new OpenIdConnectConfiguration()
            {
                TokenEndpoint = Default.Issuer + "oauth/token",
                Issuer = Default.Issuer + "2"
            };
            invalidIssuerConfig.SigningKeys.Add(KeyingMaterial.DefaultX509Key_2048);

            var validationParameters = new TokenValidationParameters
            {
                ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(invalidIssuerConfig),
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var testTelemetryClient = new MockTelemetryInstrumentation();
            try
            {
                AadIssuerValidator.GetAadIssuerValidator(Default.AadV1Authority).ConfigurationManagerV1 = validationParameters.ConfigurationManager;
                var handler = new JwtSecurityTokenHandler()
                {
                    _telemetryClient = testTelemetryClient
                };
                handler.ValidateToken(Default.AsymmetricJws, validationParameters, out _);
            }
            catch (Exception)
            {
                // ignore exceptions
            }

            var expectedCounterTagList = new Dictionary<string, object>
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, TelemetryConstants.LKG },
            };

            Assert.Equal(expectedCounterTagList, testTelemetryClient.ExportedItems);
        }
    }
}
