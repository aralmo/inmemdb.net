using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetIMDB.Wrapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB_Tests
{
    [TestClass]
    public class WrapperBuilderTests
    {
        [TestMethod]
        public void BuildTypeFromInterfaceILTests()
        {
            Type type = WrapperTypeBuilder.GetWrapperType<ITestModel>();
            ITestModel instance = (ITestModel)Activator.CreateInstance(type);
            
            //set data and offset
            instance.GetType().GetField("data").SetValue(instance, new byte[] { 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0 });
            
            instance.GetType().GetField("offset").SetValue(instance,0);
                
            int a = instance.ID;
            int b = instance.Cuenta;

            instance.GetType().GetField("offset").SetValue(instance, 8);

            int c = instance.ID;
            int d = instance.Cuenta;
            
            Assert.AreEqual(1, a);
            Assert.AreEqual(2, b);
            Assert.AreEqual(3, c);
            Assert.AreEqual(4, d);
        }

        [TestMethod]
        public void SameInterfaceGeneratedTwice()
        {
            //el test trata de asegurarse de que no se producirá una excepción al instanciar varios repositorios que usen la misma interfaz.
            Type type = WrapperTypeBuilder.GetWrapperType<ITestModel>();
            Type type_bis = WrapperTypeBuilder.GetWrapperType<ITestModel>();

            
        }

        public interface ITestModel
        {
            int ID { get; }
            int Cuenta { get; }
        }

    }
}
