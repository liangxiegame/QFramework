using System;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class BindCollector
    {
	    /// <summary>
		/// 
		/// </summary>
		/// <param name="rootTrans"></param>
		/// <param name="curTrans"></param>
		/// <param name="transFullName"></param>
		public static void SearchBinds(Transform curTrans, string transFullName,PanelCodeInfo panelCodeInfo = null, ElementCodeInfo parentElementCodeInfo = null,Type leafPanelType = null)
		{
			foreach (Transform childTrans in curTrans)
			{
				var uiMark = childTrans.GetComponent<IBindOld>();

				if (null != uiMark)
				{
					if (null == parentElementCodeInfo)
					{
						if (!panelCodeInfo.BindInfos.Any(markedObjInfo => markedObjInfo.TypeName.Equals(uiMark.Transform.name)))
						{
							panelCodeInfo.BindInfos.Add(new BindInfo
							{
								TypeName = uiMark.Transform.name,
								BindScript = uiMark,
								PathToRoot = CodeGenHelper.PathToParent(childTrans, panelCodeInfo.GameObjectName)
							});
							panelCodeInfo.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}
					else
					{
						if (!parentElementCodeInfo.BindInfos.Any(markedObjInfo => markedObjInfo.TypeName.Equals(uiMark.Transform.name)))
						{
							parentElementCodeInfo.BindInfos.Add(new BindInfo()
							{
								TypeName = uiMark.Transform.name,
								BindScript = uiMark,
								PathToRoot = CodeGenHelper.PathToParent(childTrans, parentElementCodeInfo.BehaviourName)
							});
							parentElementCodeInfo.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}

					if (uiMark.GetBindType() != BindType.DefaultUnityElement)
					{
						var elementCodeData = new ElementCodeInfo
						{
							BehaviourName = uiMark.TypeName,
							BindInfo = new BindInfo
							{
								BindScript = uiMark
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
						if (leafPanelType != null && uiMark.Transform.GetComponent(leafPanelType))
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
}