using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace QFramework.ResSystem
{
	public class QConfigManager
	{
//		//	private XmlDocument xmlDoc;
//		private static QConfigManager mInstance;
//		public List<MarkItem> markItems;
//		private const string configFileUrl = "Assets/QGameData/config_assetbundle.xml";
//
//		public static QConfigManager Instance {
//			get {
//				if (mInstance == null) {
//					mInstance = new QConfigManager ();
//				}
//				return mInstance;
//			}
//		}
//
//		public QConfigManager ()
//		{
//			Init ();
//		}
//
//		private void Init ()
//		{
//			if (markItems == null) {
//				markItems = new List<MarkItem> ();
//			} else {
//				markItems.Clear ();
//			}
//			if (!File.Exists (configFileUrl)) {
//				//如果不存在，则创建一个默认的
//				SaveConfigFile ();
//			} 
//			InitItems ();
//		}
//
//		private void InitItems ()
//		{
//		
//			TextAsset configFile = AssetDatabase.LoadAssetAtPath<TextAsset> (configFileUrl);
//			XmlDocument xmlDoc = new XmlDocument ();
//			xmlDoc.LoadXml (configFile.text);
//			XmlNodeList nodes = xmlDoc.SelectSingleNode ("config").SelectNodes ("item");
//			foreach (XmlNode node in nodes) {
//				MarkItem markItem = new MarkItem ();
//				markItem.name = node.Attributes ["name"].Value.ToString ();
//				markItem.path = node.Attributes ["path"].Value.ToString ();
//				if (node.Attributes ["type"] != null) {
//					markItem.type = node.Attributes ["type"].Value.ToString ();
//				}
//				if (markItem.IsExist ()) {
//					markItems.Add (markItem);
//				} 
//			}
//		}

//		public void CheckItems ()
//		{
//			for (int i = 0; i < markItems.Count; i++) {
//				MarkItem item = markItems [i];
//				if (!item.IsExist ()) {
//					markItems.Remove (item);
//				}
//			}
//		}

//		public string AddItem (string path)
//		{
//			DirectoryInfo dir = new DirectoryInfo (path);
//			MarkItem item = new MarkItem ();
//			item.name = dir.Name;
//			item.path = path;
//			markItems.Add (item);
//			SaveConfigFile ();
//			return dir.Name;
//		}
//
//
//		public void RemoveItem (string path)
//		{
//			MarkItem item = GetItem (path);
//			if (item != null) {
//				markItems.Remove (item);
//			}
//			SaveConfigFile ();
//		}
//
//		public bool ContainsItem (string path)
//		{
//			if (GetItem (path) != null) {
//				return true;
//			}
//			return false;
//		}
//
//		public MarkItem GetItem (string path)
//		{
//			for (int i = 0; i < markItems.Count; i++) {
//				MarkItem item = markItems [i];
//				if (item.path == path) {
//					return item;
//				}
//			}
//			return null;
//		}
//
//
//
//		public void SaveConfigFile ()
//		{
//			XmlDocument xmlDoc = new XmlDocument ();
//			XmlNode configElement = xmlDoc.AppendChild (xmlDoc.CreateElement ("config"));
//			for (int i = 0; i < markItems.Count; i++) {
//				MarkItem item = markItems [i];
//				if (item.IsExist ()) {
//					XmlElement element = xmlDoc.CreateElement ("item");//添加Node2节点
//					element.SetAttribute ("name", item.name);
//					element.SetAttribute ("path", item.path);
//					element.SetAttribute ("type", item.type);
//					configElement.AppendChild (element);
//				}
//			}
//			xmlDoc.Save (configFileUrl);
//			AssetDatabase.Refresh ();
//		}
//
//
//
//		public void Dispose ()
//		{
//
//			mInstance = null;
//
//		}

	}

//	public class MarkItem
//	{
//		public const string TYPE_ITEM_ASSETBUNDLE = "assetbundle";
//		public const string TYPE_ITEM_CUSTOM = "zip";
//		public const string TYPE_ITEM_FILE = "file";
//		public string name = "";
//		public string path = "";
//		public string type = "";
//
//		public bool IsExist ()
//		{
//
//			if (AssetDatabase.LoadMainAssetAtPath (path) != null) {
//				return true;
//			} else {
//				return false;
//			}
//		}
//
//	}
}