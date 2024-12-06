// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.IdentityModel.Logging
{
    internal interface ITelemetryInstrumentation
    {
        internal void LogOperationDuration(
            long durationInMilliseconds);

        internal void LogOperationDuration(
            long durationInMilliseconds,
            string exceptionType);

        internal void IncrementOperationCounter(
            string operationStatus);

        internal void IncrementOperationCounter(
            string operationStatus,
            string exceptionType);
    }
}
