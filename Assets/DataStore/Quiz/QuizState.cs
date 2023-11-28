using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataStore.Quiz
{
    public class QuizState
    {
        public Question CurrentQuestion;

        public int CurrentQuestionNumber => m_CurrentQuestionIndex;

        public int QuestionAmount => m_QuestionList.Count;

        public static QuizState Default => m_Default ??= new();
        private static QuizState m_Default;

        private List<Question> m_QuestionList;
        private string m_AppVersion;
        private int m_CurrentQuestionIndex;

        private QuizState()
        {
            m_QuestionList = new();
        }

        public void SetAppVersion(string version) => m_AppVersion = version;

        public void Load(string assetJson, Action<List<Question>> onFinished, Action<string> onFailed = null)
        {
            m_QuestionList.Clear();

            try
            {
                var schema = new SchemaQuizUse(m_AppVersion).Deserialize(assetJson);

                if(schema == null)
                {
                    Debug.LogError($"Unexpected error, quiz json file was read but schema mismatch");
                    return;
                }

                var quizList = schema.Data;

                if(quizList == null)
                {
                    Debug.LogError($"Unexpected error, quiz json file was read but not converted");
                    return;
                }

                ResetQuestionIndex();
                m_QuestionList = quizList;

                onFinished?.Invoke(quizList);
            }
            catch(Exception e)
            {
                Debug.LogError($"Unexpected error, message: {Application.version} {e.Message}");
                onFailed?.Invoke(e.Message);
            }
        }

        public void ResetQuestionIndex()
        {
            m_CurrentQuestionIndex = 0;
        }

        public void SetQuizList(List<Question> questionList) => m_QuestionList = questionList;

        public Question GetQuestionById(string id)
        {
            return IsContain(id, out var quiz) ? quiz : null;
        }

        public bool IsContain(string id, out Question question)
        {
            question = m_QuestionList.FirstOrDefault(info => info.Id == id);

            return question != default;
        }

        public bool GetNextQuestionIfExist(out Question question)
        {
            try
            {
                question = m_QuestionList[m_CurrentQuestionIndex];
                CurrentQuestion = question;

                m_CurrentQuestionIndex++;
                return true;
            }
            catch
            {
                question = null;
                CurrentQuestion = null;

                return false;
            }
        }
    }

    [Serializable]
    public class Question
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("assetFile")]
        public string AssetFile;

        [JsonProperty("assetType")]
        public QuestionAssetType QuestionAssetType;

        [JsonIgnore]
        public Answer Answer
        {
            get
            {
                return m_Answer ??= new()
                {
                    AssetAnswerFile = AssetFile,
                    CorrectAnswers = Answers
                };
            }
            set { m_Answer = value; }
        }

        [JsonProperty("answers")]
        public List<string> Answers;

        [JsonProperty("answer")]
        public Answer m_Answer;

        private const char Splitter = '/';

        public string GetAssetFileName => m_PathAssetFileSplit.Length < 2 ? "" : m_PathAssetFileSplit[1];

        public string GetRootDirectoryAssetFile => m_PathAssetFileSplit.Length < 1 ? "" : m_PathAssetFileSplit[0];

        private string[] m_PathAssetFileSplit => AssetFile.Split(Splitter);
    }

    public class Answer
    {
        [JsonProperty("assetAnswerFile")]
        public string AssetAnswerFile;

        [JsonProperty("correctAnswers")]
        public List<string> CorrectAnswers;

        private const char Splitter = '/';

        public string GetAssetAnswerFileName => m_PathAssetAnswerFileSplit.Length < 2 ? "" : m_PathAssetAnswerFileSplit[1];

        public string GetRootDirectoryAssetAnswerFile => m_PathAssetAnswerFileSplit.Length < 1 ? "" : m_PathAssetAnswerFileSplit[0];

        private string[] m_PathAssetAnswerFileSplit => AssetAnswerFile.Split(Splitter);
    }

    public enum Choice
    {
        UNKNOWN = 0,
        CHIOCE_1 = 1,
        CHIOCE_2 = 2,
        CHIOCE_3 = 3,
        CHIOCE_4 = 4,
    }

    public enum AnswerState
    {
        WRONG,
        CORRECT
    }

    public enum QuizSequenceState
    {
        COUNT_DOWN,
        QUESTION,
        SUBMITED,
        REVEAL
    }
}