/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    /// <summary> Draw enums correctly within nodes. Without it, enums show up at the wrong positions. </summary>
    /// <remarks> Enums with this attribute are not detected by EditorGui.ChangeCheck due to waiting before executing </remarks>
    public class GUIGraphEnumAttribute : PropertyAttribute
    {
    }
}