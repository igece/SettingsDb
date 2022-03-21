using System.IO;
using System.Reflection;

using SettingsDb;

using Xunit;


namespace TestUnits
{
    public class Tests
    {
        [Fact(DisplayName = "Create a settings database with default name")]
        public void SettingsFileDefaultName()
        {
            _ = new Settings();
            Assert.True(File.Exists($"{Assembly.GetEntryAssembly().GetName().Name}.db"));
        }


        [Fact(DisplayName = "Create a settings database with custom name")]
        public void SettingsFileCustomName()
        {
            _ = new Settings("Test");
            Assert.True(File.Exists("Test.db"));
        }


        [Fact(DisplayName = "Store a String")]
        public void StoreString()
        {
            var settings = new Settings();
            string originalValue = "test value";
            
            settings.Store("String", originalValue);
            var readValue = settings.Read<string>("String");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Bool")]
        public void StoreBoolean()
        {
            var settings = new Settings();
            bool originalValue = true;

            settings.Store("Boolean", originalValue);
            var readValue = settings.Read<bool>("Boolean");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32")]
        public void StoreInt32()
        {
            var settings = new Settings();
            int originalValue = 12345;

            settings.Store("Int32", originalValue);
            var readValue = settings.Read<int>("Int32");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int64")]
        public void StoreInt64()
        {
            var settings = new Settings();
            long originalValue = 12345;

            settings.Store("Int64", originalValue);
            var readValue = settings.Read<long>("Int64");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Byte array")]
        public void StoreByteArray()
        {
            var settings = new Settings();
            byte[] originalValue = new byte[] { 14, 34, 2, 54, 3 };

            settings.Store("ByteArray", originalValue);
            var readValue = settings.Read<byte[]>("ByteArray");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store a Int32 array")]
        public void StoreInt32Array()
        {
            var settings = new Settings();
            int[] originalValue = new int[] { 14, 34, 2, 54, 3 };

            settings.Store("Int32Array", originalValue);
            var readValue = settings.Read<int[]>("Int32Array");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Store an arbitrary object")]
        public void StoreArbitraryObject()
        {
            var settings = new Settings();
            var originalValue = new TestClass();

            settings.Store("ArbitraryObject", originalValue);
            var readValue = settings.Read<TestClass>("ArbitraryObject");

            Assert.Equal(originalValue, readValue);
        }


        [Fact(DisplayName = "Count total number of settings stored")]
        public void CountSettings()
        {
            var settings = new Settings();

            settings.ClearAll();

            settings.Store("Setting1", 1);
            settings.Store("Setting2", 2);
            settings.Store("Setting3", 3);

            var count = settings.Count();

            Assert.Equal(3, count);
        }
    }
}
