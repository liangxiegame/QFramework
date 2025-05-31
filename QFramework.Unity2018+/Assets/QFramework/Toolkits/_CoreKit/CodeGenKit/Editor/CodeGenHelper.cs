/****************************************************************************
 * Copyright (c) 2015 ~ 2025 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Text;
using UnityEngine;

namespace QFramework
{
    public static class CodeGenHelper
    {

        public static bool IsBelongs2ViewController(AbstractBind bind)
        {
            var gameObj = GetBindBelongs2GameObject(bind);

            if (gameObj && gameObj.GetComponent<ViewController>())
            {
                return true;
            }

            return false;
        }
        
        public static GameObject GetBindBelongs2GameObject(AbstractBind bind)
        {
            var trans = bind.Transform;
            
            while (trans.parent != null)
            {
                if (trans.parent.GetComponent<IBindGroup>() != null)
                {
                    return trans.parent.gameObject;
                }

                trans = trans.parent;
            }

            return null;
        }
        
        public static bool IsViewController(this Component component)
        {
            if (component.GetComponent<ViewController>())
            {
                return true;
            }

            return false;
        }
        
        public static string GetBindBelongs2(AbstractBind bind)
        {
            var trans = bind.Transform;
            
            while (trans.parent != null)
            {
                if (trans.parent.IsViewController())
                {
                    return trans.parent.name + "(" +  trans.parent.GetComponent<ViewController>().ScriptName  + ")";
                }
                
                if (trans.parent.IsUIPanel())
                {
                    return "UIPanel" + "(" +trans.parent.name + ")";
                }


                trans = trans.parent;
            }

            return trans.name;
        }
        
        public static bool IsUIPanel(this Component component)
        {
            if (component.GetComponent("UIPanel"))
            {
                return true;
            }

            return false;
        }
        
        public static string PathToParent(Transform trans, string parentName)
        {
            var retValue = new StringBuilder(trans.name);

            while (trans.parent != null)
            {
                if (trans.parent.name.Equals(parentName))
                {
                    break;
                }

                
                retValue = trans.parent.name.Builder().Append("/").Append(retValue);

                trans = trans.parent;
            }

            return retValue.ToString();
        }
        
        


    }
}
#endif