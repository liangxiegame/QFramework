/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2017 ~ 2021.3  liangxie
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

using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("UIRoot/Manager")]
    public partial class UIManager : QMgrBehaviour, ISingleton
    {
        void ISingleton.OnSingletonInit()
        {
        }

        private static UIManager mInstance;

        public static UIManager Instance
        {
            get
            {
                if (!mInstance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    mInstance = MonoSingletonProperty<UIManager>.Instance;
                }

                return mInstance;
            }
        }


        public void OpenUIAsync(PanelSearchKeys panelSearchKeys,Action<IPanel> onLoad)
        {
            if (panelSearchKeys.OpenType == PanelOpenType.Single)
            {
                var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

                if (retPanel == null)
                {
                    CreateUIAsync(panelSearchKeys, (panel) =>
                    {
                        retPanel = panel;
                        retPanel.Open(panelSearchKeys.UIData);
                        retPanel.Show();
                        onLoad?.Invoke(retPanel);
                    });
                }
                else
                {
                    if (retPanel.Info != null && retPanel.Info.Level != panelSearchKeys.Level)
                    {
                        UIKit.Root.SetLevelOfPanel(panelSearchKeys.Level,retPanel);
                    }
                    retPanel.Open(panelSearchKeys.UIData);
                    retPanel.Show();
                    onLoad?.Invoke(retPanel);
                }
            }
            else
            {
                CreateUIAsync(panelSearchKeys, (panel) =>
                {
                    panel.Open(panelSearchKeys.UIData);
                    panel.Show();
                    onLoad?.Invoke(panel);
                });
            }
        }

        public IPanel OpenUI(PanelSearchKeys panelSearchKeys)
        {
            if (panelSearchKeys.OpenType == PanelOpenType.Single)
            {
                var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

                if (retPanel == null)
                {
                    retPanel = CreateUI(panelSearchKeys);
                }

                if (retPanel.Info != null && retPanel.Info.Level != panelSearchKeys.Level)
                {
                    UIKit.Root.SetLevelOfPanel(panelSearchKeys.Level,retPanel);
                }
                
                retPanel.Open(panelSearchKeys.UIData);
                retPanel.Show();
                return retPanel;
            }
            else
            {
                var retPanel = CreateUI(panelSearchKeys);
                retPanel.Open(panelSearchKeys.UIData);
                retPanel.Show();
                return retPanel;
            }
        }

        /// <summary>
        /// 显示UIBehaiviour
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        public void ShowUI(PanelSearchKeys panelSearchKeys)
        {
            var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (retPanel != null)
            {
                retPanel.Show();
            }
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        public void HideUI(PanelSearchKeys panelSearchKeys)
        {
            var retPanel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (retPanel != null)
            {
                retPanel.Hide();
            }
        }

        /// <summary>
        /// 删除所有UI层
        /// </summary>
        public void CloseAllUI()
        {
            foreach (var layer in UIKit.Table)
            {
                layer.Close();
                layer.Info.Recycle2Cache();
                layer.Info = null;
            }

            UIKit.Table.Clear();
        }

        /// <summary>
        /// 隐藏所有 UI
        /// </summary>
        public void HideAllUI()
        {
            foreach (var panel in UIKit.Table)
            {
                panel.Hide();
            }
        }

        /// <summary>
        /// 关闭并卸载UI
        /// </summary>
        /// <param name="behaviourName"></param>
        public void CloseUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).LastOrDefault();

            if (panel as UIPanel)
            {
                panel.Close();
                UIKit.Table.Remove(panel);
                panel.Info.Recycle2Cache();
                panel.Info = null;
            }
        }

        public void RemoveUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault();

            if (panel != null)
            {
                UIKit.Table.Remove(panel);
            }
        }

        /// <summary>
        /// 获取UIBehaviour
        /// </summary>
        /// <param name="uiBehaviourName"></param>
        /// <returns></returns>
        public UIPanel GetUI(PanelSearchKeys panelSearchKeys)
        {
            return UIKit.Table.GetPanelsByPanelSearchKeys(panelSearchKeys).FirstOrDefault() as UIPanel;
        }

        public override int ManagerId
        {
            get { return QMgrID.UI; }
        }

        public IPanel CreateUI(PanelSearchKeys panelSearchKeys)
        {
            var panel = UIKit.Config.LoadPanel(panelSearchKeys);

            UIKit.Root.SetLevelOfPanel(panelSearchKeys.Level, panel);

            UIKit.Config.SetDefaultSizeOfPanel(panel);

            panel.Transform.gameObject.name = panelSearchKeys.GameObjName ?? panelSearchKeys.PanelType.Name;

            panel.Info = PanelInfo.Allocate(panelSearchKeys.GameObjName, panelSearchKeys.Level, panelSearchKeys.UIData,
                panelSearchKeys.PanelType, panelSearchKeys.AssetBundleName);

            UIKit.Table.Add(panel);

            panel.Init(panelSearchKeys.UIData);

            return panel;
        }

        public void CreateUIAsync(PanelSearchKeys panelSearchKeys, Action<IPanel> onPanelCreate)
        {
            UIKit.Config.LoadPanelAsync(panelSearchKeys, panel =>
            {
                UIKit.Root.SetLevelOfPanel(panelSearchKeys.Level, panel);

                UIKit.Config.SetDefaultSizeOfPanel(panel);

                panel.Transform.gameObject.name = panelSearchKeys.GameObjName ?? panelSearchKeys.PanelType.Name;

                panel.Info = PanelInfo.Allocate(panelSearchKeys.GameObjName, panelSearchKeys.Level,
                    panelSearchKeys.UIData,
                    panelSearchKeys.PanelType, panelSearchKeys.AssetBundleName);

                UIKit.Table.Add(panel);

                panel.Init(panelSearchKeys.UIData);

                onPanelCreate(panel);
            });
        }
    }
}