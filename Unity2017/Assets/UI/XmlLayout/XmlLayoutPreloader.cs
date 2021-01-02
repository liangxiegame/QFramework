using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Xml
{    
    public class XmlLayoutPreloader : MonoBehaviour
    {        
        public void Preload()
        {
            StartCoroutine(Preload_Internal());
        }

        IEnumerator Preload_Internal()
        {
            //DateTime startTime = DateTime.Now;

            var tagHandlerNames = XmlLayoutUtilities.GetXmlTagHandlerNames();                        

            var customAttributeNames = XmlLayoutUtilities.GetCustomAttributeNames();

            foreach (var tagHandlerName in tagHandlerNames)
            {
                // instantiate the tag handler
                var tagHandler = XmlLayoutUtilities.GetXmlTagHandler(tagHandlerName);

                // load the prefab
                XmlLayoutUtilities.LoadResource<GameObject>(tagHandler.prefabPath);                
            }

            foreach (var customAttributeName in customAttributeNames)
            {
                XmlLayoutUtilities.GetCustomAttribute(customAttributeName);                
            }

            //Debug.Log((DateTime.Now - startTime).TotalMilliseconds + "ms");

            yield return null;
        }        
    }
}
