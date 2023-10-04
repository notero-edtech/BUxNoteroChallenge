using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class Lang
{  
    public enum PhonemeEnum
    {
        RhubarbA,
        RhubarbB,
        RhubarbC,
        RhubarbD,
        RhubarbE,
        RhubarbF,
        RhubarbG,
        RhubarbH,
        RhubarbX
    }

    public enum EmotionEnum
    {
        Normal,
        Anger,
        Disgust,
        Fear,
        Happiness,
        Sadness,
        Surprise,
        Undefined = int.MaxValue           
    }
        
    public class Phonemes
    {
        public List<Phoneme> phonemes = new List<Phoneme>();

        public void Init(string s)
        {
            phonemes.Clear();

            string[] lines = Regex.Split(s, "\r\n|\r|\n");
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;

                string[] tabs = line.Split('\t');
                if(Enum.TryParse<Lang.PhonemeEnum>("Rhubarb" + tabs[1].Trim(), out var phonemeEnum))
                {
                    if (float.TryParse(tabs[0].Trim(), out var time))
                    {
                        var p = new Phoneme(time, phonemeEnum);
                        phonemes.Add(p);
                    }
                } else
                {
                    if(debug) Debug.LogWarning("Could not parse phoneme : " + "Rhubarb" + tabs[1].Trim());
                }                
            }
        }
    }

    public class Phoneme
    {
        public float time;
        public PhonemeEnum phoneme;
        public bool fired;

        public Phoneme(float time, PhonemeEnum phoneme)
        {
            this.time = time;
            this.phoneme = phoneme;           
            this.fired = false;
        }
    }

    public static LanguageCode SystemLanguageToLangCode(this SystemLanguage systemLanguage)
    {
        if (systemLanguage == SystemLanguage.Afrikaans)
            return LanguageCode.AF;
        else if (systemLanguage == SystemLanguage.Arabic)
            return LanguageCode.AR;
        else if (systemLanguage == SystemLanguage.Basque)
            return LanguageCode.BA;
        else if (systemLanguage == SystemLanguage.Belarusian)
            return LanguageCode.BE;
        else if (systemLanguage == SystemLanguage.Bulgarian)
            return LanguageCode.BG;
        else if (systemLanguage == SystemLanguage.Catalan)
            return LanguageCode.CA;
        else if (systemLanguage == SystemLanguage.Chinese)
            return LanguageCode.ZH;
        else if (systemLanguage == SystemLanguage.Czech)
            return LanguageCode.CS;
        else if (systemLanguage == SystemLanguage.Danish)
            return LanguageCode.DA;
        else if (systemLanguage == SystemLanguage.Dutch)
            return LanguageCode.NL;
        else if (systemLanguage == SystemLanguage.English)
            return LanguageCode.EN;
        else if (systemLanguage == SystemLanguage.Estonian)
            return LanguageCode.ET;
        else if (systemLanguage == SystemLanguage.Faroese)
            return LanguageCode.FA;
        else if (systemLanguage == SystemLanguage.Finnish)
            return LanguageCode.FI;
        else if (systemLanguage == SystemLanguage.French)
            return LanguageCode.FR;
        else if (systemLanguage == SystemLanguage.German)
            return LanguageCode.DE;
        else if (systemLanguage == SystemLanguage.Greek)
            return LanguageCode.EL;
        else if (systemLanguage == SystemLanguage.Hebrew)
            return LanguageCode.HE;
        else if (systemLanguage == SystemLanguage.Hungarian)
            return LanguageCode.HU;
        else if (systemLanguage == SystemLanguage.Icelandic)
            return LanguageCode.IS;
        else if (systemLanguage == SystemLanguage.Indonesian)
            return LanguageCode.ID;
        else if (systemLanguage == SystemLanguage.Italian)
            return LanguageCode.IT;
        else if (systemLanguage == SystemLanguage.Japanese)
            return LanguageCode.JA;
        else if (systemLanguage == SystemLanguage.Korean)
            return LanguageCode.KO;
        else if (systemLanguage == SystemLanguage.Latvian)
            return LanguageCode.LA;
        else if (systemLanguage == SystemLanguage.Lithuanian)
            return LanguageCode.LT;
        else if (systemLanguage == SystemLanguage.Norwegian)
            return LanguageCode.NO;
        else if (systemLanguage == SystemLanguage.Polish)
            return LanguageCode.PL;
        else if (systemLanguage == SystemLanguage.Portuguese)
            return LanguageCode.PT;
        else if (systemLanguage == SystemLanguage.Romanian)
            return LanguageCode.RO;
        else if (systemLanguage == SystemLanguage.Russian)
            return LanguageCode.RU;
        else if (systemLanguage == SystemLanguage.SerboCroatian)
            return LanguageCode.SH;
        else if (systemLanguage == SystemLanguage.Slovak)
            return LanguageCode.SK;
        else if (systemLanguage == SystemLanguage.Slovenian)
            return LanguageCode.SL;
        else if (systemLanguage == SystemLanguage.Spanish)
            return LanguageCode.ES;
        else if (systemLanguage == SystemLanguage.Swedish)
            return LanguageCode.SW;
        else if (systemLanguage == SystemLanguage.Thai)
            return LanguageCode.TH;
        else if (systemLanguage == SystemLanguage.Turkish)
            return LanguageCode.TR;
        else if (systemLanguage == SystemLanguage.Ukrainian)
            return LanguageCode.UK;
        else if (systemLanguage == SystemLanguage.Vietnamese)
            return LanguageCode.VI;
        else if (systemLanguage == SystemLanguage.Hungarian)
            return LanguageCode.HU;
        else if (systemLanguage == SystemLanguage.Unknown)
            return LanguageCode.Unassigned;
        return LanguageCode.Unassigned;
    }

    public enum LanguageCode
    {
        Unassigned = 0,
        //null
        AA = 1,
        //Afar
        AB = 2,
        //Abkhazian
        AF = 3,
        //Afrikaans
        AM = 4,
        //Amharic
        AR = 5,
        //Arabic
        AS = 6,
        //Assamese
        AY = 7,
        //Aymara
        AZ = 8,
        //Azerbaijani
        BA = 9,
        //Bashkir
        BE = 10,
        //Byelorussian
        BG = 11,
        //Bulgarian
        BH = 12,
        //Bihari
        BI = 13,
        //Bislama
        BN = 14,
        //Bengali
        BO = 15,
        //Tibetan
        BR = 16,
        //Breton
        CA = 17,
        //Catalan
        CO = 18,
        //Corsican
        CS = 19,
        //Czech
        CY = 20,
        //Welch
        DA = 21,
        //Danish
        DE = 22,
        //German
        DZ = 23,
        //Bhutani
        EL = 24,
        //Greek
        EN = 25,
        //English
        EO = 26,
        //Esperanto
        ES = 27,
        //Spanish
        ET = 28,
        //Estonian
        EU = 29,
        //Basque
        FA = 30,
        //Persian
        FI = 31,
        //Finnish
        FJ = 32,
        //Fiji
        FO = 33,
        //Faeroese
        FR = 34,
        //French
        FY = 35,
        //Frisian
        GA = 36,
        //Irish
        GD = 37,
        //Scots Gaelic
        GL = 38,
        //Galician
        GN = 39,
        //Guarani
        GU = 40,
        //Gujarati
        HA = 41,
        //Hausa
        HI = 42,
        //Hindi
        HE = 43,
        //Hebrew
        HR = 44,
        //Croatian
        HU = 45,
        //Hungarian
        HY = 46,
        //Armenian
        IA = 47,
        //Interlingua
        ID = 48,
        //Indonesian
        IE = 49,
        //Interlingue
        IK = 50,
        //Inupiak
        IN = 51,
        //former Indonesian
        IS = 52,
        //Icelandic
        IT = 53,
        //Italian
        IU = 54,
        //Inuktitut (Eskimo)
        IW = 55,
        //former Hebrew
        JA = 56,
        //Japanese
        JI = 57,
        //former Yiddish
        JW = 58,
        //Javanese
        KA = 59,
        //Georgian
        KK = 60,
        //Kazakh
        KL = 61,
        //Greenlandic
        KM = 62,
        //Cambodian
        KN = 63,
        //Kannada
        KO = 64,
        //Korean
        KS = 65,
        //Kashmiri
        KU = 66,
        //Kurdish
        KY = 67,
        //Kirghiz
        LA = 68,
        //Latin
        LN = 69,
        //Lingala
        LO = 70,
        //Laothian
        LT = 71,
        //Lithuanian
        LV = 72,
        //Latvian, Lettish
        MG = 73,
        //Malagasy
        MI = 74,
        //Maori
        MK = 75,
        //Macedonian
        ML = 76,
        //Malayalam
        MN = 77,
        //Mongolian
        MO = 78,
        //Moldavian
        MR = 79,
        //Marathi
        MS = 80,
        //Malay
        MT = 81,
        //Maltese
        MY = 82,
        //Burmese
        NA = 83,
        //Nauru
        NE = 84,
        //Nepali
        NL = 85,
        //Dutch
        NO = 86,
        //Norwegian
        OC = 87,
        //Occitan
        OM = 88,
        //(Afan) Oromo
        OR = 89,
        //Oriya
        PA = 90,
        //Punjabi
        PL = 91,
        //Polish
        PS = 92,
        //Pashto, Pushto
        PT = 93,
        //Portuguese
        QU = 94,
        //Quechua
        RM = 95,
        //Rhaeto-Romance
        RN = 96,
        //Kirundi
        RO = 97,
        //Romanian
        RU = 98,
        //Russian
        RW = 99,
        //Kinyarwanda
        SA = 100,
        //Sanskrit
        SD = 101,
        //Sindhi
        SG = 102,
        //Sangro
        SH = 103,
        //Serbo-Croatian
        SI = 104,
        //Singhalese
        SK = 105,
        //Slovak
        SL = 106,
        //Slovenian
        SM = 107,
        //Samoan
        SN = 108,
        //Shona
        SO = 109,
        //Somali
        SQ = 110,
        //Albanian
        SR = 111,
        //Serbian
        SS = 112,
        //Siswati
        ST = 113,
        //Sesotho
        SU = 114,
        //Sudanese
        SV = 115,
        //Swedish
        SW = 116,
        //Swahili
        TA = 117,
        //Tamil
        TE = 118,
        //Tegulu
        TG = 119,
        //Tajik
        TH = 120,
        //Thai
        TI = 121,
        //Tigrinya
        TK = 122,
        //Turkmen
        TL = 123,
        //Tagalog
        TN = 124,
        //Setswana
        TO = 125,
        //Tonga
        TR = 126,
        //Turkish
        TS = 127,
        //Tsonga
        TT = 128,
        //Tatar
        TW = 129,
        //Twi
        UG = 130,
        //Uigur
        UK = 131,
        //Ukrainian
        UR = 132,
        //Urdu
        UZ = 133,
        //Uzbek
        VI = 134,
        //Vietnamese
        VO = 135,
        //Volapuk
        WO = 136,
        //Wolof
        XH = 137,
        //Xhosa
        YI = 138,
        //Yiddish
        YO = 139,
        //Yoruba
        ZA = 140,
        //Zhuang
        ZH = 141,
        //Chinese
        ZU = 142
        //Zulu

    }
}
