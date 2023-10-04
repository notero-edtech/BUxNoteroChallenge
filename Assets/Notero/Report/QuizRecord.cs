using FileHelpers;
using System.Collections.Generic;

namespace Notero.Report
{
    /// <summary>
    /// For server storage. To generate .CSV file, use adapter 'new QuizRecordToCSV(...)'.
    /// </summary>
    public class QuizRecord
    {
        public string Station;
        public string StudentID;
        public int CorrectAnswerAmount;
        public int QuestionsAmount;
        public List<StudentQuizAnswer> Answers;
    }

    /// <summary>
    /// Adapter to convert QuizRecord to QuizRecordToCSV.
    /// </summary>
    public class QuizRecordToCSV : QuizRecordCSV
    {
        //
        public QuizRecordToCSV(QuizRecord record)
        {
            Station = record.Station;
            StudentID = record.StudentID;
            ScoreFraction = string.Format("{0}/{1}", record.CorrectAnswerAmount, record.QuestionsAmount);
            ScoreDetails = ScoreConvert(record);
        }

        private string[] ScoreConvert(QuizRecord record)
        {
            var detail = record.Answers;
            string[] cache = new string[record.QuestionsAmount];

            for(int i = 0; i < detail.Count; i++)
            {
                cache[i] = "No Answer";

                var answer = detail.Find(x => x.QuestionIndex == i);
                if(answer == null) continue;
                if(answer.Answer == "") continue;
                cache[i] = answer.IsCorrect ? "Correct" : "Wrong";
            }

            return cache;
        }
    }

    /// <summary>
    /// QuizRecordCSV is for converting data to CSV file.
    /// </summary>
    [DelimitedRecord(",")]
    public class QuizRecordCSV
    {
        public string Station;
        public string StudentID;
        public string ScoreFraction;
        public string[] ScoreDetails;
    }

    public class StudentQuizAnswer
    {
        public string QuestionID;
        public int QuestionIndex;
        public string Answer = "";
        public bool IsCorrect = false;
    }
}