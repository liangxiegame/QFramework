/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if NETFX_CORE
using System.Threading.Tasks;
using System.Linq;
#endif

using Loxodon.Log;

namespace BindKit.Binding.Paths
{
    public class TypeFinderUtils
    {
#if NETFX_CORE
        private static readonly ILog log = LogManager.GetLogger(typeof(TypeFinderUtils));
#endif

        private static List<Assembly> assemblies = new List<Assembly>();

        public static Type FindType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
#if NETFX_CORE
            Task<List<Assembly>> task = GetAssemblies();
            task.Wait();
            List<Assembly> assemblies = task.Result;
#else
            List<Assembly> assemblies = GetAssemblies();
#endif
            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetType(typeName, false, false);
                if (type != null)
                    return type;
            }

            var name = string.Format(".{0}", typeName);
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName.EndsWith(name))
                        return type;
                }
            }
            return null;
        }

#if NETFX_CORE
        private static async Task<List<Assembly>> GetAssemblies()
        {
            if (assemblies.Count > 0)
                return assemblies;

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files.Where(file => file.FileType == ".dll" || file.FileType == ".exe"))
            {
                try
                {
                    if (Regex.IsMatch(file.Name, "^((mscorlib)|(nunit)|(ucrtbased)|(Microsoft)|(ClrCompression)|(BridgeInterface)|(System)|(UnityEngine)|(UnityPlayer)|(Loxodon.Log)|(WinRTLegacy))"))
                        continue;

                    assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }
                catch (Exception e) 
                {
                     if (log.IsWarnEnabled)
                        log.Warn("Loads assembly:{0}", e);
                }
            }
            return assemblies;
        }
#else
        private static List<Assembly> GetAssemblies()
        {
            if (assemblies.Count > 0)
                return assemblies;

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in listAssembly)
            {
                var name = assembly.FullName;
                if (Regex.IsMatch(name, "^((mscorlib)|(nunit)|(System)|(UnityEngine)|(Loxodon.Log))"))
                    continue;

                assemblies.Add(assembly);
            }
            return assemblies;
        }
#endif
    }
}
