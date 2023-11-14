using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct QuizInfoMessage
    {
        [JsonProperty]
        public int CurrentQuizNumber;

        [JsonProperty]
        public int QuestionAmount;
    }
}
