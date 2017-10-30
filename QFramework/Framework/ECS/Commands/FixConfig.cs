/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace Entitas.CodeGeneration.CodeGenerator.CLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils;
    using QFramework;

    public class FixConfig : ECSCommand
    {
        public override string Trigger
        {
            get { return "fix"; }
        }

        public override string Description
        {
            get { return "Adds missing or removes unused keys interactively"; }
        }

        public override string Example
        {
            get { return "entitas fix"; }
        }

        public override void Execute(string[] args)
        {
            if (AssertProperties())
            {
                var properties = LoadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                ForceAddKeys(config.DefaultProperties, properties);

                Type[] types = null;

                try
                {
                    types = CodeGeneratorUtil.LoadTypesFromPlugins(properties);
                    GetConfigurables(types, config);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var askedRemoveKeys = new HashSet<string>();
                var askedAddKeys = new HashSet<string>();
                while (Fix(askedRemoveKeys, askedAddKeys, types, config, properties))
                {
                }
            }
        }

        static Dictionary<string, string> GetConfigurables(Type[] types, CodeGeneratorConfig config)
        {
            return CodeGeneratorUtil.GetConfigurables(
                CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.DataProviders),
                CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.CodeGenerators),
                CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.PostProcessors)
            );
        }

        static void ForceAddKeys(Dictionary<string, string> requiredProperties, Properties properties)
        {
            var requiredKeys = requiredProperties.Keys.ToArray();
            var missingKeys = Helper.GetMissingKeys(requiredKeys, properties);

            foreach (var key in missingKeys)
            {
                Helper.ForceAddKey("Will add missing key", key, requiredProperties[key], properties);
            }
        }

        static bool Fix(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, Type[] types,
            CodeGeneratorConfig config, Properties properties)
        {
            var changed = FixPlugins(askedRemoveKeys, askedAddKeys, types, config, properties);

            ForceAddKeys(GetConfigurables(types, config), properties);

            var requiredKeys = config.DefaultProperties.Merge(GetConfigurables(types, config)).Keys.ToArray();
            RemoveUnusedKeys(askedRemoveKeys, requiredKeys, properties);

            return changed;
        }

        static bool FixPlugins(HashSet<string> askedRemoveKeys, HashSet<string> askedAddKeys, Type[] types,
            CodeGeneratorConfig config, Properties properties)
        {
            var changed = false;

            var unavailableDataProviders =
                CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(types, config.DataProviders);
            var unavailableCodeGenerators =
                CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(types, config.CodeGenerators);
            var unavailablePostProcessors =
                CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(types, config.PostProcessors);

            var availableDataProviders =
                CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(types, config.DataProviders);
            var availableCodeGenerators = CodeGeneratorUtil.GetAvailable<ICodeGenerator>(types, config.CodeGenerators);
            var availablePostProcessors =
                CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(types, config.PostProcessors);

            foreach (var key in unavailableDataProviders.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.RemoveValue("Remove unavailable data provider", key, config.DataProviders,
                    values => config.DataProviders = values, properties);
                askedRemoveKeys.Add(key);
                changed = true;
            }

            foreach (var key in unavailableCodeGenerators.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.RemoveValue("Remove unavailable code generator", key, config.CodeGenerators,
                    values => config.CodeGenerators = values, properties);
                askedRemoveKeys.Add(key);
                changed = true;
            }

            foreach (var key in unavailablePostProcessors.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.RemoveValue("Remove unavailable post processor", key, config.PostProcessors,
                    values => config.PostProcessors = values, properties);
                askedRemoveKeys.Add(key);
                changed = true;
            }

            foreach (var key in availableDataProviders.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.AddValue("Add available data provider", key, config.DataProviders,
                    values => config.DataProviders = values, properties);
                askedAddKeys.Add(key);
                changed = true;
            }

            foreach (var key in availableCodeGenerators.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.AddValue("Add available code generator", key, config.CodeGenerators,
                    values => config.CodeGenerators = values, properties);
                askedAddKeys.Add(key);
                changed = true;
            }

            foreach (var key in availablePostProcessors.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.AddValue("Add available post processor", key, config.PostProcessors,
                    values => config.PostProcessors = values, properties);
                askedAddKeys.Add(key);
                changed = true;
            }

            return changed;
        }

        static void RemoveUnusedKeys(HashSet<string> askedRemoveKeys, string[] requiredKeys, Properties properties)
        {
            var unused = Helper.GetUnusedKeys(requiredKeys, properties);
            foreach (var key in unused.Where(k => !askedRemoveKeys.Contains(k)))
            {
                Helper.RemoveKey("Remove unused key", key, properties);
                askedRemoveKeys.Add(key);

            }
        }
    }
}