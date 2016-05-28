﻿//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace PhotoSharingApp.AppService.Shared.Caching
{
    /// <summary>
    /// A In-memory cache implementation using <see cref="MemoryCache" />.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        /// <summary>
        /// Gets or sets the expiration timespan.
        /// </summary>
        public TimeSpan ExpirationTimeSpan { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            foreach (var element in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(element.Key);
            }
        }

        /// <summary>
        /// Determines whether the cache has an element with the given key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>True, if element in cache. Otherwise, false.</returns>
        public bool Contains(string cacheKey)
        {
            var item = MemoryCache.Default.Get(cacheKey);
            return item != null;
        }

        /// <summary>
        /// Gets or sets an object into the cache.
        /// </summary>
        /// <typeparam name="T">Type of object to get or insert.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="getItemFunc">Function that will be called if key cannot be found.</param>
        /// <returns>Object that is either found in the cache or resolved by given function.</returns>
        public virtual async Task<T> GetOrInsert<T>(Func<Task<T>> getItemFunc, string cacheKey) where T : class
        {
            var item = MemoryCache.Default.Get(cacheKey) as T;

            if (item == null)
            {
                item = await getItemFunc();

                if (item != null)
                {
                    MemoryCache.Default.Add(cacheKey, item, DateTime.UtcNow.Add(ExpirationTimeSpan));
                }
            }

            return item;
        }
    }
}