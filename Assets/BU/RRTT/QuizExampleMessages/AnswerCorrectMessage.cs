using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
{
    public struct AnswerCorrectMessage
    {
        [JsonProperty]
        public string Answer;
    }
}
