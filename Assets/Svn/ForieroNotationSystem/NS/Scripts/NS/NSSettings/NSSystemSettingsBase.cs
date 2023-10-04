/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using ForieroEngine.Music.NotationSystem;
using UnityEngine;

public abstract class NSSystemSettingsBase<T> : ScriptableObject where T : NSSystemSpecificSettingsBase
{
    public NSSystemSettings nsSystemSettings;
    public T nsSystemSpecificSettings;
    protected static NSBehaviour NSB => NSBehaviour.instance;
    public abstract void Init();
}
