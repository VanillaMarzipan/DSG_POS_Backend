using Microsoft.AspNetCore.Http;
using System;

namespace PointOfSale.Services
{
    public interface ICookieManagement
    {
        string GetRequestCookieValue(IRequestCookieCollection cookies, string key);
        void UpdateResponseCookieWithValue<T>(IResponseCookies cookies, string key, T value, TimeSpan cookieLifetime);
        void UpdateResponseCookieWithValue<T>(IResponseCookies cookies, string key, T value);
        void ClearAllCookies(IRequestCookieCollection requestCookies, IResponseCookies responseCookies);
        bool IsCookieDataValid(IRequestCookieCollection requestCookies);
    }

    public class CookieManagement : ICookieManagement
    {
        public static readonly string StoreNumberCookieName = "storenumber";
        public static readonly string RegisterNumberCookieName = "registernumber";
        public static readonly string RegisterIdCookieName = "registerid";
        public static readonly string ValidationCookieName = "validation";

        public virtual string GetRequestCookieValue(IRequestCookieCollection cookies, string key)
        {
            string value = null;

            if (!string.IsNullOrWhiteSpace(key) && cookies != null)
            {
                if (cookies.TryGetValue(key.Trim().ToLower(), out value))
                    return value;
            }

            return null;
        }

        public virtual void UpdateResponseCookieWithValue<T>(IResponseCookies cookies, string key, T value, TimeSpan cookieLifetime)
        {
            // TODO - should this throw an exception when given a null cookies object?
            if (cookies != null)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.UtcNow.Add(cookieLifetime));
                cookies.Append(key.Trim().ToLower(), $"{value}", cookieOptions);
            }
        }

        // TODO: NOTE: generic just in case we need to do type-specific serializations later
        public virtual void UpdateResponseCookieWithValue<T>(IResponseCookies cookies, string key, T value)
        {
            UpdateResponseCookieWithValue(cookies, key, value, DateTime.UtcNow.AddYears(20) - DateTime.UtcNow);
        }

        public virtual void ClearAllCookies(IRequestCookieCollection requestCookies, IResponseCookies responseCookies)
        {
            foreach (var cookie in requestCookies)
            {
                if (cookie.Key.ToLower() == StoreNumberCookieName ||
                    cookie.Key.ToLower() == RegisterNumberCookieName ||
                    cookie.Key.ToLower() == RegisterIdCookieName ||
                    cookie.Key.ToLower() == ValidationCookieName)
                {
                    responseCookies.Delete(cookie.Key);
                }
            }
        }

        public virtual bool IsCookieDataValid(IRequestCookieCollection requestCookies)
        {
            bool storeNumberFound = false;
            bool registerNumberFound = false;
            bool registerIdFound = false;
            bool validationFound = false;

            foreach (var cookie in requestCookies)
            {
                if (cookie.Key.ToLower() == StoreNumberCookieName)
                {
                    if (storeNumberFound)
                        return false;

                    storeNumberFound = true;
                }
                else if (cookie.Key.ToLower() == RegisterNumberCookieName)
                {
                    if (registerNumberFound)
                        return false;

                    registerNumberFound = true;
                }
                else if (cookie.Key.ToLower() == RegisterIdCookieName)
                {
                    if (registerIdFound)
                        return false;

                    registerIdFound = true;
                }
                else if (cookie.Key.ToLower() == ValidationCookieName)
                {
                    if (validationFound)
                        return false;

                    validationFound = true;
                }
            }

            if (storeNumberFound == false || registerNumberFound == false || registerIdFound == false)
                return false;

            return true;
        }
    }
}
