/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Systems;

public static partial class NSTestSuiteCode01Pitches
{
    public static void _01aPitchesPitches(NS ns)
    {
        if (ns is NSRollingLeftRightSystem)
        {
            throw new NotImplementedException();
        }
        else if (ns is NSRollingTopBottomSystem)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
