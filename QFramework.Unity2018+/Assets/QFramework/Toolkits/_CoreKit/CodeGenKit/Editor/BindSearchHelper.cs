/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public static class BindSearchHelper
    {
	    /// <summary>
	    /// ViewController 专用搜索器，不支持递归
	    /// </summary>
	    /// <param name="task"></param>
        public static void Search(CodeGenTask task)
        {
            var bindGroupTransforms = task.GameObject.GetComponentsInChildren<IBindGroup>(true)
                .Select(g => g.As<Component>().transform)
                .Where(t => t != task.GameObject.transform);

            var binds = task.GameObject.GetComponentsInChildren<IBind>(true)
                .Where(b => b.Transform != task.GameObject.transform);


            foreach (var bind in binds)
            {
                if (bindGroupTransforms.Any(g => bind.Transform.IsChildOf(g) && bind.Transform != g))
                {
                }
                else
                {
                    task.BindInfos.Add(new BindInfo()
                    {
                        TypeName = bind.TypeName,
                        MemberName = bind.Transform.gameObject.name,
                        BindScript = bind,
                        PathToRoot = PathToParent(bind.Transform, task.GameObject.name),
                    });
                }
            }
        }

        public static string PathToParent(Transform trans, string parentName)
        {
            var retValue = new StringBuilder(trans.name);

            while (trans.parent)
            {
                if (trans.parent.name.Equals(parentName))
                {
                    break;
                }

                retValue.AddPrefix("/").AddPrefix(trans.parent.name);

                trans = trans.parent;
            }

            return retValue.ToString();
        }

        public static List<Object> GetSelectableBindTypeOnGameObject(GameObject gameObject)
        {
            var objects = new List<Object>();
            objects.AddRange(gameObject.GetComponents<Component>().Where(component => !(component is Bind)));
            objects.Add(gameObject);
            return objects;
        }

        public static string[] GetSelectableBindTypeFullNameOnGameObject(GameObject gameObject)
        {
            return GetSelectableBindTypeOnGameObject(gameObject)
                .Select(o => o.GetType().FullName).ToArray();
        }
        
        
	    /// <summary>
		/// UIKit 专用的搜索器，支持递归
		/// </summary>
		/// <param name="rootTrans"></param>
		/// <param name="curTrans"></param>
		/// <param name="transFullName"></param>
		public static void SearchBinds(Transform curTrans, string transFullName,PanelCodeInfo panelCodeInfo = null, ElementCodeInfo parentElementCodeInfo = null,Type leafPanelType = null)
		{
			// 遍历所有子节点
			foreach (Transform childTrans in curTrans)
			{

				var bind = childTrans.GetComponent<IBindOld>();

				if (null != bind)
				{
					if (null == parentElementCodeInfo)
					{
						if (!panelCodeInfo.BindInfos.Any(markedObjInfo => markedObjInfo.TypeName.Equals(bind.Transform.name)))
						{
							panelCodeInfo.BindInfos.Add(new BindInfo
							{
								TypeName = bind.Transform.name,
								BindScript = bind,
								PathToRoot = CodeGenHelper.PathToParent(childTrans, panelCodeInfo.GameObjectName)
							});
							panelCodeInfo.DicNameToFullName.Add(bind.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}
					else
					{
						if (!parentElementCodeInfo.BindInfos.Any(markedObjInfo => markedObjInfo.TypeName.Equals(bind.Transform.name)))
						{
							parentElementCodeInfo.BindInfos.Add(new BindInfo()
							{
								TypeName = bind.Transform.name,
								BindScript = bind,
								PathToRoot = CodeGenHelper.PathToParent(childTrans, parentElementCodeInfo.BehaviourName)
							});
							parentElementCodeInfo.DicNameToFullName.Add(bind.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}

					// 如果有 ViewController 则不再递归子节点
					var viewController = childTrans.GetComponent<ViewController>();
					if (viewController)
					{
						continue;
					}

					if (bind.GetBindType() != BindType.DefaultUnityElement)
					{
						var elementCodeData = new ElementCodeInfo
						{
							BehaviourName = bind.TypeName,
							BindInfo = new BindInfo
							{
								BindScript = bind
							}
						};

						if (null == parentElementCodeInfo)
						{
							panelCodeInfo.ElementCodeDatas.Add(elementCodeData);
						}
						else
						{
							parentElementCodeInfo.ElementCodeDatas.Add(elementCodeData);
						}

						
						SearchBinds(childTrans, transFullName + childTrans.name + "/",panelCodeInfo, elementCodeData);
					}
					else
					{
						// 如果是标记的叶子节点则不再继续搜索
						if (leafPanelType != null && bind.Transform.GetComponent(leafPanelType))
						{
							
						} else {
							SearchBinds(childTrans, transFullName + childTrans.name + "/", panelCodeInfo,
								parentElementCodeInfo);
						}
					}
				}
				else
				{
					SearchBinds(childTrans, transFullName + childTrans.name + "/",panelCodeInfo, parentElementCodeInfo);
				}
			}
		}
    }
    
    public class PanelCodeInfo
    {
	    public          string                     GameObjectName;
	    public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
	    public readonly List<BindInfo>             BindInfos         = new List<BindInfo>();
	    public readonly List<ElementCodeInfo>      ElementCodeDatas  = new List<ElementCodeInfo>();
        
    }
    
    public class ElementCodeInfo
    {
	    public          BindInfo                   BindInfo;
	    public          string                     BehaviourName;
	    public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
	    public readonly List<BindInfo>             BindInfos         = new List<BindInfo>();
	    public readonly List<ElementCodeInfo>      ElementCodeDatas  = new List<ElementCodeInfo>();
    }
}
#endif