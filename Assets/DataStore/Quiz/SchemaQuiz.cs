using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Notero.Utilities;

namespace DataStore.Quiz
{
    public class SchemaQuiz
    {
        [JsonProperty("schemaVersion")]
        public string SchemaVersion;

        [JsonProperty("data")]
        public List<Question> Data;
    }

    public class SchemaQuizUse : Schema<SchemaQuiz>
    {
        public SchemaQuizUse(string appVersion) : base(appVersion) { }

        public override SchemaQuiz Deserialize(string quizJson)
        {
            if(Reader<SchemaQuiz>(quizJson, out var schema1_8_0))
            {
                CurrentSchema = schema1_8_0;
            }
            else if(Reader<List<Question>>(quizJson, out var data, false))
            {
                CurrentSchema = new()
                {
                    SchemaVersion = "<1.8.0",
                    Data = data
                };
            }
            else
            {
                throw new Exception("Schema version not support.");
            }

            return CurrentSchema;
        }
    }
}