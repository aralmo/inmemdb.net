using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB
{
    //ToDo: implementar todos los tipos primitivos. 
    //char, boolean, Date, byte

    public unsafe static class PointerCastHelper
    {
        public static short BytesToShort(byte[] data, long offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return *(short*)ptr;
            }
        }
        public static byte[] ShortToBytes(short value)
        {
            byte[] bytes = new byte[2];
            fixed (byte* ptr = bytes)
            {
                *((short*)ptr) = value;
            }
            return bytes;
        }

        public static int BytesToInt(byte[] data, long offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return *(int*)ptr;
            }
        }
        public static byte[] IntToBytes(int value)
        {
            byte[] bytes = new byte[4];
            fixed (byte* b = bytes)
            {
                *((int*)b) = value;
            }

            return bytes;
        }
        public static long BytesToLong(byte[] data, long offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return *((long*)ptr);
            }
        }
        public static byte[] LongToBytes(long value)
        {
            byte[] bytes = new byte[8];
            fixed (byte* ptr = bytes)
            {
                *((long*)ptr) = value;
            }
            return bytes;
        }
        public static double BytesToDouble(byte[] data, long offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return *(double*)ptr;
            }
        }
        public static byte[] DoubleToBytes(double value)
        {
            byte[] bytes = new byte[8];
            fixed (byte* b = bytes)
            {
                *((double*)b) = value;
            }

            return bytes;
        }
        
        public static int TypeSizeInMemory(Type type)
        {
            //no hay necesidad de rendimiento en este punto, estamos cargando el ensamblado.
            return new Dictionary<Type, int>()
            {
                { typeof(int),4 },
                { typeof(long),8 },
                { typeof(short),2 },
                { typeof(byte),1 },
                { typeof(double),8 },
                { typeof(bool),1 },
                { typeof(string),-1 }//tamaño variable
            }[type];

        }
        public static string GetByteToTypeMethod(Type type)
        {
            if (type == typeof(int))
                return "BytesToInt";

            if (type == typeof(long))
                return "BytesToLong";

            if (type == typeof(short))
                return "BytesToShort";

            if (type == typeof(char))
                return "BytesToChar";

            if (type == typeof(double))
                return "BytesToDouble";

            if (type == typeof(string))
                return "BytesToString";

            if (type == typeof(bool))
                return "BytesToBool";

            return string.Empty;
        }
        public static string GetTypeToByteMethod(Type type)
        {
            if (type == typeof(int))
                return "IntToBytes";

            if (type == typeof(long))
                return "LongToBytes";

            if (type == typeof(short))
                return "ShortToBytes";

            if (type == typeof(char))
                return "CharToBytes";

            if (type == typeof(double))
                return "DoubleToBytes";

            if (type == typeof(string))
                return "StringToBytes";

            if (type == typeof(bool))
                return "BoolToBytes";

            return string.Empty;
        }
 
        /*
          Cabecera del string
         ---------------------
          4bytes - Longitud(cantidad de bytes)
          4bytes - Encoding
          nbytes - Contenido         
        */

        public static string BytesToString(byte[] data, long offset)
        {
            int encoding = BytesToShort(data, offset + 4);
            int length = BytesToInt(data, offset);

            //creo un buffer porque GetString de encoding no acepta un long como offset.
            byte[] buffer = new byte[length];
            Array.Copy(data, offset + 8, buffer, 0, length);

            return Encoding.GetEncoding(encoding).GetString(buffer);
        }
        public static byte[] StringToBytes(string value)
        {
            return StringToBytes(value, 1252);//ANSI-Latin
        }
        public static byte[] StringToBytes(string value, int codepage)
        {
            if (value == null)
                return Constants.EMPTY_STRING;

            Encoding encoding = Encoding.GetEncoding(codepage);
            int text_bytes_count = encoding.GetByteCount(value);
            byte[] bytes = new byte[text_bytes_count + 8];

            IntToBytes(text_bytes_count).CopyTo(bytes, 0);
            IntToBytes(encoding.CodePage).CopyTo(bytes, 4);

            encoding.GetBytes(value).CopyTo(bytes, 8);

            return bytes;
        }

        public static long GetStringOffsetAtIndex(byte[] data, long first_text_offset, int text_index)
        {
            long offset = first_text_offset;
            for (int n = 0; n < text_index; n++)
            {
                fixed (byte* ptr = &data[offset])
                {
                    //avanzamos hasta el siguiente texto
                    offset += (*(int*)ptr) + 8;
                }
            }

            return offset;
        }
        public static string GetStringAtOffset(byte[] data, long first_text_offset, int text_index)
        {
            long offset = GetStringOffsetAtIndex(data,first_text_offset, text_index);
            return PointerCastHelper.BytesToString(data, offset);            
        }
    }
}
