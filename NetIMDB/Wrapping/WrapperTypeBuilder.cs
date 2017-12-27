using NetIMDB.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NetIMDB.Wrapping
{
    public static class WrapperTypeBuilder
    {
        static ModuleBuilder module_builder;

        static Dictionary<Type, Type> generated_types_cache = new Dictionary<Type,Type>();

        static WrapperTypeBuilder()
        {
            //inicialización de los constructores de tipo dinámico
            AssemblyName assembly_name = new AssemblyName(Constants.WRAPPER_ASSEMBLY_NAME);

            AssemblyBuilder assembly_builder =
                AppDomain.CurrentDomain
                .DefineDynamicAssembly(assembly_name, AssemblyBuilderAccess.Run);

            module_builder =
                assembly_builder.DefineDynamicModule(Constants.DYNAMIC_MODULE_NAME);
        }

        public static Type GetWrapperType<TModelInterface>()
        {
            ValidateSourceInterface(typeof(TModelInterface));

            //creamos el tipo.
            Type wrapper = BuildWrapperType(typeof(TModelInterface));

            return wrapper;
        }

        private static Type BuildWrapperType(Type interface_type)
        {
            //si anteriormente se había creado un tipo para esta interfaz lo devolvemos.
            if (generated_types_cache.ContainsKey(interface_type))
            {
                return generated_types_cache[interface_type];
            }

            string type_name =
                interface_type.FullName.Replace('.', '_') +
                Constants.WRAPPER_TYPE_NAME_SUFIX;

            //definimos la nueva classe wrapper.
            TypeBuilder type_builder = module_builder.DefineType(type_name,
                TypeAttributes.Class |
                TypeAttributes.Public |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass,
                null,
                Type.EmptyTypes);
            
            //Añadimos la implementación de la interfaz del modelo.
            type_builder.AddInterfaceImplementation(interface_type);

            //creamos una variable estática en cla clase que contendrá
            //un puntero al array de bytes del origen de datos.
            FieldBuilder data_field = type_builder
                .DefineField("data", typeof(Byte[]),
                FieldAttributes.Public | FieldAttributes.Static);

            //creamos otra variable para almacenar el offset del registro actual
            FieldBuilder offset_field = type_builder
                .DefineField("offset", typeof(long), FieldAttributes.Public);

            //obtenemos las propiedades del tipo de interfaz del modelo
            var properties = SerializationHelper.GetPropertiesOrderForInterface(interface_type);

            //offset de la propiedad dentro del conjunto de bytes del registro.
            int prop_offset = 0;
            int first_string_offset = -1;
            int string_index = 0;

            //implementamos las propiedades
            foreach (var prop in properties)
            {
                //método de transformación del array de bytes al tipo de la propiedad.
                string cast_method_name = PointerCastHelper.GetByteToTypeMethod(prop.PropertyType);

                PropertyBuilder prop_builder = type_builder
                    .DefineProperty(prop.Name,
                    PropertyAttributes.HasDefault,
                    prop.PropertyType,
                    Type.EmptyTypes);

                //definimos el método get
                MethodBuilder pGet = type_builder.DefineMethod("get_" + prop.Name,
                MethodAttributes.Virtual |
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                prop.PropertyType, Type.EmptyTypes);

                var getIl = pGet.GetILGenerator();

                //emitimos el código que convierte el array de bytes encontrado en el 
                //offset de la variable del modelo a un puntero del tipo de retorno.
                if (prop.PropertyType == typeof(string))
                {
                    if (first_string_offset == -1)
                        first_string_offset = prop_offset;

                    //llamámos al método que nos dá el offset de este índice de string. GetStringOffsetAtIndex
                    getIl.Emit(OpCodes.Ldsfld, data_field);
                    getIl.Emit(OpCodes.Ldarg_0);
                    //sumamos el offset del primer campo con el del registro
                    getIl.Emit(OpCodes.Ldfld, offset_field);
                    getIl.Emit(OpCodes.Ldc_I4, prop_offset);
                    getIl.Emit(OpCodes.Conv_I8);
                    getIl.Emit(OpCodes.Add);
                    //añadimos el indice del string que estamos cargando.
                    getIl.Emit(OpCodes.Ldc_I4, string_index++);
                    //obtenemos el offset del string.
                    getIl.Emit(OpCodes.Call, typeof(PointerCastHelper).GetMethod("GetStringAtOffset"));
                    getIl.Emit(OpCodes.Ret);

                }
                else
                {
                    getIl.Emit(OpCodes.Nop);
                    getIl.Emit(OpCodes.Ldsfld, data_field);
                    getIl.Emit(OpCodes.Ldarg_0);
                    getIl.Emit(OpCodes.Ldfld, offset_field);
                    getIl.Emit(OpCodes.Ldc_I4, prop_offset);
                    getIl.Emit(OpCodes.Conv_I8);
                    getIl.Emit(OpCodes.Add);
                    getIl.Emit(OpCodes.Call, typeof(PointerCastHelper).GetMethod(PointerCastHelper.GetByteToTypeMethod(prop.PropertyType)));
                    getIl.Emit(OpCodes.Ret);
                    prop_offset += PointerCastHelper.TypeSizeInMemory(prop.PropertyType);
                }

                prop_builder.SetGetMethod(pGet);

                type_builder.DefineMethodOverride(pGet, interface_type.GetMethod("get_" + prop.Name));
                
            }
            

            //Añadimos el nuevo tipo a la caché de creación.
            Type created =  type_builder.CreateType();
            generated_types_cache.Add(interface_type, created);
            return created;
        }

        public static void ValidateSourceInterface(Type interfaceType)
        {
            Type[] allowed_property_types = new Type[] { 
                typeof(int), 
                typeof(long), 
                typeof(short), 
                typeof(byte), 
                typeof(char), 
                typeof(double), 
                typeof(string), 
                typeof(bool) };

            //Forzamos a las buenas prácticas. Los modelos deben ser interfaces.
            if (!interfaceType.IsInterface)
                throw new NotSupportedException("El tipo de modelo no está soportado, únicamente se admiten interfaces como parámetro genérico.");

            var properties = interfaceType.GetProperties();
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(decimal))
                    throw new InvalidOperationException("No se permiten campos decimales, considere usar double en su lugar.");
                else
                {
                    if (!allowed_property_types.Contains(prop.PropertyType))
                        throw new InvalidOperationException(string.Format("El tipo de la propiedad {0} no es válido, sólo se permiten propiedades de los siguientes tipos primitivo (int, long, short, byte, double, string, bool)", prop.Name));
                }

                if (!prop.GetMethod.IsPublic)
                    throw new InvalidOperationException(string.Format("La propiedad {0} es inválida, el método get debe ser público.", prop.Name));
            }
        }


    }
}
