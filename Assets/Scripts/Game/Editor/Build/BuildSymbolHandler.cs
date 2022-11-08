using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Build
{
    public static class BuildSymbolHandler
    {
        public static bool IsSymbolEnabled(string symbol)
        {
            return IsSymbolEnabled(symbol, BuildTargetGroup.Android) && IsSymbolEnabled(symbol, BuildTargetGroup.iOS) &&
                   IsSymbolEnabled(symbol, BuildTargetGroup.Standalone);
        }

        public static bool IsAnySymbolEnabled(string[] symbols)
        {
            foreach (var symbol in symbols)
            {
                if (IsSymbolEnabled(symbol))
                {
                    return true;
                }
            }

            return false;
        }

        public static void EnableSymbol(string symbol)
        {
            EnableSymbol(symbol, BuildTargetGroup.Android);
            EnableSymbol(symbol, BuildTargetGroup.iOS);
            EnableSymbol(symbol, BuildTargetGroup.Standalone);
            Debug.Log($"开启Symbol {symbol}");
        }

        public static void DisableSymbol(string symbol)
        {
            DisableSymbol(symbol, BuildTargetGroup.Android);
            DisableSymbol(symbol, BuildTargetGroup.iOS);
            DisableSymbol(symbol, BuildTargetGroup.Standalone);
            Debug.Log($"关闭symbol {symbol}");
        }

        private static bool IsSymbolEnabled(string symbol, BuildTargetGroup buildTargetGroup)
        {
            var defineSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defineSymbols = defineSymbolString.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            return defineSymbols.Contains(symbol);
        }

        private static void EnableSymbol(string symbol, BuildTargetGroup buildTargetGroup)
        {
            var defineSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defineSymbols = defineSymbolString.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (defineSymbols.Contains(symbol))
            {
                return;
            }

            defineSymbols.Add(symbol);
            var result = string.Join(";", defineSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, result);
        }

        private static void DisableSymbol(string symbol, BuildTargetGroup buildTargetGroup)
        {
            var defineSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defineSymbols = defineSymbolString.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (defineSymbols.Contains(symbol))
            {
                defineSymbols.Remove(symbol);
                var result = string.Join(";", defineSymbols);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, result);
            }
        }
    }
}