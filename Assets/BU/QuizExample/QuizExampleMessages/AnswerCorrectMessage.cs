using Newtonsoft.Json;

namespace BU.QuizExample.QuizExampleMessages
{
    public struct AnswerCorrectMessage
    {
        [JsonProperty]
        public string Answer;
    }
}
