/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.MusicXML.MXL;
using ForieroEngine.Music.Organizations;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static Action OnSessionInitialized { get; set; } = null;

        public static partial class Session
        {
            public static string Name  { get; set; } = "";
            public static string Description { get; set; } = "";
            public static string Instructions { get; set; } = "";
            public static HandsEnum Hands { get; set; } = HandsEnum.Unknown;
            public static SyllabusEnum Syllabus { get; set; } = SyllabusEnum.Undefined;

            public static TimeProvider TimeProvider { get; set; } = TimeProvider.Unknown;
            public static AudioProvider AudioProvider { get; set; } = AudioProvider.Unknown;

            public static byte[] StudentMusicXML { get; set; } = null;
            public static AudioClip AccompanimentClip { get; set; } = null;

            public static Action OnStatsReady { get; set; } = null;
           
            public static void Init()
            {
                Stats.Reset();
                Audio.Init(AudioProvider, AccompanimentClip);
                Time.Init(TimeProvider);
                NSBehaviour.instance.ns.LoadMusicXML(StudentMusicXML.IsZip() ? StudentMusicXML.Unzip() : StudentMusicXML);
                OnSessionInitialized?.Invoke();
            }
            
            public static void Init(Classes.Session session)
            {
                if(session.system != SystemEnum.Undefined) InitSystem(session.system);
                
                Name = session.name;
                Description = session.description;
                Instructions = session.instructions;
                Hands = session.handsEnum;
                StudentMusicXML = session.xml.bytes;
                AccompanimentClip = session.accompaniment;
                TimeProvider = session.timeProvider;
                AudioProvider = session.audioProvider;
        
                Init();
            }
        }
    }
}
