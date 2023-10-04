/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.MusicXML.Xsd;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

namespace ForieroEngine.Music.NotationSystem
{
    public enum TimeProvider
    {
        DSPTime,
        Time,
        AudioSource,
        Midi,
        Unknown = int.MaxValue
    }

    public enum SystemFontEnum
    {
        Clean,
        Jazz
    }

    public enum DivisionEnum
    {
        [InspectorName("1\\1")] One = 1,
        [InspectorName("1\\2")] Two = 2,
        [InspectorName("1\\3")] Three = 3,
        [InspectorName("1\\4")] Four = 4,
        [InspectorName("1\\5")] Five = 5,
        [InspectorName("1\\6")] Six = 6,
        [InspectorName("1\\7")] Seven = 7,
        [InspectorName("1\\8")] Eight = 8,
        [InspectorName("1\\9")] Nine = 9,
        [InspectorName("1\\10")] Ten = 10
    }
    
    public enum SystemEnum
    {
        Default = 0,
        PageLayoutTicker = 10,
        RollingTicker = 20,
        RollingLeftRight = 30,
        RollingRightLeft = 31,
        RollingTopBottom = 40,
        RollingBottomTop = 41,
        Custom = 50,
        Undefined = int.MaxValue
    }

    public enum AudioProvider
    {
        UnityAudioSource,
        //BASS24AudioSource,
        Unknown = int.MaxValue
    }

    public enum HandsEnum
    {
        Left, 
        Both, 
        Right,
        Unknown = int.MaxValue
    }
    
    public enum NoteDisplayEnum
    {
        Normal,
        Whole,
        None,
        Custom,
        Undefined = int.MaxValue
    }

    public enum ToneNamesEnum
    {
        ToneNames,
        SolfegeFixed,
        SolfegeMovable,
        SolfegeFixedSymbolic,
        SolfegeMovableSymbolic,
        Undefined = int.MaxValue
    }

    public enum NSObjectCheckEnum
    {
        Constrained,
        Allowed,
        Undefined = int.MaxValue
    }

    public enum YesNoEnum
    {
        Yes,
        No,
        Undefined = int.MaxValue
    }

    public enum OverUnderEnum
    {
        Over,
        Under,
        Undefined = int.MaxValue
    }

    public enum TieTypeEnum
    {
        Start,
        Stop,
        @Continue,
        LetRing,
        Undefined = int.MaxValue
    }

    public enum SpacingMode
    {
        MusicXML,
        Duration,
        Optical,
        Undefined = int.MaxValue
    }
   
    public enum DisplayMode
    {
        Line,
        Page,
        Unknown = int.MaxValue
    }

    public enum PlaybackMode
    {
        Ticker,
        Scrolling,
        ScrollingNoRepeats
    }

    public enum AntialiasingEnum
    {
        Camera,
        QualitySettings,
        Undefined = int.MaxValue
    }

    public enum PedalEnum
    {
        Pedal,
        PedalLetter,
        Stop,
        Sostenuto,
        SostenutoLetter,
        Undefined = int.MaxValue
    }

    public enum PlacementEnum
    {
        Above,
        Below,
        Undefined = int.MaxValue
    }

    public enum OrientationEnum
    {
        Normal,
        Inverted,
        Undefined = int.MaxValue
    }

    public enum RoundingEnum
    {
        Round,
        Ceil,
        Floor,
        Even,
        Odd,
        Undefined = int.MaxValue
    }

    public enum PixelAlignEnum
    {
        Round,
        Ceil,
        Floor,
        Undefined = int.MaxValue
    }

    public enum PoolEnum
    {
        NS_MOVABLE = 0,
        NS_FIXED = 1,
        NS_PARENT = 2,
        NS_FIXED_OVERLAY = 3,
        NS_MOVABLE_OVERLAY = 4,
    }

    public enum PivotEnum
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum ScreenPositionEnum
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum ShiftStepEnum
    {
        Whole = 1,
        Half = 2,
        Quarter = 4,
        Eight = 8,
        Sixteenth = 16
    }

