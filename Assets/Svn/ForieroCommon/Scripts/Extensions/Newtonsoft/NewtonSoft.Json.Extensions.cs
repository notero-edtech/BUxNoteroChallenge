using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json.Schema;
//using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ForieroEngine
{
    public static class NewtonSoftJsonExtensions
    {
        public static T TryParse<T>(this string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData)) return default;

            T t;

            try { t = JsonConvert.DeserializeObject<T>(jsonData); }
            catch (System.Exception) { t = default; }
            return t;

            //JSchemaGenerator generator = new JSchemaGenerator();
            //JSchema parsedSchema = generator.Generate(typeof(T));
            //JObject jObject = JObject.Parse(jsonData);

            //if (jObject.IsValid(parsedSchema))
            //{
            //    return JsonConvert.DeserializeObject<T>(jsonData);
            //}
            //else
            //{
            //    if (Debug.isDebugBuild) { Debug.LogError("INVALID JSON DATA FOR CLASS : " + typeof(T)); }
            //    return default(T);
            //}
        }
    }
}
