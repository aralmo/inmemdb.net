using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetIMDB;
using NetIMDB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB_Tests
{
    [TestClass]
    public class SerializeTests
    {
        [TestMethod]
        public void Serialize()
        {
            var model = new testmodel(){
                value = 1,
                value3 = 2D,
                text1 ="texto 1",
                text2 = "otro texto de prueba"
            };
            byte[] serialization = SerializationHelper.Serialize<ITestModel>(model);
            
            //según la ordenación, lo primero debería ser el campo más grande, el double, después el entero y los strings.

            Assert.AreEqual(1, PointerCastHelper.BytesToInt(serialization, 8));
            Assert.AreEqual(2D, PointerCastHelper.BytesToDouble(serialization, 0));
            Assert.AreEqual("texto 1", PointerCastHelper.GetStringAtOffset(serialization, 12, 0));
            Assert.AreEqual("otro texto de prueba", PointerCastHelper.GetStringAtOffset(serialization, 12, 1));
        }

        [TestMethod]
        public void SerializeAndDeserializeHeader()
        {
            byte[] header2 = SerializationHelper.SerializeHeader(100, true);

            int largo;
            bool borrado;

            Assert.Inconclusive("No es válido, rehacer");

            SerializationHelper.DeSerializeHeader(BitConverter.GetBytes(100 | 0x8000), out largo, out borrado);
            Assert.AreEqual(100,largo);
            Assert.AreEqual(true,borrado);

            SerializationHelper.DeSerializeHeader(BitConverter.GetBytes(30000), out largo, out borrado);
            Assert.AreEqual(30000,largo);
            Assert.AreEqual(false,borrado);

        }
    }

    class testmodel : ITestModel
    {
        public int value { get; set; }
        public double value3 { get; set; }
        public string text1 { get; set; }
        public string text2 { get; set; }
    }
    interface ITestModel
    {
        int value { get; }
        double value3 { get; }
        string text1 { get; }
        string text2 { get; }

    }
}
