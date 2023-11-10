using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct QuizResultMessage
    {
        [JsonProperty]
        public int CorrectAnswerAmount;
    }
}