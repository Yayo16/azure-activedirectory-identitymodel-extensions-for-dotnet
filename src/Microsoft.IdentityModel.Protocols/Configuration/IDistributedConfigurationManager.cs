﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Protocols;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDistributedConfigurationManager<T> where T : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="metadataAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T> GetConfigurationAsync(string metadataAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="metadataAddress"></param>
    /// <param name="configuration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetConfigurationAsync(string metadataAddress, T configuration, CancellationToken cancellationToken = default);
}
