using System;
using System.Collections.Generic;
using Moralis.WebGL.Platform.Abstractions;

namespace Moralis.WebGL.Platform.Operations
{
    class MoralisIncrementOperation : IMoralisFieldOperation
    {

        static IDictionary<Tuple<Type, Type>, Func<object, object, object>> Adders { get; } = new Dictionary<Tuple<Type, Type>, Func<object, object, object>>
        {
            [new Tuple<Type, Type>(typeof(sbyte), typeof(sbyte))] = (left, right) => (sbyte)left + (sbyte)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(short))] = (left, right) => (sbyte)left + (short)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(int))] = (left, right) => (sbyte)left + (int)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(long))] = (left, right) => (sbyte)left + (long)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(float))] = (left, right) => (sbyte)left + (float)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(double))] = (left, right) => (sbyte)left + (double)right,
            [new Tuple<Type, Type>(typeof(sbyte), typeof(decimal))] = (left, right) => (sbyte)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(byte))] = (left, right) => (byte)left + (byte)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(short))] = (left, right) => (byte)left + (short)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(ushort))] = (left, right) => (byte)left + (ushort)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(int))] = (left, right) => (byte)left + (int)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(uint))] = (left, right) => (byte)left + (uint)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(long))] = (left, right) => (byte)left + (long)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(ulong))] = (left, right) => (byte)left + (ulong)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(float))] = (left, right) => (byte)left + (float)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(double))] = (left, right) => (byte)left + (double)right,
            [new Tuple<Type, Type>(typeof(byte), typeof(decimal))] = (left, right) => (byte)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(short), typeof(short))] = (left, right) => (short)left + (short)right,
            [new Tuple<Type, Type>(typeof(short), typeof(int))] = (left, right) => (short)left + (int)right,
            [new Tuple<Type, Type>(typeof(short), typeof(long))] = (left, right) => (short)left + (long)right,
            [new Tuple<Type, Type>(typeof(short), typeof(float))] = (left, right) => (short)left + (float)right,
            [new Tuple<Type, Type>(typeof(short), typeof(double))] = (left, right) => (short)left + (double)right,
            [new Tuple<Type, Type>(typeof(short), typeof(decimal))] = (left, right) => (short)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(ushort))] = (left, right) => (ushort)left + (ushort)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(int))] = (left, right) => (ushort)left + (int)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(uint))] = (left, right) => (ushort)left + (uint)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(long))] = (left, right) => (ushort)left + (long)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(ulong))] = (left, right) => (ushort)left + (ulong)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(float))] = (left, right) => (ushort)left + (float)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(double))] = (left, right) => (ushort)left + (double)right,
            [new Tuple<Type, Type>(typeof(ushort), typeof(decimal))] = (left, right) => (ushort)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(int), typeof(int))] = (left, right) => (int)left + (int)right,
            [new Tuple<Type, Type>(typeof(int), typeof(long))] = (left, right) => (int)left + (long)right,
            [new Tuple<Type, Type>(typeof(int), typeof(float))] = (left, right) => (int)left + (float)right,
            [new Tuple<Type, Type>(typeof(int), typeof(double))] = (left, right) => (int)left + (double)right,
            [new Tuple<Type, Type>(typeof(int), typeof(decimal))] = (left, right) => (int)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(uint))] = (left, right) => (uint)left + (uint)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(long))] = (left, right) => (uint)left + (long)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(ulong))] = (left, right) => (uint)left + (ulong)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(float))] = (left, right) => (uint)left + (float)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(double))] = (left, right) => (uint)left + (double)right,
            [new Tuple<Type, Type>(typeof(uint), typeof(decimal))] = (left, right) => (uint)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(long), typeof(long))] = (left, right) => (long)left + (long)right,
            [new Tuple<Type, Type>(typeof(long), typeof(float))] = (left, right) => (long)left + (float)right,
            [new Tuple<Type, Type>(typeof(long), typeof(double))] = (left, right) => (long)left + (double)right,
            [new Tuple<Type, Type>(typeof(long), typeof(decimal))] = (left, right) => (long)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(char), typeof(char))] = (left, right) => (char)left + (char)right,
            [new Tuple<Type, Type>(typeof(char), typeof(ushort))] = (left, right) => (char)left + (ushort)right,
            [new Tuple<Type, Type>(typeof(char), typeof(int))] = (left, right) => (char)left + (int)right,
            [new Tuple<Type, Type>(typeof(char), typeof(uint))] = (left, right) => (char)left + (uint)right,
            [new Tuple<Type, Type>(typeof(char), typeof(long))] = (left, right) => (char)left + (long)right,
            [new Tuple<Type, Type>(typeof(char), typeof(ulong))] = (left, right) => (char)left + (ulong)right,
            [new Tuple<Type, Type>(typeof(char), typeof(float))] = (left, right) => (char)left + (float)right,
            [new Tuple<Type, Type>(typeof(char), typeof(double))] = (left, right) => (char)left + (double)right,
            [new Tuple<Type, Type>(typeof(char), typeof(decimal))] = (left, right) => (char)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(float), typeof(float))] = (left, right) => (float)left + (float)right,
            [new Tuple<Type, Type>(typeof(float), typeof(double))] = (left, right) => (float)left + (double)right,
            [new Tuple<Type, Type>(typeof(ulong), typeof(ulong))] = (left, right) => (ulong)left + (ulong)right,
            [new Tuple<Type, Type>(typeof(ulong), typeof(float))] = (left, right) => (ulong)left + (float)right,
            [new Tuple<Type, Type>(typeof(ulong), typeof(double))] = (left, right) => (ulong)left + (double)right,
            [new Tuple<Type, Type>(typeof(ulong), typeof(decimal))] = (left, right) => (ulong)left + (decimal)right,
            [new Tuple<Type, Type>(typeof(double), typeof(double))] = (left, right) => (double)left + (double)right,
            [new Tuple<Type, Type>(typeof(decimal), typeof(decimal))] = (left, right) => (decimal)left + (decimal)right
        };

        static object Add(object first, object second) => Adders.TryGetValue(new Tuple<Type, Type>(first.GetType(), second.GetType()), out Func<object, object, object> adder) ? adder(first, second) : throw new InvalidCastException($"Could not add objects of type {first.GetType()} and {second.GetType()} to each other.");


       // [JsonProperty("__op")]
        public string __op { get { return "Increment"; } }

        //[JsonProperty("amount")]
        public object amount { get; }

        public MoralisIncrementOperation(object amt) => amount = amt;



        public IMoralisFieldOperation MergeWithPrevious(IMoralisFieldOperation previous) => previous switch
        {
            null => this,
            MoralisDeleteOperation _ => new MoralisSetOperation(amount),

            // This may be a bug, but it was in the original logic.

            MoralisSetOperation { Value: string { } } => throw new InvalidOperationException("Cannot increment a non-number type."),
            MoralisSetOperation { Value: var value } => new MoralisSetOperation(Add(value, amount)),
            MoralisIncrementOperation { amount: var amt } => new MoralisIncrementOperation(Add(amt, amount)),
            _ => throw new InvalidOperationException("Operation is invalid after previous operation.")
        };

        public object Apply(object oldValue, string key)
        {
            if (oldValue is string) throw new InvalidOperationException("Cannot increment a non-number type.");

            if (oldValue == null) oldValue = 0;
            
            return Add(oldValue, amount);
        }

       

    }
}
