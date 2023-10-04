using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoiceService
{
    OSX,
    WINDOWS,
    GOOGLE,
    AMAZON,
    IBM,
    ALEXA
}

public enum VoiceGender{
    Male, 
    Female,
    Undefined = int.MaxValue
}

[System.Serializable]
public class Voice
{
    public string name = "";
    public string languageCodeRegion = "";
    public int bitRate = 22050;
    public VoiceGender voiceGender = VoiceGender.Undefined;
}

[System.Serializable]
public class LangActorVoice
{
    public Lang.LanguageCode languageCode;
    public VoiceService voiceService;
    public Voice voice;
}