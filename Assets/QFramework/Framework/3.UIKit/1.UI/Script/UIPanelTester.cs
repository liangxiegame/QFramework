/****************************************************************************
 * Copyright (c) 2018.5 liqingyun
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [System.Serializable]
    public class UIPanelTesterInfo
    {
        /// <summary>
        /// 页面的名字
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 层级名字
        /// </summary>
        public UILevel Level;
    }

    public class UIPanelTester : MonoBehaviour
    {
        /// <summary>
        /// 页面的名字
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 层级名字
        /// </summary>
        public UILevel Level;

        [SerializeField] private List<UIPanelTesterInfo> mOtherPanels;

        private void Awake()
        {
            ResMgr.Init();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);
			
            UIMgr.OpenPanel(PanelName, Level);

            mOtherPanels.ForEach(panelTesterInfo => { UIMgr.OpenPanel(panelTesterInfo.PanelName, panelTesterInfo.Level); });
        }
    }
}