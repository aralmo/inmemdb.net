using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetIMDB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB_Tests
{
    [TestClass]
    public class InMEMRepositoryTests
    {
        [TestMethod]
        public void InMemRepository_Add()
        {
            byte[] data = new byte[0];
            var repository = new InMemRepository<IrepoModel>(ref data);
            repository.Add(new repoTestModel() { Campo1 = 10006, Campo2 = 10009 });
            repository.Add(new repoTestModel() { Campo1 = 10004, Campo2 = 10002 });

            var enumerator = repository.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            IrepoModel model = (IrepoModel)enumerator.Current;
            enumerator.MoveNext();
            model = (IrepoModel)enumerator.Current;

            repository.SaveToFile(".\\diario.iaj");
        }

        [TestMethod]
        public void InMemRepository_StringsSerialization()
        {
            byte[] data = new byte[0];
            var repository = new InMemRepository<imodel2>(ref data);

            repository.Add(new model2()
            {
                field1 = 1,
                text1 =  "texto 1-1",
                text2 = "texto 1-2"
            });

            repository.Add(new model2()
            {
                field1 = 2,
                text1 = "texto 2-1",
                text2 = "texto 2-2"
            });

            repository.SaveToFile("diario_str.iaj");

            var repository2 = InMemRepository<imodel2>.FromFile("diario_str.iaj");

            int n = 1;
            foreach (imodel2 record in repository2.AsEnumerable())
            {
                Assert.AreEqual(record.field1, n);
                Assert.AreEqual(record.text1, string.Format("texto {0}-1",n));
                Assert.AreEqual(record.text2, string.Format("texto {0}-2",n));
                n++;
            }
        }

        [TestMethod]
        public void InMemRepository_Delete()
        {
            byte[] data = new byte[0];
            var repository = new InMemRepository<IrepoModel>(ref data);
            repository.Add(new repoTestModel() { Campo1 = 10006, Campo2 = 10009 });
            repository.Add(new repoTestModel() { Campo1 = 10004, Campo2 = 10002 });

            var enumerator = repository.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            IrepoModel model = (IrepoModel)enumerator.Current;

            repository.Remove(model);

            enumerator.Reset();
            enumerator.MoveNext();
            //comprobamos que se haya saltado el primer registro en esta iteración.
            Assert.AreEqual(10004,(enumerator.Current as IrepoModel).Campo1);

        }
        
        
        public interface imodel2
        {
            int field1 { get; }
            int field2 { get; }
            string text1 { get; }
            string text2 { get; }
        }
        public class model2:imodel2
        {
            public int field1
            {
                get;
                set;
            }
            public int field2
            {
                get;
                set;
            }
            public string text1
            {
                get;
                set;
            }

            public string text2
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void MergerEnumeratorTest()
        {
            int[] lista1 = new int[] { 1, 2, 3, 4, 5 };
            int[] lista2 = new int[] { 6,7,8,9,10};

            var merged = new MergedEnumerator<int>(
                new IEnumerator<int>[] 
                { 
                    lista1.AsEnumerable().GetEnumerator(), 
                    lista2.AsEnumerable().GetEnumerator() 
                });

            int suma = 0;
            foreach (var item in merged)
            {
                suma += item;
            }

            Assert.AreEqual(55, suma,"No se enumeraron todos los elementos.");
        }

        public interface IrepoModel
        {
            int Campo1 { get; }
            int Campo2 { get; }
        }
        public class repoTestModel : IrepoModel
        {

            public int Campo1
            {
                get;
                set;
            }

            public int Campo2
            {
                get;
                set;
            }
        }
    }
}
