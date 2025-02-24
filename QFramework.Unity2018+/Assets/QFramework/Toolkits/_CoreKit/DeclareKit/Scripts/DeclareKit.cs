/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class DeclareKit
    {
        internal static DeclareClassNameRuleTable ClassNameRuleTable = new DeclareClassNameRuleTable();
        public static void AttachRules(DeclareComponent declareComponent)
        {
            if (declareComponent.ClassNames.Length > 0)
            {
                foreach (var className in declareComponent.ClassNames)
                {
                    foreach (var declareRuleItem in ClassNameRuleTable.ClassNameIndex.Get(className))
                    {
                        declareRuleItem.OnRule(declareComponent);
                    }
                }
            }
        }
        
        public static IUnRegister RegisterClassRule(string className, Action<DeclareComponent> onRule)
        {
            var declareRuleItem = new DeclareClassNameRuleItem()
            {
                ClassName = className,
                OnRule = onRule
            };
                
            ClassNameRuleTable.Add(declareRuleItem);

            return new CustomUnRegister(() =>
            {
                ClassNameRuleTable.Remove(declareRuleItem);
            });
        }
        
        [MonoSingletonPath("QFramework/CoreKit/DeclareKit/DeclareComponentManager")]
        internal class DeclareComponentManager : MonoSingleton<DeclareComponentManager>
        {
            
        }
        

    }
    

}