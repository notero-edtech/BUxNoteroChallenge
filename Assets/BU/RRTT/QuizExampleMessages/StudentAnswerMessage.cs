using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
{
    public struct StudentAnswerMessage
    {
        [JsonProperty]
        public string StationId;

        [JsonProperty]
        public string Answer;
    }
}