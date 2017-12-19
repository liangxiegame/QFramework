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

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    

    public class Status : ECSCommand
    {
        public override string Trigger
        {
            get { return "status"; }
        }

        public override string Description
        {
            get { return "Lists available and unavailable plugins"; }
        }

        public override string Example
        {
            get { return "entitas status"; }
        }

        public override void Execute(string[] args)
        {
            if (AssertProperties())
            {
                var properties = LoadProperties();
                var config = new CodeGeneratorConfig();
                config.Configure(properties);

                Log.I(config.ToString());

                Type[] types = null;
                Dictionary<string, string> configurables = null;

                try
                {
                    types = CodeGeneratorUtil.LoadTypesFromPlugins(properties);
                    configurables = CodeGeneratorUtil.GetConfigurables(
                        CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(types, config.DataProviders),
                        CodeGeneratorUtil.GetUsed<ICodeGenerator>(types, config.CodeGenerators),
                        CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(types, config.PostProcessors)
                    );

                }
                catch (Exception ex)
                {
                    PrintKeyStatus(config.DefaultProperties.Keys.ToArray(), properties);
                    throw ex;
                }

                var requiredKeys = config.DefaultProperties.Merge(configurables).Keys.ToArray();

                PrintKeyStatus(requiredKeys, properties);
                PrintPluginStatus(types, config);
            }
        }

        static void PrintKeyStatus(string[] requiredKeys, Properties properties)
        {
            Helper.GetUnusedKeys(requiredKeys, properties).ForEach(key => Log.I("unused Key:{0}", key));
            Helper.GetMissingKeys(requiredKeys, properties).ForEach(key => Log.I("Missing key: {0}", key));
        }

        static void PrintPluginStatus(Type[] types, CodeGeneratorConfig config)
        {
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

            PrintUnavailable(unavailableDataProviders);
            PrintUnavailable(unavailableCodeGenerators);
            PrintUnavailable(unavailablePostProcessors);

            PrintAvailable(availableDataProviders);
            PrintAvailable(availableCodeGenerators);
            PrintAvailable(availablePostProcessors);
        }

        static void PrintUnavailable(string[] names)
        {
            names.ForEach(name => Log.I("Unavailable: {0}", name));
        }

        static void PrintAvailable(string[] names)
        {
            names.ForEach(name => Log.I("Available: {0}", name));
        }
    }
}