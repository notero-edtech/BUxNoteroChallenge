using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            m_AppVersion = Application.version;
            m_QuestionList = new();
        }

        public void Load(string assetFilePath, Action<List<Question>> onFinished)
        {
            if(!File.Exists(assetFilePath))
            {
                Debug.LogError($"Quiz file not exists, path: {assetFilePath}");
                return;
            }

            m_QuestionList.Clear();

            var fileStream = new FileStream(assetFilePath, FileMode.Open, FileAccess.Read);
            var readStream = new StreamReader(fileStream, Encoding.UTF8);
            var assetJson = readStream.ReadToEnd();

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

                readStream.Close();
                onFinished?.Invoke(quizList);
            }
            catch(Exception e)
            {
                Debug.LogError($"Unexpected error, message: {Application.version} {e.Message}");
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

        public void ResetQuestion()
        {
            if(m_QuestionList == null) return;
            m_CurrentQuestionIndex = 0;
            var question = m_QuestionList[m_CurrentQuestionIndex];
            CurrentQuestion = question;
        }
    }

    public class Question
    {
        [JsonProperty("id")]
        public readonly string Id;

        [JsonProperty("assetFile")]
        public readonly string AssetFile;

        [JsonProperty("assetType")]
        public readonly QuestionAssetType QuestionAssetType;

        public Answer Answer
        {
            get
            {
                if(m_Answer == null)
                {
                    m_Answer = new(
                        assetAnswerFile: AssetFile,
                        correctAnswers: Answers
                    );
                }

                return m_Answer;
            }
            private set
            {
                m_Answer = value;
            }
        }

        [JsonProperty("answers")]
        public readonly HashSet<string> Answers;

        [JsonProperty("answer")]
        private Answer m_Answer;

        private const char Splitter = '/';

        public string GetAssetFileName => m_PathAssetFileSplit.Length < 2 ? "" : m_PathAssetFileSplit[1];

        public string GetRootDirectoryAssetFile => m_PathAssetFileSplit.Length < 1 ? "" : m_PathAssetFileSplit[0];

        private string[] m_PathAssetFileSplit => AssetFile.Split(Splitter);

        public Question(string id, string assetFile, Answer answer, QuestionAssetType questionAssetType)
        {
            Id = id;
            AssetFile = assetFile;
            Answer = answer;
            QuestionAssetType = questionAssetType;
        }
    }

    public class Answer
    {
        [JsonProperty("assetAnswerFile")]
        public readonly string AssetAnswerFile;

        [JsonProperty("correctAnswers")]
        public readonly HashSet<string> CorrectAnswers;

        public Answer(string assetAnswerFile, HashSet<string> correctAnswers)
        {
            AssetAnswerFile = assetAnswerFile;
            CorrectAnswers = correctAnswers;
        }

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