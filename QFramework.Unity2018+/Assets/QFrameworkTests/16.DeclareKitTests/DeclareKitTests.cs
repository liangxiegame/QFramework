using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace QFramework.Tests
{
    public class DeclareClassNameRuleTableTests
    {
        [Test]
        public void Add_Then_Get_Returns_Item()
        {
            var table = DeclareKitTestHelper.CreateRuleTable();
            var ruleItem = DeclareKitTestHelper.CreateRuleItem("View");

            DeclareKitTestHelper.AddRule(table, ruleItem);

            var rules = DeclareKitTestHelper.GetRules(table, "View");
            Assert.AreEqual(1, rules.Length);
            Assert.AreSame(ruleItem, rules[0]);
        }

        [Test]
        public void Get_Returns_Empty_When_No_Match()
        {
            var table = DeclareKitTestHelper.CreateRuleTable();

            var rules = DeclareKitTestHelper.GetRules(table, "Unknown");

            Assert.IsEmpty(rules);
        }

        [Test]
        public void Remove_Then_Get_Empty()
        {
            var table = DeclareKitTestHelper.CreateRuleTable();
            var ruleItem = DeclareKitTestHelper.CreateRuleItem("View");
            DeclareKitTestHelper.AddRule(table, ruleItem);

            DeclareKitTestHelper.RemoveRule(table, ruleItem);

            Assert.IsEmpty(DeclareKitTestHelper.GetRules(table, "View"));
        }

        [Test]
        public void Multiple_Same_ClassName()
        {
            var table = DeclareKitTestHelper.CreateRuleTable();
            var firstRule = DeclareKitTestHelper.CreateRuleItem("View");
            var secondRule = DeclareKitTestHelper.CreateRuleItem("View");

            DeclareKitTestHelper.AddRule(table, firstRule);
            DeclareKitTestHelper.AddRule(table, secondRule);

            var rules = DeclareKitTestHelper.GetRules(table, "View");
            Assert.AreEqual(2, rules.Length);
            Assert.IsTrue(rules.Contains(firstRule));
            Assert.IsTrue(rules.Contains(secondRule));
        }

        [Test]
        public void Clear_Removes_All()
        {
            var table = DeclareKitTestHelper.CreateRuleTable();
            DeclareKitTestHelper.AddRule(table, DeclareKitTestHelper.CreateRuleItem("View"));
            DeclareKitTestHelper.AddRule(table, DeclareKitTestHelper.CreateRuleItem("Controller"));

            DeclareKitTestHelper.ClearRules(table);

            Assert.IsEmpty(DeclareKitTestHelper.GetRules(table, "View"));
            Assert.IsEmpty(DeclareKitTestHelper.GetRules(table, "Controller"));
        }
    }

    public class DeclareKitTests
    {
        [SetUp]
        public void SetUp()
        {
            DeclareKitTestHelper.ResetDeclareKitRuleTable();
        }

        [TearDown]
        public void TearDown()
        {
            DeclareKitTestHelper.ResetDeclareKitRuleTable();
        }

        [Test]
        public void RegisterClassRule_Returns_UnRegister()
        {
            Action<DeclareComponent> onRule = component => { };

            var unregister = DeclareKit.RegisterClassRule("View", onRule);

            Assert.IsNotNull(unregister);
        }

        [Test]
        public void UnRegister_Removes_Rule()
        {
            Action<DeclareComponent> onRule = component => { };
            var unregister = DeclareKit.RegisterClassRule("View", onRule);
            Assert.AreEqual(1, DeclareKitTestHelper.GetRegisteredRuleCount("View"));

            unregister.UnRegister();

            Assert.AreEqual(0, DeclareKitTestHelper.GetRegisteredRuleCount("View"));
        }
    }

    internal static class DeclareKitTestHelper
    {
        private const BindingFlags StaticNonPublic = BindingFlags.Static | BindingFlags.NonPublic;
        private const BindingFlags InstancePublic = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags InstancePublicOrNonPublic =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static object CreateRuleTable()
        {
            return Activator.CreateInstance(RuleTableType, true);
        }

        public static object CreateRuleItem(string className)
        {
            var ruleItem = Activator.CreateInstance(RuleItemType, true);
            RuleItemType.GetField("ClassName", InstancePublicOrNonPublic).SetValue(ruleItem, className);
            RuleItemType.GetField("OnRule", InstancePublicOrNonPublic)
                .SetValue(ruleItem, (Action<DeclareComponent>)(component => { }));
            return ruleItem;
        }

        public static void AddRule(object table, object ruleItem)
        {
            table.GetType().GetMethod("Add", InstancePublic).Invoke(table, new[] { ruleItem });
        }

        public static void RemoveRule(object table, object ruleItem)
        {
            table.GetType().GetMethod("Remove", InstancePublic).Invoke(table, new[] { ruleItem });
        }

        public static void ClearRules(object table)
        {
            table.GetType().GetMethod("Clear", InstancePublic).Invoke(table, null);
        }

        public static object[] GetRules(object table, string className)
        {
            var classNameIndex = table.GetType().GetField("ClassNameIndex", InstancePublic).GetValue(table);
            var rules = (IEnumerable)classNameIndex.GetType().GetMethod("Get", InstancePublic)
                .Invoke(classNameIndex, new object[] { className });
            return rules.Cast<object>().ToArray();
        }

        public static void ResetDeclareKitRuleTable()
        {
            DeclareKitRuleTableField.SetValue(null, CreateRuleTable());
        }

        public static int GetRegisteredRuleCount(string className)
        {
            return GetRules(DeclareKitRuleTableField.GetValue(null), className).Length;
        }

        private static Type RuleTableType
        {
            get { return typeof(DeclareKit).Assembly.GetType("QFramework.DeclareClassNameRuleTable", true); }
        }

        private static Type RuleItemType
        {
            get { return typeof(DeclareKit).Assembly.GetType("QFramework.DeclareClassNameRuleItem", true); }
        }

        private static FieldInfo DeclareKitRuleTableField
        {
            get { return typeof(DeclareKit).GetField("ClassNameRuleTable", StaticNonPublic); }
        }
    }
}
