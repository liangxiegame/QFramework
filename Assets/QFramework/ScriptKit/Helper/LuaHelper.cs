/****************************************************************************
 * Copyright (c) 2019.2 vin129
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/
namespace QFramework
{
    using UnityEngine;
    using System.Reflection;
    using LuaInterface;
    public class ColliderData
    {
        public Collider Collider;
        public bool Clicked;
    };

    public static class LuaHelper
    {
        public static System.Type GetType(string classname)
        {
            Assembly assb = Assembly.GetExecutingAssembly();
            System.Type t = null;
            t = assb.GetType(classname); ;
            if (t == null)
            {
                t = assb.GetType(classname);
            }
            return t;
        }

        //public static string LoadAsString(string strFileName)
        //{
        //    return LuaFileUtils.Instance.ReadAsString(strFileName);
        //}

        //public static object DoFile(string strFileName)
        //{
        //    return AppFacade.Instance.LuaMgr.DoFile(strFileName)[0];
        //}

        //public static PolygonDrawer CreatePolygonDrawer()
        //{
        //    PolygonDrawer kDrawer = new PolygonDrawer();
        //    Screen2DDrawer.Instance.AddDrawer(kDrawer);
        //    return kDrawer;
        //}

        //public static LineDrawer CreateLineDrawer()
        //{
        //    LineDrawer kDrawer = new LineDrawer();
        //    Screen2DDrawer.Instance.AddDrawer(kDrawer);
        //    return kDrawer;
        //}

        //public static void RemoveLineDrawer(LineDrawer kLineDrawer)
        //{
        //    if(null != Screen2DDrawer.Instance)
        //        Screen2DDrawer.Instance.RemoveDrawer(kLineDrawer);
        //}

        public static string LoadLocalTable(string fileName)
        {
            TextAsset jsonfile = Resources.Load("Tables/Common/" + fileName) as TextAsset;
            if (jsonfile == null)
                return "";
            string json = jsonfile.text;
            return json;
        }
        public static void OpenURL(string url)
        {
            UnityEngine.Application.OpenURL(url);
        }
        #region GameObject Process Module
        public static GameObject LoadPrefab(string strPrefab, GameObject kParent)
        {
            //if(string.IsNullOrEmpty(strPrefab))
            //    return null;
            //GameObject kObj = AppFacade.Instance.ResMgr.Load(strPrefab) as GameObject;
            //if(null != kObj)
            //    kObj = GameObject.Instantiate(kObj);
            //kObj.transform.parent = kParent.transform;
            //kObj.transform.localPosition = Vector3.zero;
            //string strName = kObj.transform.name;
            //strName = strName.Replace("(Clone)","");
            //kObj.transform.name = strName;
            //return kObj;
            return null;
        }
        public static void SetParent(GameObject obj, GameObject parent)
        {
            if (null == obj || null == parent)
            {
                return;
            }
            RectTransform kTs = obj.GetComponent<RectTransform>();
            kTs.SetParent(parent.transform, false);
            kTs.localPosition = new Vector2(0, 0);
        }

        public static void SetDefaultLocalPos(GameObject kObj)
        {
            kObj.transform.localPosition = Vector2.zero;
        }

        public static void SetLocalPositionVector2(GameObject kObj, float x, float y)
        {
            if (kObj == null)
                return;
            kObj.transform.localPosition = new Vector2(x, y);
        }

        public static void SetLocalPositionVector3(GameObject kObj, float x, float y, float z)
        {
            if (kObj == null)
                return;
            kObj.transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetLoaclScale(GameObject kObj, float x, float y, float z)
        {
            if (kObj == null)
                return;
            kObj.transform.localScale = new Vector3(x, y, z);
        }

        public static void SetSizeDelta(GameObject kObj, float w, float h)
        {
            if (kObj == null)
                return;
            var retT = kObj.GetComponent<RectTransform>();
            if (!retT)
                return;
            retT.sizeDelta = new Vector2(w, h);
        }

        public static void DestroyGameObject(GameObject kObj)
        {
            if (null != kObj)
                GameObject.Destroy(kObj);
        }


        public static GameObject Find(string strName)
        {
            return GameObject.Find(strName);
        }

        public static GameObject FindChild(GameObject kObj, string strName)
        {
            if (null == kObj)
                return null;

            Transform kTransform = kObj.transform.Find(strName);
            if (null == kTransform)
                return null;
            return kTransform.gameObject;
        }

        public static GameObject FindParent(GameObject obj, string name)
        {
            if (null == obj)
            {
                return null;
            }

            Transform trans = obj.transform;
            while (trans != null)
            {
                if (trans.name.Equals(name))
                {
                    return trans.gameObject;
                }
                trans = trans.parent;
            }

            return null;
        }

        public static int GetChildCount(GameObject kObj)
        {
            if (null == kObj)
                return 0;
            return kObj.transform.childCount;
        }

        public static GameObject GetChild(GameObject kObj, int iChildId)
        {
            if (null == kObj || iChildId < 0 || iChildId >= kObj.transform.childCount)
                return null;
            Transform kTs = kObj.transform.GetChild(iChildId);
            if (null == kTs)
                return null;
            return kTs.gameObject;
        }
        public static string GetChildName(GameObject kObj, int iChildId)
        {
            if (null == kObj || iChildId < 0 || iChildId >= kObj.transform.childCount)
                return null;
            Transform kTs = kObj.transform.GetChild(iChildId);
            if (null == kTs)
                return null;
            return kTs.name;
        }

        public static void DestroyAllChildren(GameObject kObj)
        {
            if (kObj == null)
                return;
            foreach (Transform child in kObj.transform)
                GameObject.Destroy(child.gameObject);
        }

        #endregion GameObject Process Module
        #region Raycast
        public static ColliderData Raycast(string strLayerName, float fDistance)
        {
            Vector2 kPos;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 0)
                    return m_kColliderData;
                Touch touch = Input.GetTouch(0);
                kPos = touch.position;
                TouchPhase phase = touch.phase;
                m_kColliderData.Clicked = phase == TouchPhase.Ended;
            }
            else
            {
                var isClicked = Input.GetMouseButtonUp(0);
                if (!isClicked)
                    return m_kColliderData;
                kPos = Input.mousePosition;
                m_kColliderData.Clicked = true;
            }
            int iLayerMask = 0;
            iLayerMask |= (1 << LayerMask.NameToLayer(strLayerName));
            var ray = Camera.main.ScreenPointToRay(kPos);
            RaycastHit hit;
            var isCollide = Physics.Raycast(ray, out hit, Mathf.Infinity, iLayerMask);
            if (!isCollide)
            {
                m_kColliderData.Clicked = false;
                return m_kColliderData;
            }

            m_kColliderData.Collider = hit.collider;
            return m_kColliderData;
        }

        public static bool IsClicked()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
                return true;
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            return true;
#endif
            return false;
        }
        #endregion

        #region Log Module

        public static void Log(string str)
        {
            QFramework.Log.I(str);
        }
        public static void ColorLog(string str, string strColor)
        {
            QFramework.Log.I(str);
        }

        public static void LogWarning(string str)
        {
            QFramework.Log.W(str);
        }

        public static void LogError(string str)
        {
            QFramework.Log.W(str);
        }
        #endregion Log Module

        #region Network
        public static void DoPost(string strURL, string strBody, LuaFunction kCallbackFunc)
        {

        }

        public static void RestNetworkHandler()
        {

        }


        public static byte[] ConvertStringToBytes(string _text)
        {
            return System.Text.Encoding.UTF8.GetBytes(_text);
        }
        public static string ConvertBytesToString(byte[] _bytes)
        {
            return System.Text.Encoding.UTF8.GetString(_bytes);
        }

        public static Vector3 ScreenPointToLocalPos(Canvas _canvas, Vector3 _screenPoint)
        {
            _screenPoint.z = 0;

            Vector2 t_scr = new Vector2(_screenPoint.x, _screenPoint.y);
            Vector2 t_pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), t_scr, _canvas.worldCamera, out t_pos))
            {
                _screenPoint.x = t_pos.x;
                _screenPoint.y = t_pos.y;
            }

            return _screenPoint;
        }

        public static Canvas GetUIRootCanvas(GameObject go)
        {
            return go.transform.GetComponentInParent<Canvas>();
        }
        #endregion Network



        //创建克隆
        public static GameObject CreatClone(GameObject kObj, GameObject Parent)
        {
            if (kObj == null)
            {
                Debug.LogError("kObj is Null!");
                return null;
            }

            if (Parent == null)
            {
                Debug.LogError("Parent is Null!");
                return null;
            }

            GameObject clone = Object.Instantiate(kObj) as GameObject;
            clone.transform.SetParent(Parent.transform);
            clone.transform.localPosition = new Vector2(0, 0);
            clone.transform.localScale = new Vector3(1, 1, 1);

            return clone;
        }

        //创建克隆
        public static GameObject CreatCloneSaveScale(GameObject kObj, GameObject Parent)
        {
            if (kObj == null)
            {
                Debug.LogError("kObj is Null!");
                return null;
            }

            if (Parent == null)
            {
                Debug.LogError("Parent is Null!");
                return null;
            }

            GameObject clone = Object.Instantiate(kObj) as GameObject;
            clone.transform.SetParent(Parent.transform);
            clone.transform.localPosition = new Vector2(0, 0);
            clone.transform.localScale = new Vector3(kObj.transform.localScale.x, kObj.transform.localScale.y, 1);

            return clone;
        }


        private static ColliderData m_kColliderData = new ColliderData();
    }
}
