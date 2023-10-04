using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
	{
        public static T CloneBinaryFormater<T>(this T source)
        {
            if (!typeof(T).IsSerializable) { throw new ArgumentException("The type must be serializable.", "source"); }
            
            if (ReferenceEquals(source, null)) { return default(T); }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T CloneNewtonsoftJson<T>(this T source)
        => ReferenceEquals(source, null) ? default(T) : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        
        public static T CloneUnityJson<T>(this T source)
        => ReferenceEquals(source, null) ? default(T) : JsonUtility.FromJson<T>(JsonUtility.ToJson(source));
    }
}
