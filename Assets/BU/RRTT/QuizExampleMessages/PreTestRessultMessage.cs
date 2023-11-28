using Newtonsoft.Json;

namespace BU.RRTT.QuizExampleMessages
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