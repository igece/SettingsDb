using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SettingsDb;

using Xunit;

namespace TestUnits
{
    public class TestsAsync
    {
        [Fact(DisplayName = "Store a String (async version)")]
        public async Task StoreStringAsync()
        {
            var settings = new Settings();
            string originalValue = "test value";
            
            await settings.StoreAsync("String", originalValue);
            var readValue = await settings.ReadAsync<string>("String");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Bool (async version)")]
        public async Task StoreBoolean()
        {
            var settings = new Settings();
            bool originalValue = true;

            await settings.StoreAsync("Boolean", originalValue);
            var readValue = await settings.ReadAsync<bool>("Boolean");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32 (async version)")]
        public async Task StoreInt32Async()
        {
            var settings = new Settings();
            int originalValue = 12345;

            await settings.StoreAsync("Int32", originalValue);
            var readValue = await settings.ReadAsync<int>("Int32");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int64 (async version)")]
        public async Task StoreInt64Async()
        {
            var settings = new Settings();
            long originalValue = 12345;

            await settings.StoreAsync("Int64", originalValue);
            var readValue = await settings.ReadAsync<long>("Int64");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Byte array (async version)")]
        public async Task StoreByteArrayAsync()
        {
            var settings = new Settings();
            byte[] originalValue = new byte[] { 14, 34, 2, 54, 3 };

            await settings.StoreAsync("ByteArray", originalValue);
            var readValue = await settings.ReadAsync<byte[]>("ByteArray");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32 array (async version)")]
        public async Task StoreInt32ArrayAsync()
        {
            var settings = new Settings();
            int[] originalValue = new int[] { 14, 34, 2, 54, 3 };

            await settings.StoreAsync("Int32Array", originalValue);
            var readValue = await settings.ReadAsync<int[]>("Int32Array");

            Assert.Equal(originalValue, readValue);
        }


        class TestClass
        {
            public int Property1 { get; set; } = 1;

            public List<string> Property2 { get; set; } = new List<string>(new string[] { "value1", "value2" });

            public override bool Equals(object obj)
            {
                if (obj is not TestClass other)
                    return false;

                return Property1 == other.Property1 &&
                   Enumerable.SequenceEqual(Property2, other.Property2);
            }
        }


        [Fact(DisplayName = "Store an arbitrary object (async version)")]
        public async Task StoreArbitraryObject()
        {
            var settings = new Settings();
            var originalValue = new TestClass();

            await settings.StoreAsync("ArbitraryObject", originalValue);
            var readValue = await settings.ReadAsync<TestClass>("ArbitraryObject");

            Assert.Equal(originalValue, readValue);
        }
    }
}
