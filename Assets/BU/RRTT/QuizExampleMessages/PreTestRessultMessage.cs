using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct PreTestResultMessage
    {
        [JsonProperty]
        public bool HasScore;

        [JsonProperty]
        public int Score;

        [JsonProperty]
        public int FullScore;
    }
}