    public enum StaveEnum
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Undefined = int.MaxValue
    }

    public enum BarLineEnum
    {
        Regular,
        Dotted,
        Dashed,
        Heavy,
        LightLight,
        LightHeavy,
        HeavyLight,
        HeavyHeavy,
        Tick,
        Short,
        Undefined = int.MaxValue
    }

    public enum ClefEnum
    {
        Treble,
        Bass,
        Soprano,
        MezzoSoprano,
        Alto,
        Tenor,
        Baritone,
        Percussion,
        PercussionUnpitched,
        TAB,
        Jianpu,
        C,
        Undefined = int.MaxValue
    }

    public enum AccidentalEnum
    {
        DoubleFlat,
        Flat,
        Natural,
        Sharp,
        DoubleSharp,
        Undefined = int.MaxValue
    }

    public enum NoteHeadEnum
    {
        Do,
        Re,
        Mi,
        Fa,
        FaUp,
        So,
        La,
        Ti,
        Slash,
        Triangle,
        Diamond,
        Square,
        Cross,
        X,
        CircleX,
        InvertedTriangle,
        ArrowDown,
        ArrowUp,
        Circled,
        CircleDot,
        LeftTriangle,
        Rectangle,
        Slashed,
        BackSlashed,
        Normal,
        Cluster,
        Other,
        None,
        Undefined = int.MaxValue
    }

    public enum NoteEnum
    {
        Long,
        Breve,
        Whole,
        Half,
        Quarter,
        Item8th,
        Item16th,
        Item32nd,
        Item64th,
        Item128th,
        Item256th,
        Item512th,
        Item1024th,
        Undefined = int.MaxValue,
    }

    public enum RestEnum
    {
        Breve,
        Whole,
        Half,
        Quarter,
        Item8th,
        Item16th,
        Item32nd,
        Item64th,
        Item128th,
        Item256th,
        Item512th,
        Item1024th,
        Undefined = int.MaxValue,
    }

    // numbers represents number of flags //
    public enum FlagEnum
    {
        Item8th = 1,
        Item16th = 2,
        Item32nd = 3,
        Item64th = 4,
        Item128th = 5,
        Undefined = int.MaxValue
    }

    public enum DirectionEnum
    {
        Up = 1,
        Down = 2,
        Right = 3,
        Left = 4,
        Undefined = int.MaxValue
    }

    public enum VerticalDirectionEnum
    {
        Up = 1,
        Down = 2,
        Undefined = int.MaxValue
    }

    public enum HorizontalDirectionEnum
    {
        Right = 3,
        Left = 4,
        Undefined = int.MaxValue
    }

    public enum StemEnum
    {
        Down,
        Up,
        Double,
        Undefined = int.MaxValue
    }

    public enum BeamEnum
    {
        Start,
        Continue,
        End,
        Undefined = int.MaxValue,
    }

    public enum StepEnum
    {
        C = 0,
        D = 1,
        E = 2,
        F = 3,
        G = 4,
        A = 5,
        B = 6,
        H = 6,
        Undefined = int.MaxValue
    }

    public enum SolfegeEnum
    {
        Do = 0,
        Re = 1,
        Mi = 2,
        Fa = 3,
        So = 4,
        La = 5,
        Ti = 6,
        Si = 6,
        Undefined = int.MaxValue
    }

    public enum OctaveEnum
    {
        Zero = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5,
        Sixth = 6,
        Seventh = 7,
        Eight = 8,
        Undefined = int.MaxValue
    }

    public enum KeyModeEnum
    {
        Major,
        Minor,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian,
        Ionian,
        Locrian,
        Undefined = int.MaxValue
    }

    public enum KeySignatureEnum
    {
        CFlatMaj = -7,
        GFlatMaj_EFlatMin = -6,
        DFlatMaj_BFlatMin = -5,
        AFlatMaj_FMin = -4,
        EFlatMaj_CMin = -3,
        BFlatMaj_GMin = -2,
        FMaj_DMin = -1,
        CMaj_AMin = 0,
        GMaj_EMin = 1,
        DMaj_BMin = 2,
        AMaj_FSharpMin = 3,
        EMaj_CSharpMin = 4,
        BMaj_GSharpMin = 5,
        FSharpMaj_DSharpMin = 6,
        CSharpMaj = 7,
        Undefined = int.MaxValue
    }

    public enum TimeSignatureEnum
    {
        Common,
        Cut,
        SingleNumber,
        Note,
        DottedNote,
        Normal,
        Undefined = int.MaxValue
    }

    public enum ArticulationEnum
    {
        Accent,
        BreathMark,
        Caesura,
        DetachedLegato,
        Doit,
        FallOff,
        Plop,
        Scoop,
        Spiccato,
        Staccatissimo,
        Staccato,
        Stress,
        StrongAccent,
        Tenuto,
        Unstress,
        Undefined = int.MaxValue
    }

    public enum FermataEnum
    {
        VeryShort,
        Short,
        Normal,
        Long,
        VeryLong,
        Undefined = int.MaxValue
    }

    public enum FeedbackEnum
    {
        TooEarly, 
        Early, 
        Perfect, 
        Late, 
        TooLate, 
        Missed, 
        Undefined = int.MaxValue
    }

    public enum Stretch
    {
        Auto, 
        Specified, 
        Undefined = int.MaxValue
    }
}
