using System.Collections.Generic;
using ForieroEditor.SceneManager;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneNodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private SceneGraphView _graphView;
    private SceneGraph _window;
    private Texture2D _indentationIcon;

    public void Init(SceneGraph window, SceneGraphView graphView)
    {
        _window = window;
        _graphView = graphView;

        //Indentation hack for search window as a transparent icon
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
           new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
           new SearchTreeGroupEntry(new GUIContent("Scene Manager"), 1),
           new SearchTreeEntry(new GUIContent("Scene Node", _indentationIcon))
           {
               userData = new SceneNode(), level = 2
           },
           //new SearchTreeEntry(new GUIContent("Hello World")) 
        };

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        
        switch (SearchTreeEntry.userData)
        {
            case SceneNode sceneNode:
                //create node here
                return true;
            default:
                return false;
        }        
    }    
}
