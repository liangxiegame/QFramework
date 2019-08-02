using System;
using System.Linq;
using Invert.Common;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class UnityDrawer : IPlatformDrawer
    {
        private UnityStyleProvider mStyles;

        public UnityStyleProvider Styles
        {
            get { return mStyles ?? (mStyles = new UnityStyleProvider()); }
            set { mStyles = value; }
        }

        // TODO DRAWER Eliminate Vector3 convertion
        public void DrawPolyLine(Vector2[] lines, Color color)
        {
            Handles.color = color;
            Handles.DrawPolyLine(lines.Select(x => new Vector3(x.x, x.y, 0)).ToArray());
        }

        public void DrawLine(Vector3[] lines, Color color)
        {
            Handles.color = color;
            Handles.DrawPolyLine(lines);
        }

        private string mCurrentTooltip;
        private GUIStyle mTextWrappingTextArea;

        public void SetTooltipForRect(Rect rect, string tooltip)
        {
            bool isMouseOver = rect.Contains(Event.current.mousePosition);
            if (isMouseOver) mCurrentTooltip = tooltip;
        }

        public string GetTooltip()
        {
            return mCurrentTooltip;
        }

        public void ClearTooltip()
        {
            mCurrentTooltip = null;
        }


        // TODO DRAWER Beziers with texture
        public void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent,
            Color color, float width)
        {
            Handles.color = color;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, null, width);
        }

        // TODO DRAWER Add Scale parameter
        public Vector2 CalculateTextSize(string text, object styleObject)
        {
            var style = ((GUIStyle)styleObject);
            return style.CalcSize(new GUIContent(text));
        }

        // TODO DRAWER Add Scale parameter
        public float CalculateTextHeight(string text, object styleObject, float width)
        {
            var style = (GUIStyle)styleObject;
            return style.CalcHeight(new GUIContent(text), width);
        }

        public Vector2 CalculateImageSize(string imageName)
        {
            var image = ElementDesignerStyles.GetSkinTexture(imageName);
            if (image != null)
            {
                return new Vector2(image.width, image.height);
            }
            return Vector2.zero;
        }

        // TODO DRAWER Add tooltip parameter
        public void DrawLabel(Rect rect, string label, object style, DrawingAlignment alignment = DrawingAlignment.MiddleLeft)
        {
            var guiStyle = (GUIStyle)style;
            var oldAlignment = guiStyle.alignment;
            guiStyle.alignment = ((TextAnchor)(int)alignment);
            GUI.Label(rect, label, guiStyle);
            guiStyle.alignment = oldAlignment;
        }

        // TODO DRAWER Add tooltip parameter | Change the way it is done and separate icon from icon
        public void DrawLabelWithIcon(Rect rect, string label, string iconName, object style,
            DrawingAlignment alignment = DrawingAlignment.MiddleLeft)
        {
            var s = (GUIStyle)style;
            s.alignment = ((TextAnchor)(int)alignment);
            //GUI.Label(rect, label, s);
            GUI.Label(rect, new GUIContent(label, ElementDesignerStyles.GetSkinTexture(iconName)), s);
        }


        public void DrawStretchBox(Rect scale, object nodeBackground, float offset)
        {
            DrawExpandableBox(scale, nodeBackground, string.Empty, offset);
        }


        public void DrawStretchBox(Rect scale, object nodeBackground, Rect offset)
        {
            //var rectOffset = new RectOffset(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y), Mathf.RoundToInt(offset.width), Mathf.RoundToInt(offset.height));

            var rectOffset = new RectOffset((int)offset.x, (int)offset.y, (int)offset.width, (int)offset.height);

            DrawExpandableBox(scale, (GUIStyle)nodeBackground,
                string.Empty, rectOffset);
        }

        public void DrawExpandableBox(Rect rect, object style, string text, float offset = 12)
        {
            var guiStyle = (GUIStyle)style;
            var oldBorder = guiStyle.border;
            guiStyle.border = new RectOffset(
                (int)(offset),
                (int)(offset),
                (int)(offset),
                (int)(offset));
            GUI.Box(rect, text, guiStyle);
            guiStyle.border = oldBorder;
        }

        public void DrawExpandableBox(Rect rect, object style, string text, RectOffset offset)
        {
            var guiStyle = (GUIStyle)style;
            var oldBorder = guiStyle.border;
            GUI.Box(rect, text, guiStyle);
            guiStyle.border = oldBorder;
        }

        //TODO DRAWER introduce Tooptip parameter
        public void DoButton(Rect scale, string label, object style, System.Action action, System.Action rightClick = null)
        {
            var s = style == null ? ElementDesignerStyles.EventSmallButtonStyle : (GUIStyle)style;

            if (GUI.Button(scale, label, s))
            {
                if (Event.current.button == 0) action();
                else
                {
                    if (rightClick != null)
                        rightClick();
                }
            }
        }

        //TODO DRAWER introduce tooltip param
        public void DoButton(Rect scale, string label, object style, Action<Vector2> action, Action<Vector2> rightClick = null)
        {
            var s = style == null ? ElementDesignerStyles.EventSmallButtonStyle : (GUIStyle)style;

            if (GUI.Button(scale, label, s))
            {
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition) - new Vector2(0, 22);
                if (Event.current.button == 0) action(mousePos);
                else
                {
                    if (rightClick != null)
                        rightClick(mousePos);
                }
            }
        }

        public void DrawWarning(Rect rect, string key)
        {
            EditorGUI.HelpBox(rect, key, MessageType.Warning);
        }

        public void DrawError(Rect rect, string key)
        {
            EditorGUI.HelpBox(rect, key, MessageType.Error);
        }

        public void DrawInfo(Rect rect, string key)
        {
            EditorGUI.HelpBox(rect, key, MessageType.Info);
        }

        public void DrawImage(Rect bounds, string texture, bool b)
        {
            DrawImage(bounds, Styles.Image(texture), b);
        }

        public void DrawImage(Rect bounds, object texture, bool b)
        {
            GUI.DrawTexture(bounds, texture as Texture2D, ScaleMode.ScaleToFit, true);
        }

        //TODO DRAWER Introduce tooltip parameter
        public void DrawPropertyField(PropertyFieldViewModel fieldViewModel, float scale)
        {
            //base.Draw(scale);
            GUILayout.BeginArea(fieldViewModel.Bounds.Scale(scale), ElementDesignerStyles.SelectedItemStyle);
            EditorGUIUtility.labelWidth = fieldViewModel.Bounds.width * 0.55f;
            DrawInspector(fieldViewModel, new GUIStyle(EditorStyles.boldLabel) { normal = new GUIStyleState() { textColor = new Color(0.77f, 0.77f, 0.77f) } });
            GUILayout.EndArea();
        }

        public void DrawPropertyField(Rect r, PropertyFieldViewModel fieldViewModel, float scale)
        {
            DrawInspector(r, fieldViewModel, new GUIStyle(EditorStyles.boldLabel) { normal = new GUIStyleState() { textColor = new Color(0.77f, 0.77f, 0.77f) } });
        }



        public void EndRender()
        {
            EditorGUI.FocusTextInControl("EditingField");
        }

        public void DrawRect(Rect boundsRect, Color color)
        {
            EditorGUI.DrawRect(boundsRect, color);
        }

        public void DrawNodeHeader(Rect boxRect, object backgroundStyle, bool isCollapsed, float scale, object image)
        {
            if (image != null) (backgroundStyle as GUIStyle).ForNormalState(image as Texture2D);

            Rect adjustedBounds;
            if (isCollapsed)
            {
                //adjustedBounds = new Rect(boxRect.x - 9, boxRect.y + 1, boxRect.width + 19, boxRect.height + 9);
                adjustedBounds = new Rect(boxRect.x, boxRect.y, boxRect.width, (boxRect.height) * scale);
                DrawStretchBox(adjustedBounds, backgroundStyle, 20 * scale);
            }
            else
            {
                //adjustedBounds = new Rect(boxRect.x - 9, boxRect.y + 1, boxRect.width + 19, boxRect.height-6 * scale);
                adjustedBounds = new Rect(boxRect.x, boxRect.y, boxRect.width, (boxRect.height) * scale);
                DrawStretchBox(adjustedBounds,
                    backgroundStyle,
                    new Rect(20 * scale, 20 * scale, 35 * scale, 22 * scale)
                    );
            }
        }

        public void DoToolbar(Rect toolbarTopRect, DesignerWindow designerWindow, ToolbarPosition position)
        {
            if (designerWindow == null) throw new ArgumentNullException("designerWindow");

            if (designerWindow.Toolbar == null) throw new ArgumentNullException("designerWindow.Toolbar");
            GUILayout.BeginArea(toolbarTopRect);
            if (toolbarTopRect.y > 20)
            {
                designerWindow.Toolbar.GoBottom();
            }
            else
            {
                designerWindow.Toolbar.Go();
            }
            GUILayout.EndArea();
        }

        public void DoTabs(Rect tabsRect, DesignerWindow designerWindow)
        {
            EditorGUI.DrawRect(tabsRect, InvertGraphEditor.Settings.BackgroundColor);
            var color = new Color(InvertGraphEditor.Settings.BackgroundColor.r * 0.8f,
                InvertGraphEditor.Settings.BackgroundColor.g * 0.8f, InvertGraphEditor.Settings.BackgroundColor.b * 0.8f);
            EditorGUI.DrawRect(tabsRect, color);
            if (designerWindow != null && designerWindow.Designer != null)
            {
                GUILayout.BeginArea(tabsRect);
                GUILayout.BeginHorizontal();

                foreach (var tab in designerWindow.Designer.Tabs.ToArray())
                {
                    if (tab == null) continue;
                    if (tab.Name == null)
                        continue;
                    var isCurrent = designerWindow.Workspace != null && designerWindow.Workspace.CurrentGraph != null && tab.Identifier == designerWindow.Workspace.CurrentGraph.Identifier;
                    if (GUILayout.Button(tab.Name,
                        isCurrent
                            ? ElementDesignerStyles.TabStyle
                            : ElementDesignerStyles.TabInActiveStyle, GUILayout.MinWidth(150)))
                    {
                        var projectService = InvertGraphEditor.Container.Resolve<WorkspaceService>();

                        if (Event.current.button == 1)
                        {

                            var isLastGraph = projectService.CurrentWorkspace.Graphs.Count() <= 1;

                            if (!isLastGraph)
                            {
                                var tab1 = tab;
                                projectService.Repository.RemoveAll<WorkspaceGraph>(p => p.WorkspaceId == projectService.CurrentWorkspace.Identifier && p.GraphId == tab1.Identifier);
                                var lastGraph = projectService.CurrentWorkspace.Graphs.LastOrDefault();
                                if (isCurrent && lastGraph != null)
                                {
                                    designerWindow.SwitchDiagram(lastGraph);
                                }

                            }
                        }
                        else
                        {
                            designerWindow.SwitchDiagram(projectService.CurrentWorkspace.Graphs.FirstOrDefault(p => p.Identifier == tab.Identifier));
                        }

                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        public void DisableInput()
        {
            if (!GUI.enabled) return;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a * 2);
            GUI.enabled = false;
        }

        public void EnableInput()
        {
            if (GUI.enabled) return;
            GUI.enabled = true;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a / 2);
        }

        public void BeginRender(object sender, MouseEvent mouseEvent)
        {
            DiagramDrawer.IsEditingField = false;
        }

        public void DrawColumns(Rect rect, params Action<Rect>[] columns)
        {
            var columnsLength = columns.Length;
            var columnFactor = rect.width / columnsLength;

            for (int index = 0; index < columns.Length; index++)
            {
                var item = columns[index];
                var newRect = new Rect(rect.x + (index * columnFactor), rect.y, columnFactor, rect.height);
                item(newRect);
            }
        }
        public void DrawColumns(Rect rect, float[] columnWidths, params Action<Rect>[] columns)
        {
            var x = 0f;
            for (int index = 0; index < columns.Length; index++)
            {
                var item = columns[index];
                if (index == columns.Length - 1)
                {
                    // Use the remaining width of this item
                    var width = rect.width - x;
                    var newRect = new Rect(rect.x + x, rect.y, width, rect.height);
                    item(newRect);
                }
                else
                {
                    var newRect = new Rect(rect.x + x, rect.y, columnWidths[index], rect.height);
                    item(newRect);

                }

                x += columnWidths[index];
            }
        }

        public void DrawingComplete()
        {
            //if (DiagramDrawer.IsEditingField)
            //{
            //GUI.FocusControl("EditingField");
            //}
            //else
            //{
            //}
        }

        public void DrawTextbox(string id, Rect rect, string value, object itemTextEditingStyle, Action<string, bool> valueChangedAction)
        {
            //EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("EditingField");
            DiagramDrawer.IsEditingField = true;
            var newName = EditorGUI.TextField(rect, value, (GUIStyle)itemTextEditingStyle);

            valueChangedAction(newName, false);
            //if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newName))
            //{
            //}
            if (Event.current.keyCode == KeyCode.Return)
            {
                valueChangedAction(value, true);
            }
            EditorGUI.FocusTextInControl("EditingField");
        }

        public void DrawingStarted()
        {

        }

        public static bool WhiteLabels = true;
        public virtual void DrawInspector(PropertyFieldViewModel d, GUIStyle labelStyle)
        {
            var labelWidth = 140;
            var labelWidtho = GUILayout.ExpandWidth(true);

            var colorCache = GUI.color;
            GUI.color = Color.white;
            if (d.InspectorType == InspectorType.GraphItems)
            {
                var item = d.CachedValue as IGraphItem;
                var text = "--Select--";
                if (item != null)
                {
                    text = item.Label;
                }
                //GUILayout.BeginHorizontal();

                if (GUILayout.Button(d.Label + ": " + text, ElementDesignerStyles.ButtonStyle))
                {
                    var type = d.Type;

                    var items = InvertGraphEditor.CurrentDiagramViewModel.CurrentRepository.AllOf<IGraphItem>().Where(p => type.IsAssignableFrom(p.GetType()));

                    var menu = new SelectionMenu();

                    foreach (var graphItem in items)
                    {
                        var graphItem1 = graphItem;
                        menu.AddItem(new SelectionMenuItem(graphItem, () =>
                        {
                            InvertApplication.Execute(() =>
                            {
                                d.Setter(d.DataObject, graphItem1);
                            });
                        }));
                    }

                    InvertApplication.SignalEvent<IShowSelectionMenu>(_ => _.ShowSelectionMenu(menu));



                    //
                    //                    InvertGraphEditor.WindowManager.InitItemWindow(items, 
                    //                        
                    //                    },true);

                }
                SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                GUI.color = colorCache;
                //GUILayout.EndHorizontal();
                return;
            }


            if (d.Type == typeof(string))
            {
                if (d.InspectorType == InspectorType.TextArea)
                {
                    EditorGUILayout.LabelField(d.Name, labelWidtho);
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);
                    EditorGUI.BeginChangeCheck();
                    d.CachedValue = EditorGUILayout.TextArea((string)d.CachedValue, GUILayout.Height(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);

                    }
                    if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                    {
                        InvertApplication.Execute(() =>
                        {

                        });
                    }
                }
                else if (d.InspectorType == InspectorType.TypeSelection)
                {
                    GUILayout.BeginHorizontal();
                    //GUILayout.Label(d.ViewModel.Name);

                    if (GUILayout.Button((string)d.CachedValue))
                    {
                        d.NodeViewModel.Select();
                        // TODO 2.0 Open Selection?
                    }
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);


                    GUILayout.EndHorizontal();
                }

                else
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle);
                    d.CachedValue = EditorGUILayout.TextField((string)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }

            }
            else
            {
                if (d.Type == typeof(int))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.IntField((int)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(float))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.FloatField((float)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(Vector2))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.Vector2Field(string.Empty, (Vector3)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }

                else if (d.Type == typeof(Vector3))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.Vector3Field(string.Empty, (Vector3)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }

                }
                else if (d.Type == typeof(Color))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.ColorField((Color)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }

                }
                else if (d.Type == typeof(Vector4))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.Vector4Field(string.Empty, (Vector4)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(bool))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.Toggle((bool)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (typeof(Enum).IsAssignableFrom(d.Type))
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(d.Name, labelStyle, labelWidtho);
                    d.CachedValue = EditorGUILayout.EnumPopup((Enum)d.CachedValue);
                    GUILayout.EndHorizontal();
                    SetTooltipForRect(GUILayoutUtility.GetLastRect(), d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        InvertApplication.Execute(() =>
                        {
                            d.Setter(d.DataObject, d.CachedValue);
                        });

                    }
                }
                else if (d.Type == typeof(Type))
                {
                    //InvertGraphEditor.WindowManager.InitTypeListWindow();
                }
            }

            GUI.color = colorCache;

        }

        public GUIStyle TextWrappingTextArea
        {
            get
            {
                if (mTextWrappingTextArea == null)
                {
                    mTextWrappingTextArea = new GUIStyle(EditorStyles.textArea);
                    mTextWrappingTextArea.wordWrap = true;
                }
                return mTextWrappingTextArea;
            }
            set { mTextWrappingTextArea = value; }
        }

        public virtual void DrawInspector(Rect rect, PropertyFieldViewModel d, GUIStyle labelStyle)
        {
            var colorCache = GUI.color;
            GUI.color = Color.white;
            var labelArea = rect.LeftHalf();
            var fieldArea = rect.RightHalf();
            var labelWidtho = GUILayout.Width(140);

            if (d.InspectorType == InspectorType.GraphItems)
            {
                var item = d.CachedValue as IGraphItem;
                var text = "--Select--";
                if (item != null)
                {
                    text = item.Label;
                }

                if (GUI.Button(rect, d.Label + ": " + text, ElementDesignerStyles.ButtonStyle))
                {
                    var type = d.Type;

                    var items = InvertGraphEditor.CurrentDiagramViewModel.CurrentRepository.AllOf<IGraphItem>().Where(p => type.IsAssignableFrom(p.GetType()));

                    var menu = new SelectionMenu();
                    menu.AddItem(new SelectionMenuItem(string.Empty,"[None]", () =>
                    {
                        InvertApplication.Execute(() =>
                        {
                            d.Setter(d.DataObject, null);
                        });
                    }));
                    foreach (var graphItem in items)
                    {
                        var graphItem1 = graphItem;
                        menu.AddItem(new SelectionMenuItem(graphItem1, () =>
                        {
                            InvertApplication.Execute(() =>
                            {
                                d.Setter(d.DataObject, graphItem1);
                            });
                        }));
                    }

                    InvertApplication.SignalEvent<IShowSelectionMenu>(_ => _.ShowSelectionMenu(menu));
                }
                SetTooltipForRect(rect, d.InspectorTip);

                GUI.color = colorCache;
                return;
            }


            if (d.Type == typeof(string))
            {
                if (d.InspectorType == InspectorType.TextArea)
                {
                    labelArea = rect.WithHeight(17).InnerAlignWithUpperRight(rect);
                    fieldArea = rect.Below(labelArea).Clip(rect).PadSides(2);
                    EditorGUI.LabelField(labelArea, d.Name, labelStyle);
                    SetTooltipForRect(rect, d.InspectorTip);
                    EditorGUI.BeginChangeCheck();
                    d.CachedValue = EditorGUI.TextArea(fieldArea, (string)d.CachedValue, TextWrappingTextArea);
                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                    if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                    {
                        InvertApplication.Execute(() =>
                        {

                        });
                    }
                }
                else if (d.InspectorType == InspectorType.TypeSelection)
                {

                    if (GUI.Button(rect, (string)d.CachedValue))
                    {
                        d.NodeViewModel.Select();
                        // TODO 2.0 Open Selection?
                    }
                    SetTooltipForRect(rect, d.InspectorTip);


                }

                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.LabelField(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.TextField(fieldArea, (string)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }

            }
            else
            {
                if (d.Type == typeof(int))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.IntField(fieldArea, (int)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(float))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.FloatField(fieldArea, (float)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(Vector2))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.Vector2Field(fieldArea, string.Empty, (Vector2)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }

                else if (d.Type == typeof(Vector3))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.Vector3Field(fieldArea, string.Empty, (Vector3)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }

                }
                else if (d.Type == typeof(Color))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.ColorField(fieldArea, (Color)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }

                }
                else if (d.Type == typeof(Vector4))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.Vector4Field(fieldArea, string.Empty, (Vector4)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (d.Type == typeof(bool))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.Toggle(fieldArea, (bool)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {

                        d.Setter(d.DataObject, d.CachedValue);
                    }
                }
                else if (typeof(Enum).IsAssignableFrom(d.Type))
                {
                    EditorGUI.BeginChangeCheck();
                    GUI.Label(labelArea, d.Name, labelStyle);
                    d.CachedValue = EditorGUI.EnumPopup(fieldArea, (Enum)d.CachedValue);
                    SetTooltipForRect(rect, d.InspectorTip);

                    if (EditorGUI.EndChangeCheck())
                    {
                        InvertApplication.Execute(() =>
                        {
                            d.Setter(d.DataObject, d.CachedValue);
                        });

                    }
                }
                else if (d.Type == typeof(Type))
                {
                    //InvertGraphEditor.WindowManager.InitTypeListWindow();
                }
            }

            GUI.color = colorCache;
        }

        public void DrawConnector(float scale, ConnectorViewModel viewModel)
        {

        }
    }
}