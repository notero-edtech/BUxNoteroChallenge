using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ForieroEditor.SceneManager
{
    public partial class SceneGraphView : GraphView
    {
        List<Edge> Edges => edges.ToList();
        List<SceneNode> Nodes => nodes.ToList().Cast<SceneNode>().ToList();
        readonly SceneGraph _window;
        readonly SceneNodeSearchWindow _searchWindow;

        readonly MiniMap _miniMap;

        SceneSettings _settings;

        public SceneGraphView(SceneGraph window)
        {
            _window = window;

            styleSheets.Add(Resources.Load<StyleSheet>("SceneGraph"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var grid = new GridBackground();
            {                                
                Insert(0, grid);
                grid.StretchToParentSize();
            }
                        
            _searchWindow = AddSearchWindow();

            //Blackboard();
            _miniMap = AddMiniMap();
            UpdateMiniMapPosition();

            AddSceneInspector();
            AddSceneSettingsInspector();

            this.graphViewChanged = OnGraphChange;
            this.viewTransformChanged = (b) => {
                
            };
        }

        public void InitGraph(SceneSettings settings)
        {
            this._settings = settings;

            this.ClearGraph();

            if (!this._settings) return;

            this.CreateNodes();
            this.CreateEdges();
            this.CreateList();
        }

        public void UpdatePosition()
        {
            UpdateMiniMapPosition();
            UpdateListPosition();
            UpdateInspectorPosition();
            UpdateSettingsInspectorPosition();
        }

        void UpdateMiniMapPosition() => _miniMap.SetPosition(new Rect(10, 25, 200, 140));

        private MiniMap AddMiniMap()
        {
            var miniMap = new MiniMap() { anchored = true };                        
            Add(miniMap);
            return miniMap;
        }

        private void Blackboard()
        {
            var blackboard = new Blackboard(this);

            blackboard.Add(new BlackboardSection() { title = "Exposed Properties" });
            blackboard.addItemRequested = _blackboard =>
            {
                //_graphView.AddPropertyToBakcboard();
            };
            blackboard.SetPosition(new Rect(5, 25, 200, 300));
            Add(blackboard);
        }

        private GraphViewChange OnGraphChange(GraphViewChange change)
        {                        
            if (change.edgesToCreate != null)
            {
                foreach (Edge edge in change.edgesToCreate)
                {
                    
                }
            }

            if (change.elementsToRemove != null)
            {
                foreach (GraphElement e in change.elementsToRemove)
                {                    
                    if (e.GetType() == typeof(Edge))
                    {
                        var edge = e as Edge;                                               
                        var node = Nodes.Where(n => n == edge.output.node).First();
                        node?.ResetPort(edge.output);                                                
                     }
                }
            }

            if (change.movedElements != null)
            {
                foreach (GraphElement e in change.movedElements)
                {
                    if (e.GetType() == typeof(Node))
                    {
                       
                    }
                }
            }

            return change;
        }
                
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private SceneNodeSearchWindow AddSearchWindow()
        {
            var searchWindow = ScriptableObject.CreateInstance<SceneNodeSearchWindow>();
            searchWindow.Init(_window, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
            return searchWindow;
        }
                                
        void CreateNode(SceneSettings.SceneItem item) => AddElement(new SceneNode(item, _settings, this));
        void CreateNodes() => _settings.sceneItems.ForEach((s) => CreateNode(s));
        
        public void CreateEdges()
        {
            foreach(var inputNode in Nodes)
            {
                foreach (var outputsNode in Nodes)
                {
                    foreach (var outputPort in outputsNode.item.exits.Select(p => p.port))
                    {                        
                        if (Equals(inputNode.inputPort.name, outputPort.name))
                        {                            
                            LinkPorts(outputPort, inputNode.inputPort);
                        }
                    }
                }
            }
        }

        void LinkPorts(Port output, Port input)
        {
            var edge = new Edge
            {
                output = output,
                input = input
            };

            edge.input.Connect(edge);
            edge.output.Connect(edge);
            Add(edge);
        }

        public void ClearGraph()
        {
            foreach (var node in Nodes)
            {
                //Remove edges that are connected to this graph
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => RemoveElement(edge));

                //Then remove the node
                RemoveElement(node);
            }
        }
    }
}
