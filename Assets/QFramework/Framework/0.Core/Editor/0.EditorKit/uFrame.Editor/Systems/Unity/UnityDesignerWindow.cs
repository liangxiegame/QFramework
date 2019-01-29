using QFramework.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QFramework.GraphDesigner.Unity
{
    public class UnityDesignerWindow : DiagramPlugin
        , IDesignerWindowEvents
        , IDrawDesignerWindow
        , IDesignerWindowLostFocus
        , IInputPanningHandler
        , IRepaintWindow
        , ICommandExecuted
        , IExecuteCommand<ScrollGraphCommand>
        , IGraphLoaded
        , IQueryDiagramScroll
        , IToolbarQuery
    {
        private DesignerWindow _designerWindow;
        private Vector2 _scrollPosition;

        public DesignerWindow DesignerWindow
        {
            get
            {
                return _designerWindow ?? InvertApplication.Container.Resolve<DesignerWindow>();
            }
            set { _designerWindow = value; }
        }

        public void AfterDrawGraph(Rect diagramRect)
        {
            GUI.EndScrollView();
        }

        public void BeforeDrawGraph(Rect diagramRect)
        {
            _scrollPosition = GUI.BeginScrollView(diagramRect, _scrollPosition, DesignerWindow.DiagramViewModel.DiagramBounds.Pad(0,0,15,15));
        }

        public void AfterDrawDesignerWindow(Rect windowRect)
        {
            
        }

        public void DrawComplete()
        {
          
        }

        public void ProcessInput()
        {
            HandleInput(MouseEvent);
            HandlePanning();
        }

        public void ProcessKeyboardInput()
        {
            
        }

        public void HandlePanning(Vector2 delta)
        {
            _scrollPosition += delta;
            if (_scrollPosition.x < 0)
                _scrollPosition.x = 0;
            if (_scrollPosition.y < 0)
                _scrollPosition.y = 0;

            if (_scrollPosition.x > DesignerWindow.DiagramRect.width - DesignerWindow.DiagramRect.x)
            {
                _scrollPosition.x = DesignerWindow.DiagramRect.width - DesignerWindow.DiagramRect.x;
            }
            if (_scrollPosition.y > DesignerWindow.DiagramRect.height - DesignerWindow.DiagramRect.y)
            {
                _scrollPosition.y = DesignerWindow.DiagramRect.height - DesignerWindow.DiagramRect.y;
            }
        }
        public void DrawDesigner(float width, float height)
        {
            if (EditorApplication.isCompiling)
            {
                InvertApplication.SignalEvent<ITaskProgressEvent>(_=>_.Progress(99f,"Waiting On Unity...", true));
            }

            InvertApplication.SignalEvent<IDrawUFrameWindow>(_ => _.Draw(width, height, _scrollPosition, 1f));
        }

        public void DesignerWindowLostFocus()
        {
            ModifierKeyStates = null;
            MouseEvent = null;
        }

        public void Repaint()
        {
            ElementsDesigner.Instance.Repaint();

        }
        private MouseEvent _event;

        private ModifierKeyState _modifierKeyStates;
        private Vector2? _forceScrollPosition;

        public ModifierKeyState ModifierKeyStates
        {
            get { return _modifierKeyStates ?? (_modifierKeyStates = new ModifierKeyState()); }
            set { _modifierKeyStates = value; }
        }

        public MouseEvent MouseEvent
        {
            get { return _event ?? (_event = new MouseEvent(ModifierKeyStates, DesignerWindow.DiagramDrawer)); }
            set { _event = value; }
        }

        public bool IsMiddleMouseDown { get; set; }

        public Event LastEvent { get; set; }

        public Event LastMouseDownEvent { get; set; }

        public Vector2 PanStartPosition { get; set; }
        private void HandleInput(MouseEvent mouse)
        {

            var e = Event.current;
            if (e == null)
            {
                return;
            }
            if (DesignerWindow == null) return;

            MouseEvent.DefaultHandler = DesignerWindow.DiagramDrawer;

                var handler = MouseEvent.CurrentHandler;
                if (handler == null)
                {
                    MouseEvent = null;

                    InvertApplication.Log("Handler is null");
                    return;
                }

                if (e.type == EventType.MouseDown)
                {
                    mouse.MouseDownPosition = mouse.MousePosition;
                    mouse.IsMouseDown = true;
                    mouse.MouseButton = e.button;
                    mouse.ContextScroll = _scrollPosition;
                    if (e.button == 1)
                    {
                        e.Use();
                        handler.OnRightClick(mouse);
                    }
                    else if (e.clickCount > 1)
                    {
                        handler.OnMouseDoubleClick(mouse);
                        mouse.IsMouseDown = false;
                    }
                    else
                    {
                        handler.OnMouseDown(mouse);
                    }
                    InvertApplication.SignalEvent<IMouseDown>(_=>_.MouseDown(mouse));
                    LastMouseDownEvent = e;
                    
                }
                if (e.rawType == EventType.MouseUp)
                {
                    mouse.MouseUpPosition = mouse.MousePosition;
                    mouse.IsMouseDown = false;
                    handler.OnMouseUp(mouse);
                    InvertApplication.SignalEvent<IMouseUp>(_ => _.MouseUp(mouse));
                }
                else if (e.rawType == EventType.KeyDown)
                {
                }
                else
                {
                    var mp = (e.mousePosition); // * (1f / 1f /* Scale */);

                    mouse.MousePosition = mp;
                    mouse.MousePositionDelta = mouse.MousePosition - mouse.LastMousePosition;
                    mouse.MousePositionDeltaSnapped = mouse.MousePosition.Snap(12f*1f /* Scale */) -
                                                      mouse.LastMousePosition.Snap(12f*1f);
                    handler.OnMouseMove(mouse);
                    mouse.LastMousePosition = mp;
                    if (e.type == EventType.MouseMove)
                    {
                        e.Use();
                        //Signal<IRepaintWindow>(_=>_.Repaint());
                        //Repaint();
                    }
                }

            if (LastEvent != null)
            {
                if (LastEvent.type == EventType.KeyUp)
                {
                    if (LastEvent.keyCode == KeyCode.LeftShift || LastEvent.keyCode == KeyCode.RightShift)
                        mouse.ModifierKeyStates.Shift = false;
                    if (LastEvent.keyCode == KeyCode.LeftControl || LastEvent.keyCode == KeyCode.RightControl || LastEvent.keyCode == KeyCode.LeftCommand || LastEvent.keyCode == KeyCode.RightCommand)
                        mouse.ModifierKeyStates.Ctrl = false;
                    if (LastEvent.keyCode == KeyCode.LeftAlt || LastEvent.keyCode == KeyCode.RightAlt)
                        mouse.ModifierKeyStates.Alt = false;
                }
            }

            if (LastEvent != null)
            {
                if (LastEvent.type == EventType.KeyDown)
                {
                    if (LastEvent.keyCode == KeyCode.LeftShift || LastEvent.keyCode == KeyCode.RightShift)
                        mouse.ModifierKeyStates.Shift = true;
                    if (LastEvent.keyCode == KeyCode.LeftControl || LastEvent.keyCode == KeyCode.RightControl || LastEvent.keyCode == KeyCode.LeftCommand || LastEvent.keyCode == KeyCode.RightCommand)
                        mouse.ModifierKeyStates.Ctrl = true;
                    if (LastEvent.keyCode == KeyCode.LeftAlt || LastEvent.keyCode == KeyCode.RightAlt)
                        mouse.ModifierKeyStates.Alt = true;
                }
                // Debug.Log(string.Format("Shift: {0}, Alt: {1}, Ctrl: {2}",ModifierKeyStates.Shift,ModifierKeyStates.Alt,ModifierKeyStates.Ctrl));
            }

            LastEvent = Event.current;
            
            if (DiagramDrawer.IsEditingField)
            {
                // TODO 2.0 Get this out of here
                if (LastEvent.keyCode == KeyCode.Return)
                {
                    var selectedGraphItem = DesignerWindow.DiagramViewModel.SelectedGraphItem;
                    DesignerWindow.DiagramViewModel.DeselectAll();
                    if (selectedGraphItem != null)
                    {
                        selectedGraphItem.Select();
                    }
                    return;
                }
                return;
            }


            var evt = Event.current;
            if (evt != null && evt.isKey && evt.type == EventType.KeyUp)
            {
                Signal<IKeyboardEvent>(_ =>
                {
                    if (_.KeyEvent(evt.keyCode, mouse.ModifierKeyStates))
                    {
                        evt.Use();
                    }

                });

            }
        }

        private void HandlePanning()
        {
            if (Event.current.button == 2 && Event.current.type == EventType.MouseDown)
            {
                IsMiddleMouseDown = true;
                PanStartPosition = Event.current.mousePosition;
            }
            if (Event.current.button == 2 && Event.current.rawType == EventType.MouseUp && IsMiddleMouseDown)
            {
                IsMiddleMouseDown = false;
            }
            if (IsMiddleMouseDown)
            {
                var delta = PanStartPosition - Event.current.mousePosition;
                Signal<IInputPanningHandler>(_ => _.HandlePanning(delta));
            }
        }

        public void CommandExecuted(ICommand command)
        {
            MouseEvent = null;
        }

        public void Execute(ScrollGraphCommand command)
        {
            //TODO MINOR: enchance logic of calculating the right scrollposition for better user experience
            _forceScrollPosition = command.Position;
            if (DesignerWindow != null)
            {
                _forceScrollPosition -= new Vector2(DesignerWindow.DiagramRect.width / 2, DesignerWindow.DiagramRect.height/ 2);
            }
        }

        public void GraphLoaded()
        {
            if (_forceScrollPosition.HasValue)
            {
                _scrollPosition = _forceScrollPosition.Value;
                _forceScrollPosition = null;
            }
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            //ui.AddCommand(new ToolbarItem()
            //{
            //    Title = "Woop Woop",
            //    Command = new LambdaCommand("Woop Woop", () =>
            //    {
            //        var window = EditorWindow.GetWindow<uFrameInspectorWindow>(typeof(UnityDesignerWindow));
            //        window.ShowUtility();
            //    })
            //});
        }

        public void QueryDiagramScroll(ref Vector2 scroll)
        {
            scroll = _forceScrollPosition ?? _scrollPosition;
        }
    }
}