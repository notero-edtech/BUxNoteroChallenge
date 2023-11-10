using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct StudentAnswerMessage
    {
        [JsonProperty]
        public string StationId;

        [JsonProperty]
        public string Answer;
    }
}