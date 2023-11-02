using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct AnswerCorrectMessage
    {
        [JsonProperty]
        public string Answer;
    }
}
