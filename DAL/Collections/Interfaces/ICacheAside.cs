using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Collections.Interfaces
{
    public interface ICacheAside
    {
        T GetOrAdd<T>(string key, Func<T> factory, TimeSpan? expiresIn = null);
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiresIn = null);

        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        bool TryGet<T>(string key, out T value);
        Task<(bool success, T value)> TryGetAsync<T>(string key);

        void Set<T>(string key, T value, TimeSpan? expiresIn = null);
        Task SetAsync<T>(string key, T value, TimeSpan? expiresIn = null);
        T Set<T>(string key, Func<T> factory, TimeSpan? expiresIn = null);
        Task<T> SetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiresIn = null);

        void Remove(string key);
        Task RemoveAsync(string key);
    }
}
