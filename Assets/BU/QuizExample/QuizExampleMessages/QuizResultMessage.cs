using Newtonsoft.Json;

namespace BU.QuizExample.QuizExampleMessages
{
    public struct QuizResultMessage
    {
        [JsonProperty]
        public int CorrectAnswerAmount;
    }
}