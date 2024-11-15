﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable enable
namespace Microsoft.IdentityModel.Tokens
{
    internal class AudienceValidationError : ValidationError
    {
        public AudienceValidationError(
            MessageDetail messageDetail,
            Type exceptionType,
            StackFrame stackFrame,
            IList<string>? tokenAudiences,
            IList<string>? validAudiences,
            ValidationFailureType? failureType = null,
            Exception? innerException = null)
            : base(messageDetail, exceptionType, stackFrame, failureType ?? ValidationFailureType.AudienceValidationFailed, innerException)
        {
            TokenAudiences = tokenAudiences;
            ValidAudiences = validAudiences;
        }

        /// <summary>
        /// Creates an instance of an <see cref="Exception"/> using <see cref="ValidationError"/>
        /// </summary>
        /// <returns>An instance of an Exception.</returns>
        internal override Exception GetException()
        {
            if (ExceptionType == typeof(SecurityTokenInvalidAudienceException))
            {
                var exception = new SecurityTokenInvalidAudienceException(MessageDetail.Message, InnerException) { InvalidAudience = Utility.SerializeAsSingleCommaDelimitedString(TokenAudiences) };
                exception.SetValidationError(this);

                return exception;
            }

            return base.GetException(ExceptionType, null);
        }

        protected IList<string>? TokenAudiences { get; set; }
        protected IList<string>? ValidAudiences { get; set; }
    }
}
#nullable restore
