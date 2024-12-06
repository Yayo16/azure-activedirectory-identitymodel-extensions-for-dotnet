// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.IdentityModel.Logging.Tests
{
    public class MockTelemetryInstrumentation : ITelemetryInstrumentation
    {
        public Dictionary<string, object> ExportedItems = new Dictionary<string, object>();
        public Dictionary<string, object> ExportedHistogramItems = new Dictionary<string, object>();

        public void ClearExportedItems()
        {
            ExportedItems.Clear();
        }

        public void IncrementOperationCounter(string operationStatus)
        {
            ExportedItems.Add(TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer);
            ExportedItems.Add(TelemetryConstants.OperationStatusTag, operationStatus);
        }

        public void IncrementOperationCounter(string operationStatus, string exceptionType)
        {
            ExportedItems.Add(TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer);
            ExportedItems.Add(TelemetryConstants.OperationStatusTag, operationStatus);
            ExportedItems.Add(TelemetryConstants.ExceptionTypeTag, exceptionType);
        }

        public void LogOperationDuration(long durationInMilliseconds)
        {
            ExportedHistogramItems.Add(TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer);
        }

        public void LogOperationDuration(long durationInMilliseconds, string exceptionType)
        {
            ExportedHistogramItems.Add(TelemetryConstants.IdentityModelVersionTag, IdentityModelTelemetryUtil.ClientVer);
            ExportedHistogramItems.Add(TelemetryConstants.ExceptionTypeTag, exceptionType);
        }
    }
}
