using System;
using System.Collections.Generic;

namespace Table
{
    public static partial class MotorcycleTable
    {
        public class Value
        {
            /// <summary>
            /// id
            /// </summary>
            public int Id {get; internal set;}
            /// <summary>
            /// 预制
            /// </summary>
            public string EntityName {get; internal set;}
        }
        
        public static Value[] values;
        public static Dictionary<int, Value> valueDic = new Dictionary<int, Value>();
        public static Value GetValueOrThrErr(int id)
        {
            if (valueDic.TryGetValue(id,out var result))
            {
                return result;
            }
            throw new Exception($"Motorcycle表没有id {id},请检查");
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
                {
                    var stringByteLength = BitConverter.ToInt16(bytes, readPosition);
                    readPosition += 2;
                    if (stringByteLength == 0)
                    {
                        value.EntityName = "";
                    }
                    else
                    {
                        value.EntityName = System.Text.Encoding.UTF8.GetString(bytes, readPosition, stringByteLength);
                        readPosition += stringByteLength;
                    }
                }
                
                values[i] = value;
                valueDic.Add(value.Id, value);
            }
        }
    }
}
