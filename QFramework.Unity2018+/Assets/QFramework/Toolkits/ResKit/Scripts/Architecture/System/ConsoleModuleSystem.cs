using System.Linq;
using UnityEngine;

namespace QFramework
{
    internal class ConsoleModuleSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            var titleStyle =  FluentGUIStyle.Label()
                .FontSize(30);
            var subTitleStyle = FluentGUIStyle.Label()
                .FontSize(20);

            ConsoleKit.AddModule(new ConsoleModule()
                .Title("ResKit")
                .OnGUI(() =>
                {
                    GUILayout.BeginVertical("box");

                    GUILayout.Label("ResKit", titleStyle);
                    GUILayout.Space(10);
                    GUILayout.Label("ResInfo", subTitleStyle);
                    ResMgr.Instance.Table.ToList().ForEach(res => { GUILayout.Label((res as Res)?.ToString()); });
                    GUILayout.Space(10);

                    GUILayout.Label("Pools", subTitleStyle);
                    GUILayout.Label($"ResSearchRule:{SafeObjectPool<ResSearchKeys>.Instance.CurCount}");
                    GUILayout.Label($"ResLoader:{SafeObjectPool<ResLoader>.Instance.CurCount}");
                    GUILayout.EndVertical();
                })
            );
        }
    }
}