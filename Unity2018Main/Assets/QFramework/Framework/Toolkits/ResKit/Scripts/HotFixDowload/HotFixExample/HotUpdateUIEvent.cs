using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class HotUpdateUIEvent : MonoBehaviour
    {
        public Image HotFixedSlider;
        public Text HotSliderSpeedText;
        public Text HotSliderInfoText;
        public Text HotContenText;
        public Text TitleText;
        public Text HotFixDesText;
        public Button ConfirmBtn;
        public Button CancleBtn;

        public void Show(string title, string str, UnityEngine.Events.UnityAction confirmAction, UnityEngine.Events.UnityAction cancleAction)
        {
   
            TitleText.text = title;
            HotFixDesText.text = str;

            ConfirmBtn.onClick.AddListener(()=>confirmAction());

            CancleBtn.onClick.AddListener(() => cancleAction());
        }
    }
}

