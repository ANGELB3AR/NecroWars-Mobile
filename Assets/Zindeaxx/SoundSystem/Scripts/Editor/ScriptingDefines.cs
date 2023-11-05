using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Zindeaxx.SoundSystem
{

    public static class DefineSymbols
    {
        private const char DEFINE_SEPARATOR = ';';
        private static readonly List<string> _scriptingDefines = new List<string>();

        public static void Add(params string[] extraDefines)
        {
            _scriptingDefines.Clear();
            _scriptingDefines.AddRange(GetDefines());
            _scriptingDefines.AddRange(extraDefines.Except(_scriptingDefines));
            UpdateDefines(_scriptingDefines);
        }

        public static void Remove(params string[] extraDefines)
        {
            _scriptingDefines.Clear();
            _scriptingDefines.AddRange(GetDefines().Except(extraDefines));
            UpdateDefines(_scriptingDefines);
        }

        public static void Clear()
        {
            _scriptingDefines.Clear();
            UpdateDefines(_scriptingDefines);
        }

        private static IEnumerable<string> GetDefines() => PlayerSettings.GetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup).Split(DEFINE_SEPARATOR).ToList();

        private static void UpdateDefines(List<string> allDefines) => PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(DEFINE_SEPARATOR.ToString(),
                        allDefines.ToArray()));

    }

}