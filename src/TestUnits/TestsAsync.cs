using System.Threading.Tasks;

using SettingsDb;

using Xunit;
using Xunit.Extensions.AssemblyFixture;


namespace TestUnits
{
    public class TestsAsync : IAssemblyFixture<AssemblyTestsFixture>
    {
        [Fact(DisplayName = "Store a String (async version)")]
        public async Task StoreStringAsync()
        {
            var settings = new Settings("StoreStringAsync");
            string originalValue = "test value";
            
            await settings.StoreAsync("String", originalValue);
            var readValue = await settings.ReadAsync<string>("String");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Boolean (async version)")]
        public async Task StoreBooleanAsync()
        {
            var settings = new Settings("StoreBooleanAsync");
            bool originalValue = true;

            await settings.StoreAsync("Boolean", originalValue);
            var readValue = await settings.ReadAsync<bool>("Boolean");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32 (async version)")]
        public async Task StoreInt32Async()
        {
            var settings = new Settings("StoreInt32Async");
            int originalValue = int.MaxValue;

            await settings.StoreAsync("Int32", originalValue);
            var readValue = await settings.ReadAsync<int>("Int32");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int64 (async version)")]
        public async Task StoreInt64Async()
        {
            var settings = new Settings("StoreInt64Async");
            long originalValue = long.MaxValue;

            await settings.StoreAsync("Int64", originalValue);
            var readValue = await settings.ReadAsync<long>("Int64");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Byte array (async version)")]
        public async Task StoreByteArrayAsync()
        {
            var settings = new Settings("StoreByteArrayAsync");
            byte[] originalValue = new byte[] { 14, 34, 2, 54, 3 };

            await settings.StoreAsync("ByteArray", originalValue);
            var readValue = await settings.ReadAsync<byte[]>("ByteArray");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32 array (async version)")]
        public async Task StoreInt32ArrayAsync()
        {
            var settings = new Settings("StoreInt32ArrayAsync");
            int[] originalValue = new int[] { 14, 34, 2, 54, 3 };

            await settings.StoreAsync("Int32Array", originalValue);
            var readValue = await settings.ReadAsync<int[]>("Int32Array");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store an arbitrary object (async version)")]
        public async Task StoreArbitraryObjectAsync()
        {
            var settings = new Settings("StoreArbitraryObjectAsync");
            var originalValue = new TestClass();

            await settings.StoreAsync("ArbitraryObject", originalValue);
            var readValue = await settings.ReadAsync<TestClass>("ArbitraryObject");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Count total number of settings stored (async version)")]
        public async Task CountSettingsAsync()
        {
            var settings = new Settings("CountSettingsAsync");

            await settings.StoreAsync("Setting1", 1);
            await settings.StoreAsync("Setting2", 2);
            await settings.StoreAsync("Setting3", 3);

            var count = await settings.CountAsync();

            Assert.Equal(3, count);
        }
    }
}
