using Newtonsoft.Json;

namespace BU.QuizExample.QuizExampleMessages
{
    public struct StateChangeMessage
    {
        [JsonProperty]
        public int NextQuizState;
    }
}