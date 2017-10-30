using System;
using System.Linq;
using Entitas.Utils;
using QFramework;

namespace Entitas.CodeGeneration.CodeGenerator.CLI
{

    public static class Helper
    {

        public static string[] GetUnusedKeys(string[] requiredKeys, Properties properties)
        {
            return properties.Keys
                .Where(key => !requiredKeys.Contains(key))
                .ToArray();
        }

        public static string[] GetMissingKeys(string[] requiredKeys, Properties properties)
        {
            return requiredKeys
                .Where(key => !properties.HasKey(key))
                .ToArray();
        }

        public static bool GetUserDecision(char accept = 'y', char cancel = 'n')
        {
            char keyChar;
            do
            {
                keyChar = Console.ReadKey(true).KeyChar;
            } while (keyChar != accept && keyChar != cancel);

            return keyChar == accept;
        }

        public static void ForceAddKey(string message, string key, string value, Properties properties)
        {
            Log.I(message + ": '" + key + "'");
            Console.ReadKey(true);
            properties[key] = value;
            Preferences.SaveProperties(properties);
            Log.I("Added: " + key);
        }

        public static void AddKey(string question, string key, string value, Properties properties)
        {
            Log.I(question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision())
            {
                properties[key] = value;
                Preferences.SaveProperties(properties);
                Log.I("Added: " + key);
            }
        }

        public static void RemoveKey(string question, string key, Properties properties)
        {
            Log.W(question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision())
            {
                properties.RemoveProperty(key);
                Preferences.SaveProperties(properties);
                Log.W("Removed: " + key);
            }
        }

        public static void RemoveValue(string question, string value, string[] values, Action<string[]> updateAction,
            Properties properties)
        {
            Log.W(question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision())
            {
                var valueList = values.ToList();
                valueList.Remove(value);
                updateAction(valueList.ToArray());
                Preferences.SaveProperties(properties);
                Log.W("Removed: " + value);
            }
        }

        public static void AddValue(string question, string value, string[] values, Action<string[]> updateAction,
            Properties properties)
        {
            Log.I(question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision())
            {
                var valueList = values.ToList();
                valueList.Add(value);
                updateAction(CodeGeneratorUtil.GetOrderedNames(valueList.ToArray()));
                Preferences.SaveProperties(properties);
                Log.I("Added: " + value);
            }
        }
    }
}