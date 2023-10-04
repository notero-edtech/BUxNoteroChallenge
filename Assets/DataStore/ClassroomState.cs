using System.Collections.Generic;
using System.Linq;

namespace DataStore
{
    public class ClientInfo
    {
        public string StationId;
        public ulong SessionId;
        public bool HasLessonInfo;
        public bool HasLessonCoverFile;
        public bool HasLessonThemeFile;
        public bool HasGameResult;
        public Student Student;
        public GameResult GameResult;

        private readonly List<string> m_DownloadedLessonContentFiles = new List<string>();
        private readonly List<string> m_DownloadedLibraryContentFiles = new List<string>();
        private readonly List<string> m_DownloadedLessonNodeFiles = new List<string>();

        public void AddDownloadedLessonFileByLessonId(string id)
        {
            if(!string.IsNullOrEmpty(id)) m_DownloadedLessonContentFiles.Add(id);
        }

        public void AddDownloadedLibraryFileBySongId(string id)
        {
            if(!string.IsNullOrEmpty(id)) m_DownloadedLibraryContentFiles.Add(id);
        }

        public bool HasLessonByLessonId(string id) => m_DownloadedLessonContentFiles.Exists(lessonId => lessonId == id);

        public bool HasLibraryContentBySongId(string id) => m_DownloadedLibraryContentFiles.Exists(songId => songId == id);

        public void AddDownloadedLessonNodeFileByNodeId(string id)
        {
            if(!string.IsNullOrEmpty(id)) m_DownloadedLessonNodeFiles.Add(id);
        }

        public bool HasNodeByNodeId(string id) => m_DownloadedLessonNodeFiles.Exists(nodeId => nodeId == id);
    }

    public struct GameResult
    {
        public int PerfectCount;
        public int GoodCount;
        public int OopsCount;
        public int StarCount;
        public float AccuracyPercent;
        public float StudentCurrentScore;
    }

    // public static class GameResultSerializer
    // {
    //     public static void Write(this NetworkWriter writer, GameResult value)
    //     {
    //         writer.WriteInt(value.PerfectCount);
    //         writer.WriteInt(value.GoodCount);
    //         writer.WriteInt(value.OopsCount);
    //         writer.WriteInt(value.StarCount);
    //         writer.WriteFloat(value.AccuracyPercent);
    //         writer.WriteFloat(value.StudentCurrentScore);
    //     }
    //
    //     public static GameResult Read(this NetworkReader reader)
    //     {
    //         var value = new GameResult();
    //         value.PerfectCount = reader.ReadInt();
    //         value.GoodCount = reader.ReadInt();
    //         value.OopsCount = reader.ReadInt();
    //         value.StarCount = reader.ReadInt();
    //         value.AccuracyPercent = reader.ReadFloat();
    //         value.StudentCurrentScore = reader.ReadFloat();
    //         return value;
    //     }
    // }

    public class Student
    {
        public string Id;
        public string DisplayName;
        public int TotalProgression;
        public HashSet<LessonProgression> LessonProgressions;

        public void AddLessonProgression(LessonProgression lessonProgression)
        {
            LessonProgressions.Add(lessonProgression);
        }
    }

    public class LessonProgression
    {
        public string LessonId;
        public HashSet<NodeProgression> NodeProgressions;
        public int TotalProgression => NodeProgressions.Sum(node => node.Progression);
    }

    public class NodeProgression
    {
        public string NodeId;
        public int Progression;
    }

    public class ClassroomState
    {
        public int ClientCount => Clients.Count;

        public Dictionary<string, ClientInfo> Clients { get; private set; } = new();

        public void AddClientInfo(string stationId, ClientInfo clientInfo)
        {
            Clients ??= new Dictionary<string, ClientInfo>();
            Clients[stationId] = clientInfo;
        }

        public void RemoveClientInfo(string stationId)
        {
            if(!Clients.TryGetValue(stationId, out var client)) return;
            Clients.Remove(client.StationId);
        }

        public ClientInfo GetClientInfoByStationId(string stationId)
        {
            return Clients.TryGetValue(stationId, out ClientInfo info) ? info : null;
        }

        public bool GetClientInfoBySessionId(ulong sessionId, out ClientInfo clientInfo)
        {
            clientInfo = Clients.Values.FirstOrDefault(info => info.SessionId == sessionId);

            return clientInfo != null;
        }

        public void UpdateStudentDataToClientInfo(string stationId, Student studentData)
        {
            if(Clients.TryGetValue(stationId, out var info)) info.Student = studentData;
        }

        public void UpdateStudentDataToClientHasGameResult(string stationId, bool hasGameResult)
        {
            if(Clients.TryGetValue(stationId, out var info)) info.HasGameResult = hasGameResult;
        }
    }
}