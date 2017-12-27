using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public class InMemRepository<TModel> : IRepository<TModel>, IRepositoryPersistence<TModel>
    {
        byte[] data;
        FieldInfo offset_field;
        FieldInfo data_field;

        MemoryStream new_data;
        Type emited_model_type;

        public InMemRepository(ref byte[] data)
        {
            this.data = data;

            //generamos el modelo que implementa la interfaz.
            this.emited_model_type = NetIMDB.Wrapping.WrapperTypeBuilder.GetWrapperType<TModel>();

            //offset property
            offset_field = emited_model_type.GetField("offset");
            data_field = emited_model_type.GetField("data");

            //Almacenará los nuevos registros, así evitamos redimensionar data con cada registro nuevo.
            new_data = new MemoryStream();
        }

        public void Add(TModel entity)
        {
            //serializamos el modelo y lo guardamos en new_data
            byte[] serialized_model = SerializationHelper.Serialize(entity);
            byte[] header = SerializationHelper.SerializeHeader(serialized_model.Length, false);
            new_data.Write(header, 0, Constants.RECORD_HEADER_SIZE);
            new_data.Write(serialized_model, 0, serialized_model.Length);
        }

        public void Remove(TModel entity)
        {
            //optimize: Este método puede ejecutarse mucho mas rápido.

            //obtenemos el offset del header que es justo 4bytes antes del offset de registro
            long offset = (long) offset_field.GetValue(entity) - Constants.RECORD_HEADER_SIZE;
            bool deleted;
            int length;

            byte[] record_data = (byte[]) data_field.GetValue(entity);
            SerializationHelper.DeSerializeHeader(record_data, offset, out length, out deleted);
            SerializationHelper.SerializeHeader(length, true).CopyTo(record_data, offset);
        }

        public IEnumerable<TModel> AsEnumerable()
        {

            var enumerators = new IEnumerator<TModel>[]{
                new InMemRecordEnumerator<TModel>(data, emited_model_type),
                new InMemRecordEnumerator<TModel>(new_data.ToArray(), emited_model_type)
            };
            
            //devolvemos un enumerador de mezcla entre data y new_data para que se enumeren también los nuevos registros.
            return new MergeEnumerable<TModel>(enumerators);
        }
        
        public void SaveToStream(Stream output_stream)
        {
            new MemoryStream(data) { Position = 0 }.CopyTo(output_stream);
            new_data.Position = 0;
            new_data.CopyTo(output_stream);
            new_data.Position = new_data.Length;
        }

        public void SaveToFile(string filename)
        {
            using (var fs = File.Create(filename))
            {
                SaveToStream(fs);
            }
        }

        public static InMemRepository<TModel> FromFile(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            return new InMemRepository<TModel>(ref data);
        }
    }
}
