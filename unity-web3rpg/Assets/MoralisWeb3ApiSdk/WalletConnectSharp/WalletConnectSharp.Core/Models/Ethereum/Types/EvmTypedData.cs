using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace WalletConnectSharp.Core.Models.Ethereum.Types
{
    public class EvmTypedData<T>
    {
        public static readonly Dictionary<Type, string> TypeMap = new Dictionary<Type, string>()
        {
            {typeof(Address), "address"},
            {typeof(bool), "bool"},
            {typeof(int), "uint32"},
            {typeof(long), "int64"},
            {typeof(uint), "uint32"},
            {typeof(ulong), "uint64"},
            {typeof(short), "int16"},
            {typeof(ushort), "uint16"},
            {typeof(byte), "int8"},
            {typeof(sbyte), "uint8"},
            {typeof(string), "string"},
        };

        public Dictionary<string, EvmTypeInfo[]> types = new Dictionary<string, EvmTypeInfo[]>();
        public string primaryType;
        public EIP712Domain domain;
        public T message;

        public EvmTypedData(T data, EIP712Domain domain)
        {
            this.message = data;
            this.domain = domain;
            this.primaryType = typeof(T).Name;

            AddTypeData(typeof(EIP712Domain));
            AddTypeData(typeof(T));
        }

        public void AddTypeData(Type type)
        {
            var tname = type.Name;
            if (types.ContainsKey(tname))
                return;
            
            List<EvmTypeInfo> infos = new List<EvmTypeInfo>();
            BindingFlags bindingFlags = BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance;

            foreach (var field in type.GetFields(bindingFlags))
            {
                string name = field.Name;
                var fieldType = field.FieldType;
                var evmType = (EvmTypeAttribute)field.GetCustomAttribute(typeof(EvmTypeAttribute), true);
                var shouldIgnore = (EvmIgnoreAttribute) field.GetCustomAttribute(typeof(EvmIgnoreAttribute), true);

                if (shouldIgnore != null)
                    continue;
                
                
                string typeName;
                if (evmType != null)
                {
                    typeName = evmType.TypeName;
                } 
                else if (TypeMap.ContainsKey(fieldType))
                {
                    typeName = TypeMap[fieldType];
                }
                else if (
                    (type.IsValueType && !type.IsPrimitive) ||
                    (type.IsClass)
                    )
                {
                    AddTypeData(fieldType);
                    typeName = fieldType.Name;
                }
                else
                {
                    throw new SerializationException("Field " + name + " has no valid EVM type mapping. Try adding a [EvmType(\"...\")] to this field");
                }

                var typeInfo = new EvmTypeInfo(name, typeName);
                
                infos.Add(typeInfo);
            }

            types.Add(tname, infos.ToArray());
        }
    }
}