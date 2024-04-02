using Microsoft.AspNetCore.Http;
using Moq;
using PointOfSale.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PointOfSale.UnitTests
{
    public class CookieManagementShould
    {
        [Fact]
        public void ReturnExistingRequestCookieValue()
        {
            string expectedValue = "12";

            CookieManagement cookieManagement = new CookieManagement();
            Mock<IRequestCookieCollection> mockRequestCookieCollection = new Mock<IRequestCookieCollection>();
            mockRequestCookieCollection.Setup(c => c.TryGetValue(It.IsAny<string>(), out expectedValue)).Returns(true);

            string actualValue = cookieManagement.GetRequestCookieValue(mockRequestCookieCollection.Object, "StoreNumber");
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void ReturnNullWhenRequestCookieKeyDoesNotExist()
        {
            string expectedValue = null;

            CookieManagement cookieManagement = new CookieManagement();
            Mock<IRequestCookieCollection> mockRequestCookieCollection = new Mock<IRequestCookieCollection>();
            mockRequestCookieCollection.Setup(c => c.TryGetValue(It.IsAny<string>(), out expectedValue)).Returns(false);

            string actualValue = cookieManagement.GetRequestCookieValue(mockRequestCookieCollection.Object, "StoreNumber");
            Assert.Null(actualValue);
        }

        [Fact]
        public void ReturnNullWhenLookupKeyIsNull()
        {
            CookieManagement cookieManagement = new CookieManagement();
            Mock<IRequestCookieCollection> mockRequestCookieCollection = new Mock<IRequestCookieCollection>();

            string actualValue = cookieManagement.GetRequestCookieValue(mockRequestCookieCollection.Object, null);
            Assert.Null(actualValue);
        }

        [Fact]
        public void ReturnNullWhenLookupKeyIsEmpty()
        {
            CookieManagement cookieManagement = new CookieManagement();
            Mock<IRequestCookieCollection> mockRequestCookieCollection = new Mock<IRequestCookieCollection>();

            string actualValue = cookieManagement.GetRequestCookieValue(mockRequestCookieCollection.Object, "");
            Assert.Null(actualValue);
        }

        [Fact]
        public void ReturnNullWhenLookupKeyIsWhitespace()
        {
            CookieManagement cookieManagement = new CookieManagement();
            Mock<IRequestCookieCollection> mockRequestCookieCollection = new Mock<IRequestCookieCollection>();

            string actualValue = cookieManagement.GetRequestCookieValue(mockRequestCookieCollection.Object, "   ");
            Assert.Null(actualValue);
        }

        [Fact]
        public void ReturnNullWhenRequestCookiesAreNull()
        {
            CookieManagement cookieManagement = new CookieManagement();

            string actualValue = cookieManagement.GetRequestCookieValue(null, "StoreNumber");
            Assert.Null(actualValue);
        }

        [Fact]
        public void UpdateResponseCookieWithGivenValue()
        {
            CookieManagement cookieManagement = new CookieManagement();
            Mock<IResponseCookies> mockResponseCookies = new Mock<IResponseCookies>();

            cookieManagement.UpdateResponseCookieWithValue(mockResponseCookies.Object, "TestKey", 123);
            mockResponseCookies.Verify(c => c.Append("testkey", "123", It.IsAny<CookieOptions>()), Times.Once());
        }

        [Fact]
        public void UpdateResponseCookieWithGivenValueAndTimespan()
        {
            TimeSpan expectedTimeSpan = new TimeSpan(0, 5, 0);
            CookieManagement cookieManagement = new CookieManagement();
            Mock<IResponseCookies> mockResponseCookies = new Mock<IResponseCookies>();

            cookieManagement.UpdateResponseCookieWithValue(mockResponseCookies.Object, "TestKey", 123, expectedTimeSpan);
            mockResponseCookies.Verify(c => c.Append("testkey", "123", It.IsAny<CookieOptions>()), Times.Once());
        }

        [Fact]
        public void DoNothingWhenGivenNullCookieObject()
        {
            CookieManagement cookieManagement = new CookieManagement();
            cookieManagement.UpdateResponseCookieWithValue(null, "TestKey", 123);
        }

        [Fact]
        public void RemoveAllCookiesFromResponse()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue(CookieManagement.StoreNumberCookieName, "TestValue1");
            requestCookies.AddKeyValue(CookieManagement.RegisterNumberCookieName, "TestValue2");
            requestCookies.AddKeyValue(CookieManagement.RegisterIdCookieName, "TestValue3");
            requestCookies.AddKeyValue("MockKey1", "MockValue1");

            MockResponseCookies responseCookies = new MockResponseCookies();
            responseCookies.Append(CookieManagement.StoreNumberCookieName, "TestValue1");
            responseCookies.Append(CookieManagement.RegisterNumberCookieName, "TestValue2");
            responseCookies.Append(CookieManagement.RegisterIdCookieName, "TestValue3");
            responseCookies.Append("MockKey1", "MockValue1");

            Assert.Equal(4, responseCookies.Count);

            CookieManagement cookieManagement = new CookieManagement();
            cookieManagement.ClearAllCookies(requestCookies, responseCookies);

            Assert.Equal(1, responseCookies.Count);
            Assert.Equal("MockValue1", responseCookies["MockKey1"]);
        }

        [Fact]
        public void ReturnTrueWhenCookieDataIsValid()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.True(response);
        }

        [Fact]
        public void ReturnTrueWhenCookieDataIsValidWithValidationKey()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());
            requestCookies.AddKeyValue("validation", "true");

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.True(response);
        }

        [Fact]
        public void ReturnTrueWhenCookieDataIsValidWithAspNetCoreKey()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());
            requestCookies.AddKeyValue(".AspNetCore.Antiforgery.123456", "X-CSRF");

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.True(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMissingStoreNumber()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMissingRegisterNumber()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMissingRegisteId()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMultipleStoreNumbers()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("Storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMultipleRegisterNumbers()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("Registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnFalseWhenCookieDataHasMultipleRegisterIds()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());
            requestCookies.AddKeyValue("Registerid", Guid.NewGuid().ToString());

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.False(response);
        }

        [Fact]
        public void ReturnTrueWhenCookieDataHasValidDataWithOtherKeys()
        {
            MockRequestCookies requestCookies = new MockRequestCookies();
            requestCookies.AddKeyValue("storenumber", "42");
            requestCookies.AddKeyValue("registernumber", "1");
            requestCookies.AddKeyValue("registerid", Guid.NewGuid().ToString());
            requestCookies.AddKeyValue("extrakey", "extravalue");

            CookieManagement cookieManagement = new CookieManagement();
            bool response = cookieManagement.IsCookieDataValid(requestCookies);
            Assert.True(response);
        }
    }

    public class MockRequestCookies : IRequestCookieCollection
    {
        public string this[string key] => _cookies.GetValueOrDefault(key);

        public int Count => _cookies.Count;

        public ICollection<string> Keys => _cookies.Keys;

        private Dictionary<string, string> _cookies = new Dictionary<string, string>();

        public void AddKeyValue(string key, string value)
        {
            _cookies.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _cookies.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }

        public bool TryGetValue(string key, out string value)
        {
            return _cookies.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }
    }

    public class MockResponseCookies : IResponseCookies
    {
        public string this[string key] => _cookies.GetValueOrDefault(key);

        public int Count => _cookies.Count;

        private Dictionary<string, string> _cookies = new Dictionary<string, string>();

        public void Append(string key, string value)
        {
            _cookies.Add(key, value);
        }

        public void Append(string key, string value, CookieOptions options)
        {
            _cookies.Add(key, value);
        }

        public void Delete(string key)
        {
            _cookies.Remove(key);
        }

        public void Delete(string key, CookieOptions options)
        {
            _cookies.Remove(key);
        }
    }
}
