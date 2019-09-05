using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class CodeFileSyncSystem : DiagramPlugin
        //, 
        //IDataRecordRemoved,
        , ICommandExecuted,
        ICommandExecuting,
        IExecuteCommand<LambdaFileSyncCommand>

        //IDataRecordPropertyBeforeChange
    {
        public List<IDataRecord> ChangedRecords { get; set; }

        public void RecordRemoved(IDataRecord record)
        {

            var items = record.GetCodeGeneratorsForNode(Container.Resolve<DatabaseService>().CurrentConfiguration).ToArray();
            var deleteList = new List<string>();
            foreach (var item in items)
            {
                var fullpath = Path.Combine(Application.dataPath, item.RelativeFullPathName);
                if (File.Exists(fullpath))
                {
                    deleteList.Add(fullpath);
                }
            }
            if (deleteList.Count > 0)
            {
                if (InvertGraphEditor.Platform.MessageBox("Warning",
               string.Format("You've deleted a record that has {0} file(s) associated with it.  Would you like to remove them?", deleteList.Count),
               "YES (Recommended)", "NO they are special!"))
                {
                    foreach (var item in deleteList)
                    {
                        File.Delete(item);
                    }
                }
                this.Execute(new SaveAndCompileCommand() {ForceCompileAll = true});
            }
           

            
        }

        public struct GenFileInfo
        {
            public GenFileInfo(string fullPath, OutputGenerator generator)
            {

                FullPath = fullPath;
                Generator = generator;
            }


            public string FullPath;
            public OutputGenerator Generator;
        }
        public void CommandExecuted(ICommand command)
        {
            if (command is ApplyRenameCommand)
            {
                RenameApplied(command as ApplyRenameCommand);
            }
            if (!(command is IFileSyncCommand)) return;
            var items = Container.Resolve<IRepository>().AllOf<IDataRecord>().ToArray();
            var gensNow = new Dictionary<string, GenFileInfo>();
            foreach (var p in InvertGraphEditor.GetAllCodeGenerators(
                Container.Resolve<DatabaseService>().CurrentConfiguration, items))
            {
                var key = Path.GetFileName(p.Filename);
                if (!gensNow.ContainsKey(key))
                    gensNow.Add(key, new GenFileInfo(Path.Combine(Application.dataPath, p.RelativeFullPathName), p));
            }

            var deleteList = new List<string>();
            foreach (var item in Gens)
            {
                if (!gensNow.ContainsKey(item.Key))
                {
                    // Its been removed or renamed
                    
                    if (File.Exists(item.Value.FullPath))
                        deleteList.Add(item.Value.FullPath);
                }
                else
                {
                    var nowItem = gensNow[item.Key];
                    if (nowItem.Generator.ObjectData == item.Value.Generator.ObjectData)
                    {
                        // It has been moved
                        if (File.Exists(item.Value.FullPath))
                            File.Move(item.Value.FullPath, nowItem.FullPath);
                    }
                }
            }
          
            Gens.Clear();
            GC.Collect();
            IsRename = false;
            if (deleteList.Count > 0)
            {
                if (InvertGraphEditor.Platform.MessageBox("Warning", 
                    string.Format("You've deleted a record that has {0} file(s) associated with it.  Would you like to remove them?", deleteList.Count), 
                    "YES (Recommended)", "NO they are special!"))
                {
                    foreach (var item in deleteList)
                    {
                        File.Delete(item);
                    }
                }
                this.Execute(new SaveAndCompileCommand() { ForceCompileAll = true });
            }

        }
        private void RenameApplying(ApplyRenameCommand applyRenameCommand)
        {
            RenameGens = InvertGraphEditor.GetAllCodeGenerators(
                Container.Resolve<DatabaseService>().CurrentConfiguration, new IDataRecord[] { applyRenameCommand.Item })
                .Select(p => Path.Combine(Application.dataPath, p.RelativeFullPathName)).ToArray();
        }

        public string[] RenameGens { get; set; }

        private void RenameApplied(ApplyRenameCommand applyRenameCommand)
        {
            var gensNow = InvertGraphEditor.GetAllCodeGenerators(
                Container.Resolve<DatabaseService>().CurrentConfiguration, new IDataRecord[] { applyRenameCommand.Item })
                .Select(p => Path.Combine(Application.dataPath, p.RelativeFullPathName)).ToArray();

            if (gensNow.Length == RenameGens.Length)
            {
                for (var i = 0; i < gensNow.Length; i++)
                {
                    if (File.Exists(RenameGens[i]))
                        File.Move(RenameGens[i], gensNow[i]);
                }
            }

        }

        public void CommandExecuting(ICommand command)
        {
            if (command is ApplyRenameCommand)
            {
                RenameApplying(command as ApplyRenameCommand);
            }
            if (!(command is IFileSyncCommand)) return;
            IsRename = command is ApplyRenameCommand;
            var items = Container.Resolve<IRepository>().AllOf<IDataRecord>().ToArray();
            Gens = new Dictionary<string, GenFileInfo>();
            foreach (var p in InvertGraphEditor.GetAllCodeGenerators(
                Container.Resolve<DatabaseService>().CurrentConfiguration, items))
            {
                var key = Path.GetFileName(p.Filename);
                if (!Gens.ContainsKey(key))
                Gens.Add(key,new GenFileInfo(Path.Combine(Application.dataPath, p.RelativeFullPathName), p));
            }
             



        }


        public bool IsRename { get; set; }

        public Dictionary<string, GenFileInfo> Gens { get; set; }

        public void BeforePropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {

        }

        public void Execute(LambdaFileSyncCommand command)
        {
            Execute(new LambdaCommand(command.Title,command.Action));
        }
    }
}