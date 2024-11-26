﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.TestUtils;

using Xunit;

#nullable enable
namespace Microsoft.IdentityModel.Tokens.Saml2.Extensibility.Tests
{
    public partial class Saml2SecurityTokenHandlerValidateTokenAsyncTests
    {
        [Theory, MemberData(nameof(Lifetime_ExtensibilityTestCases), DisableDiscoveryEnumeration = true)]
        public async Task ValidateTokenAsync_LifetimeValidator_Extensibility(LifetimeExtensibilityTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.{nameof(ValidateTokenAsync_LifetimeValidator_Extensibility)}", theoryData);
            context.IgnoreType = false;
            for (int i = 0; i < theoryData.ExtraStackFrames; i++)
                theoryData.LifetimeValidationError!.AddStackFrame(new StackFrame(false));

            try
            {
                ValidationResult<ValidatedToken> validationResult = await theoryData.Saml2SecurityTokenHandler.ValidateTokenAsync(
                    theoryData.Saml2Token!,
                    theoryData.ValidationParameters!,
                    theoryData.CallContext,
                    CancellationToken.None);

                if (validationResult.IsValid)
                {
                    context.Diffs.Add("validationResult.IsValid == true, expected false");
                }
                else
                {
                    ValidationError validationError = validationResult.UnwrapError();
                    IdentityComparer.AreValidationErrorsEqual(validationError, theoryData.LifetimeValidationError, context);
                    theoryData.ExpectedException.ProcessException(validationError.GetException(), context);
                }
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<LifetimeExtensibilityTheoryData> Lifetime_ExtensibilityTestCases
        {
            get
            {
                var theoryData = new TheoryData<LifetimeExtensibilityTheoryData>();
                CallContext callContext = new CallContext();
                var utcNow = DateTime.UtcNow;
                var utcPlusOneHour = utcNow + TimeSpan.FromHours(1);

                #region return CustomLifetimeValidationError
                // Test cases where delegate is overridden and return a CustomLifetimeValidationError
                // CustomLifetimeValidationError : LifetimeValidationError, ExceptionType: SecurityTokenInvalidLifetimeException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "CustomLifetimeValidatorDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.CustomLifetimeValidatorDelegate,
                    extraStackFrames: 2)
                {
                    ExpectedException = new ExpectedException(
                        typeof(SecurityTokenInvalidLifetimeException),
                        nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorDelegate)),
                    LifetimeValidationError = new CustomLifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(SecurityTokenInvalidLifetimeException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 160),
                        utcNow,
                        utcPlusOneHour)
                });

