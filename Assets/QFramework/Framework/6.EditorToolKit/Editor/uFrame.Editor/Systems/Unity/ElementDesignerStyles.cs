using System;
using System.Reflection;
using Invert.Common.UI;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity.Schemas;
using UnityEditor;
using UnityEngine;

namespace Invert.Common
{
    public static class ElementDesignerStyles
    {
        private static GUIStyle _addButtonStyle;
        private static GUIStyle _addButtonStyleUnscaled;
        private static GUIStyle _arrowDownButtonStyle;
        private static Texture2D _arrowDownTexture;
        private static Texture2D _arrowLeftEmptyTexture;
        private static Texture2D _arrowLeftTexture;
        private static Texture2D _arrowRightEmptyTexture;
        private static Texture2D _arrowRightTexture;
        private static Texture2D _arrowUpTexture;
        private static GUIStyle _background;
        private static GUIStyle _boxHighlighter1;
        private static GUIStyle _boxHighlighter2;
        private static GUIStyle _boxHighlighter3;
        private static GUIStyle _boxHighlighter4;
        private static GUIStyle _boxHighlighter5;
        private static GUIStyle _boxHighlighter6;
        private static GUIStyle _breakpointButtonStyle;
        private static GUIStyle _buttonStyle;
        private static Texture2D _circleLeftTexture;
        private static Texture2D _circleRightTexture;
        private static Texture2D _circleTexture;
        private static GUIStyle _clearItemStyle;
        private static GUIStyle _collapseArrowRight;
        private static Texture2D _collapseRightArrowTexture;
        private static GUIStyle _commandBarClosedStyle;
        private static GUIStyle _diagramBox1;
        private static GUIStyle _diagramBox10;
        private static GUIStyle _diagramBox11;
        private static GUIStyle _diagramBox2;
        private static GUIStyle _diagramBox3;
        private static GUIStyle _diagramBox4;
        private static GUIStyle _diagramBox5;
        private static GUIStyle _diagramBox6;
        private static GUIStyle _diagramBox7;
        private static GUIStyle _diagramBox8;
        private static GUIStyle _diagramBox9;
        private static GUIStyle _diagramTitle;
        private static GUIStyle _eventActiveButtonStyle;
        private static Texture2D _eventActiveTexture;
        private static GUIStyle _eventButtonLargeStyle;
        private static GUIStyle _eventButtonStyle;
        private static GUIStyle _eventForwardButtonStyle;
        private static GUIStyle _eventInActiveButtonStyle;
        private static Texture2D _eventInActiveTexture;
        private static GUIStyle _eventSmallButtonStyle;
        private static GUIStyle _eyeBall;
        private static GUIStyle _gridLines;
        private static GUIStyle _headerStyle;
        private static GUIStyle _item1;
        private static GUIStyle _item2;
        private static GUIStyle _item3;
        private static GUIStyle _item4;
        private static GUIStyle _item5;
        private static GUIStyle _item6;
        private static GUIStyle _itemStyle;
        private static GUIStyle _itemTextEditingStyle;
        private static GUIStyle _navigateNextStyle;
        private static GUIStyle _navigatePreviousStyle;
        private static GUIStyle _nodeBackground;
        private static GUIStyle _nodeCollapse;
        private static GUIStyle _nodeExpand;
        private static GUIStyle _nodeHeader1;
        private static GUIStyle _nodeHeader10;
        private static GUIStyle _nodeHeader11;
        private static GUIStyle _nodeHeader12;
        private static GUIStyle _nodeHeader13;
        private static GUIStyle _nodeHeader2;
        private static GUIStyle _nodeHeader3;
        private static GUIStyle _nodeHeader4;
        private static GUIStyle _nodeHeader5;
        private static GUIStyle _nodeHeader6;
        private static GUIStyle _nodeHeader7;
        private static GUIStyle _nodeHeader8;
        private static GUIStyle _nodeHeader9;
        private static GUIStyle _overlay;
        private static GUIStyle _pasteButtonStyle;
        private static GUIStyle _removeButtonStyle;
        private static float _scale = 0.0f;
        private static GUIStyle _sceneViewBar;
        private static GUIStyle _selectedItemStyle;
        private static GUIStyle _seperatorStyle;
        private static GUIStyle _subHeaderStyle;
        private static GUIStyle _subLabelStyle;
        private static GUIStyle _tag1;
        private static GUIStyle _tag2;
        private static GUIStyle _titleBarStyle;
        private static GUIStyle _toolbarStyle;
        private static GUIStyle _toolbarStyleCollapsed;
        private static GUIStyle _viewBarSubTitleStyle;
        private static GUIStyle _viewBarTitleStyle;
        private static GUIStyle _viewModelHeaderEditingStyle;
        private static GUIStyle _viewModelHeaderStyle;
        private static GUIStyle _eventButtonStyleSmall;
        private static GUIStyle _tabStyle;
        private static GUIStyle _tabInActiveStyle;
        private static Texture2D _arrowRightTexture2;
        private static Texture2D _diagramCircleConnector;
        private static GUIStyle _graphTitleLabel;
        private static GUIStyle _headerTitleStyle;
        private static INodeStyleSchema _baseNodeStyleSchema;
        private static INodeStyleSchema _viewNodeStyleSchema;
        private static IConnectorStyleSchema _connectorStyleSchemaTriangle;
        private static IConnectorStyleSchema _connectorStyleSchemaCircle;
        private static INodeStyleSchema _baseNormalStyleSchema;
        private static INodeStyleSchema _nodeStyleSchemaMinimalistic;
        private static INodeStyleSchema _nodeStyleSchemaBold;
        private static GUIStyle _nodeBackgroundBorderless;
        private static GUIStyle _breadcrumbBoxStyle;
        private static GUIStyle _breadcrumbTitleStyle;
        private static GUIStyle _breadcrumbBoxActiveStyle;
        private static IBreadcrumbsStyleSchema _defaultBreadcrumbsStyleSchema;
        private static GUIStyle _tabBoxStyle;
        private static GUIStyle _tabBoxActiveStyle;
        private static GUIStyle _tabTitleStyle;
        private static GUIStyle _wizardBoxStyle;
        private static GUIStyle _wizardSubBoxStyle;
        private static GUIStyle _wizardActionButtonStyle;
        private static GUIStyle _wizardActionTitleStyle;
        private static GUIStyle _wizardSubBoxTitleStyle;
        private static GUIStyle _tooltipBoxStyle;
        private static GUIStyle _wizardListItemBoxStyle;
        private static GUIStyle _searchBarText;
        private static GUIStyle _listItemTitleStyle;
        private static GUIStyle _darkButtonStyle;
        private static GUIStyle _darkInspectorLabel;
        private static GUIStyle _wizardActionButtonStyleSmall;

        public static GUIStyle DarkInspectorLabel
        {
            get { return _darkInspectorLabel ?? (_darkInspectorLabel = new GUIStyle(EditorStyles.label)).WithAllStates(Color.white); }
            set { _darkInspectorLabel = value; }
        }

