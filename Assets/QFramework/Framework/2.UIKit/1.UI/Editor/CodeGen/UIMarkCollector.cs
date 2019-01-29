using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class UIMarkCollector
    {
	    public static PanelCodeData mPanelCodeData;

        	/// <summary>
		/// 
		/// </summary>
		/// <param name="rootTrans"></param>
		/// <param name="curTrans"></param>
		/// <param name="transFullName"></param>
		public static void FindAllMarkTrans(Transform curTrans, string transFullName, ElementCodeData parentElementCodeData = null)
		{
			foreach (Transform childTrans in curTrans)
			{
				var uiMark = childTrans.GetComponent<IMark>();

				if (null != uiMark)
				{
					if (null == parentElementCodeData)
					{
						if (!mPanelCodeData.MarkedObjInfos.Any(markedObjInfo => markedObjInfo.Name.Equals(uiMark.Transform.name)))
						{
							mPanelCodeData.MarkedObjInfos.Add(new MarkedObjInfo
							{
								Name = uiMark.Transform.name,
								MarkObj = uiMark,
								PathToElement = CodeGenUtil.PathToParent(childTrans, mPanelCodeData.PanelName)
							});
							mPanelCodeData.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}
					else
					{
						if (!parentElementCodeData.MarkedObjInfos.Any(markedObjInfo => markedObjInfo.Name.Equals(uiMark.Transform.name)))
						{
							parentElementCodeData.MarkedObjInfos.Add(new MarkedObjInfo()
							{
								Name = uiMark.Transform.name,
								MarkObj = uiMark,
								PathToElement = CodeGenUtil.PathToParent(childTrans, parentElementCodeData.BehaviourName)
							});
							parentElementCodeData.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}


					if (uiMark.GetUIMarkType() != UIMarkType.DefaultUnityElement)
					{
						var elementCodeData = new ElementCodeData
						{
							BehaviourName = uiMark.ComponentName,
							MarkedObjInfo = new MarkedObjInfo
							{
								MarkObj = uiMark
							}
						};

						if (null == parentElementCodeData)
						{
							mPanelCodeData.ElementCodeDatas.Add(elementCodeData);
						}
						else
						{
							parentElementCodeData.ElementCodeDatas.Add(elementCodeData);
						}

						FindAllMarkTrans(childTrans, transFullName + childTrans.name + "/", elementCodeData);
					}
					else
					{
						FindAllMarkTrans(childTrans, transFullName + childTrans.name + "/", parentElementCodeData);
					}
				}
				else
				{
					FindAllMarkTrans(childTrans, transFullName + childTrans.name + "/", parentElementCodeData);
				}
			}
		}
    }
}