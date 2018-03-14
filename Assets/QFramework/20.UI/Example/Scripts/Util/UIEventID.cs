using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEventID
{
    public enum MenuPanel
    {
        EnumStar = (int) 1,

        GameStar,
        Setting,


        EnumEnd,
    }

    public enum SettingPanel
    {
        EnumStar = MenuPanel.EnumEnd,

        ShowSetting,

        EnumEn,
    }
}