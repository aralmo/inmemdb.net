using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetIMDB;
using System.Text;
using System.IO;

namespace NetIMDB_Tests
{
    [TestClass]
    public class PointerCastHelper_Tests
    {
        [TestMethod]
        public void Int()
        {
            var real_bytes = BitConverter.GetBytes(12345678);
            Assert.AreEqual(12345678, PointerCastHelper.BytesToInt(real_bytes, 0), "No se convirtió el puntero correctamente");

            Assert.AreEqual(real_bytes[0], PointerCastHelper.IntToBytes(12345678)[0], "No se convirtió a un array de bytes correctamente");
            Assert.AreEqual(real_bytes[1], PointerCastHelper.IntToBytes(12345678)[1], "No se convirtió a un array de bytes correctamente");
            Assert.AreEqual(real_bytes[2], PointerCastHelper.IntToBytes(12345678)[2], "No se convirtió a un array de bytes correctamente");
            Assert.AreEqual(real_bytes[3], PointerCastHelper.IntToBytes(12345678)[3], "No se convirtió a un array de bytes correctamente");
        }
        [TestMethod]
        public void Double()
        {
            var real_bytes = BitConverter.GetBytes(12345678.23);
            Assert.AreEqual(12345678.23, PointerCastHelper.BytesToDouble(real_bytes, 0), "No se convirtió el puntero correctamente");

            for (int n = 0; n < 8; n++)
            {
                Assert.AreEqual(real_bytes[n], PointerCastHelper.DoubleToBytes(12345678.23)[n], "No se hizo la conversión correctamente");
            }
        }
        [TestMethod]
        public void Short()
        {
            var real_bytes = BitConverter.GetBytes(short.MaxValue);
            Assert.AreEqual(short.MaxValue, PointerCastHelper.BytesToShort(real_bytes, 0), "No se convirtió el puntero correctamente");

            for (int n = 0; n < 2; n++)
            {
                Assert.AreEqual(real_bytes[n], PointerCastHelper.ShortToBytes(short.MaxValue)[n], "No se hizo la conversión correctamente");
            }
        }
        [TestMethod]
        public void Long()
        {
            var real_bytes = BitConverter.GetBytes(long.MaxValue);
            Assert.AreEqual(long.MaxValue, PointerCastHelper.BytesToLong(real_bytes, 0), "No se convirtió el puntero correctamente");
            Assert.AreEqual(BitConverter.ToInt64(PointerCastHelper.LongToBytes(long.MaxValue), 0), long.MaxValue, "No se reconvirtieron los bytes correctamente");
        }
        [TestMethod]
        public void String()
        {
            string test = "String de prueba para el test";

            string result = PointerCastHelper.BytesToString(
            PointerCastHelper.StringToBytes(test), 0);

            Assert.AreEqual(test, result, "Los textos no coinciden");

        }
        [TestMethod]
        public void GetStringOffsetAtIndex()
        {
            //cargamos 3 strings en un array
            MemoryStream ms = new MemoryStream();
            byte[] bytes = PointerCastHelper.StringToBytes("texto 1");
            ms.Write(bytes,0,bytes.Length);

            bytes = PointerCastHelper.StringToBytes("texto 2");
            ms.Write(bytes,0,bytes.Length);

            bytes = PointerCastHelper.StringToBytes("texto 3");
            ms.Write(bytes,0,bytes.Length);

            byte[] data = ms.ToArray();

            long offset2 = PointerCastHelper.GetStringOffsetAtIndex(data, 0, 1);
            long offset3 = PointerCastHelper.GetStringOffsetAtIndex(data, 0, 2);            
            Assert.AreEqual(15,offset2,"El offset del segundo string debería ser 16");

            Assert.AreEqual("texto 1", PointerCastHelper.BytesToString(data, 0), "No coinciden los textos");
            Assert.AreEqual("texto 2", PointerCastHelper.BytesToString(data, offset2), "No coinciden los textos");
            Assert.AreEqual("texto 3", PointerCastHelper.BytesToString(data, offset3), "No coinciden los textos");

        }
    }
}
