using System;
using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// サービスロケーター
    /// </summary>
    public class ServiceLocator
    {
        private static readonly Lazy<ServiceLocator> SingletonInstance = new Lazy<ServiceLocator>(() => new ServiceLocator());
        public static ServiceLocator Instance => SingletonInstance.Value;
        
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        private ServiceLocator()
        {
        }

        public void Register<T>(T service)
        {
            _services[typeof(T)] = service;
        }
        
        public void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }
        
        public bool IsRegistered<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public T Resolve<T>()
        {
            if(_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            
            // サービスが見つからない場合はエラー
            throw new InvalidOperationException($"Service not found: {typeof(T)}");
        }
    }
}