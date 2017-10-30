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
    using System.IO;
    using Utils;
    using QFramework;

    public class NewConfig : ECSCommand
    {
        public override string Trigger
        {
            get { return "new"; }
        }

        public override string Description
        {
            get { return "Creates new Entitas.properties config with default values"; }
        }

        public override string Example
        {
            get { return "entitas new [-f]"; }
        }

        public override void Execute(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + Preferences.PATH;

            if ( /*args.IsForce() ||*/ !File.Exists(path))
            {
                var defaultConfig = new CodeGeneratorConfig();
                var properties = new Properties(defaultConfig.DefaultProperties);
                defaultConfig.Configure(properties);

                var propertiesString = defaultConfig.ToString();
                File.WriteAllText(path, propertiesString);

                Log.I("Created " + path);
                Log.I(propertiesString);

                new EditConfig().Execute(args);
            }
            else
            {
                Log.W(path + " already exists!");
                Log.I("Use entitas new -f to overwrite the exiting file.");
                Log.I("Use entitas edit to open the exiting file.");
            }
        }
    }
}