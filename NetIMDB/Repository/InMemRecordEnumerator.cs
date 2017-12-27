using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public class InMemRecordEnumerator<TModel> : IEnumerator<TModel>
    {
        byte[] data;
        long current_offset = -1;
        Type model_wrapper_type;
        
        FieldInfo data_property;
        FieldInfo offset_property;

        public InMemRecordEnumerator(byte[] data, Type model_wrapper_type)
        {
            this.model_wrapper_type = model_wrapper_type;
            data_property = model_wrapper_type.GetField("data");
            offset_property = model_wrapper_type.GetField("offset");

            this.data = data;
        }

        public TModel Current
        {
            get { return (TModel)GetCurrent(); }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get
            {
                return GetCurrent();
            }
        }

        private object GetCurrent()
        {
            return InstanciateModel(current_offset + Constants.RECORD_HEADER_SIZE); //quitamos el entero que nos indica el tamaño.
        }

        private object InstanciateModel(long offset)
        {
            var instance = Activator.CreateInstance(model_wrapper_type);
            data_property.SetValue(instance, data);
            offset_property.SetValue(instance, offset);

            return instance;
        }

        public bool MoveNext()
        {
            if (data.Length == 0)
                return false;

            if (current_offset == -1)
            {
                current_offset = 0;
                
                //todo: optimize! importante que esto sea óptimo dado que es el iterador principal
                //movemos el cursor hacia adelante el largo del registro.
                bool deleted;
                int length;
                SerializationHelper
                    .DeSerializeHeader(data, current_offset, out length, out deleted);

                if (deleted)
                    return MoveNext();

                if (data.Length > 0)
                    return true;
            }
            else
            {
                //todo: optimize! importante que esto sea óptimo dado que es el iterador principal
                int length = PointerCastHelper.BytesToInt(data, current_offset);
                //movemos el cursor hacia adelante el largo del registro.
                current_offset += length + Constants.RECORD_HEADER_SIZE;

                if (current_offset >= data.Length)
                    return false;

                bool deleted = (data[current_offset + 5] & 0x01) == 0x01;
                if (deleted)
                {
                    return MoveNext();
                }

                if (data.Length > current_offset)//???
                    return true;                

            }

            return false;
        }

        public void Reset()
        {
            current_offset = -1;
        }
    }
}
