using UnityEngine;

public class PopupStringList : PropertyAttribute
{
    public delegate string[] GetStringList();

    public PopupStringList(string[] list)
    {
        List = list;
    }

    public string[] List
    {
        get;
        private set;
    }
}
