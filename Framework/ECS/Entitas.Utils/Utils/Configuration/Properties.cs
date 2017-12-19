using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QFramework
{
    public class Properties
    {
        public string[] Keys
        {
            get { return mDict.Keys.ToArray(); }
        }

        public string[] Values
        {
            get { return mDict.Values.ToArray(); }
        }

        public int Count
        {
            get { return mDict.Count; }
        }

        const string PlaceholderPattern = @"\${(.+?)}";

        public string this[string key]
        {
            get
            {
                return Regex.Replace(
                    mDict[key],
                    PlaceholderPattern,
                    match => mDict[match.Groups[1].Value]);
            }
            set
            {
                mDict[key.Trim()] = value
                    .TrimStart()
                    .Replace("\\n", "\n")
                    .Replace("\\t", "\t");
            }
        }

        readonly Dictionary<string, string> mDict;

        public Properties() : this(string.Empty)
        {
        }

        public Properties(string properties)
        {
            properties = ConvertLineEndings(properties);
            mDict = new Dictionary<string, string>();
            var lines = GetLinesWithProperties(properties);
            AddProperties(MergeMultilineValues(lines));
        }

        public Properties(Dictionary<string, string> properties)
        {
            mDict = new Dictionary<string, string>(properties);
        }

        public bool HasKey(string key)
        {
            return mDict.ContainsKey(key);
        }

        public void AddProperties(Dictionary<string, string> properties, bool overwriteExisting)
        {
            foreach (var kv in properties)
            {
                if (overwriteExisting || !HasKey(kv.Key))
                {
                    this[kv.Key] = kv.Value;
                }
            }
        }

        public void RemoveProperty(string key)
        {
            mDict.Remove(key);
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>(mDict);
        }

        void AddProperties(string[] lines)
        {
            var keyValueDelimiter = new[] {'='};
            var properties = lines.Select(
                line => line.Split(keyValueDelimiter, 2)
            );
            foreach (var property in properties)
            {
                if (property.Length != 2)
                {
                    throw new InvalidKeyPropertiesException(property[0]);
                }

                this[property[0]] = property[1];
            }
        }

        static string ConvertLineEndings(string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        static string[] GetLinesWithProperties(string properties)
        {
            var delimiter = new[] {'\n'};
            return properties
                .Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.TrimStart(' '))
                .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
                .ToArray();
        }

        static string[] MergeMultilineValues(string[] lines)
        {
            var currentProperty = string.Empty;
            return lines.Aggregate(new List<string>(), (acc, line) =>
            {
                currentProperty += line;
                if (currentProperty.EndsWith("\\", StringComparison.Ordinal))
                {
                    currentProperty = currentProperty.Substring(
                        0, currentProperty.Length - 1
                    );
                }
                else
                {
                    acc.Add(currentProperty);
                    currentProperty = string.Empty;
                }

                return acc;
            }).ToArray();
        }

        public override string ToString()
        {
            return mDict.Aggregate(string.Empty, (properties, kv) =>
            {
                var content = kv.Value
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t");

                return properties + kv.Key + " = " + content + "\n";
            });
        }
    }

    public class InvalidKeyPropertiesException : Exception
    {
        public readonly string Key;

        public InvalidKeyPropertiesException(string key) : base("Invalid key: " + key)
        {
            Key = key;
        }
    }
}