#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

using System;
using Exanite.Core.Utilities;
using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public class SerializationUtilityTests
    {
        [Test]
        public void SerializeType_IsReversedBy_DeserializeType()
        {
            void Test(Type? type)
            {
                Assert.AreEqual(type, SerializationUtility.DeserializeType(SerializationUtility.SerializeType(type)));
            }

            Test(null);
            Test(typeof(int));
            Test(typeof(object));
            Test(typeof(NestedClass));
            Test(typeof(NestedClass.DoubleNestedClass));
            Test(typeof(GenericClass<>));
            Test(typeof(GenericClass<int>));
            Test(typeof(GenericClass<NestedClass>));
            Test(typeof(GenericClass<int, NestedClass>));
            Test(typeof(GenericClass<GenericClass<int, string>, NestedClass>));
            Test(typeof(GenericClass<GenericClass<int, string>, NestedClass.DoubleNestedClass>));
            Test(typeof(GenericClass<GenericClass<int, string>.NestedClassInGenericClass, NestedClass.DoubleNestedClass>));
        }

        public class NestedClass
        {
            public class DoubleNestedClass {}
        }

        public class GenericClass<T1> {}

        public class GenericClass<T1, T2>
        {
            public class NestedClassInGenericClass {}
        }
    }
}
