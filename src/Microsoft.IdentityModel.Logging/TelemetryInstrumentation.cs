// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Microsoft.IdentityModel.Logging
{
    internal class TelemetryInstrumentation : ITelemetryInstrumentation
    {
        public void IncrementOperationCounter(string operationStatus)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, operationStatus }
            };

            IdentityModelTelemetry.IncrementOperationCounter(tagList);
        }

        public void IncrementOperationCounter(string operationStatus, string exceptionType)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer },
                { TelemetryConstants.OperationStatusTag, operationStatus },
                { TelemetryConstants.ExceptionTypeTag, exceptionType }
            };

            IdentityModelTelemetry.IncrementOperationCounter(tagList);
        }

        public void LogOperationDuration(long durationInMilliseconds)
        {
            var tagList = new TagList()
            {
                { TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer }
            };

            IdentityModelTelemetry.RecordTotalDurationHistogram(durationInMilliseconds, tagList);
        }

        public void LogOperationDuration(long durationInMilliseconds, string exceptionType)
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
