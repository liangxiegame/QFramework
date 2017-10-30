/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEditor;

	/// <summary>
	/// data type format select：json/protobuf
	/// </summary>
	public static class SaveDataFormatEditor
	{
		private const string MENU_PROTOBUF = "PuTaoTool/Framework/SaveDataFormat/Protobuf";
		private const string MENU_JSON = "PuTaoTool/Framework/SaveDataFormat/Json";
		private const string KEY_USEPROTOBUF = "UseProtobuf";

		[MenuItem(MENU_JSON, false, 1)]
		public static void SaveDataJson()
		{
			EditorPrefs.SetBool("UseProtobuf", false);
		}

		[MenuItem(MENU_JSON, true)]
		public static bool SaveDataJsonValidate()
		{
			bool useProtobuf = EditorPrefs.GetBool(KEY_USEPROTOBUF, false);

			Menu.SetChecked(MENU_JSON, !useProtobuf);
			Menu.SetChecked(MENU_PROTOBUF, useProtobuf);

			return true;
		}

		[MenuItem(MENU_PROTOBUF, false, 2)]
		public static void SaveDataProtobuf()
		{
			EditorPrefs.SetBool("UseProtobuf", true);
		}

		[MenuItem(MENU_PROTOBUF, true)]
		public static bool SaveDataProtobufValidate()
		{
			bool useProtobuf = EditorPrefs.GetBool(KEY_USEPROTOBUF, false);

			Menu.SetChecked(MENU_JSON, !useProtobuf);
			Menu.SetChecked(MENU_PROTOBUF, useProtobuf);
			return true;
		}
	}
}