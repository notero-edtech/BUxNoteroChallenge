/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.MIDIUnified;
using ForieroEngine.Music.MusicXML.Xsd;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSExtensionsMusicXML
    {
        public static PercussionEnum ToPercussionEnum(this unpitched unp, notehead nh)
        {
            if (unp == null) return PercussionEnum.Undefined;
            var octave = unp.displayoctave.ToInt(4);
            var step = unp.displaystep.ToNS();
            var nhv = nh?.Value ?? noteheadvalue.normal; 
            switch (step)
            {
                case StepEnum.C:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.AcousticSnare;
                                //case noteheadvalue.square: return PercussionEnum.ElectronicPad;
                            }
                            break;
                        case 6:
                            switch (nhv)
                            {
                                case noteheadvalue.cross: return PercussionEnum.SplashCymbal;
                            }
                            break;
                    }
                    break;
                case StepEnum.D:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.cross: return PercussionEnum.PedalHiHat;
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.LowMidTom;
                            }
                            break;
                    }
                    break;
                case StepEnum.E:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.AcousticBassDrum;
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.HighMidTom;
                                //case noteheadvalue.cross: return PercussionEnum.Rim;
                                case noteheadvalue.triangle: return PercussionEnum.Cowbell;
                            }
                            break;
                    }
                    break;
                case StepEnum.F:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.BassDrum1;
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.HighTom;
                                case noteheadvalue.cross: return PercussionEnum.RideCymbal1;
                                case noteheadvalue.diamond: return PercussionEnum.RideBell;
                            }
                            break;
                    }
                    break;
                case StepEnum.G:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.LowFloorTom;
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.cross: return PercussionEnum.ClosedHiHat;
                            }
                            break;
                    }
                    break;
                case StepEnum.A:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.HighFloorTom;
                            }
                            break;
                        case 5:
                            switch (nhv)
                            {
                                case noteheadvalue.cross: return PercussionEnum.CrashCymbal1;
                            }
                            break;
                    }
                    break;
                case StepEnum.B:
                    switch (octave)
                    {
                        case 4:
                            switch (nhv)
                            {
                                case noteheadvalue.none:
                                case noteheadvalue.normal: return PercussionEnum.LowMidTom;
                                case noteheadvalue.cross: return PercussionEnum.SideStick;
                                case noteheadvalue.triangle: return PercussionEnum.Tambourine;
                            }
                            break;
                        case 5: 
                            switch (nhv)
                            {
                                case noteheadvalue.cross: return PercussionEnum.CrashCymbal2;
                            }
                            break;
                    }
                    break;
            }
            return PercussionEnum.Undefined;
        }
    }
}
