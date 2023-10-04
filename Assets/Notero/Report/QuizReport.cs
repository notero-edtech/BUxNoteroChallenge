using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Notero.Report
{
    public class QuizReport
    {
        public string ClassroomTag;
        public string Year;
        public string Semester;
        public DateTime Timestamp;
        public string Book;
        public string Chapter;
        public string SequenceType;
        public string Mission;
        public IEnumerable<QuizRecordCSV> Records;
        public int QuestionAmount;
        public string AppVersion;
        public string ContentVersion;
        public string License;

        public void WriteFile(string fileName)
        {
            File.WriteAllText(fileName, ToString());
        }

        public override string ToString()
        {
            var engine = new FileHelperEngine<QuizRecordCSV>();

            string result = $"CLASSROOM TAG,{ClassroomTag}" //Header block
                + $"\nYEAR,{Year}"
                + $"\nSEMESTER,{Semester}"
                + $"\nDATE,{Timestamp.ToString("dd'/'MM'/'yyyy")}"
                + $"\nTIME,{Timestamp.ToString("HH':'mm")}"
                + $"\nBOOK,{Book}"
                + $"\nCHAPTER,{Chapter}"
                + $"\nSEQUENCE TYPE,{SequenceType}"
                + $"\nMISSION,{Mission}"
                + $"\nNUMBER OF QUESTIONS,{QuestionAmount}"
                + $"\nAPPLICATION VERSION,{AppVersion}"
                + $"\nCONTENT VERSION,{ContentVersion}"
                + $"\nLICENSE,{License}"
                + $"\n" //end of header block
                + $"\nSeat Number,Student ID,Result"; //Table heads

            string questionID = "";
            for(int i = 0; i < QuestionAmount; i++)
            {
                questionID += "," + (i + 1);
            }
            result += questionID;
            //end of table heads

            result += '\n' + engine.WriteString(Records);

            return result;
        }
    }
}
