/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;

public class NSPObjectWrapper : ScriptableObject
{
    public NSObject o;

    public static UnityEngine.Object Instantiate(NSObject o, UnityEngine.Object nspObjectWrapper)
    {
        if (!nspObjectWrapper)
        {
            nspObjectWrapper = ScriptableObject.CreateInstance<NSPObjectWrapper>();
        }

        (nspObjectWrapper as NSPObjectWrapper).o = o;

        return nspObjectWrapper;
    }
}
