// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.IdentityModel.Logging
{
    internal interface ITelemetryInstrumentation
    {
        internal void LogOperationDuration(
            string clientVer,
            long durationInMilliseconds);

        internal void LogOperationDuration(
            string clientVer,
            long durationInMilliseconds,
            string exceptionType);

        internal void IncrementOperationCounter(
            string clientVer,
            string operationStatus);

        internal void IncrementOperationCounter(
            string clientVer,
            string operationStatus,
            string exceptionType);
    }
}
