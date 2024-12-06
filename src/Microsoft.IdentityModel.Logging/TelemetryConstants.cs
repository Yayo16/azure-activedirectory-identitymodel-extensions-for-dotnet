// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.IdentityModel.Logging
{
    internal static class TelemetryConstants
    {
        // Static attribute tags
        public const string IdentityModelVersionTag = "IdentityModelVersion";
        public const string MetadataAddressTag = "MetadataAddress";
        public const string OperationStatusTag = "OperationStatus";
        public const string ExceptionTypeTag = "ExceptionType";

        // Configuration manager refresh statuses
        public const string Automatic = "Microsoft.IdentityModel.Protocols.Automatic";
        public const string Direct = "Microsoft.IdentityModel.Protocols.Direct";
        public const string FirstRefresh = "Microsoft.IdentityModel.Protocols.FirstRefresh";
        public const string LKG = "Microsoft.IdentityModel.Protocols.LastKnownGood";

        // Configuration manager exception types
        public const string ConfigurationInvalid = "Microsoft.IdentityModel.Protocols.ConfigurationInvalid";
        public const string ConfigurationRetrievalFailed = "Microsoft.IdentityModel.Protocols.ConfigurationRetrievalFailed";
    }
}
