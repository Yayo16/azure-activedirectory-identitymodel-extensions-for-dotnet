//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

#pragma warning disable 1591

using System.Collections.Generic;

namespace Microsoft.IdentityModel.Protocols.WsSecurity
{
    /// <summary>
    /// Provides constants for WS-Security 1.0 and 1.1.
    /// </summary>
    public abstract class WsSecurityConstants : WsConstantsBase
    {
        public static readonly IList<string> KnownNamespaces = new List<string> { "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd" };

        public static WsSecurity10Constants WsSecurity10 => new WsSecurity10Constants();

        public static WsSecurity11Constants WsSecurity11 => new WsSecurity11Constants();

        public string FragmentBaseAddress { get; protected set; }

        public WsSecurityEncodingTypes EncodingTypes { get; protected set; }
    }

    /// <summary>
    /// Provides constants for WS-Security 1.0.
    /// </summary>
    public class WsSecurity10Constants : WsSecurityConstants
    {
        public WsSecurity10Constants()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity10;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0";
            Prefix = "wsse";
            Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        }
    }

    /// <summary>
    /// Provides constants for WS-Security 1.1.
    /// </summary>
    public class WsSecurity11Constants : WsSecurityConstants
    {
        public WsSecurity11Constants()
        {
            EncodingTypes = WsSecurityEncodingTypes.WsSecurity11;
            FragmentBaseAddress = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1";
            Namespace = "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd";
            Prefix = "wsse11";
        }
    }
}
