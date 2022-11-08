#if UNITY_EDITOR
using System;

namespace Game.GameSystem
{
    /// <summary>
    /// 记录了编辑器中可用的进入System
    /// </summary>
    public static class EntrySystemConfig
    {
        public const string SystemNameIndexSaveKey = "Editor_SystemNameIndex";

        private static readonly Type[] availableTypes =
        {
            typeof(GameStartSystem),
        };

        private static string[] entrySystemNames;
        public static string[] EntrySystemNames
        {
            get
            {
                if (entrySystemNames == null)
                {
                    entrySystemNames = new string[availableTypes.Length];
                    for (var i = 0; i < availableTypes.Length; i++)
                    {
                        entrySystemNames[i] = availableTypes[i].Name.Replace("System", "");
                    }
                }

                return entrySystemNames;
            }
        }

        public static SystemBase GetEntrySystem(int systemNameIndex)
        {
            var systemType = availableTypes[0];
            if (systemNameIndex >= 0 && systemNameIndex < availableTypes.Length)
            {
                systemType = availableTypes[systemNameIndex];
            }

            var instance = Activator.CreateInstance(systemType);

            return instance as SystemBase;
        }
    }
}
#endif