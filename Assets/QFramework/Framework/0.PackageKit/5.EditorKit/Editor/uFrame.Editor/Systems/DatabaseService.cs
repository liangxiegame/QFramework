using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Invert.Data;
using QF.Json;
using UnityEngine;

namespace QF.GraphDesigner
{
    public static class uFrameVersion
    {
        public const int MajorVersion = 1;
        public const int MinorVersion = 0;
        public const int BuildVersion = 0;
    }
    public class DatabaseService : DiagramPlugin,  
        IDataRecordInserted, 
        IDataRecordRemoved, 
        IDataRecordPropertyBeforeChange,
        IDataRecordPropertyChanged,
        IDataRecordRemoving,
        IDataRecordManagerRefresh,
        IChangeDatabase,
        IToolbarQuery,
        IContextMenuQuery,
        IExecuteCommand<ChangeDatabaseCommand>,
        IExecuteCommand<SaveCommand>,
        IExecuteCommand<CreateDatabaseCommand>,
        IExecuteCommand<EditDatabaseCommand>
        
    {
        private Dictionary<string, uFrameDatabaseConfig> mConfigurations;

        public uFrameDatabaseConfig CurrentConfiguration { get; set; }

        // Important make sure we intialize very late so that other plugins can register graph configurations
        public override decimal LoadPriority
        {
            get { return 50000; }
        }

        public override bool Required
        {
            get { return true; }
        }

        public string CurrentDatabaseIdentifier
        {
            get { return InvertGraphEditor.Prefs.GetString("CurrentDatabaseIdentifier", string.Empty); }
            set {InvertGraphEditor.Prefs.SetString("CurrentDatabaseIdentifier",value); }
        }

        

        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);
            var path = DbRootPath;
            var dbDirectories = Directory.GetDirectories(path,"*.db",SearchOption.AllDirectories);
            foreach (var item in dbDirectories)
            {
                var db = new TypeDatabase(new JsonRepositoryFactory(item));
                var config = GetConfig(db, Path.GetFileNameWithoutExtension(item));
                config.FullPath = item;
                container.RegisterInstance<IGraphConfiguration>(config, config.Identifier);
                
            }
           
            CurrentConfiguration = Configurations.ContainsKey(CurrentDatabaseIdentifier)
                ? Configurations[CurrentDatabaseIdentifier]
                : Configurations.Values.FirstOrDefault();

