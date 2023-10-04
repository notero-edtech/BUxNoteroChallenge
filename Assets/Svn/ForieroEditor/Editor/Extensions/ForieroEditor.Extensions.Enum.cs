using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Extensions
{
    public static partial class ForieroEditorExtensions
    {
        public static bool IsObsolete(this Enum value, List<string> includes, List<string> excludes)
        {
            if (includes != null && includes.Contains(value.ToString())) return true;
            if (excludes != null && excludes.Contains(value.ToString())) return false;
            
            var enumType = value.GetType();
            //var enumName = enumType.GetEnumName(value);
            var enumName = value.ToString();
            var fieldInfo = enumType.GetField(enumName);
            return Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));
        }

        public static bool IsObsolete(this Enum value)
        {
            var enumType = value.GetType();
            var enumName = enumType.GetEnumName(value);
            var fieldInfo = enumType.GetField(enumName);
            return Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute));
        }

        public static void AddSymbolDefine(this BuildTargetGroup buildTargetGroup, string define)
        {
            //Debug.Log($"MPB | AddSymbolDefine {buildTargetGroup} {buildTargetGroup.ToString()} : {define}");
            if (buildTargetGroup == BuildTargetGroup.Unknown) return;
            if (buildTargetGroup.HasSymbolDefine(define)) return;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defines = new List<string>(symbols.Split(';'));
            defines.Add(define);           
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defines.ToArray()));
        }

        public static bool HasSymbolDefine(this BuildTargetGroup buildTargetGroup, string define)
        {
            //Debug.Log($"MPB | HasSymbolDefine {buildTargetGroup} {buildTargetGroup.ToString()} : {define}");
            if (buildTargetGroup == BuildTargetGroup.Unknown) return false;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defines = new List<string>(symbols.Split(';'));
            return defines.Any((d) => d.Equals(define));     
        }

        public static void RemoveSymbolDefine(this BuildTargetGroup buildTargetGroup, string define)
        {
            //Debug.Log($"MPB | RemoveSymbolDefine {buildTargetGroup} {buildTargetGroup.ToString()} : {define}");
            if (buildTargetGroup == BuildTargetGroup.Unknown) return;
            if (!buildTargetGroup.HasSymbolDefine(define)) return;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var defines = new List<string>(symbols.Split(';'));
            defines.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defines.ToArray()));
        }
    }
}