        public static GUIStyle AddButtonStyle
        {
            get
            {
                if (_addButtonStyle == null)
                    _addButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Plus"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f * Scale,
                        fixedWidth = 16f * Scale
                    };
                return _addButtonStyle;
            }
        }
        public static GUIStyle TabStyle
        {
            get
            {
                if (_tabStyle == null)
                    _tabStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Tab"), textColor = InvertGraphEditor.Settings.TabTextColor },
                        //padding = new RectOffset(10, 10, 10, 10),
                        border = new RectOffset(11,11,0,0),
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = 31,
                        //fixedWidth = 16f * Scale
                    };
                return _tabStyle;
            }
            set { _tabStyle = value; }
        }       
        
        public static GUIStyle TabCloseButtonStyle
        {
            get
            {
                if (_tabStyle == null)
                    _tabStyle = new GUIStyle().WithAllStates("CloseIcon", Color.clear);
                return _tabStyle;
            }
            set { _tabStyle = value; }
        }       
        
        
        public static GUIStyle TabInActiveStyle
        {
            get
            {
                if (_tabInActiveStyle == null)
                    _tabInActiveStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("TabInActive"), textColor = InvertGraphEditor.Settings.TabTextColor },
                        //padding = new RectOffset(10, 10, 10, 10),
                        border = new RectOffset(11,11,0,0),
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = 31,
                        //fixedWidth = 16f * Scale
                    };
                return _tabInActiveStyle;
            }
        }
        public static GUIStyle AddButtonStyleUnscaled
        {
            get
            {
                if (_addButtonStyleUnscaled == null)
                    _addButtonStyleUnscaled = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Plus"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };
                return _addButtonStyleUnscaled;
            }
        }

        public static Texture2D ArrowDownTexture
        {
            get
            {
                return _arrowDownTexture ?? (_arrowDownTexture = GetSkinTexture("DiagramArrowDown"));
            }
        }

        public static Texture2D ArrowLeftEmptyTexture
        {
            get
            {
                return _arrowLeftEmptyTexture ?? (_arrowLeftEmptyTexture = GetSkinTexture("DiagramArrowLeftEmpty"));
            }
        }
        public static Texture2D DiagramCircleConnector
        {
            get
            {
                return _diagramCircleConnector ?? (_diagramCircleConnector = GetSkinTexture("DiagramCircleConnector"));
            }
        }
        public static Texture2D ArrowLeftTexture
        {
            get
            {
                return _arrowLeftTexture ?? (_arrowLeftTexture = GetSkinTexture("DiagramArrowLeft"));
            }
        }

        public static Texture2D ArrowRightEmptyTexture
        {
            get
            {
                return _arrowRightEmptyTexture ?? (_arrowRightEmptyTexture = GetSkinTexture("DiagramArrowRightEmpty"));
            }
        }

        public static Texture2D ArrowRightTexture
        {
            get
            {
                return _arrowRightTexture ?? (_arrowRightTexture = GetSkinTexture("DiagramArrowRight"));
            }
        }

        public static Texture2D ArrowRightTexture2
        {
            get
            {
                return _arrowRightTexture2 ?? (_arrowRightTexture2 = GetSkinTexture("ArrowRight"));
            }
        }
        public static Texture2D ArrowUpTexture
        {
            get
            {
                return _arrowUpTexture ?? (_arrowUpTexture = GetSkinTexture("DiagramArrowUp"));
            }
        }

        public static GUIStyle Background
        {
            get
            {
                if (_background == null)
                    _background = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Background"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _background;
            }
        }

        public static GUIStyle BoxHighlighter1
        {
            get
            {
                if (_boxHighlighter2 == null)
                    _boxHighlighter2 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter1"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter2;
            }
        }

        public static GUIStyle BoxHighlighter2
        {
            get
            {
                if (_boxHighlighter1 == null)
                    _boxHighlighter1 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter2"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(44, 44, 35, 35),
                        padding = new RectOffset(10, 10, 7, 7),
                        margin = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter1;
            }
        }

        public static GUIStyle BoxHighlighter3
        {
            get
            {
                if (_boxHighlighter3 == null)
                    _boxHighlighter3 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter3"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(44, 44, 35, 35),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter3;
            }
        }

        public static GUIStyle BoxHighlighter4
        {
            get
            {
                if (_boxHighlighter4 == null)
                    _boxHighlighter4 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter4"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter4;
            }
        }

        public static GUIStyle BoxHighlighter5
        {
            get
            {
                if (_boxHighlighter5 == null)
                    _boxHighlighter5 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter5"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter5;
            }
        }

        public static GUIStyle BoxHighlighter6
        {
            get
            {
                if (_boxHighlighter6 == null)
                    _boxHighlighter6 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("BoxHighlighter6"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _boxHighlighter6;
            }
        }

        public static GUIStyle BreakpointButtonStyle
        {
            get
            {
                if (_breakpointButtonStyle == null)
                    _breakpointButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Breakpoint"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };

                return _breakpointButtonStyle;
            }
        }

        public static GUIStyle ButtonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    var textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
                    _buttonStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("CommandBar"),
                            textColor = textColor
                        },
                        focused =
                        {
                            background = GetSkinTexture("CommandBar"),
                            textColor = textColor
                        },
                        active = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        hover = { background = GetSkinTexture("CommandBar_Hover"), textColor = textColor },
                        onHover =
                        {
                            background = GetSkinTexture("CommandBar_Hover"),
                            textColor = textColor
                        },
                        onFocused = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        onNormal = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        onActive = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        fixedHeight = 25,
                        alignment = TextAnchor.MiddleCenter,
                    };
                }
                return _buttonStyle;
            }
        }     
        
        public static GUIStyle DarkButtonStyle
        {
            get
            {
                if (_darkButtonStyle == null)
                {
                    var textColor =  new Color(0.8f, 0.8f, 0.8f);
                    _darkButtonStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("CommandBar"),
                            textColor = textColor
                        },
                        focused =
                        {
                            background = GetSkinTexture("CommandBar"),
                            textColor = textColor
                        },
                        active = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        hover = { background = GetSkinTexture("CommandBar_Hover"), textColor = textColor },
                        onHover =
                        {
                            background = GetSkinTexture("CommandBar_Hover"),
                            textColor = textColor
                        },
                        onFocused = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        onNormal = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        onActive = { background = GetSkinTexture("EventButton"), textColor = textColor },
                        fixedHeight = 25,
                        alignment = TextAnchor.MiddleCenter,
                    };
                }
                return _darkButtonStyle;
            }
        }

        public static Texture2D CircleLeftTexture
        {
            get
            {
                return _circleLeftTexture ?? (_circleLeftTexture = GetSkinTexture("DiagramCircleLeft"));
            }
        }

        public static Texture2D CircleRightTexture
        {
            get
            {
                return _circleRightTexture ?? (_circleRightTexture = GetSkinTexture("DiagramCircleRight"));
            }
        }

        public static GUIStyle ClearItemStyle
        {
            get
            {
                if (_clearItemStyle == null)
                    _clearItemStyle = new GUIStyle
                    {
                        normal = { textColor = new Color(0.6f, 0.6f, 0.8f) },
                        stretchHeight = true,
                        stretchWidth = true,
                        fontSize = Mathf.RoundToInt(8f * Scale),
                        
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };

                return _clearItemStyle;
            }
        }

        public static GUIStyle CollapseDown
        {
            get
            {
                if (_arrowDownButtonStyle == null)
                    _arrowDownButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CollapseDown"), textColor = Color.white },
                        margin = new RectOffset(35, 0, 8, 0),
                        padding = new RectOffset(25, 0, 0, 0),
                        fixedHeight = 16f * Scale,
                        fixedWidth = 16f * Scale
                    };
                return _arrowDownButtonStyle;
            }
        }

        public static GUIStyle CollapseRight
        {
            get
            {
                if (_collapseArrowRight == null)
                    _collapseArrowRight = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CollapseRight"), textColor = Color.white },
                        margin = new RectOffset(35, 0, 10, 0),
                        padding = new RectOffset(10, 0, 0, 0),
                        fixedHeight = 16f * Scale,
                        fixedWidth = 16f * Scale
                    };
                return _collapseArrowRight;
            }
        }

        public static Texture2D CollapseRightArrowTexture
        {
            get
            {
                return _collapseRightArrowTexture ?? (_collapseRightArrowTexture = GetSkinTexture("CollapseRightArrow"));
            }
        }

        public static GUIStyle CommandBarClosedStyle
        {
            get
            {
                if (_commandBarClosedStyle == null)
                    _commandBarClosedStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CommandBar"), textColor = Color.white },
                        padding = new RectOffset(7, 2, 4, 2),
                        margin = new RectOffset(0, 0, 0, 0),
                        fixedHeight = 22f,

                        stretchHeight = true,
                    };
                return _commandBarClosedStyle;
            }
        }

        public static GUIStyle DiagramBox1
        {
            get
            {
                //if (_diagramBox1 == null)
                _diagramBox1 = new GUIStyle
                {
                    normal = { background = GetSkinTexture("Box1"), textColor = new Color(0.3f, 0.3f, 0.3f) },

                    stretchHeight = true,
                    stretchWidth = true,
                    // border = new RectOffset(44, 50, 20, 34),
                    padding = new RectOffset(7, 7, 7, 7)
                };

                return _diagramBox1;
            }
        }

        public static GUIStyle DiagramBox10
        {
            get
            {
                if (_diagramBox10 == null)
                    _diagramBox10 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box10"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox10;
            }
        }

        public static GUIStyle DiagramBox11
        {
            get
            {
                if (_diagramBox11 == null)
                    _diagramBox11 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box11"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox11;
            }
        }

        public static GUIStyle DiagramBox2
        {
            get
            {
                if (_diagramBox2 == null)
                    _diagramBox2 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box2"), textColor = Color.white },

                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox2;
            }
        }

        public static GUIStyle DiagramBox3
        {
            get
            {
                if (_diagramBox3 == null)
                    _diagramBox3 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box3"), textColor = new Color(0.2f, 0.2f, 0.2f) },

                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox3;
            }
        }

        public static GUIStyle DiagramBox4
        {
            get
            {
                if (_diagramBox4 == null)
                    _diagramBox4 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box4"), textColor = new Color(0.22f, 0.22f, 0.22f) },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox4;
            }
        }

        public static GUIStyle DiagramBox5
        {
            get
            {
                if (_diagramBox5 == null)
                    _diagramBox5 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box5"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox5;
            }
        }

        public static GUIStyle DiagramBox6
        {
            get
            {
                if (_diagramBox6 == null)
                    _diagramBox6 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box6"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox6;
            }
        }

        public static GUIStyle DiagramBox7
        {
            get
            {
                if (_diagramBox7 == null)
                    _diagramBox7 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box7"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox7;
            }
        }

        public static GUIStyle DiagramBox8
        {
            get
            {
                if (_diagramBox8 == null)
                    _diagramBox8 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box8"), textColor = new Color(0.9f, 0.9f, 0.9f) },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox8;
            }
        }

        public static GUIStyle DiagramBox9
        {
            get
            {
                if (_diagramBox9 == null)
                    _diagramBox9 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Box9"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        border = new RectOffset(171, 180, 120, 135),
                        padding = new RectOffset(7, 7, 7, 7)
                    };

                return _diagramBox9;
            }
        }

        public static GUIStyle DiagramTitle
        {
            get
            {
                if (_diagramTitle == null)
                {
                    _diagramTitle = new GUIStyle()
                    {
                        normal = { background = GetSkinTexture("SelectedNodeItem"), textColor = new Color(0.45f, 0.45f, 0.45f) },
                        padding = new RectOffset(4, 4, 4, 4),
                        stretchWidth = true,
                        stretchHeight = true,

                        fontSize = 18,// Mathf.RoundToInt(10f * Scale),
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _diagramTitle;
            }
        }

        public static Texture2D EventActiveTexture
        {
            get
            {
                return _eventActiveTexture ?? (_eventActiveTexture = GetSkinTexture("EventActive"));
            }
        }

        public static GUIStyle EventButtonLargeStyle
        {
            get
            {
                var textColor = Color.white;
                if (_eventButtonLargeStyle == null)
                    _eventButtonLargeStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CommandBar"), textColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey },
                        active = { background = CommandBarClosedStyle.normal.background },
                        stretchHeight = true,

                        fixedHeight = 35,
                        border = new RectOffset(3, 3, 3, 3),

                        padding = new RectOffset(25, 0, 5, 5)
                    };

                return _eventButtonLargeStyle;
            }
        }
        public static GUIStyle EventButtonStyleSmall
        {
            get
            {
                var textColor = Color.white;
                if (_eventButtonStyleSmall == null)
                    _eventButtonStyleSmall = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("EventButton"), textColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey },
                        active = { background = CommandBarClosedStyle.normal.background },
                        stretchHeight = true,

                        fixedHeight = 25,
                        border = new RectOffset(3, 3, 3, 3),

                        padding = new RectOffset(25, 0, 5, 5)
                    };

                return _eventButtonStyleSmall;
            }
        }
        public static GUIStyle EventButtonStyle
        {
            get
            {
                if (_eventButtonStyle == null)
                    _eventButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CommandBar"), textColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey },
                        active = { background = CommandBarClosedStyle.normal.background },
                        stretchHeight = true,

                        fixedHeight = 55,
                        border = new RectOffset(3, 3, 3, 3),

                        padding = new RectOffset(25, 0, 5, 5)
                    };

                return _eventButtonStyle;
            }
        }

        public static Texture2D EventInActiveTexture
        {
            get
            {
                return _eventInActiveTexture ?? (_eventInActiveTexture = GetSkinTexture("EventInActive"));
            }
        }

        public static GUIStyle EventSmallButtonStyle
        {
            get
            {
                if (_eventSmallButtonStyle == null)
                    _eventSmallButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("EventButton"), textColor = new Color(01f,1f,1f,0.7f) },
                        active = { background = CommandBarClosedStyle.normal.background },
                       alignment = TextAnchor.MiddleCenter,
                       fontSize = 10
                        
                    };

                return _eventSmallButtonStyle;
            }
        }

        public static GUIStyle EyeBall
        {
            get
            {
                if (_eyeBall == null)
                {
                    var textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
                    _eyeBall = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("EyeBall"),
                            textColor = textColor
                        },
                        focused =
                        {
                            background = GetSkinTexture("EyeBall"),
                            textColor = textColor
                        },
                        active = { background = GetSkinTexture("EyeBall2"), textColor = textColor },
                        hover = { background = GetSkinTexture("EyeBall"), textColor = textColor },
                        onHover =
                        {
                            background = GetSkinTexture("EyeBall"),
                            textColor = textColor
                        },
                        onFocused = { background = GetSkinTexture("EyeBall2"), textColor = textColor },
                        onNormal = { background = GetSkinTexture("EyeBall2"), textColor = textColor },
                        onActive = { background = GetSkinTexture("EyeBall2"), textColor = textColor },
                        fixedHeight = 48,
                        fixedWidth = 36,
                        alignment = TextAnchor.MiddleCenter,
                    };
                }
                return _eyeBall;
            }
        }

        public static GUIStyle GridLines
        {
            get
            {
                if (_gridLines == null)
                    _gridLines = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("GridLines"), textColor = Color.white },
                        stretchHeight = false,
                        stretchWidth = false,

                        //border = new RectOffset(171, 180, 120, 135),
                        //padding = new RectOffset(7, 7, 7, 7)
                    };

                return _gridLines;
            }
        }

        public static GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle()
                    {
                        normal = { textColor = InvertGraphEditor.Settings.SectionTitleColor },//new Color(0.35f, 0.35f, 0.35f) },
                        //padding = new RectOffset(4, 4, 4, 4),
                        fontSize = Mathf.RoundToInt(10f * Scale),
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _headerStyle;
            }
            set { _headerStyle = null; }
        }

        public static GUIStyle Item1
        {
            get
            {
                if (_item1 == null)
                    _item1 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item1"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        fixedHeight = 18f * Scale,
                        fontSize = Mathf.RoundToInt(9f * Scale),
                        alignment = TextAnchor.MiddleCenter
                    };

                return _item1;
            }
        }

        public static GUIStyle Item2
        {
            get
            {
                if (_item2 == null)
                    _item2 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item2"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        //fixedHeight = 18f * Scale,
                        fontSize = Mathf.RoundToInt(9f * Scale),
                        alignment = TextAnchor.MiddleCenter
                    };

                return _item2;
            }
        }

        public static GUIStyle Item3
        {
            get
            {
                if (_item3 == null)
                    _item3 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item3"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        //fixedHeight = 18f * Scale,
                        fontSize = Mathf.RoundToInt(9f * Scale),
                        alignment = TextAnchor.MiddleCenter
                    };

                return _item3;
            }
        }

        public static GUIStyle Item4
        {
            get
            {
                if (_item4 == null)
                    _item4 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item4"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        fontSize = Mathf.RoundToInt(9f * Scale),
                        fixedHeight = 18f * Scale,
                        alignment = TextAnchor.MiddleCenter
                    };

                return _item4;
            }
        }

        public static GUIStyle Item5
        {
            get
            {
                if (_item5 == null)
                    _item5 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item5"), textColor = new Color(0.8f, 0.8f, 0.8f) },
                        stretchHeight = false,
                        fontSize = Mathf.RoundToInt(9 * Scale),
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = 18f * Scale,
                    };

                return _item5;
            }
        }

        public static GUIStyle Item6
        {
            get
            {
                if (_item6 == null)
                    _item6 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item6"), textColor = Color.white },
                        stretchHeight = true,
                        fixedHeight = 25f * Scale,
                        fontSize = Mathf.RoundToInt(9 * Scale),
                        alignment = TextAnchor.MiddleCenter
                    };

                return _item6;
            }
        }

        public static GUIStyle ItemStyle
        {
            get
            {
                return Item5;
                //if (_itemStyle == null)
                //    _itemStyle = new GUIStyle
                //    {
                //        normal = { background = GetSkinTexture("Item5"), textColor = new Color(0.8f, 0.8f, 0.8f) },
                //        padding = new RectOffset(2, 6, 2, 2),
                //        margin = new RectOffset(0, 0, 0, 0),
                //        fixedHeight = 18f,

                //        alignment = TextAnchor.MiddleLeft,
                //        fontSize = 9,
                //        //fontStyle = FontStyle.Bold
                //    };
                //return _itemStyle;
            }
        }

        public static GUIStyle ItemTextEditingStyle
        {
            get
            {
                if (_itemTextEditingStyle == null)
                {
                    _itemTextEditingStyle = new GUIStyle();
                    _itemTextEditingStyle.normal.background = null;
                    _itemTextEditingStyle.normal.textColor = InvertGraphEditor.Settings.SectionItemColor; //new Color(0.65f,0.65f,0.65f);
                    _itemTextEditingStyle.active.background = null;
                    _itemTextEditingStyle.hover.background = null;
                    _itemTextEditingStyle.focused.background = null;
                    _itemTextEditingStyle.alignment = TextAnchor.MiddleCenter;
                    _itemTextEditingStyle.fontSize = Mathf.RoundToInt(10 * Scale);
                    
                    //_itemTextEditingStyle.fontStyle = FontStyle.Bold;
                    //_itemTextEditingStyle.padding = new RectOffset(0, 0, 6, 0);
                    //_viewModelHeaderEditingStyle.fontSize = Mathf.RoundToInt(12*Scale);
                }
                return _itemTextEditingStyle;
            }
            set { _itemTextEditingStyle = value; }
        }

        public static GUIStyle NavigateNextStyle
        {
            get
            {
                if (_navigateNextStyle == null)
                {
                    var textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
                    _navigateNextStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("NavigateNext"),
                            textColor = textColor
                        },
                        focused =
                        {
                            background = GetSkinTexture("NavigateNext2"),
                            textColor = textColor
                        },
                        active = { background = GetSkinTexture("NavigateNext"), textColor = textColor },
                        hover = { background = GetSkinTexture("NavigateNext2"), textColor = textColor },
                        onHover =
                        {
                            background = GetSkinTexture("NavigateNext"),
                            textColor = textColor
                        },
                        onFocused = { background = GetSkinTexture("NavigateNext2"), textColor = textColor },
                        onNormal = { background = GetSkinTexture("NavigateNext"), textColor = textColor },
                        onActive = { background = GetSkinTexture("NavigateNext2"), textColor = textColor },
                        fixedHeight = 48,
                        fixedWidth = 36,
                        alignment = TextAnchor.MiddleCenter,
                    };
                }
                return _navigateNextStyle;
            }
        }

        public static GUIStyle NavigatePreviousStyle
        {
            get
            {
                if (_navigatePreviousStyle == null)
                {
                    var textColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
                    _navigatePreviousStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("NavigatePrevious"),
                            textColor = textColor
                        },
                        focused =
                        {
                            background = GetSkinTexture("NavigatePrevious2"),
                            textColor = textColor
                        },
                        active = { background = GetSkinTexture("NavigatePrevious"), textColor = textColor },
                        hover = { background = GetSkinTexture("NavigatePrevious2"), textColor = textColor },
                        onHover =
                        {
                            background = GetSkinTexture("NavigatePrevious"),
                            textColor = textColor
                        },
                        onFocused = { background = GetSkinTexture("NavigatePrevious2"), textColor = textColor },
                        onNormal = { background = GetSkinTexture("NavigatePrevious"), textColor = textColor },
                        onActive = { background = GetSkinTexture("NavigatePrevious"), textColor = textColor },
                        fixedHeight = 48,
                        fixedWidth = 36,
                        alignment = TextAnchor.MiddleCenter,
                    };
                }
                return _navigatePreviousStyle;
            }
        }

        public static GUIStyle NodeBackground
        {
            get
            {
                //if (_diagramBox1 == null)
                _nodeBackground = new GUIStyle
                {
                    normal = { background = GetSkinTexture("Box12"), textColor = new Color(0.82f, 0.82f, 0.82f) },
                    
                    stretchHeight = true,
                    stretchWidth = true,
                    
                    //border = new RectOffset(20,20,20,20)
                     border = new RectOffset(44, 50, 20, 34),
                    //padding = new RectOffset(9,1,19,9)
                   
                    //padding = new RectOffset(7, 7, 7, 7)
                };

                return _nodeBackground;
            }
        }
        
        public static GUIStyle NodeBackgroundBorderless
        {
            get
            {
                //if (_diagramBox1 == null)
                _nodeBackgroundBorderless = new GUIStyle
                {
                    normal = { background = GetSkinTexture("Box12Borderless"), textColor = new Color(0.82f, 0.82f, 0.82f) },
                    
                    stretchHeight = true,
                    stretchWidth = true,
                    
                    //border = new RectOffset(20,20,20,20)
                     border = new RectOffset(44, 50, 20, 34),
                    //padding = new RectOffset(9,1,19,9)
                   
                    //padding = new RectOffset(7, 7, 7, 7)
                };

                return _nodeBackgroundBorderless;
            }
        }

        public static GUIStyle NodeCollapse
        {
            get
            {
                if (_nodeCollapse == null)
                    _nodeCollapse = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("NodeCollapse"), textColor = Color.white },
                    };
                return _nodeCollapse;
            }
        }

        public static GUIStyle NodeExpand
        {
            get
            {
                if (_nodeExpand == null)
                    _nodeExpand = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("NodeExpand"), textColor = Color.white },
                    };
                return _nodeExpand;
            }
        }

        public static GUIStyle NodeHeader1
        {
            get
            {
                return _nodeHeader1 ?? (_nodeHeader1 = CreateHeader("Header1", new Color(0.8f, 0.8f, 0.8f)));
            }
        }

        public static GUIStyle NodeHeader10
        {
            get
            {
                return _nodeHeader10 ?? (_nodeHeader10 = CreateHeader("Header10", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader11
        {
            get
            {
                return _nodeHeader11 ?? (_nodeHeader11 = CreateHeader("Header11", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader12
        {
            get
            {
                return _nodeHeader12 ?? (_nodeHeader12 = CreateHeader("Header12", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader13
        {
            get
            {
                return _nodeHeader13 ?? (_nodeHeader13 = CreateHeader("Header13", new Color(0.1f, 0.1f, 0.1f)));
            }
        }

        public static GUIStyle NodeHeader2
        {
            get
            {
                return _nodeHeader2 ?? (_nodeHeader2 = CreateHeader("Header2", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader3
        {
            get
            {
                return _nodeHeader3 ?? (_nodeHeader3 = CreateHeader("Header3", new Color(0.23f, 0.23f, 0.23f)));
            }
        }

        public static GUIStyle NodeHeader4
        {
            get
            {
                return _nodeHeader4 ?? (_nodeHeader4 = CreateHeader("Header4", new Color(0.32f, 0.32f, 0.32f)));
            }
        }

        public static GUIStyle NodeHeader5
        {
            get
            {
                return _nodeHeader5 ?? (_nodeHeader5 = CreateHeader("Header5", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader6
        {
            get
            {
                return _nodeHeader6 ?? (_nodeHeader6 = CreateHeader("Header6", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader7
        {
            get
            {
                return _nodeHeader7 ?? (_nodeHeader7 = CreateHeader("Header7", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader8
        {
            get
            {
                return _nodeHeader8 ?? (_nodeHeader8 = CreateHeader("Header8", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle NodeHeader9
        {
            get
            {
                return _nodeHeader9 ?? (_nodeHeader9 = CreateHeader("Header9", new Color(0.82f, 0.82f, 0.82f)));
            }
        }

        public static GUIStyle Overlay
        {
            get
            {
                if (_overlay == null)
                    _overlay = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Item1"), textColor = Color.white },
                        stretchHeight = true,
                        stretchWidth = true,
                        fontSize = Mathf.RoundToInt(16f * Scale),
                        alignment = TextAnchor.MiddleCenter
                    };

                return _overlay;
            }
        }

        public static GUIStyle PasteButtonStyle
        {
            get
            {
                if (_pasteButtonStyle == null)
                    _pasteButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("edit-paste"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };
                return _pasteButtonStyle;
            }
        }

        public static GUIStyle RemoveButtonStyle
        {
            get
            {
                if (_removeButtonStyle == null)
                    _removeButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Remove"), textColor = Color.white },

                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };
                return _removeButtonStyle;
            }
        }

        public static float Scale
        {
            get
            {
                if (_scale == 0.0f)
                {
                    return _scale = EditorPrefs.GetFloat("ElementDesigner_Scale", 1f);
                }
                return _scale;
            }
            set
            {
                EditorPrefs.SetFloat("ElementDesigner_Scale", value);
                _scale = value;
                foreach (var field in typeof(ElementDesignerStyles).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                {
                    if (field.FieldType == typeof(GUIStyle))
                        field.SetValue(null, null);
                }
            }
        }

        public static GUIStyle BreadcrumbTitleStyle
        {
            get
            {
                return _breadcrumbTitleStyle ?? (_breadcrumbTitleStyle = new GUIStyle()
                {
                    wordWrap = true,
                    fontSize = 13
                }).WithAllStates(new Color(0.8f, 0.8f, 0.8f));
            }
        }

        public static GUIStyle TabBoxStyle
        {
            get { 
                return _tabBoxStyle ?? (_tabBoxStyle = new GUIStyle()
                {
                    border = new RectOffset(11, 11, 11, 11),
                }).WithAllStates("TabBox", new Color(1,1,1,0.8f));
            }
            set { _tabBoxStyle = value; }
        }

        public static GUIStyle TabTitleStyle
        {
            get {
                return _tabTitleStyle ?? (_tabTitleStyle = new GUIStyle()
                {
                    fontSize = 13
                }).WithAllStates(new Color(1f,1f,1f,0.8f));
            }
            set { _tabTitleStyle = value; }
        }

        public static GUIStyle TabBoxActiveStyle
        {
            get { return _tabBoxActiveStyle ?? (_tabBoxActiveStyle = new GUIStyle()
            {
                border = new RectOffset(11, 11, 11, 11),
            }).WithAllStates("TabBoxActive", new Color(1, 1, 1, 0.8f));
            }
            set { _tabBoxActiveStyle = value; }
        }

        public static GUIStyle BreadcrumbBoxStyle
        {
            get
            {
                return _breadcrumbBoxStyle ?? (_breadcrumbBoxStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                    border = new RectOffset(10, 10, 10, 10)
                }).WithAllStates("Box21", Color.white);
            }
            set { _breadcrumbBoxStyle = value; }
        }   
     
        public static GUIStyle BreadcrumbBoxActiveStyle
        {
            get
            {
                return _breadcrumbBoxActiveStyle ?? (_breadcrumbBoxActiveStyle = new GUIStyle()
                {
                    border = new RectOffset(10, 10, 10, 10)
                }).WithAllStates("Box21Active", Color.clear);
            }
            set { _breadcrumbBoxActiveStyle = value; }
        }   


        public static GUIStyle SceneViewBar
        {
            get
            {
                //if (_diagramBox1 == null)
                _sceneViewBar = new GUIStyle
                {
                    normal = { background = GetSkinTexture("SceneViewBar"), textColor = new Color(0.3f, 0.3f, 0.3f) },

                    stretchHeight = true,
                    stretchWidth = true,
                    // border = new RectOffset(44, 50, 20, 34),
                    padding = new RectOffset(7, 7, 7, 7)
                };

                return _sceneViewBar;
            }
        }

        public static GUIStyle SelectedItemStyle
        {
            get
            {
                if (_selectedItemStyle == null)
                {
                    _selectedItemStyle = new GUIStyle()
                    {
                        normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
                        active = { textColor = new Color(0.7f, 0.7f, 0.7f) },
                        stretchHeight = true,
                        padding = new RectOffset(0, 5, 0, 0),
                        fixedHeight = Mathf.RoundToInt(18 * Scale),
                        fontSize = Mathf.RoundToInt(10 * Scale),
                        alignment = TextAnchor.MiddleRight
                    };
                }
                return _selectedItemStyle;
            }
        }

        public static GUIStyle SeperatorStyle
        {
            get
            {
                if (_seperatorStyle == null)
                    _seperatorStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Seperator"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        stretchHeight = true,
                        fixedWidth = 3f
                    };
                return _seperatorStyle;
            }
        }

        public static string SkinName
        {
            get { return "images"; }
            //EditorGUIUtility.isProSkin ? "ProSkin" : "FreeSkin"; }
        }

        public static GUIStyle SubHeaderStyle
        {
            get
            {
                if (_subHeaderStyle == null)
                    _subHeaderStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("EventButton"), textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : Color.black },
                        padding = new RectOffset(2, 2, 2, 2),
                        margin = new RectOffset(0, 0, 0, 0),
                        fixedHeight = 18f,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 9,
                        //fontStyle = FontStyle.Bold
                    };
                return _subHeaderStyle;
            }
        }

        public static GUIStyle SubLabelStyle
        {
            get
            {
                if (_subLabelStyle == null)
                    _subLabelStyle = new GUIStyle
                    {
                        normal = new GUIStyleState() { textColor = new Color(0.6f, 0.6f, 0.6f) },
                        wordWrap = true,
                        fontSize = 9,
                        alignment = TextAnchor.MiddleCenter
                    };

                return _subLabelStyle;
            }
        }

        public static GUIStyle Tag1
        {
            get
            {
                if (_tag1 == null)
                    _tag1 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Tag1"), textColor = new Color(0.75f, 0.75f, 0.75f) },
                        padding = new RectOffset(8, 8, 5, 3),
                        border = new RectOffset(10, 10, 10, 0),
                        fixedHeight = 19f * Scale,
                        stretchWidth = true,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = Mathf.RoundToInt(10 * Scale),
                        fontStyle = FontStyle.Normal
                    };
                return _tag1;
            }
        }

        public static GUIStyle Tag2
        {
            get
            {
                if (_tag2 == null)
                    _tag2 = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Tag1"), textColor = Color.white },
                        padding = new RectOffset(7, 7, 5, 3),
                        border = new RectOffset(10, 10, 10, 0),
                        fixedHeight = 19f * Scale,
                        stretchWidth = true,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = Mathf.RoundToInt(12 * Scale),
                        fontStyle = FontStyle.Bold
                    };
                return _tag2;
            }
        }

        public static GUIStyle TitleBarStyle
        {
            get
            {
                if (_titleBarStyle == null)
                    _titleBarStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("Background"), textColor = new Color(0.7f, 0.7f, 0.7f) },
                        padding = new RectOffset(2, 2, 2, 2),
                        margin = new RectOffset(0, 0, 0, 0),
                        fixedHeight = 45f,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 14,
                        fontStyle = FontStyle.Bold
                        //fontStyle = FontStyle.Bold
                    };
                return _titleBarStyle;
            }
        }

        //public static unsafe GUIStyle SubHeaderStyle
        //{
        //    get
        //    {
        //        if (_subHeaderStyle == null)
        //            _subHeaderStyle = new GUIStyle
        //            {
        //                normal = { background = UBStyles.GetSkinTexture("EventButton"), textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.2f, 0.2f, 0.2f) },
        //                padding = new RectOffset(2, 2, 2, 2),
        //                margin = new RectOffset(0, 0, 0, 0),
        //                fixedHeight = 18f,
        //                alignment = TextAnchor.MiddleCenter,
        //                fontSize = 9,
        //                //fontStyle = FontStyle.Bold
        //            };
        //        return _subHeaderStyle;
        //    }
        //}
        public static GUIStyle ToolbarStyle
        {
            get
            {
                if (_toolbarStyle == null)
                    _toolbarStyle = new GUIStyle
                    {
                        normal =
                        {
                            background = GetSkinTexture("CommandExpanded"),
                            textColor = !EditorGUIUtility.isProSkin ?
                                new Color(0.2f, 0.2f, 0.2f) : new Color(0.7f, 0.7f, 0.7f)
                        },
                        //active = { background = CommandBarClosedStyle.normal.background },
                        fixedHeight = 28,
                        border = new RectOffset(3, 3, 3, 3),

                        stretchHeight = true,

                        //padding = new RectOffset(5, 5, 5, 0)
                    };

                return _toolbarStyle;
            }
        }

        //public static string SkinName
        //{
        //    get { return "images"; }
        //    //EditorGUIUtility.isProSkin ? "ProSkin" : "FreeSkin"; }
        //}
        public static GUIStyle ToolbarStyleCollapsed
        {
            get
            {
                if (_toolbarStyleCollapsed == null)
                    _toolbarStyleCollapsed = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("CommandExpanded"), textColor = new Color(0.7f, 0.7f, 0.7f) },
                        //active = { background = CommandBarClosedStyle.normal.background },
                        fixedHeight = 28,
                        border = new RectOffset(3, 3, 3, 3),

                        stretchHeight = true,

                        //padding = new RectOffset(5, 5, 5, 0)
                    };

                return _toolbarStyleCollapsed;
            }
        }

        public static GUIStyle TriggerActiveButtonStyle
        {
            get
            {
                if (_eventActiveButtonStyle == null)
                    _eventActiveButtonStyle = new GUIStyle
                    {
                        normal = { background = EventActiveTexture, textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };

                return _eventActiveButtonStyle;
            }
        }

        public static GUIStyle TriggerForwardButtonStyle
        {
            get
            {
                if (_eventForwardButtonStyle == null)
                    _eventForwardButtonStyle = new GUIStyle
                    {
                        normal = { background = GetSkinTexture("EventForward"), textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };

                return _eventForwardButtonStyle;
            }
        }

        public static GUIStyle TriggerInActiveButtonStyle
        {
            get
            {
                if (_eventInActiveButtonStyle == null)
                    _eventInActiveButtonStyle = new GUIStyle
                    {
                        normal = { background = EventInActiveTexture, textColor = Color.white },
                        padding = new RectOffset(10, 10, 10, 10),
                        fixedHeight = 16f,
                        fixedWidth = 16f
                    };
                return _eventInActiveButtonStyle;
            }
        }

        public static GUIStyle ViewBarSubTitleStyle
        {
            get
            {
                if (_viewBarSubTitleStyle == null)
                    _viewBarSubTitleStyle = new GUIStyle
                    {
                        normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 10,
                        //                    fontStyle = FontStyle.Bold
                        //fontStyle = FontStyle.Bold
                    };
                return _viewBarSubTitleStyle;
            }
        }

        public static GUIStyle WizardBoxStyle
        {
            get { return _wizardBoxStyle ?? (_wizardBoxStyle = new GUIStyle()
            {
                border = new RectOffset(4,4,4,4)
            }).WithAllStates("Box31",Color.white); }
            set { _wizardBoxStyle = value; }
        }      

        public static GUIStyle TooltipBoxStyle
        {
            get
            {
                return _tooltipBoxStyle ?? (_tooltipBoxStyle = new GUIStyle()
            {
                border = new RectOffset(4,4,4,4)
            }).WithAllStates("Box34",Color.white); }
            set { _tooltipBoxStyle = value; }
        }      
        
        public static GUIStyle WizardSubBoxStyle
        {
            get
            {
                return _wizardSubBoxStyle ?? (_wizardSubBoxStyle = new GUIStyle()
            {
                border = new RectOffset(4,4,4,4)
            }).WithAllStates("Box32",Color.white); }
            set { _wizardSubBoxStyle = value; }
        }       
        
        public static GUIStyle WizardActionButtonStyle
        {
            get
            {
                return _wizardActionButtonStyle ?? (_wizardActionButtonStyle = new GUIStyle()
            {
                border = new RectOffset(14,14,14,14),
                alignment = TextAnchor.MiddleCenter
            }).WithAllStates("Box33",Color.white)
            .WithHoveredState("Box33_Hovered",Color.white);}
            set { _wizardActionButtonStyle = value; }
        }    
        
        public static GUIStyle WizardActionButtonStyleSmall
        {
            get
            {
                return _wizardActionButtonStyleSmall ?? (_wizardActionButtonStyleSmall = new GUIStyle()
            {
                border = new RectOffset(14,14,14,14),
                alignment = TextAnchor.MiddleCenter
            }).WithAllStates("Box33_Small",Color.white)
            .WithHoveredState("Box33_Small_Hovered",Color.white);}
            set { _wizardActionButtonStyleSmall = value; }
        }

        public static GUIStyle WizardSubBoxTitleStyle
        {
            get
            {
                if (_wizardSubBoxTitleStyle == null)
                    _wizardSubBoxTitleStyle = new GUIStyle
                    {
                        normal = { textColor = new Color(0.95f, 0.95f, 9.95f) },
                        fontStyle = FontStyle.Bold
                        //fontStyle = FontStyle.Bold
                    }.WithFont("Verdana",16);
                return _wizardSubBoxTitleStyle;
            }
        }

        public static GUIStyle WizardListItemBoxStyle
        {
            get { return _wizardListItemBoxStyle ?? (_wizardListItemBoxStyle = new GUIStyle()
            {
                fontSize = 9,
                padding = new RectOffset(5,0,0,0),
                alignment = TextAnchor.MiddleLeft,
                border = new RectOffset(2,2,2,2)
            }.WithAllStates("Box35",Color.white)); }
            set { _wizardListItemBoxStyle = value; }
        }

        public static GUIStyle ViewBarTitleStyle
        {
            get
            {
                if (_viewBarTitleStyle == null)
                    _viewBarTitleStyle = new GUIStyle
                    {
                        normal = { textColor = new Color(0.95f, 0.95f, 9.95f) },
                        alignment = TextAnchor.MiddleRight,
                        fontSize = 16,
                        fontStyle = FontStyle.Bold
                        //fontStyle = FontStyle.Bold
                    };
                return _viewBarTitleStyle;
            }
        }

      

        public static GUIStyle ViewModelHeaderEditingStyle
        {
            get
            {
                if (_viewModelHeaderEditingStyle == null)
                {
                    _viewModelHeaderEditingStyle = new GUIStyle(EditorStyles.textField);
                    _viewModelHeaderEditingStyle.normal.background = null;
                    _viewModelHeaderEditingStyle.active.background = null;
                    _viewModelHeaderEditingStyle.hover.background = null;
                    _viewModelHeaderEditingStyle.focused.background = null;
                    _viewModelHeaderEditingStyle.alignment = TextAnchor.MiddleCenter;
                    _viewModelHeaderEditingStyle.fontStyle = FontStyle.Bold;
                    //_viewModelHeaderEditingStyle.fontSize = Mathf.RoundToInt(12*Scale);
                }
                return _viewModelHeaderEditingStyle;
            }
        }

        public static GUIStyle ViewModelHeaderStyle
        {
            get
            {
                if (_viewModelHeaderStyle == null)
                {
                    _viewModelHeaderStyle = new GUIStyle(GUIStyle.none)
                    {
                        normal = { textColor = Color.white },
                        //padding = new RectOffset(0, 12, 10, 4),
                        fixedHeight = 25 * Scale,
                        fontSize = Mathf.RoundToInt(11 * Scale),

                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _viewModelHeaderStyle;
            }
        }

        public static GUIStyle GraphTitleLabel
        {
            get
            {
                return _graphTitleLabel ?? (_graphTitleLabel = new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        //textColor = new Color(InvertGraphEditor.Settings.BackgroundColor.r * 0.3f, InvertGraphEditor.Settings.BackgroundColor.g * 0.3f, InvertGraphEditor.Settings.BackgroundColor.b * 0.3f, 1f)
                        textColor = InvertGraphEditor.Settings.TabTextColor
                    },
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true
                });
            }
            set { _graphTitleLabel = null; }

        }

        public static IConnectorStyleSchema ConnectorStyleSchemaTriangle
        {
            get { return _connectorStyleSchemaTriangle ?? (_connectorStyleSchemaTriangle = new UnityConnectorStyleSchema()
                .WithDefaultIcons()
               ); }
        }   
        
        public static IConnectorStyleSchema ConnectorStyleSchemaCircle
        {
            get
            {
                return _connectorStyleSchemaCircle ?? (_connectorStyleSchemaCircle = new UnityConnectorStyleSchema()
                .WithInputIcons("ConnectorCircleEmpty", "ConnectorCircleFilled")
                .WithOutputIcons("ConnectorCircleEmpty", "ConnectorCircleFilled")
                .WithTwoWayIcons("ConnectorCircleEmpty", "ConnectorCircleFilled")
                .WithPad(2,0,0,0)
                ); }
        }


        public static IBreadcrumbsStyleSchema DefaultBreadcrumbsStyleSchema
        {
            get { return _defaultBreadcrumbsStyleSchema ?? (_defaultBreadcrumbsStyleSchema = new UnityBreadcrumbsStyleSchema()); }
            set { _defaultBreadcrumbsStyleSchema = value; }
        }

        public static INodeStyleSchema BaseNodeStyleSchema
        {
            get
            {
                return _baseNodeStyleSchema ?? (_baseNodeStyleSchema = new UnityNodeStyleSchema()
                    .WithTitle(true)
                    .WithTitleFont(null, 12, new Color32(0xff, 0xff, 0xff, 0xff), FontStyle.Bold)
                    .WithSubTitleFont(null, 10, new Color32(0xf8, 0xf8, 0xf8, 0xff), FontStyle.Normal)
                    .WithHeaderPadding(new RectOffset(10,5,6,5))
                    .RecomputeStyles());
            }
        }

        public static INodeStyleSchema NodeStyleSchemaNormal
        {
            get
            {
                return _baseNormalStyleSchema ??
                       (_baseNormalStyleSchema = BaseNodeStyleSchema.Clone()
                       .WithTitle(true)
                       .WithIcon(true)
                       .WithSubTitle(true)
                       .WithTitleFont(null, 12, null, null)
                       .WithSubTitleFont(null, 10, null, null)
                       .RecomputeStyles());
            }
        }

        public static INodeStyleSchema NodeStyleSchemaMinimalistic
        {
            get
            {
                return _nodeStyleSchemaMinimalistic ??
                       (_nodeStyleSchemaMinimalistic = BaseNodeStyleSchema.Clone()
                           .WithTitle(true)
                           .WithIcon(true)
                           .WithSubTitle(false)
                           .WithHeaderPadding(new RectOffset(10, 5, 9, 9))
                           .WithTitleFont(null, 11, null, FontStyle.Bold)
                           .RecomputeStyles());
            }
        }

        public static INodeStyleSchema NodeStyleSchemaBold
        {
            get
            {
                if (_nodeStyleSchemaBold == null)
                {
                    _nodeStyleSchemaBold = BaseNodeStyleSchema.Clone()
                    .WithTitle(true)
                    .WithIcon(true)
                    .WithSubTitle(true)
                    .WithHeaderPadding(new RectOffset(10, 5, 8, 8))
                    .WithTitleFont(null, 16, null, FontStyle.Bold)
                    .WithSubTitleFont(null, 15, null, FontStyle.Normal)
                    .RecomputeStyles();
                }

                return _nodeStyleSchemaBold;
            }
        }

        public static GUIStyle WizardActionTitleStyle
        {
            get { return _wizardActionTitleStyle ?? (_wizardActionTitleStyle = new GUIStyle()
            {
                wordWrap = true,
                alignment = TextAnchor.LowerCenter
            }).WithFont("Verdana",12)
            .WithAllStates(new Color(0.8f,0.8f,0.8f)); }
            set { _wizardActionTitleStyle = value; }
        }

        public static GUIStyle SearchBarTextStyle
        {
            get { return _searchBarText ?? (_searchBarText = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            }).WithFont("Verdana",13); }
            set { _searchBarText = value; }
        }

        public static GUIStyle ListItemTitleStyle
        {
            get
            {
                return _listItemTitleStyle ?? (_listItemTitleStyle = new GUIStyle()
                {
                    richText = true,
                    wordWrap = true,
                    alignment = TextAnchor.LowerCenter
                }).WithFont("Verdana", 11)
                    .WithAllStates(new Color(0.8f, 0.8f, 0.8f));
            }
            set { _listItemTitleStyle = value; }
        }
        public static GUIStyle CreateHeader(string texture, Color color)
        {
            return new GUIStyle
             {
                 normal = { background = GetSkinTexture(texture), textColor = color },
                 padding = new RectOffset(-9,1,19,9),
                 stretchHeight = true,
                 stretchWidth = true,
                 // fixedHeight = 36,
                 // border = new RectOffset(44, 50, 20, 34),
                 //padding = new RectOffset(7, 7, 7, 7)
             };
        }

        public static void DoTilebar(string label)
        {
            var rect = GUIHelpers.GetRect(TitleBarStyle);
            DrawExpandableBox(rect, TitleBarStyle, label);
            //GUI.Box(rect, label, TitleBarStyle);
        }

        public static void DrawExpandableBox(Rect rect, GUIStyle style, string text, float offset = 12)
        {
            var oldBorder = style.border;
            style.border = new RectOffset(
                Mathf.RoundToInt(offset),
                Mathf.RoundToInt(offset),
                Mathf.RoundToInt(offset),
                Mathf.RoundToInt(offset));
            GUI.Box(rect, text, style);
            style.border = oldBorder;
        }

        public static void DrawExpandableBox(Rect rect, GUIStyle style, string text, RectOffset offset)
        {
         //   style.border = offset;

            GUI.Box(rect, text, style);
        }

        public static void FindClosestPoints(Vector3[] a, Vector3[] b,
            out Vector3 pointA, out Vector3 pointB, out int indexA, out int indexB)
        {
            var distance = Single.MaxValue;
            pointA = a[0];
            pointB = b[0];
            var i1 = 0;
            var i2 = 0;
            for (var i = 0; i < a.Length; i++)
            {
                var pA = a[i];
                for (var z = 0; z < b.Length; z++)
                {
                    var pB = b[z];

                    var newDistance = (pA - pB).sqrMagnitude;
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        pointA = pA;
                        pointB = pB;
                        i1 = i;
                        i2 = z;
                    }
                }
            }
            indexA = i1;
            indexB = i2;
        }

        public static GUIStyle GetHighlighter(string highlighter)
        {
            if (String.IsNullOrEmpty(highlighter)) return BoxHighlighter1;
            switch (highlighter.ToUpper())
            {
                case "YIELD":
                    return Item3;

                case "COMPUTED":
                    return Item2;

                default:
                    return BoxHighlighter1;
            }
        }

        public static Texture2D GetSkinTexture(string name)
        {
            return Resources.Load<Texture2D>(String.Format("{0}/{1}", SkinName, name));
        }

        //public static void DrawNodeCurve(Rect start, Rect end, Color color, float width, NodeCurvePointStyle startStyle = NodeCurvePointStyle.Circle, NodeCurvePointStyle endStyle = NodeCurvePointStyle.Triangle)
        //{
        //    //var swap = start.center.x < end.center.x;
        //    var startPosLeft = new Vector3(start.x, (start.y + start.height / 2f) - 4, 0f);
        //    var startPosRight = new Vector3(start.x + start.width, (start.y + start.height / 2f) - 4, 0f);

        //    var endPosLeft = new Vector3(end.x, (end.y + end.height / 2f) + 4, 0f);
        //    var endPosRight = new Vector3(end.x + end.width, (end.y + end.height / 2f) + 4, 0f);

        //    Vector3 startPos;
        //    Vector3 endPos;
        //    int index1;
        //    int index2;
        //    FindClosestPoints(new[] { startPosLeft, startPosRight },
        //        new[] { endPosLeft, endPosRight }, out startPos, out endPos, out index1, out index2);

        //    //endPos = TestClosest(startPos, end);

        //    bool startRight = index1 == 0 && index2 == 0 ? index1 == 1 : index2 == 1;
        //    bool endRight = index1 == 1 && index2 == 1 ? index2 == 1 : index1 == 1;

        //    var startTan = startPos + (endRight ? Vector3.right : Vector3.left) * 50;

        //    var endTan = endPos + (startRight ? Vector3.right : Vector3.left) * 50;

        //    var shadowCol = new Color(0, 0, 0, 0.1f);

        //    for (int i = 0; i < 3; i++) // Draw a shadow

        //        Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

        //    Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, width * Scale);

        //    startPos.y -= 8 * Scale;
        //    startPos.x -= 8 * Scale;
        //    if (startStyle == NodeCurvePointStyle.Circle)
        //    {
        //        GUI.DrawTexture(new Rect(startPos.x, startPos.y, 16f * Scale, 16f * Scale), CircleTexture);
        //    }
        //    else
        //    {
        //        GUI.DrawTexture(new Rect(startPos.x, startPos.y, 16f * Scale, 16f * Scale), startRight ? ArrowRightTexture : ArrowLeftTexture);
        //    }

        //    endPos.y -= 8 * Scale;
        //    endPos.x -= 8 * Scale;
        //    if (endStyle == NodeCurvePointStyle.Triangle)
        //    {
        //        GUI.DrawTexture(new Rect(endPos.x, endPos.y, 16f * Scale, 16f * Scale), startRight ? ArrowLeftTexture : ArrowRightTexture, ScaleMode.ScaleToFit);
        //    }
        //    else
        //    {
        //        GUI.DrawTexture(new Rect(endPos.x, endPos.y, 16f * Scale, 16f * Scale), CircleTexture, ScaleMode.ScaleToFit);
        //    }

        //}
        public static Vector3 TestClosest(Vector2 a, Rect b)
        {
            var result = new Vector2(a.x, a.y);
            result.y = Mathf.Clamp(a.y, b.y, b.y + b.height);
            result.x = Mathf.Clamp(a.x, b.x, b.x + b.width);

            return result;
        }//testclosest
    }
}