            if (CurrentConfiguration != null)
            {
                container.RegisterInstance<IGraphConfiguration>(CurrentConfiguration);
           
                container.RegisterInstance<IRepository>(CurrentConfiguration.Database);
                
                //var typeDatabase = container.Resolve<IRepository>();
                CurrentConfiguration.Database.AddListener<IDataRecordInserted>(this);
                CurrentConfiguration.Database.AddListener<IDataRecordRemoved>(this);
                CurrentConfiguration.Database.AddListener<IDataRecordPropertyChanged>(this);
                CurrentConfiguration.Database.AddListener<IDataRecordPropertyBeforeChange>(this);
            }
            else
            {
                InvertApplication.Log("A uFrameDatabase doesn't exist.");
            }
           
        }

        private static string DbRootPath
        {
            get
            {
                var path = Application.dataPath;
                if (path.EndsWith("Assets"))
                {
                    path = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                }
                return path;
            }
        }

        private uFrameDatabaseConfig GetConfig(TypeDatabase db, string title)
        {
            var config = db.GetSingle<uFrameDatabaseConfig>();

            if (config == null)
            {
                config = new uFrameDatabaseConfig
                {
                    CodeOutputPath = "Assets/Code",
                    Namespace = title,
                    BuildVersion = uFrameVersion.BuildVersion,
                    MajorVersion = uFrameVersion.MajorVersion,
                    MinorVersion = uFrameVersion.MinorVersion,
                };

                db.Add(config);
                db.Commit();
            }
            config.Database = db;
            config.Title = title;
            if (!Configurations.ContainsKey(config.Identifier))
            Configurations.Add(config.Identifier, config);
            return config;
        }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            //foreach (var item in Configurations.Values)
            //{
               
            //}
        }

        public Dictionary<string, uFrameDatabaseConfig> Configurations
        {
            get { return mConfigurations ?? (mConfigurations = new Dictionary<string, uFrameDatabaseConfig>()); }
            set { mConfigurations = value; }
        }

        public void RecordInserted(IDataRecord record)
        {
            if (SilentMode) return;
            InvertApplication.SignalEvent<IDataRecordInserted>(_ =>
            {
                if (_ != this) _.RecordInserted(record);
            });
        }

        public static bool SilentMode = false;
        public void RecordRemoved(IDataRecord record)
        {
            //TODO Check already invoked in JsonFileRecordManager and FastJsonFileRecordManager
            if (SilentMode) return;
            InvertApplication.SignalEvent<IDataRecordRemoved>(_ =>
            {
                if (_ != this) _.RecordRemoved(record);
            }); 
        }

        public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            if (SilentMode) return;
            InvertApplication.SignalEvent<IDataRecordPropertyChanged>(_ =>
            {
                if (_ != this) _.PropertyChanged(record, name, previousValue, nextValue);
            });
        }
        public void BeforePropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            if (SilentMode) return;
            InvertApplication.SignalEvent<IDataRecordPropertyBeforeChange>(_ =>
            {
                if (_ != this) _.BeforePropertyChanged(record, name, previousValue, nextValue);
            });
        }
        public void ChangeDatabase(IGraphConfiguration configuration)
        {
            var configRecord = configuration as IDataRecord;
            if (configRecord != null) CurrentDatabaseIdentifier = configRecord.Identifier;
            InvertApplication.Container = null;
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            ui.AddCommand(new ToolbarItem()
            {
                Title = CurrentConfiguration == null ? "Change Database" : "Database: " + CurrentConfiguration.Title,
                Command = new ChangeDatabaseCommand(),
                Position = ToolbarPosition.BottomLeft,
                Order = -1
            });
            //ui.AddCommand(new ToolbarItem()
            //{
            //    Title = "Save",
            //    Command = new SaveCommand(),
            //    Position = ToolbarPosition.Right
            //});
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
          
        }

        public void Execute(ChangeDatabaseCommand command)
        {
            Signal<IShowContextMenu>(_=>_.Show(null,command));

            
        }

        public void Execute(SaveCommand command)
        {
            Container.Resolve<IRepository>().Commit();
        }

        public void Execute(CreateDatabaseCommand command)
        {
            if (Directory.Exists(command.Name))
            {
                throw new Exception(string.Format("Database {0} already exists.", command.Name));
            }
            var dbDir = Directory.CreateDirectory(Path.Combine(DbRootPath, command.Name + ".db"));
            var db = new TypeDatabase(new JsonRepositoryFactory(dbDir.FullName));
            var config = GetConfig(db, command.Name);
            config.Namespace = command.Namespace;
            config.Title = command.Name;
            config.CodeOutputPath = command.CodePath;
            config.Namespace = command.Namespace ?? config.Namespace;
            config.FullPath = dbDir.FullName;
            config.Database = db;
            db.Commit();
            CurrentDatabaseIdentifier = config.Identifier;
            InvertApplication.Container = null;
            if(InvertApplication.Container!=null)
                InvertApplication.SignalEvent<INotify>(_ => _.Notify(command.Name + " Database " + " has been created!", NotificationIcon.Info));

        }

        public void Execute(EditDatabaseCommand command)
        {
            command.Configuration.Namespace = command.Namespace;
            command.Configuration.CodeOutputPath = command.CodePath;
            command.Configuration.Database.Commit();
            Signal<INotify>(_=>_.Notify(string.Format("Changes to {0} database were applied!", command.Configuration.Title),NotificationIcon.Info));
        }

        public void RecordRemoving(IDataRecord record)
        {
            if (SilentMode) return;
            InvertApplication.SignalEvent<IDataRecordRemoving>(_ =>
            {
                if (_ != this) _.RecordRemoving(record);
            }); 
        }

        public void ManagerRefreshed(IDataRecordManager manager)
        {
            InvertApplication.SignalEvent<IDataRecordManagerRefresh>(_ =>
            {
                if (_ != this) _.ManagerRefreshed(manager);
            }); 
        }
    }

    public interface IUpgradeDatabase
    {
        void UpgradeDatabase(uFrameDatabaseConfig item);
    }

    public interface IQueryImportable
    {
        void QueryImportList(List<FileInfo> files);
    }

    public class ExportUICommand : Command
    {
        
    }

    public class ImportCommand : Command
    {
        public string Filename { get; set; }
        public IGraphData Graph;
    }
    public class ExportGraphCommand : Command
    {
        public string Filename { get; set; }
        public IGraphData Graph;
    }
    public class ExportWorkspaceCommand : Command
    {
        public Workspace Workspace;
    }
    public class ExportDatabaseCommand : Command
    {
        public uFrameDatabaseConfig Database;
    }
    public class ImportExportSystem : DiagramPlugin
        , IToolbarQuery
        , IContextMenuQuery
        , IQueryImportable
        , IExecuteCommand<ExportGraphCommand>
        , IExecuteCommand<ExportWorkspaceCommand>
        , IExecuteCommand<ExportDatabaseCommand>
        , IExecuteCommand<ImportCommand>
    {
        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);

        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            ui.AddCommand(new ToolbarItem()
            {
                Title = "Export",Position = ToolbarPosition.BottomLeft,
                Command = new LambdaCommand("Export", () =>
                {
                    Signal<IShowContextMenu>(_ => _.Show(null, new ExportUICommand()));
                })
            });
            ui.AddCommand(new ToolbarItem()
            {
                Title = "Import",
                Position = ToolbarPosition.BottomLeft,
                Command = new ImportCommand()
                {

                },
            });
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
            var exportUI = obj.OfType<ExportUICommand>().FirstOrDefault();
            if (exportUI != null)
            {
                var ws = InvertApplication.Container.Resolve<WorkspaceService>();
                if (ws != null && ws.CurrentWorkspace != null)
                {
              
                    ui.AddCommand(new ContextMenuItem()
                    {
                        Title = "Export Database",
                        Group = "Export",
                        Command = new ExportDatabaseCommand()
                        {
                            Database = Container.Resolve<DatabaseService>().CurrentConfiguration
                        },
                    });
                    ui.AddCommand(new ContextMenuItem()
                    {
                        Title = "Export Workspace",
                        Group = "Export",
                        Command = new ExportWorkspaceCommand()
                        {
                            Workspace = ws.CurrentWorkspace
                        },
                    });
                    // ui.AddSeparator();
                    if (ws.CurrentWorkspace.CurrentGraph != null)
                    {
                        ui.AddCommand(new ContextMenuItem()
                        {
                            Title = "Export Current Graph",
                            Group = "Export",
                            Command = new ExportGraphCommand()
                            {
                                Graph = ws.CurrentWorkspace.CurrentGraph
                            },
                        });
                    }
                }
              

            }

            var diagram = obj.OfType<DiagramViewModel>().FirstOrDefault();
            if (diagram == null) return;
            //var graph = diagram.GraphData;
            //ui.AddCommand(new ContextMenuItem()
            //{
            //    Title = "Export this graph",
            //    Command = new LambdaCommand("Export Graph", () =>
            //    { 

            //    })
            //});


        }

        private string ExportItems(List<IDataRecord> items, IRepository graph, uFrameExport export)
        {
            var repo = graph as TypeDatabase;
            if (repo == null) throw new Exception("Couldn't export, must be TypeDatabase");
            var actualItems = items;

            var ids = items.Select(p => p.Identifier).ToArray();
            foreach (var record in graph.AllOf<IDataRecord>())
            {
                if (record is Workspace) continue;
                if (!record.ForeignKeys.Any()) continue;
                if (record.ForeignKeys.All(x => ids.Any(p=>p != null && x != null && (p == x || p.StartsWith(x)))))
                {
                    if (!actualItems.Contains(record))
                        actualItems.Add(record);
                }
            }
            actualItems.RemoveAll(p => p.Identifier.Contains(":"));

            export.Repositories = actualItems.GroupBy(p => p.GetType()).Select(p => new ExportedRepository()
            {
                Records = p.Select(x => new ExportedRecord()
                {
                    Data = InvertJsonExtensions.SerializeObject(x).ToString(true),
                    Identifier = x.Identifier
                }).ToList(),
                Type = p.Key.FullName
            }).ToList();
            var config = Container.Resolve<IGraphConfiguration>();
            var generators = InvertGraphEditor.GetAllFileGenerators(config,
                actualItems.ToArray(), true);
            export.CodeFiles = generators.Where(p => File.Exists(p.SystemPath)).Select(p => new ExportedCode()
            {
                Code = File.ReadAllText(p.SystemPath),
                RelativePath = p.AssetPath.Replace(config.CodeOutputPath, "")
            }).ToList();

            var contents = InvertJsonExtensions.SerializeObject(export).ToString(true);
            return contents;
        }

        public void Traverse(IDataRecord item, List<IDataRecord> items)
        {
            if (items.Contains(item)) return;
            items.Add(item);
            var treeItem = item as IDataHeirarchy;
            if (treeItem != null)
            foreach (var child in treeItem.ChildRecords)
            {
                Traverse(child, items);
            }
            
        }

        public void QueryImportList(List<FileInfo> files)
        {
            new DirectoryInfo(Application.dataPath);
        }

        public void Execute(ExportGraphCommand command)
        {
            var export = new uFrameExport();
            var items = new List<IDataRecord>();
            Traverse(command.Graph, items);
            var contents = ExportItems(items, command.Graph.Repository, export);
            var fileSave = new ShowSaveFileDialog()
            {
                Extension = "ufdata",
                DefaultName = command.Graph.Name
            };
            Execute(fileSave);
            if (fileSave.Result != null)
            File.WriteAllText(fileSave.Result, contents);
        }

        public void Execute(ExportWorkspaceCommand command)
        {
            var export = new uFrameExport();
            var items = new List<IDataRecord>();
            Traverse(command.Workspace, items);
            var contents = ExportItems(items, command.Workspace.Repository, export);
            var fileSave = new ShowSaveFileDialog()
            {
                Extension = "ufdata",
                DefaultName = command.Workspace.Name
            };
            Execute(fileSave);
            if (fileSave.Result != null)
                File.WriteAllText(fileSave.Result, contents);
        }

        public string UpdateNamespace(string fileContents, string ns)
        {
            string resultString = null;
            try
            {
                resultString = Regex.Replace(fileContents, @"(namespace )([a-z0-9._]+?) +?(\{?)", "${1}" + ns + "${3}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            catch (ArgumentException ex)
            {
                // Syntax error in the regular expression
                resultString = fileContents;
            }
            return resultString;
        }

        public string UnEscape(string s)
        {
            StringBuilder sb = new StringBuilder();
            Regex r = new Regex("\\\\[abfnrtv?\"'\\\\]|\\\\[0-3]?[0-7]{1,2}|\\\\u[0-9a-fA-F]{4}|.");
            MatchCollection mc = r.Matches(s, 0);

            foreach (Match m in mc)
            {
                if (m.Length == 1)
                {
                    sb.Append(m.Value);
                }
                else
                {
                    if (m.Value[1] >= '0' && m.Value[1] <= '7')
                    {
                        int i = 0;

                        for (int j = 1; j < m.Length; j++)
                        {
                            i *= 8;
                            i += m.Value[j] - '0';
                        }

                        sb.Append((char)i);
                    }
                    else if (m.Value[1] == 'u')
                    {
                        int i = 0;

                        for (int j = 2; j < m.Length; j++)
                        {
                            i *= 16;

                            if (m.Value[j] >= '0' && m.Value[j] <= '9')
                            {
                                i += m.Value[j] - '0';
                            }
                            else if (m.Value[j] >= 'A' && m.Value[j] <= 'F')
                            {
                                i += m.Value[j] - 'A' + 10;
                            }
                            else if (m.Value[j] >= 'a' && m.Value[j] <= 'f')
                            {
                                i += m.Value[j] - 'a' + 10;
                            }
                        }

                        sb.Append((char)i);
                    }
                    else
                    {
                        switch (m.Value[1])
                        {
                            case 'a':
                                sb.Append('\a');
                                break;
                            case 'b':
                                sb.Append('\b');
                                break;
                            case 'f':
                                sb.Append('\f');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'v':
                                sb.Append('\v');
                                break;
                            default:
                                sb.Append(m.Value[1]);
                                break;
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public void Execute(ImportCommand command)
        {
            var fileDialog = new ShowOpenFileDialog()
            {
                Extension = "ufdata",
                
                Title = "Import"
            };
            this.Execute(fileDialog);
            var file = fileDialog.Result;
            var uFrameExport = InvertJsonExtensions.DeserializeObject<uFrameExport>(File.ReadAllText(file));
            if (uFrameExport != null)
            {
                try
                {
                    DatabaseService.SilentMode = true;
                    var currentConfig = Container.Resolve<IGraphConfiguration>();
                    var repository = Container.Resolve<IRepository>() as TypeDatabase;
                    if (repository != null)
                    {
                        repository.Import(uFrameExport.Repositories);
                    }
                    foreach (var codeFile in uFrameExport.CodeFiles)
                    {
                        var path = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 7),
                            currentConfig.CodeOutputPath + codeFile.RelativePath);
                        var dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        File.WriteAllText(path, UpdateNamespace(codeFile.Code, currentConfig.Namespace));
                    }
                }
                finally
                {
                    DatabaseService.SilentMode = false;
                }
               
            }
        }

        public void Execute(ExportDatabaseCommand command)
        {
            var export = new uFrameExport();
            var items = command.Database.Repository.AllOf<IDataRecord>().Where(p=>!(p is uFrameDatabaseConfig)).ToList();
        
            var contents = ExportItems(items, command.Database.Repository, export);
            var fileSave = new ShowSaveFileDialog()
            {
                Extension = "ufdata",
                DefaultName = command.Database.Title
            };
            Execute(fileSave);
            if (fileSave.Result != null)
                File.WriteAllText(fileSave.Result, contents);
        }
    }

    public class uFrameExport
    {
        [JsonProperty]
        public List<ExportedRepository> Repositories { get; set; }


        [JsonProperty]
        public List<ExportedCode> CodeFiles { get; set; }




    }

    public class ExportedCode
    {
        [JsonProperty]
        public string RelativePath { get; set; }

        [JsonProperty]
        public string Code { get; set; }
    }

}
