/****************************************************************************
 * Copyright (c) 2021.8 liangxiegame MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class ActionKitUniRxActionExample : MonoBehaviour
    {
        void Start()
        {
            var btn1 = transform.Find("Btn1").GetComponent<Button>();
            var btn2 = transform.Find("Btn2").GetComponent<Button>();
            var btn3 = transform.Find("Btn3").GetComponent<Button>();

            btn1.Hide();
            btn2.Hide();
            btn3.Hide();

            this.Sequence()
                .UniRx(() =>
                {
                    btn1.Show();
                    return btn1.OnClickAsObservable().First().Do(_ => btn1.Hide());
                })
                .UniRx(() =>
                {
                    btn2.Show();
                    return btn2.OnClickAsObservable().First().Do(_ => btn2.Hide());
                })
                .UniRx(() =>
                {
                    btn3.Show();
                    return btn3.OnClickAsObservable().First().Do(_ => btn3.Hide());
                })
                .Begin();
        }
    }
}