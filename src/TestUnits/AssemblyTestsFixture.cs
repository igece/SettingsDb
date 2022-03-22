using System.IO;

using Xunit;
using Xunit.Extensions.AssemblyFixture;

[assembly: TestFramework(AssemblyFixtureFramework.TypeName, AssemblyFixtureFramework.AssemblyName)]


namespace TestUnits
{
    public class AssemblyTestsFixture
    {
        public AssemblyTestsFixture()
        {
            foreach (var testDbFile in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.db"))
                File.Delete(testDbFile);
        }
    }
}
