using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF.Json;
using QF;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class UndoSystem : DiagramPlugin,
        IDataRecordPropertyBeforeChange,
        IDataRecordInserted,
        IDataRecordRemoved,
        IExecuteCommand<UndoCommand>,
        IExecuteCommand<RedoCommand>,
        IExecuteCommand<TestyCommand>,
        IToolbarQuery,
        ICommandExecuting,
        ICommandExecuted,
        IKeyboardEvent
    {
        public string CurrentUndoGroupId = null;
        private List<UndoItem> _undoItems;

        public List<UndoItem> UndoItems
        {
            get { return _undoItems ?? (_undoItems = new List<UndoItem>()); }
            set { _undoItems = value; }
        }

        public void BeforePropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            if (IsUndoRedo) return;
            if (CurrentUndoGroupId == null || record is UndoItem || record is RedoItem) return;
            try
            {
                var undoItem = new UndoItem();
                undoItem.Time = DateTime.Now;
                undoItem.Group = CurrentUndoGroupId;
                undoItem.DataRecordId = record.Identifier;
                undoItem.Data = InvertJsonExtensions.SerializeObject(record).ToString();
                undoItem.RecordType = record.GetType().AssemblyQualifiedName;
                undoItem.Type = UndoType.Changed;
                undoItem.Name = CurrentName;
                UndoItems.Add(undoItem);
            }
            catch (Exception ex)
            {

            }
        }

        public void RecordInserted(IDataRecord record)
        {
            if (IsUndoRedo) return;
            if (CurrentUndoGroupId == null || record is UndoItem || record is RedoItem) return;
            var undoItem = new UndoItem();
            undoItem.Time = DateTime.Now;
            undoItem.Group = CurrentUndoGroupId;
            undoItem.DataRecordId = record.Identifier;
            undoItem.Data = InvertJsonExtensions.SerializeObject(record).ToString();
            undoItem.RecordType = record.GetType().AssemblyQualifiedName;
            undoItem.Type = UndoType.Inserted;
            undoItem.Name = CurrentName;

            UndoItems.Add(undoItem);
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (IsUndoRedo) return;
            if (CurrentUndoGroupId == null || record is UndoItem || record is RedoItem) return;
            var undoItem = new UndoItem();
            undoItem.Time = DateTime.Now;
            undoItem.Group = CurrentUndoGroupId;
            undoItem.DataRecordId = record.Identifier;
            undoItem.Data = InvertJsonExtensions.SerializeObject(record).ToString();
            undoItem.RecordType = record.GetType().AssemblyQualifiedName;
            undoItem.Name = CurrentName;
            undoItem.Type = UndoType.Removed;
            UndoItems.Add(undoItem);
        }
        public bool IsUndoRedo { get; set; }
        public void Execute(UndoCommand command)
        {
            Repository = Container.Resolve<IRepository>();
            var undoGroup = Repository.All<UndoItem>().GroupBy(p => p.Group).LastOrDefault();
            if (undoGroup == null) return;
            IsUndoRedo = true;
            try
            {
                foreach (var undoItem in undoGroup)
                {
                    // Create redo item
                    var redoItem = new RedoItem();
                    redoItem.Data = undoItem.Data;
                    redoItem.Group = undoItem.Group;
                    redoItem.DataRecordId = undoItem.DataRecordId;
                    redoItem.Name = undoItem.Name;
                    redoItem.Time = undoItem.Time;
                    redoItem.Type = undoItem.Type;
                    redoItem.RecordType = undoItem.RecordType;
                    redoItem.UndoData = InvertJsonExtensions.SerializeObject(undoItem).ToString();

                    if (undoItem.Type == UndoType.Inserted)
                    {
                        var record = Repository.GetById<IDataRecord>(undoItem.DataRecordId);
                        redoItem.Data = InvertJsonExtensions.SerializeObject(record).ToString();
                        Repository.Remove(record);
                        redoItem.Type = UndoType.Removed;
                    }
                    else if (undoItem.Type == UndoType.Removed)
                    {

                        var obj =
                            InvertJsonExtensions.DeserializeObject(Type.GetType(undoItem.RecordType),
                                JSON.Parse(undoItem.Data).AsObject) as IDataRecord;
                        Repository.Add(obj);
                        redoItem.Type = UndoType.Inserted;
                        redoItem.Data = InvertJsonExtensions.SerializeObject(obj).ToString();
                    }
                    else
                    {
                        var record = Repository.GetById<IDataRecord>(undoItem.DataRecordId);
                        // We don't want to signal any events on deserialization
                        record.Repository = null;
                        redoItem.Data = InvertJsonExtensions.SerializeObject(record).ToString();
                        InvertJsonExtensions.DeserializeExistingObject(record, JSON.Parse(undoItem.Data).AsObject);
                        record.Changed = true;
                        record.Repository = Repository;

                    }
                    Repository.Remove(undoItem);
                    Repository.Add(redoItem);
                }
            }
            catch (Exception ex)
            {
                // If we don't catch the exception IsUndoRedo won't be set back to fals causing cascading issues
            }
            IsUndoRedo = false;
            Repository.Commit();
        }

        public void Execute(RedoCommand command)
        {
            IsUndoRedo = true;
            Repository = Container.Resolve<IRepository>();
            var redoGroup = Repository.All<RedoItem>().GroupBy(p => p.Group).LastOrDefault();
            if (redoGroup == null) return;
            foreach (var redoItem in redoGroup)
            {
                // Create redo item
                var undoItem = InvertJsonExtensions.DeserializeObject(typeof(UndoItem), JSON.Parse(redoItem.UndoData)) as UndoItem;


                if (redoItem.Type == UndoType.Inserted)
                {
                    var record = Repository.GetById<IDataRecord>(redoItem.DataRecordId);

                    Repository.Remove(record);

                }
                else if (redoItem.Type == UndoType.Removed)
                {

                    var obj = InvertJsonExtensions.DeserializeObject(Type.GetType(redoItem.RecordType), JSON.Parse(redoItem.Data).AsObject) as IDataRecord;
                    Repository.Add(obj);
                }
                else
                {
                    var record = Repository.GetById<IDataRecord>(redoItem.DataRecordId);
                    // We don't want to signal any events on deserialization
                    record.Repository = null;
                    InvertJsonExtensions.DeserializeExistingObject(record, JSON.Parse(redoItem.Data).AsObject);
                    record.Changed = true;
                    record.Repository = Repository;

                }
                Repository.Remove(redoItem);
                Repository.Add(undoItem);
            }
            IsUndoRedo = false;
            Repository.Commit();

        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            var repo = Container.Resolve<IRepository>();
            if (repo == null) return;
            var undoItem = repo.All<UndoItem>().LastOrDefault();
            if (undoItem != null)
            {
                ui.AddCommand(new ToolbarItem()
                {
                    Title = "Undo",
                    Command = new UndoCommand(),
                    Position = ToolbarPosition.BottomRight,
                    Order = -2
                });
            }
            var redoItem = repo.All<RedoItem>().LastOrDefault();
            if (redoItem != null)
            {
                ui.AddCommand(new ToolbarItem()
                {
                    Title = "Redo",
                    Command = new RedoCommand(),
                    Position = ToolbarPosition.BottomRight,
                    Order = -1

                });
            }


        }

        public void CommandExecuting(ICommand command)
        {
            if (command is UndoCommand) return;

            CurrentUndoGroupId = DateTime.Now.Hour.ToString() + DateTime.Now.Minute + DateTime.Now.Second.ToString();
            CurrentName = command.Title;
            UndoItems.Clear();
        }

        public string CurrentName { get; set; }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            Repository = container.Resolve<IRepository>();
        }

        public IRepository Repository { get; set; }

        public void CommandExecuted(ICommand command)
        {
            if (Repository == null) return;
            if (command is UndoCommand || command is RedoCommand) return;
            CurrentUndoGroupId = null;
            if (Repository != null)
            {
                foreach (var item in UndoItems)
                {
                    Repository.Add(item);
                }
                Repository.RemoveAll<RedoItem>();
               
            }
            var items = Repository.All<UndoItem>().Reverse();
            foreach (var item in items.Skip(20))
                Repository.Remove(item);

            Repository.Commit();
        }

        public void Execute(TestyCommand command)
        {
            var sb = new StringBuilder();
            foreach (var item in InvertApplication.Plugins.OrderBy(p => p.Title))
            {
                sb.AppendLine(item.Title);
            }
            InvertApplication.Log(sb.ToString());
        }

        public bool KeyEvent(KeyCode keyCode, ModifierKeyState state)
        {
            if (state.Ctrl && keyCode == KeyCode.Z)
            {
                InvertApplication.Execute(new UndoCommand());
                return true;
            }
            if (state.Ctrl && keyCode == KeyCode.Y)
            {
                InvertApplication.Execute(new RedoCommand());
                return true;
            }
            return false;
        }
    }
}
