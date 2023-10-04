using FileHelpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Notero.Report
{
    public class GameplayReport
    {
        public string ClassroomTag;
        public string Year;
        public string Semester;
        public DateTime Timestamp;
        public string SequenceType;
        public float BPM;
        public string IsMetronomeOn;
        public IEnumerable<GameplayRecord> Records;

        public void WriteFile(string fileName)
        {
            File.WriteAllText(fileName, ToString());
        }
    }

    public class LessonGameplayReport : GameplayReport
    {
        public string Book;
        public string Chapter;
        public string Mission;
        public string License;

        public override string ToString()
        {
            var engine = new FileHelperEngine<GameplayRecord>();

            var result = $"CLASSROOM TAG,{ClassroomTag}"
                + $"\nYEAR,{Year}"
                + $"\nSEMESTER,{Semester}"
                + $"\nDATE,{Timestamp:dd'/'MM'/'yyyy}"
                + $"\nTIME,{Timestamp:HH':'mm}"
                + $"\nBOOK,{Book}"
                + $"\nCHAPTER,{Chapter}"
                + $"\nSEQUENCE TYPE,{SequenceType}"
                + $"\nMISSION,{Mission}"
                + $"\nBPM,{BPM}"
                + $"\nMETRONOME,{IsMetronomeOn}"
                + $"\nLICENSE,{License}"
                + $"\n"
                + $"\n,Accuracy,Perfect,Good,Oops,Score,Finished";
            result += '\n' + engine.WriteString(Records);

            return result;
        }
    }

    public class LibraryGameplayReport : GameplayReport
    {
        public string songName;
        public string artist;
        public string genre;
        public string composer;
        public string arranger;
        public string publisher;
        public string License;

        public override string ToString()
        {
            var engine = new FileHelperEngine<GameplayRecord>();

            var result = $"CLASSROOM TAG,{ClassroomTag}"
                + $"\nYEAR,{Year}"
                + $"\nSEMESTER,{Semester}"
                + $"\nDATE,{Timestamp:dd'/'MM'/'yyyy}"
                + $"\nTIME,{Timestamp:HH':'mm}"
                + $"\nSONG NAME,{songName}"
                + $"\nARTIST,{artist}"
                + $"\nGENRE,{genre}"
                + $"\nCOMPOSER,{composer}"
                + $"\nARRANGER,{arranger}"
                + $"\nPUBLISHER,{publisher}"
                + $"\nGAMEPLAY MODE,{SequenceType}"
                + $"\nBPM,{BPM}"
                + $"\nMETRONOME,{IsMetronomeOn}"
                + $"\nLICENSE,{License}"
                + $"\n"
                + $"\n,Accuracy,Perfect,Good,Oops,Score,Finished";
            result += '\n' + engine.WriteString(Records);


            return result;
        }
    }
}
