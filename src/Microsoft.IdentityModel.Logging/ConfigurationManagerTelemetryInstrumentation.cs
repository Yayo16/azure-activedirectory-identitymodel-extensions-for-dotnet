// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Microsoft.IdentityModel.Logging
{
    internal class ConfigurationManagerTelemetryInstrumentation : ITelemetryInstrumentation
    {
        public void IncrementOperationCounter(string clientVer, string operationStatus)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, operationStatus }
            };

            IdentityModelTelemetry.IncrementConfigurationManagerCounter(tagList);
        }

        public void IncrementOperationCounter(string clientVer, string operationStatus, string exceptionType)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, operationStatus },
                { TelemetryConstants.ExceptionTypeTag, exceptionType }
            };

            IdentityModelTelemetry.IncrementConfigurationManagerCounter(tagList);
        }

        public void LogOperationDuration(string clientVer, long durationInMilliseconds)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer }
            };

            IdentityModelTelemetry.RecordTotalDurationHistogram(durationInMilliseconds, tagList);
        }

        public void LogOperationDuration(string clientVer, long durationInMilliseconds, string exceptionType)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.ExceptionTypeTag, exceptionType }
            };

            IdentityModelTelemetry.RecordTotalDurationHistogram(durationInMilliseconds, tagList);
        }
    }
}
