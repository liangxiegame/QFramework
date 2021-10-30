using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class HotUpdateCheck : MonoBehaviour
    {
        [Header("服务器配置文件路径")]
        [Space(30)]
        public string ServerConfigURL;

        public static HotUpdateCheck Instance;

        HotUpdateUIEvent hotUpdateUIEvent;
        float m_SumTime = 0;

        private void Start()
        {
            Instance = this;

            hotUpdateUIEvent = GetComponent<HotUpdateUIEvent>();

            hotUpdateUIEvent.HotFixedSlider.fillAmount = 0;
            hotUpdateUIEvent.HotSliderInfoText.text = string.Empty;


            HotPatchManager.Instance.Init(this);

         

            HotPatchManager.Instance.ServerInfoError += ServerInfoError;
            HotPatchManager.Instance.ItemError += ItemError;


            if (HotPatchManager.Instance.ComputeUnPackFile())
            {
                hotUpdateUIEvent.HotSliderSpeedText.text = "初始化中.....";
                HotPatchManager.Instance.StartUnackFile(() =>
                {
                    m_SumTime = 0;
                    HotFix();
                });
            }
            else
            {
                HotFix();
            }

        }

        private void Update()
        {
            if (HotPatchManager.Instance.StartUnPack)
            {
                m_SumTime += Time.deltaTime;
                hotUpdateUIEvent.HotFixedSlider.fillAmount = HotPatchManager.Instance.GetUnpackProgress();
                float speed = (HotPatchManager.Instance.AlreadyUnPackSize / 1024.0f) / m_SumTime;
                hotUpdateUIEvent.HotSliderSpeedText.text = string.Format("{0:F} M/S", speed);
            }

            if (HotPatchManager.Instance.StartDownload)
            {
                m_SumTime += Time.deltaTime;
                hotUpdateUIEvent.HotFixedSlider.fillAmount = HotPatchManager.Instance.GetProgress();
                float speed = (HotPatchManager.Instance.GetLoadSize() / 1024.0f) / m_SumTime;
                hotUpdateUIEvent.HotSliderSpeedText.text = string.Format("{0:F} M/S", speed);
            }
        }

        private void OnDestroy()
        {
            HotPatchManager.Instance.ServerInfoError -= ServerInfoError;
            HotPatchManager.Instance.ItemError -= ItemError;
        }


        void ServerInfoError()
        {
            hotUpdateUIEvent.Show("服务器列表获取失败", "服务器列表获取失败，请检查网络链接是否正常？尝试重新下载！", CheckVersion, Application.Quit);
        }

        void ItemError(string all)
        {
            hotUpdateUIEvent.Show("资源下载失败", string.Format("{0}等资源下载失败，请重新尝试下载！", all), AnewDownload, Application.Quit);
        }

        void HotFix()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //提示网络错误，检测网络链接是否正常
                hotUpdateUIEvent.Show("网络链接失败", "网络链接失败，请检查网络链接是否正常？", () =>
                { Application.Quit(); }, () => { Application.Quit(); });
            }
            else
            {
                CheckVersion();
            }
        }

        void CheckVersion()
        {
            HotPatchManager.Instance.CheckVersion((hot) =>
            {
                if (hot)
                {
                    //提示玩家是否确定热更下载
                    hotUpdateUIEvent.Show("热更", string.Format("当前版本为{0},有{1:F}M大小热更包，是否确定下载？", HotPatchManager.Instance.CurVersion, HotPatchManager.Instance.LoadSumSize / 1024.0f), OnClickStartDownLoad, OnClickCancleDownLoad);
                }
                else
                {
                    StartOnFinish();
                }
            });
        }


        void OnClickStartDownLoad()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    hotUpdateUIEvent.Show("下载确认", "当前使用的是手机流量，是否继续下载？", StartDownLoad, OnClickCancleDownLoad);

                }
                else
                {
                    StartDownLoad();
                }
            }
            else
            {
                StartDownLoad();
            }
        }

        void OnClickCancleDownLoad()
        {
            Application.Quit();
        }

        /// <summary>
        /// 正式开始下载
        /// </summary>
        void StartDownLoad()
        {

            hotUpdateUIEvent.TitleText.transform.parent.gameObject.SetActive(false);
            hotUpdateUIEvent.HotSliderInfoText.text = "下载中...";
            hotUpdateUIEvent.HotSliderInfoText.gameObject.Show();

            hotUpdateUIEvent.HotContenText.text = HotPatchManager.Instance.CurrentPatches.Des;
            StartCoroutine(HotPatchManager.Instance.StartDownLoadAB(StartOnFinish));
        }

        /// <summary>
        /// 下载完成回调，或者没有下载的东西直接进入游戏
        /// </summary>
        void StartOnFinish()
        {
            HotPatchManager.Instance.ServerInfoError -= ServerInfoError;
            HotPatchManager.Instance.ItemError -= ItemError;
            Debug.Log("下载完成");
          //  Destroy(gameObject);
        }

      
        void AnewDownload()
        {
            HotPatchManager.Instance.CheckVersion((hot) =>
            {
                if (hot)
                {
                    StartDownLoad();
                }
                else
                {
                    StartOnFinish();
                }
            });
        }
    }
}

