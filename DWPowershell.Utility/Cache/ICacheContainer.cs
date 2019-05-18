using System;

namespace DWPowerShell.Utility.Cache
{
    public interface ICacheContainer
    {
        void Add(string key, object value);
        void Add(string key, object value, DateTime expires);
        void Add(string key, object value, TimeSpan expiresIn, bool sliding = false);
        void Add(string key, object value, string dependencyFile);
        void Add(string key, object value, string[] dependencyFiles);
        T Get<T>(string key) where T : class;
        string[] Keys { get; }
        bool ContainsKey(string key);
        int Length { get; }
        void Remove(string key);

        object Get(string key);
        object this[string key] { get; set; }
    }
}