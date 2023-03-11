using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ShootingEditor2D
{
    public class LevelEditor : MonoBehaviour
    {
        public enum OperateMode
        {
            Draw,
            Erase
        }

        public enum BrushType
        {
            Ground,
            Player
        }
        
        private OperateMode mCurrentOperateMode;

        private BrushType mCurrentBrusyType = BrushType.Ground;

        private Lazy<GUIStyle> mModeLabelStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            alignment = TextAnchor.MiddleCenter
        });

        private Lazy<GUIStyle> mButtonStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.button)
        {
            fontSize = 30
        });

        private Lazy<GUIStyle> mRightButtonStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.button)
        {
            fontSize = 25
        });

        private void OnGUI()
        {
            var modeLabelRect = RectHelper.RectForAnchorCenter(Screen.width * 0.5f, 35, 300, 50);

            if (mCurrentOperateMode == OperateMode.Draw)
            {
                GUI.Label(modeLabelRect,mCurrentOperateMode + ":" + mCurrentBrusyType,mModeLabelStyle.Value);
            }
            else
            {
                GUI.Label(modeLabelRect,mCurrentOperateMode.ToString(),mModeLabelStyle.Value);    
            }

            

            var drawButtonRect = new Rect(10, 10, 150, 50);
            if (GUI.Button(drawButtonRect, "绘制",mButtonStyle.Value))
            {
                mCurrentOperateMode = OperateMode.Draw;
            }
            var eraseButtonRect = new Rect(10, 60, 150, 50);

            if (GUI.Button(eraseButtonRect, "橡皮",mButtonStyle.Value))
            {
                mCurrentOperateMode = OperateMode.Erase;
            }

            if (mCurrentOperateMode == OperateMode.Draw)
            {
                var groundButtonRect = new Rect(Screen.width - 110, 10, 100, 50);
                if (GUI.Button(groundButtonRect, "地块", mRightButtonStyle.Value))
                {
                    mCurrentBrusyType = BrushType.Ground;
                }

                var playerButtonRect = new Rect(Screen.width - 110, 70, 100, 50);
                if (GUI.Button(playerButtonRect, "主角", mRightButtonStyle.Value))
                {
                    mCurrentBrusyType = BrushType.Player;
                }
            }

            var saveButtonRect = new Rect(Screen.width - 110, Screen.height - 60, 100, 50);
            if (GUI.Button(saveButtonRect, "保存", mRightButtonStyle.Value))
            {
                Debug.Log("保存");

                var infos = new List<LevelItemInfo>(transform.childCount);
                
                foreach (Transform child in transform)
                {
                    infos.Add(new LevelItemInfo()
                    {
                        Name = child.name,
                        X = child.position.x,
                        Y = child.position.y
                    });
                }
                
                var document = new XmlDocument();
                var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", "");
                document.AppendChild(declaration);

                var level = document.CreateElement("Level");
                document.AppendChild(level);
                    
                foreach (var levelItemInfo in infos)
                {
                    var levelItem = document.CreateElement("LevelItem");
                    levelItem.SetAttribute("name", levelItemInfo.Name);
                    levelItem.SetAttribute("x", levelItemInfo.X.ToString());
                    levelItem.SetAttribute("y", levelItemInfo.Y.ToString());
                    level.AppendChild(levelItem);
                }

                var levelFilesFolder = Application.persistentDataPath + "/LevelFiles";

                Debug.Log(levelFilesFolder);
                
                if (!Directory.Exists(levelFilesFolder))
                {
                    Directory.CreateDirectory(levelFilesFolder);
                }

                var levelFilePath = levelFilesFolder + "/" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml";

                document.Save(levelFilePath);
            }
        }

        class LevelItemInfo
        {
            public string Name;
            public float X;
            public float Y;
        }

        public SpriteRenderer EmptyHighlight;
        

        private bool mCanDraw;
        private GameObject mCurrentObjectMouseOn;
        
        private void Update()
        {
            var mousePosition = Input.mousePosition;

            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

            mouseWorldPos.x = Mathf.Floor(mouseWorldPos.x + 0.5f);
            mouseWorldPos.y = Mathf.Floor(mouseWorldPos.y + 0.5f);
            
            mouseWorldPos.z = 0;

            if (GUIUtility.hotControl == 0)
            {
                EmptyHighlight.gameObject.SetActive(true);
            }
            else
            {
                EmptyHighlight.gameObject.SetActive(false);
            }

            if (Math.Abs(EmptyHighlight.transform.position.x - mouseWorldPos.x) < 0.1f &&
                Math.Abs(EmptyHighlight.transform.position.y - mouseWorldPos.y) < 0.1f)
            {
                
            }
            else
            {
                var highlightPos = mouseWorldPos;
                highlightPos.z = -9;
                EmptyHighlight.transform.position = highlightPos;

                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                var hit = Physics2D.Raycast(ray.origin, Vector2.zero, 20);

                if (hit.collider)
                {
                    if (mCurrentOperateMode == OperateMode.Draw)
                    {
                        EmptyHighlight.color = new Color(1, 0, 0, 0.5f);
                    } else if (mCurrentOperateMode == OperateMode.Erase)
                    {
                        EmptyHighlight.color = new Color(1, 0.5f, 0, 0.5f);
                    }

                    mCanDraw = false;
                    mCurrentObjectMouseOn = hit.collider.gameObject;
                }
                else
                {
                    if (mCurrentOperateMode == OperateMode.Draw)
                    {
                        EmptyHighlight.color = new Color(1, 1, 1, 0.5f);
                    } else if (mCurrentOperateMode == OperateMode.Erase)
                    {
                        EmptyHighlight.color = new Color(0, 0, 1, 0.5f);
                    }

                    mCanDraw = true;
                    mCurrentObjectMouseOn = null;
                }
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && GUIUtility.hotControl == 0)
            {
                if (mCanDraw && mCurrentOperateMode == OperateMode.Draw)
                {
                    if (mCurrentBrusyType == BrushType.Ground)
                    {
                        var groundPrefab = Resources.Load<GameObject>("Ground");
                        var groundGameObj = Instantiate(groundPrefab, transform);
                        groundGameObj.transform.position = mouseWorldPos;
                        groundGameObj.name = "Ground";

                        mCanDraw = false;
                    } else if (mCurrentBrusyType == BrushType.Player)
                    {
                        var groundPrefab = Resources.Load<GameObject>("Ground");
                        var groundGameObj = Instantiate(groundPrefab, transform);
                        groundGameObj.transform.position = mouseWorldPos;
                        groundGameObj.name = "Player";
                        
                        groundGameObj.GetComponent<SpriteRenderer>().color = Color.cyan;

                        mCanDraw = false;
                    }
                } else if (mCurrentObjectMouseOn && mCurrentOperateMode == OperateMode.Erase)
                {
                    Destroy(mCurrentObjectMouseOn);

                    mCurrentObjectMouseOn = null;
                }
            }
        }
    }
}