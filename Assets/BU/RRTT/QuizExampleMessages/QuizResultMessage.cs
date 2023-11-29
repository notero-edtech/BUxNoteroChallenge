using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
{
    public struct QuizResultMessage
    {
        [JsonProperty]
        public int CorrectAnswerAmount;
    }
}