using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct StateChangeMessage
    {
        [JsonProperty]
        public int NextQuizState;
    }
}