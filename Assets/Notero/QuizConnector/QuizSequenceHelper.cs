using DataStore.Quiz;
using Hendrix.Connector.Model;
using Notero.QuizConnector.Model;
using Notero.Report;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Notero.QuizConnector
{
    public static class QuizSequenceHelper
    {
        private const string StudentAnswerAmountFormat = "<color=#14C287>{0}</color> / {1}";

        private static readonly Dictionary<Choice, string> m_AnswerLabel = new()
        {
            { Choice.CHIOCE_1, "1" },
            { Choice.CHIOCE_2, "2" },
            { Choice.CHIOCE_3, "3" },
            { Choice.CHIOCE_4, "4" }
        };

        public static string GetAnswerLabel(Choice choice)
        {
            var label = "";

            if(m_AnswerLabel.TryGetValue(choice, out var labelValue)) label = labelValue;

            return label;
        }

        public static string StudentAnswerToStringFormat(int studentAnswerAmount, int studentAmount)
        {
            return string.Format(StudentAnswerAmountFormat, studentAnswerAmount, studentAmount);
        }

        public static Dictionary<string, int> GetQuestionNumber(Dictionary<string, Question> questions)
        {
            Dictionary<string, int> questionNumber = new();

            int number = 1;
            foreach(var question in questions)
            {
                questionNumber.Add(question.Key, number++);
            }

            return questionNumber;
        }

        public static List<StudentPostTestResultInfo> GenerateStudentPostTestResultInfoList(Dictionary<string, List<StudentAnswer>> studentAnswers, Dictionary<string, int> studentScore, int preTestFullScore)
        {
            var studentQuizResultList = new List<StudentPostTestResultInfo>();

            foreach(var (questionId, studentAnswerList) in studentAnswers)
            {
                var quizState = QuizState.Default;

                if(!quizState.IsContain(questionId, out var question)) continue;

                foreach(var studentAnswer in studentAnswerList)
                {
                    var student = studentQuizResultList.Find(info => info.StationId == studentAnswer.StationId);
                    var correctAnswer = question.Answer.CorrectAnswers.ElementAt(0);
                    var scoreAnswer = studentAnswer.Answer == correctAnswer ? 1 : 0;

                    if(student == null)
                    {
                        if(preTestFullScore == 0)
                        {
                            //for empty pre-test database.
                            preTestFullScore = quizState.QuestionAmount;
                        }

                        if(studentScore.TryGetValue(studentAnswer.StationId, out var preTestScore))
                        {
                            studentQuizResultList.Add(new StudentPostTestResultInfo(studentAnswer.StationId, preTestScore, scoreAnswer, preTestFullScore, quizState.QuestionAmount));
                        }
                        else
                        {
                            studentQuizResultList.Add(new StudentPostTestResultInfo(studentAnswer.StationId, -1, scoreAnswer, preTestFullScore, quizState.QuestionAmount));
                        }
                    }
                    else
                    {
                        student.CurrentScore += scoreAnswer;
                    }
                }
            }

            return studentQuizResultList;
        }

        public static List<StudentQuizResultInfo> GenerateStudentQuizResultInfoList(Dictionary<string, List<StudentAnswer>> studentAnswers)
        {
            var studentQuizResultList = new List<StudentQuizResultInfo>();

            foreach(var (questionId, studentAnswerList) in studentAnswers)
            {
                var quizState = QuizState.Default;

                if(!quizState.IsContain(questionId, out var question)) continue;

                foreach(var studentAnswer in studentAnswerList)
                {
                    var student = studentQuizResultList.Find(info => info.StationId == studentAnswer.StationId);
                    var correctAnswer = question.Answer.CorrectAnswers.ElementAt(0);
                    var scoreAnswer = studentAnswer.Answer == correctAnswer ? 1 : 0;

                    if(student == null)
                    {
                        studentQuizResultList.Add(new StudentQuizResultInfo(studentAnswer.StationId, scoreAnswer, quizState.QuestionAmount));
                    }
                    else
                    {
                        student.CurrentScore += scoreAnswer;
                    }
                }
            }

            return studentQuizResultList;
        }

        public static List<QuizRecord> GenerateStudentQuizRecordList(QuizStore quizStore)
        {
            Dictionary<string, List<StudentAnswer>> studentAnswers = quizStore.StudentAnswers;
            Dictionary<string, Question> questions = quizStore.QuizList;

            var studentQuizRecords = new List<QuizRecord>();
            var quizState = QuizState.Default;

            foreach(var (questionId, studentAnswerList) in studentAnswers)
            {
                if(!quizState.IsContain(questionId, out var question)) continue;

                foreach(var studentAnswer in studentAnswerList)
                {
                    var student = studentQuizRecords.Find(info => info.Station == studentAnswer.StationId);
                    var correctAnswer = question.Answer.CorrectAnswers.ElementAt(0);
                    var isCorrect = studentAnswer.Answer == correctAnswer;
                    var scoreAnswer = isCorrect ? 1 : 0;

                    if(student == null)
                    {
                        student = new QuizRecord()
                        {
                            Station = studentAnswer.StationId,
                            StudentID = "", //Intentionally left blank, untill data is implement in the future.
                            CorrectAnswerAmount = scoreAnswer,
                            QuestionsAmount = quizState.QuestionAmount,
                            Answers = new(quizState.QuestionAmount),
                        };

                        int questionIndex = 0;
                        foreach(var q in questions)
                        {
                            student.Answers.Add(new StudentQuizAnswer()
                            {
                                QuestionID = q.Key,
                                QuestionIndex = questionIndex,
                            });
                            questionIndex++;
                        }

                        studentQuizRecords.Add(student);
                    }
                    else
                    {
                        student.CorrectAnswerAmount += scoreAnswer;
                    }

                    var answer = student.Answers.Find(x => x.QuestionID == questionId);

                    if(answer == null) continue;

                    answer.Answer = studentAnswer.Answer;
                    answer.IsCorrect = isCorrect;
                }
            }

            return studentQuizRecords;
        }

        public static List<PostTestAnswer> GeneratePostTestAnswerList(QuizStore quizStore, string rootAssetPath = "")
        {
            return quizStore.QuizList.Values.Select((question, index) => new PostTestAnswer()
            {
                QuestionIndex = index,
                QuestionAnswer = question.Answer.CorrectAnswers.ElementAt(0),
                QuestionImagePath = Path.Combine(rootAssetPath, question.AssetFile)
            }).ToList();
        }

        public static List<PostQuestionAnswerRequest> GenerateQuestionAnswers(QuizStore quizStore)
        {
            List<PostQuestionAnswerRequest> questionAnswerList = new();

            Dictionary<string, List<StudentAnswer>> studentAnswers = quizStore.StudentAnswers;
            Dictionary<string, int> questionNumber = GetQuestionNumber(quizStore.QuizList);

            //List all the student seat numbers that had logged in.
            HashSet<string> studentIDs = new();
            foreach(var studentAnswerList in studentAnswers.Values)
            {
                foreach(var studentAnswer in studentAnswerList)
                {
                    studentIDs.Add(studentAnswer.StationId);
                }
            }

            //Add missing students to studentAnswers
            foreach(var question in questionNumber.Keys)
            {
                foreach(var studentID in studentIDs)
                {
                    if(!studentAnswers.ContainsKey(question))
                        studentAnswers.Add(question, new List<StudentAnswer>());
                    if(!studentAnswers[question].Exists(x => x.StationId == studentID))
                    {
                        studentAnswers[question].Add(new StudentAnswer()
                        {
                            StationId = studentID,
                            Answer = ""
                        });
                    }
                }
            }

            //Rearrange data for database.
            var quizState = QuizState.Default;

            foreach(var (questionId, studentsAnswerList) in studentAnswers)
            {
                List<PostStudentAnswerRequest> studentList = new();

                if(!quizState.IsContain(questionId, out var question)) continue;
                var correctAnswer = question.Answer.CorrectAnswers.ElementAt(0);

                foreach(var studentAnswer in studentsAnswerList)
                {
                    var isCorrect = studentAnswer.Answer.Equals(correctAnswer);

                    PostStudentAnswerRequest.ResultEnum result = isCorrect ? PostStudentAnswerRequest.ResultEnum.CORRECT : PostStudentAnswerRequest.ResultEnum.WRONG;

                    if(studentAnswer.Answer.Equals("")) result = PostStudentAnswerRequest.ResultEnum.NOANSWER;


                    // add student answer to StudentAnswerList
                    studentList.Add(new PostStudentAnswerRequest
                    (
                        stationId: studentAnswer.StationId,
                        studentId: "", //await value field.
                        answer: studentAnswer.Answer,
                        result: result
                    ));
                }

                // add questions to QuestionAnswerList
                var questionAnswers = new PostQuestionAnswerRequest
                (
                    number: questionNumber[questionId],
                    correctAnswer: correctAnswer,
                    studentAnswers: studentList
                );

                questionAnswerList.Add(questionAnswers);
            }

            return questionAnswerList;
        }
    }
}