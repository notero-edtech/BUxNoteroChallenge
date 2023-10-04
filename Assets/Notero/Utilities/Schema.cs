using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Notero.Utilities
{
    public abstract class Schema<T> where T : class
    {
        protected T CurrentSchema;

        public virtual T Deserialize(string json) { return default; }

        private string m_AppVersion;

        protected Schema(string appVersion)
        {
            m_AppVersion = appVersion;
        }

        protected bool Reader<TSchema>(string json, out TSchema result, bool isCheckSchema = true) where TSchema : new()
        {
            result = default;
            JToken token = JToken.Parse(json);

            try
            {
                if(isCheckSchema)
                {
                    var obj = token.ToObject<JObject>();
                    var version = (string)obj["schemaVersion"];

                    if(!(!string.IsNullOrEmpty(version) && CompareVersion(version)))
                    {
                        result = new TSchema();
                        return false;
                    }
                }

                result = JsonConvert.DeserializeObject<TSchema>(json);
                return true;
            }
            catch
            {
                result = new TSchema();
                return false;
            }
        }

        private bool CompareVersion(string version)
        {
            var appVersion = Regex.Replace(m_AppVersion, @"\((.*?)\)", "");

            var v1 = new Version(appVersion);
            var v2 = new Version(version);
            var result = v1.CompareTo(v2);

            return result is > 0 or 0;
        }
    }
}