using System;
using System.Collections.Generic;

namespace Table
{
    public static partial class ScrollSpeedTable
    {
        public class Value
        {
            /// <summary>
            /// id
            /// </summary>
            public int Id {get; internal set;}
            /// <summary>
            /// maxtime
            /// </summary>
            public float MaxTime {get; internal set;}
            /// <summary>
            /// current speed
            /// </summary>
            public float ScrollSpeed {get; internal set;}
        }
        
        public static Value[] values;
        public static Dictionary<int, Value> valueDic = new Dictionary<int, Value>();
        public static Value GetValueOrThrErr(int id)
        {
            if (valueDic.TryGetValue(id,out var result))
            {
                return result;
            }
            throw new Exception($"ScrollSpeed表没有id {id},请检查");
        }
        
        public static void Parse(byte[] bytes)
        {
            var readPosition = 0;
            var valueCount = BitConverter.ToInt32(bytes, readPosition);
            readPosition += 4;
            values = new Value[valueCount];
            valueDic.Clear();
            
            for (var i = 0; i < valueCount; i++)
            {
                var value = new Value();
                value.Id = BitConverter.ToInt32(bytes, readPosition);
                readPosition += 4;
                value.MaxTime = BitConverter.ToSingle(bytes, readPosition);
                readPosition += 4;
                value.ScrollSpeed = BitConverter.ToSingle(bytes, readPosition);
                readPosition += 4;
                
                values[i] = value;
                valueDic.Add(value.Id, value);
            }
        }
    }
}
