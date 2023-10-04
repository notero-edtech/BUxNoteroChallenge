/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem;
using ForieroEngine.Music.NotationSystem.Classes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "NS/Banks/Create - Lesson Bank")]
public class NSSessionBank : ScriptableObject
{
    public string bankName;
    [TextArea] public string description;
    public List<Session> sessions = new ();
}
