using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
{
    public struct StateChangeMessage
    {
        [JsonProperty]
        public int NextQuizState;
    }
}