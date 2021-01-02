using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml.CustomAttributes
{    
    public class AnimationControllerAttribute: CustomXmlAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {
            var animatorController = value.ToRuntimeAnimatorController();

            var animator = xmlElement.GetComponent<Animator>();
            if (animator == null) animator = xmlElement.gameObject.AddComponent<Animator>();

            animator.runtimeAnimatorController = animatorController;

            animator.StartPlayback();                        
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.Animation;
            }
        }
    }            
}
