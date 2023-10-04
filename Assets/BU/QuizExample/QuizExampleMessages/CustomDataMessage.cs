using Newtonsoft.Json;

namespace BU.QuizExample.QuizExampleMessages
{
    public struct CustomDataMessage
    {
        [JsonProperty]
        public byte[] Data;

        public CustomDataMessage(byte[] data)
        {
            Data = data;
        }
    }
}