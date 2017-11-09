using System.Collections.Generic;
using System.Linq;


namespace QFramework
{

    public delegate void GeneratorProgress(string title, string info, float progress);

    public class CodeGenerator
    {

        public event GeneratorProgress OnProgress;

        readonly ICodeGeneratorDataProvider[] mDataProviders;
        readonly ICodeGenerator[] mCodeGenerators;
        readonly ICodeGenFilePostProcessor[] _postProcessors;

        bool mCancel;

        public CodeGenerator(ICodeGeneratorDataProvider[] dataProviders,
            ICodeGenerator[] codeGenerators,
            ICodeGenFilePostProcessor[] postProcessors)
        {

            mDataProviders = dataProviders.OrderBy(i => i.Priority).ToArray();
            mCodeGenerators = codeGenerators.OrderBy(i => i.Priority).ToArray();
            _postProcessors = postProcessors.OrderBy(i => i.Priority).ToArray();
        }

        public CodeGenFile[] DryRun()
        {
            return generate(
                "[Dry Run] ",
                mDataProviders.Where(i => i.RunInDryMode).ToArray(),
                mCodeGenerators.Where(i => i.RunInDryMode).ToArray(),
                _postProcessors.Where(i => i.RunInDryMode).ToArray()
            );
        }

        public CodeGenFile[] Generate()
        {
            return generate(
                string.Empty,
                mDataProviders,
                mCodeGenerators,
                _postProcessors
            );
        }

        CodeGenFile[] generate(string messagePrefix, ICodeGeneratorDataProvider[] dataProviders,
            ICodeGenerator[] codeGenerators, ICodeGenFilePostProcessor[] postProcessors)
        {
            mCancel = false;

            var data = new List<CodeGeneratorData>();

            var total = dataProviders.Length + codeGenerators.Length + postProcessors.Length;
            int progress = 0;

            foreach (var dataProvider in dataProviders)
            {
                if (mCancel)
                {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null)
                {
                    OnProgress(messagePrefix + "Creating model", dataProvider.Name, (float) progress / total);
                }

                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            var dataArray = data.ToArray();
            foreach (var generator in codeGenerators)
            {
                if (mCancel)
                {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null)
                {
                    OnProgress(messagePrefix + "Creating files", generator.Name, (float) progress / total);
                }

                files.AddRange(generator.Generate(dataArray));
            }

            var generatedFiles = files.ToArray();
            foreach (var postProcessor in postProcessors)
            {
                if (mCancel)
                {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if (OnProgress != null)
                {
                    OnProgress(messagePrefix + "Processing files", postProcessor.Name, (float) progress / total);
                }

                generatedFiles = postProcessor.PostProcess(generatedFiles);
            }

            return generatedFiles;
        }

        public void Cancel()
        {
            mCancel = true;
        }
    }
}