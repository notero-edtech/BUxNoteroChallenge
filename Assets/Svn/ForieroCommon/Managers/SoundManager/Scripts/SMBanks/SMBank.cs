using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound Manager/Bank")]
public class SMBank : ScriptableObject
{
    #if WWISE
    #elif FMOD
    [FMODUnity.BankRef]
    #endif
    public string bankName;

    public bool autoLoad = false;
    
    public void Load()
    {
        #if WWISE
        #elif FMOD
        if(!string.IsNullOrEmpty(bankName) && !FMODUnity.RuntimeManager.HasBankLoaded(bankName)) FMODUnity.RuntimeManager.LoadBank(bankName);
        #endif
    }

    public void Unload()
    {
        #if WWISE
        #elif FMOD
        if(!string.IsNullOrEmpty(bankName) && FMODUnity.RuntimeManager.HasBankLoaded(bankName)) FMODUnity.RuntimeManager.UnloadBank(bankName);
        #endif
    }
}
