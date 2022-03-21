using System.Collections.Generic;
using System.Linq;


namespace TestUnits
{
#pragma warning disable CS0659
    class TestClass
#pragma warning restore CS0659
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
}
