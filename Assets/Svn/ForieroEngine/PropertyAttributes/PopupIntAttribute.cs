using UnityEngine;

public class PopupIntList : PropertyAttribute
{
    public bool showLabel = false;
    public delegate int[] GetIntList();

    public PopupIntList(int[] list)
    {
        List = list;
    }

    public int[] List
    {
        get;
        private set;
    }
}