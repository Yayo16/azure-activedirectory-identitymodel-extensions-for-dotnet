﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.IdentityModel.Tokens;

#nullable enable
namespace Microsoft.IdentityModel.TestUtils
{
    internal class CustomSecurityTokenInvalidIssuerException : SecurityTokenInvalidIssuerException
    {
        public CustomSecurityTokenInvalidIssuerException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }

    internal class CustomSecurityTokenException : SystemException
    {
        public CustomSecurityTokenException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
#nullable restore
