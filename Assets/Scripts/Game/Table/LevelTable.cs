using System;
using System.Collections.Generic;

namespace Table
{
    public static partial class LevelTable
    {
        public class Value
        {
            /// <summary>
            /// id
            /// </summary>
            public int Id {get; internal set;}
            /// <summary>
            /// 起点路段id
            /// </summary>
            public int StartRoadId {get; internal set;}
            /// <summary>
            /// 赛道路段id
            /// </summary>
            public int[] RoadIdsArray {get; internal set;}
            /// <summary>
            /// 终点路段ID
            /// </summary>
            public int FinishRoadId {get; internal set;}
            /// <summary>
            /// AI数量
            /// </summary>
            public int AiCount {get; internal set;}
        }
        
        public static Value[] values;
        public static Dictionary<int, Value> valueDic = new Dictionary<int, Value>();
        public static Value GetValueOrThrErr(int id)
        {
            if (valueDic.TryGetValue(id,out var result))
            {
                return result;
            }
            throw new Exception($"Level表没有id {id},请检查");
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
                value.StartRoadId = BitConverter.ToInt32(bytes, readPosition);
                readPosition += 4;
                {
                    var arrayLength = BitConverter.ToInt32(bytes, readPosition);
                    readPosition += 4;
                    value.RoadIdsArray = new int[arrayLength];
                    for(var j = 0; j < arrayLength; j++)
                    {
                        value.RoadIdsArray[j] = BitConverter.ToInt32(bytes, readPosition);
                        readPosition += 4;
                    }
                }
                value.FinishRoadId = BitConverter.ToInt32(bytes, readPosition);
                readPosition += 4;
                value.AiCount = BitConverter.ToInt32(bytes, readPosition);
                readPosition += 4;
                
                values[i] = value;
                valueDic.Add(value.Id, value);
            }
        }
    }
}
