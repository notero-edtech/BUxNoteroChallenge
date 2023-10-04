/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public class NSPlaybackRollingTest : MonoBehaviour
{

    public NSPlayback.PlaybackMode playbackMode = NSPlayback.PlaybackMode.Undefined;
    public NSPlayback.NSRollingPlayback.RollingMode rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Undefined;

    // Update is called once per frame
    void Update()
    {
        NSPlayback.playbackMode = playbackMode;
        NSPlayback.NSRollingPlayback.rollingMode = rollingMode;
    }
}
