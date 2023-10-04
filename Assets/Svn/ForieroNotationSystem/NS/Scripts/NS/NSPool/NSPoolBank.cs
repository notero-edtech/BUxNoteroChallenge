/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;


[CreateAssetMenu(menuName = "NS/Banks/Create - Pool Bank")]
public class NSPoolBank : ScriptableObject
{
    public List<PrefabPool> prefabPool;
    public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
}