                // CustomLifetimeValidationError : LifetimeValidationError, ExceptionType: CustomSecurityTokenInvalidLifetimeException : SecurityTokenInvalidLifetimeException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "CustomLifetimeValidatorCustomExceptionDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionDelegate,
                    extraStackFrames: 2)
                {
                    ExpectedException = new ExpectedException(
                        typeof(CustomSecurityTokenInvalidLifetimeException),
                        nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionDelegate)),
                    LifetimeValidationError = new CustomLifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(CustomSecurityTokenInvalidLifetimeException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 175),
                        utcNow,
                        utcPlusOneHour)
                });

                // CustomLifetimeValidationError : LifetimeValidationError, ExceptionType: NotSupportedException : SystemException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "CustomLifetimeValidatorUnknownExceptionDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.CustomLifetimeValidatorUnknownExceptionDelegate,
                    extraStackFrames: 2)
                {
                    // CustomLifetimeValidationError does not handle the exception type 'NotSupportedException'
                    ExpectedException = ExpectedException.SecurityTokenException(
                        LogHelper.FormatInvariant(
                            Tokens.LogMessages.IDX10002, // "IDX10002: Unknown exception type returned. Type: '{0}'. Message: '{1}'.";
                            typeof(NotSupportedException),
                            nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorUnknownExceptionDelegate))),
                    LifetimeValidationError = new CustomLifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorUnknownExceptionDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(NotSupportedException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 205),
                        utcNow,
                        utcPlusOneHour)
                });

                // CustomLifetimeValidationError : LifetimeValidationError, ExceptionType: NotSupportedException : SystemException, ValidationFailureType: CustomAudienceValidationFailureType
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "CustomLifetimeValidatorCustomExceptionCustomFailureTypeDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionCustomFailureTypeDelegate,
                    extraStackFrames: 2)
                {
                    ExpectedException = new ExpectedException(
                        typeof(CustomSecurityTokenInvalidLifetimeException),
                        nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionCustomFailureTypeDelegate)),
                    LifetimeValidationError = new CustomLifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.CustomLifetimeValidatorCustomExceptionCustomFailureTypeDelegate), null),
                        CustomLifetimeValidationError.CustomLifetimeValidationFailureType,
                        typeof(CustomSecurityTokenInvalidLifetimeException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 190),
                        utcNow,
                        utcPlusOneHour),
                });
                #endregion

                #region return LifetimeValidationError
                // Test cases where delegate is overridden and return an LifetimeValidationError
                // LifetimeValidationError : ValidationError, ExceptionType:  SecurityTokenInvalidLifetimeException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "LifetimeValidatorDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.LifetimeValidatorDelegate,
                    extraStackFrames: 2)
                {
                    ExpectedException = new ExpectedException(
                        typeof(SecurityTokenInvalidLifetimeException),
                        nameof(CustomLifetimeValidationDelegates.LifetimeValidatorDelegate)),
                    LifetimeValidationError = new LifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.LifetimeValidatorDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(SecurityTokenInvalidLifetimeException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 235),
                        utcNow,
                        utcPlusOneHour)
                });

                // LifetimeValidationError : ValidationError, ExceptionType:  CustomSecurityTokenInvalidLifetimeException : SecurityTokenInvalidLifetimeException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "LifetimeValidatorCustomLifetimeExceptionTypeDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.LifetimeValidatorCustomLifetimeExceptionTypeDelegate,
                    extraStackFrames: 2)
                {
                    // LifetimeValidationError does not handle the exception type 'CustomSecurityTokenInvalidLifetimeException'
                    ExpectedException = ExpectedException.SecurityTokenException(
                        LogHelper.FormatInvariant(
                            Tokens.LogMessages.IDX10002, // "IDX10002: Unknown exception type returned. Type: '{0}'. Message: '{1}'.";
                            typeof(CustomSecurityTokenInvalidLifetimeException),
                            nameof(CustomLifetimeValidationDelegates.LifetimeValidatorCustomLifetimeExceptionTypeDelegate))),
                    LifetimeValidationError = new LifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.LifetimeValidatorCustomLifetimeExceptionTypeDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(CustomSecurityTokenInvalidLifetimeException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 259),
                        utcNow,
                        utcPlusOneHour)
                });

                // LifetimeValidationError : ValidationError, ExceptionType:  CustomSecurityTokenException : SystemException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "LifetimeValidatorCustomExceptionTypeDelegate",
                    utcNow,
                    CustomLifetimeValidationDelegates.LifetimeValidatorCustomExceptionTypeDelegate,
                    extraStackFrames: 2)
                {
                    // LifetimeValidationError does not handle the exception type 'CustomSecurityTokenException'
                    ExpectedException = ExpectedException.SecurityTokenException(
                        LogHelper.FormatInvariant(
                            Tokens.LogMessages.IDX10002, // "IDX10002: Unknown exception type returned. Type: '{0}'. Message: '{1}'.";
                            typeof(CustomSecurityTokenException),
                            nameof(CustomLifetimeValidationDelegates.LifetimeValidatorCustomExceptionTypeDelegate))),
                    LifetimeValidationError = new LifetimeValidationError(
                        new MessageDetail(
                            nameof(CustomLifetimeValidationDelegates.LifetimeValidatorCustomExceptionTypeDelegate), null),
                        ValidationFailureType.LifetimeValidationFailed,
                        typeof(CustomSecurityTokenException),
                        new StackFrame("CustomLifetimeValidationDelegates.cs", 274),
                        utcNow,
                        utcPlusOneHour)
                });

                // LifetimeValidationError : ValidationError, ExceptionType: SecurityTokenInvalidLifetimeException, inner: CustomSecurityTokenInvalidLifetimeException
                theoryData.Add(new LifetimeExtensibilityTheoryData(
                    "LifetimeValidatorThrows",
                    utcNow,
                    CustomLifetimeValidationDelegates.LifetimeValidatorThrows,
                    extraStackFrames: 1)
                {
                    ExpectedException = new ExpectedException(
                        typeof(SecurityTokenInvalidLifetimeException),
                        string.Format(Tokens.LogMessages.IDX10271),
                        typeof(CustomSecurityTokenInvalidLifetimeException)),
                    LifetimeValidationError = new LifetimeValidationError(
                        new MessageDetail(
                            string.Format(Tokens.LogMessages.IDX10271), null),
                        ValidationFailureType.LifetimeValidatorThrew,
                        typeof(SecurityTokenInvalidLifetimeException),
                        new StackFrame("Saml2SecurityTokenHandler.ValidateToken.Internal.cs", 250),
                        utcNow,
                        utcPlusOneHour,
                        new SecurityTokenInvalidLifetimeException(nameof(CustomLifetimeValidationDelegates.LifetimeValidatorThrows))
                    )
                });
                #endregion

                return theoryData;
            }
        }

        public class LifetimeExtensibilityTheoryData : TheoryDataBase
        {
            internal LifetimeExtensibilityTheoryData(string testId, DateTime utcNow, LifetimeValidationDelegate lifetimeValidator, int extraStackFrames) : base(testId)
            {
                Saml2Token = (Saml2SecurityToken)Saml2SecurityTokenHandler.CreateToken(
                    new SecurityTokenDescriptor()
                    {
                        Subject = Default.SamlClaimsIdentity,
                        Issuer = Default.Issuer,
                        IssuedAt = utcNow,
                        NotBefore = utcNow,
                        Expires = utcNow + TimeSpan.FromHours(1),
                    });

                ValidationParameters = new ValidationParameters
                {
                    AlgorithmValidator = SkipValidationDelegates.SkipAlgorithmValidation,
                    AudienceValidator = SkipValidationDelegates.SkipAudienceValidation,
                    IssuerValidatorAsync = SkipValidationDelegates.SkipIssuerValidation,
                    IssuerSigningKeyValidator = SkipValidationDelegates.SkipIssuerSigningKeyValidation,
                    LifetimeValidator = lifetimeValidator,
                    SignatureValidator = SkipValidationDelegates.SkipSignatureValidation,
                    TokenReplayValidator = SkipValidationDelegates.SkipTokenReplayValidation,
                    TokenTypeValidator = SkipValidationDelegates.SkipTokenTypeValidation
                };

                ExtraStackFrames = extraStackFrames;
            }

            public Saml2SecurityToken Saml2Token { get; }

            public Saml2SecurityTokenHandler Saml2SecurityTokenHandler { get; } = new Saml2SecurityTokenHandler();

            public bool IsValid { get; set; }

            internal ValidatedLifetime ValidatedLifetime { get; set; }

            internal ValidationParameters ValidationParameters { get; }

            internal LifetimeValidationError? LifetimeValidationError { get; set; }

            internal int ExtraStackFrames { get; }
        }
    }
}
#nullable restore
