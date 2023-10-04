using UnityEngine;
using UnityEngine.Serialization;

public class TTUIButtonState : ScriptableObject
{
    [FormerlySerializedAs("name")]
    public string stateName = "";
    public TTUIButtonStyle iconStyle;
    public TTUIButtonStyle backgroundStyle;
}
