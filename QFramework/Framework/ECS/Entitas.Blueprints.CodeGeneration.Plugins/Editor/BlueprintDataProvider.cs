using System.Linq;
using Entitas.Blueprints.Unity.Editor;
using QFramework;

namespace Entitas.Blueprints.CodeGeneration.Plugins
{
    public class BlueprintDataProvider : ICodeGeneratorDataProvider
    {
        public string Name
        {
            get { return "Blueprint"; }
        }

        public int Priority
        {
            get { return 0; }
        }

        public bool IsEnabledByDefault
        {
            get { return true; }
        }

        public bool RunInDryMode
        {
            get { return true; }
        }

        readonly string[] mBlueprintNames;

        public BlueprintDataProvider()
        {
            mBlueprintNames = BinaryBlueprintInspector
                .FindAllBlueprints()
                .Select(b => b.Deserialize().name)
                .ToArray();
        }

        public CodeGeneratorData[] GetData()
        {
            return mBlueprintNames
                .Select(blueprintName =>
                {
                    var data = new BlueprintData();
                    data.SetBlueprintName(blueprintName);
                    return data;
                }).ToArray();
        }
    }

    public static class BlueprintDataProviderExtension
    {

        public const string BLUEPRINT_NAME = "blueprint_name";

        public static string GetBlueprintName(this BlueprintData data)
        {
            return (string) data[BLUEPRINT_NAME];
        }

        public static void SetBlueprintName(this BlueprintData data, string blueprintName)
        {
            data[BLUEPRINT_NAME] = blueprintName;
        }
    }
}
