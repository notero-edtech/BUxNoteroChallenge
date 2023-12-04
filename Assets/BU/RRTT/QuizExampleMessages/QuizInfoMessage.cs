using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
{
    public struct QuizInfoMessage
    {
        [JsonProperty]
        public int CurrentQuizNumber;

        [JsonProperty]
        public int QuestionAmount;
    }
}
