using System;
using System.Text;
using System.Threading.Tasks;
using DAL.Collections.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace DAL.Collections.Implementations
{
    public class CacheAside : ICacheAside
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly TimeSpan _defaultExpiry;

        public CacheAside(IMemoryCache memoryCache, IDistributedCache distributedCache, TimeSpan? defaultExpiry = null)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _defaultExpiry = defaultExpiry ?? TimeSpan.FromMinutes(1);
        }

        public T GetOrAdd<T>(string key, Func<T> factory, TimeSpan? expiresIn = null)
        {
            if (TryGetFromMemory(key, out T value))
            {
                return value;
            }

            if (TryGetFromDistributed(key, out value))
            {
                return value;
            }

            value = factory();
            SetToMemory(key, value, expiresIn ?? _defaultExpiry);
            SetToDistributed(key, value, expiresIn ?? _defaultExpiry);

            return value;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiresIn = null)
        {
            var response = await TryGetFromMemoryAsync<T>(key);
            if (response.success) { return response.value; }

            response = await TryGetFromDistributedAsync<T>(key);
            if (response.success) { return response.value; }

            var value = await factory();
            await Task.WhenAll(SetToMemoryAsync(key, value, expiresIn ?? _defaultExpiry), SetToDistributedAsync(key, value, expiresIn ?? _defaultExpiry));
            return value;
        }

        public T Get<T>(string key)
        {
            if (TryGetFromMemory(key, out T value))
            {
                return value;
            }

            if (TryGetFromDistributed(key, out value))
            {
                return value;
            }

            throw new ArgumentException("Item with key not found.");
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (TryGetFromMemory(key, out value))
            {
                return true;
            }

            if (TryGetFromDistributed(key, out value))
            {
                return true;
            }

            value = default(T);
            return false;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var response = await TryGetFromMemoryAsync<T>(key);
            if (response.success) { return response.value; }

            response = await TryGetFromDistributedAsync<T>(key);
            if (response.success) { return response.value; }

            throw new ArgumentException("Item with key not found.");
        }

        public async Task<(bool success, T value)> TryGetAsync<T>(string key)
        {
            var response = await TryGetFromMemoryAsync<T>(key);
            if (response.success) { return response; }

            response = await TryGetFromDistributedAsync<T>(key);
            if (response.success) { return response; }

            return (false, default(T));
        }

        public void Set<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            SetToMemory(key, value, expiresIn ?? _defaultExpiry);
            SetToDistributed(key, value, expiresIn ?? _defaultExpiry);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            await Task.WhenAll(SetToMemoryAsync(key, value, expiresIn ?? _defaultExpiry), SetToDistributedAsync(key, value, expiresIn ?? _defaultExpiry));
        }

        public T Set<T>(string key, Func<T> factory, TimeSpan? expiresIn = null)
        {
            var value = factory();

            SetToMemory(key, value, expiresIn ?? _defaultExpiry);
            SetToDistributed(key, value, expiresIn ?? _defaultExpiry);

            return value;
        }

        public async Task<T> SetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiresIn = null)
        {
            var value = await factory();

            await Task.WhenAll(SetToMemoryAsync(key, value, expiresIn ?? _defaultExpiry), SetToDistributedAsync(key, value, expiresIn ?? _defaultExpiry));

            return value;
        }

        public void Remove(string key)
        {
            RemoveFromMemory(key);
            RemoveFromDistributed(key);
        }

        public async Task RemoveAsync(string key)
        {
            await Task.WhenAll(RemoveFromMemoryAsync(key), RemoveFromDistributedAsync(key));
        }


        #region Helper Functions

        private bool TryGetFromMemory<T>(string key, out T value)
        {
            if (_memoryCache != null && _memoryCache.TryGetValue(key, out value))
                return true;

            value = default(T);
            return false;
        }

        private Task<(bool success, T value)> TryGetFromMemoryAsync<T>(string key)
        {
            if (_memoryCache != null && _memoryCache.TryGetValue(key, out T value))
            {
                return Task.FromResult((true, value));
            }

            return Task.FromResult((false, default(T)));
        }

        private bool TryGetFromDistributed<T>(string key, out T value)
        {
            if (_distributedCache != null)
            {
                try
                {
                    var val = _distributedCache.Get(key);
                    if (val != null && val.Length > 0)
                    {
                        var json = Encoding.UTF8.GetString(val);
                        value = JsonConvert.DeserializeObject<T>(json);

                        return true;
                    }
                }
                catch (Exception) { /* ignore miss */ }
            }

            value = default(T);
            return false;
        }

        private async Task<(bool success, T value)> TryGetFromDistributedAsync<T>(string key)
        {
            if (_distributedCache != null)
            {
                try
                {
                    var val = await _distributedCache.GetAsync(key);
                    if (val != null && val.Length > 0)
                    {
                        var json = Encoding.UTF8.GetString(val);
                        return (true, JsonConvert.DeserializeObject<T>(json));
                    }
                }
                catch (Exception) { /* ignore miss */ }
            }

            return (false, default(T));
        }

        private void SetToMemory<T>(string key, T item, TimeSpan? expiresIn = null)
        {
            _memoryCache.Set(key, item, expiresIn ?? _defaultExpiry);
        }

        private Task SetToMemoryAsync<T>(string key, T item, TimeSpan? expiresIn = null)
        {
            _memoryCache.Set(key, item, expiresIn ?? _defaultExpiry);
            return Task.CompletedTask;
        }

        private void SetToDistributed<T>(string key, T item, TimeSpan? expiresIn = null)
        {
            _distributedCache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiresIn ?? _defaultExpiry });
        }

        private async Task SetToDistributedAsync<T>(string key, T item, TimeSpan? expiresIn = null)
        {
            await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiresIn ?? _defaultExpiry });
        }

        private void RemoveFromMemory(string key)
        {
            _memoryCache.Remove(key);
        }

        private Task RemoveFromMemoryAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        private void RemoveFromDistributed(string key)
        {
            _distributedCache.Remove(key);
        }

        private async Task RemoveFromDistributedAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
        #endregion
    }
}
