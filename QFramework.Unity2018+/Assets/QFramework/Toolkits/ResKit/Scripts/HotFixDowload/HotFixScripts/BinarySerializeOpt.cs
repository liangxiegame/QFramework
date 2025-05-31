using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace QFramework
{
    public class BinarySerializeOpt
    {



        /// <summary>
        /// 类序列化成xml
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Xmlserialize(string path, System.Object obj)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                    {
                        //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                        //namespaces.Add(string.Empty, string.Empty);
                        XmlSerializer xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                }
                return true;
                ;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成xml " + obj.GetType() + "," + e);
            }
            return false;
        }

        /// <summary>
        /// 编辑器使读取xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path) where T : class
        {
            T t = default(T);
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此xml无法转成二进制: " + path + "," + e);
            }
            return t;
        }

        /// <summary>
        /// Xml的反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Object XmlDeserialize(string path, Type type)
        {
            System.Object obj = null;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    obj = xs.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此xml无法转成二进制: " + path + "," + e);
            }
            return obj;
        }




        /// <summary>
        /// 类转换成二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool BinarySerilize(string path, System.Object obj)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
            }
            return false;
        }


    }

}
