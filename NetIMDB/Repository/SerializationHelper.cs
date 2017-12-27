using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Repository
{
    public static class SerializationHelper
    {
        /// <summary>
        /// Devuelve el objeto serializado sin incluir la cabecera con el tamaño y el estado de borrado.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static byte[] Serialize<TModel>(TModel graph)
        {
            if (!typeof(TModel).IsInterface)
            {
                throw new InvalidOperationException("El modelo <TModel> debe ser una interfaz");
            }

            MemoryStream result = new MemoryStream();

            var props = GetPropertiesOrderForInterface(typeof(TModel));
            //Obtenemos las propiedades en el orden de serialización y las serializamos en result
            foreach(var prop in props)
            {
                string method =  PointerCastHelper.GetTypeToByteMethod(prop.PropertyType);
                
                byte[] value = (byte[]) typeof(PointerCastHelper)
                    .GetMethod(method,new Type[]{prop.PropertyType})
                    .Invoke(null, new object[] { prop.GetValue(graph) });

                result.Write(value,0,value.Length);
            }

            return result.ToArray();
        }

        public static byte[] SerializeHeader(int record_length, bool deleted = false)
        {
            byte[] header = new byte[5];
            PointerCastHelper.IntToBytes(record_length).CopyTo(header, 0);

            if (deleted)
                header[4] = 0x01;

            return header;
        }

        public static void DeSerializeHeader(byte[] header,out int length, out bool deleted)
        {
            DeSerializeHeader(header, 0, out length, out deleted);
        }
        public static void DeSerializeHeader(byte[] data, long offset,out int length, out bool deleted)
        {
            /*Header:
             * byte[4] longitud <- delante para mantener el alineamiento de la memoria.
             * byte flags:
             *          0x01 = Borrado
             */

            length = PointerCastHelper.BytesToInt(data, offset);
            deleted = (data[offset + 4] & 0x01) == 0x01;
        }
        
        public static IEnumerable<System.Reflection.PropertyInfo> GetPropertiesOrderForInterface(Type TModel)
        {
            if (!TModel.IsInterface)
            {
                throw new InvalidOperationException("El modelo <TModel> debe ser una interfaz");
            }

            //obtenemos las propiedades del tipo de interfaz del modelo
            var properties = TModel
                .GetProperties()
                .OrderByDescending(x => PointerCastHelper.TypeSizeInMemory(x.PropertyType));//así quedan las de tamaño variable al final.
            
            return properties;
        }

    }
}
