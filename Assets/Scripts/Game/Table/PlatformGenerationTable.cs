using System;
using System.Collections.Generic;

namespace Table
{
    public static partial class PlatformGenerationTable
    {
        public class Value
        {
            /// <summary>
            /// id
            /// </summary>
            public int Id {get; internal set;}
            /// <summary>
            /// Min Surviving Time (seconds)
            /// </summary>
            public float SurvivngMin {get; internal set;}
            /// <summary>
            /// Max Surviving Time
            /// </summary>
            public float SurvivngMax {get; internal set;}
            /// <summary>
            /// Platform IDs
            /// </summary>
            public int[] PlatformIdArray {get; internal set;}
        }
        
        public static Value[] values;
        public static Dictionary<int, Value> valueDic = new Dictionary<int, Value>();
        public static Value GetValueOrThrErr(int id)
        {
            if (valueDic.TryGetValue(id,out var result))
            {
                return result;
            }
            throw new Exception($"PlatformGeneration表没有id {id},请检查");
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
                value.SurvivngMin = BitConverter.ToSingle(bytes, readPosition);
                readPosition += 4;
                value.SurvivngMax = BitConverter.ToSingle(bytes, readPosition);
                readPosition += 4;
                {
                    var arrayLength = BitConverter.ToInt32(bytes, readPosition);
                    readPosition += 4;
                    value.PlatformIdArray = new int[arrayLength];
                    for(var j = 0; j < arrayLength; j++)
                    {
                        value.PlatformIdArray[j] = BitConverter.ToInt32(bytes, readPosition);
                        readPosition += 4;
                    }
                }
                
                values[i] = value;
                valueDic.Add(value.Id, value);
            }
        }
    }
}
