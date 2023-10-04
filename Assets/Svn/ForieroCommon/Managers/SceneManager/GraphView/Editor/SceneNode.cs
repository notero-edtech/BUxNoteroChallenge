using System.Linq;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ForieroEditor.SceneManager
{
    public class SceneNode : Node,IEdgeConnectorListener
    {
        SceneGraphView graph;
        SceneSettings settings;
        public SceneSettings.SceneItem item;
        public Port inputPort;

        Color? defaultObjectFieldColor = null;

        public override void OnSelected()
        {
            if (graph.selection.Count == 1)
            {
                Debug.Log("Selecting : " + this.title);
                graph.selectedItem = item;
            }
        }

        public override void OnUnselected()
        {
            if (graph.selectedItem == item)
            {
                Debug.Log("Unselecting : " + this.title);
                graph.selectedItem = null;
            }
        }

        void UpdateTitle()
        {
            this.title = (this.item.sceneAsset == null ? "NULL" : this.item.sceneAsset.name) + " | " + this.item.sceneProxyName;
            var title = this.Q("title");
            title.style.backgroundColor = (item.sceneAsset ? Color.green : Color.red).Brightness(0.3f);
        }

        void UpdateSceneFieldColor()
        {
            var sceneField = this.Q("scenefield");
            if (defaultObjectFieldColor == null) defaultObjectFieldColor = sceneField.style.backgroundColor.value;
            sceneField.style.backgroundColor = (item.sceneAsset ? (Color)defaultObjectFieldColor  : Color.red).Brightness(0.3f);
        }

        public SceneNode() { }

        public SceneNode(SceneSettings.SceneItem item,SceneSettings settings, SceneGraphView graph)
        {
            this.capabilities &= ~Capabilities.Collapsible;

            this.graph = graph;
            this.settings = settings;

            this.item = item;

            this.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                Debug.Log(this.GetPosition());                
            });


            UpdateTitle();

            var sceneField = new ObjectField();
            sceneField.name = "scenefield";
            sceneField.objectType = typeof(SceneAsset);
            sceneField.SetValueWithoutNotify(item.sceneAsset);
            sceneField.MarkDirtyRepaint();
            sceneField.RegisterValueChangedCallback(evt => {
                item.sceneAsset = evt.newValue as SceneAsset;
                UpdateTitle();
                UpdateSceneFieldColor();
            });
            contentContainer.Add(sceneField);
            UpdateSceneFieldColor();

            var guidLabel = new TextField();
            guidLabel.SetValueWithoutNotify(item.internalGUID);
            contentContainer.Add(guidLabel);
            guidLabel.SetEnabled(false);

            var proxyTextField = new TextField();
            proxyTextField.SetValueWithoutNotify(item.sceneProxyName);
            proxyTextField.MarkDirtyRepaint();
            proxyTextField.RegisterValueChangedCallback(evt => {
                item.sceneProxyName = evt.newValue;
                UpdateTitle();
            });
            contentContainer.Add(proxyTextField);
                        
            inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
            inputPort.name = this.item.internalGUID;
            inputPort.portName = "";

            inputPort.AddManipulator(new EdgeConnector<Edge>(this));
            this.inputContainer.Add(inputPort);

            var addPortButton = new Button(() =>
            {
                var newPort = new SceneSettings.SceneReferenceItem();
                item.exits.Add(newPort);
                AddOutputPort(newPort);
                RefreshExpandedState();
                RefreshPorts();
            })
            {
                text = "Add Port"
            };

            this.outputContainer.Add(addPortButton);
                        
            item.exits.ForEach((p) => AddOutputPort(p));

            RefreshExpandedState();
            RefreshPorts();
                        
            this.SetPosition(new Rect(item.position, new Vector2(200,200)));

            this.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                this.item.position = new Vector2(evt.newRect.x, evt.newRect.y);
                EditorUtility.SetDirty(settings);
            });            
        }

        Port GeneratePort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single) => this.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(SceneNode));
                
        private void AddOutputPort(SceneSettings.SceneReferenceItem item)
        {
            var outputPort = GeneratePort(Direction.Output, Port.Capacity.Single);
            item.port = outputPort;
            outputPort.portName = "";
            outputPort.name = item.internalGUID;
            outputPort.tooltip = $"{item.command} | {item.internalGUID}";

            var commandField = new TextField();
            commandField.style.minWidth = 50;
            commandField.SetValueWithoutNotify(item.command);
            commandField.RegisterValueChangedCallback((evt) =>
            {
                item.command = evt.newValue;
                outputPort.tooltip = $"{item.command} | {item.internalGUID}";
            });                                    
            outputPort.Add(commandField);

            var deleteButton = new Button(() => RemovePort(outputPort)) { text = "x"};
            outputPort.AddManipulator(new EdgeConnector<Edge>(this));
            outputPort.Add(deleteButton);

            this.outputContainer.Add(outputPort);
        }

        void RemovePort(Port port)
        {
            this.item.exits.Remove(this.item.exits.First(i => i.port == port));

            var targetEdge = graph.edges.ToList().Where(x => x.output == port && x.output.node == this);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                graph.RemoveElement(edge);
            }
            
            port.DisconnectAll();
            outputContainer.Remove(port);
            RefreshExpandedState();
            RefreshPorts();
        }

        public void ResetPort(Port port)
        {
            foreach(var i in item.exits)
            {
                if (i.port == port)
                {
                    i.internalGUID = "";
                    i.port.name = "";
                    i.port.tooltip = $"{i.command} | {i.internalGUID}";
                }
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {         
            if (edge.output == null) return;

            var node = edge.output.node as SceneNode;
            var item = node.item.exits.Where((p) => p.port == edge.output).First();
            item.internalGUID = "";
            item.port.name = "";
            item.port.tooltip = $"{item.command} | {item.internalGUID}";
            EditorUtility.SetDirty(settings);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {            
            var outputNode = edge.output.node as SceneNode;
            var inputNode = edge.input.node as SceneNode;
            var guid = inputNode.item.internalGUID;

            var item = outputNode.item.exits.Where((p) => p.port == edge.output).First();
            item.internalGUID = guid;
            item.port.name = guid;
            item.port.tooltip = $"{item.command} | {item.internalGUID}";
            EditorUtility.SetDirty(settings);
        }
    }
}
