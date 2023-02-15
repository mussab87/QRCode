using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Utility.Core.Utitlites;

namespace Utility.Core.Caching
{
    public static class CachingManager
    {
        public static T GetValue<T>(string cachKey, Func<T> param)
        {
            Check.Argument.IsNotEmpty(cachKey, "cachKey");
            T val = default(T);
            T defaultValue = default(T);
            try
            {
                val = CachingManager.GetValue<T>(cachKey);
                if (val.Equals(defaultValue))
                {
                    val = param();
                    CachingManager.SetValue(cachKey, val);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return val;
        }

        public static T GetValue<T>(string key)
        {
            
            T value = default(T);
            try
            {
                if (HttpRuntime.Cache != null && HttpRuntime.Cache[key] != null)
                {
                    value = (T)HttpRuntime.Cache[key];
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            return value;
        }

        public static object GetValue(string key)
        {
            
            object value = null;
            try
            {
                if (HttpRuntime.Cache != null)
                {
                    value = HttpRuntime.Cache[key];
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return value;
        }

        public static void SetValue(string key, object value)
        {            
            try
            {
                if (HttpRuntime.Cache != null)
                {
                    DateTime expirationTime = DateTime.Now.AddHours(24);
                    HttpRuntime.Cache.Insert(key, value, null, expirationTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal,  null); 
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static void SetValue(string key, object value, CacheItemUpdateCallback cacheItemUpdateCallback)
        {

            try
            {
                if (HttpRuntime.Cache != null)
                {
                    DateTime expirationTime = DateTime.Now.AddHours(24);
                    HttpRuntime.Cache.Insert(key, value, null, expirationTime, Cache.NoSlidingExpiration,  new CacheItemUpdateCallback(cacheItemUpdateCallback));
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static void Remove(string key)
        {
            
            try
            {
                if (HttpRuntime.Cache != null)
                {
                    HttpRuntime.Cache.Remove(key);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public static List<T> GetCachedReferenceData<T>(string cachKey, Func<List<T>> param)
        {
            
            Check.Argument.IsNotEmpty(cachKey, "cachKey");
            List<T> list = null;
            try
            {
                list = CachingManager.GetValue<List<T>>(cachKey);
                if (list == null || list.Count == 0)
                {
                    list = param();
                    CachingManager.SetValue(cachKey, list);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            return list;
        }

     

    }
}
