using DataStore.Quiz;
using Newtonsoft.Json;

namespace BU.RRTT.QuizExample.QuizExampleMessages
{
    public struct CurrentQuestionMessage
    {
        [JsonProperty]
        public string Id;

        [JsonProperty]
        public string AssetFile;

        [JsonProperty]
        public string AssetAnswerFile;

        [JsonProperty]
        public QuestionAssetType QuestionAssetType;
    }
}