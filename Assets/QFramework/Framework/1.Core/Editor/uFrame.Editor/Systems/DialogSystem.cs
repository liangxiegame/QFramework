using QF.GraphDesigner;
using QF;
using UnityEditor;

public class DialogSystem : DiagramPlugin
    ,IExecuteCommand<ShowSaveFileDialog>
    ,IExecuteCommand<ShowOpenFileDialog>
    ,IExecuteCommand<ShowOpenFolderDialog>
    ,IExecuteCommand<ShowSaveFolderDialog>
{
    public override void Initialize(QFrameworkContainer container)
    {
        base.Initialize(container);
    }

    public void Execute(ShowSaveFileDialog command)
    {
        command.Result = EditorUtility.SaveFilePanel(command.Title,command.Directory, command.DefaultName, command.Extension);
    }

    public void Execute(ShowOpenFileDialog command)
    {
        command.Result = EditorUtility.OpenFilePanel(command.Title, command.Directory, command.Extension);
    }

    public void Execute(ShowOpenFolderDialog command)
    {
        command.Result = EditorUtility.OpenFolderPanel(command.Title, command.Folder, command.DefaultName);
    }

    public void Execute(ShowSaveFolderDialog command)
    {
        command.Result = EditorUtility.SaveFolderPanel(command.Title, command.Folder, command.DefaultName);
    }
}

public class ShowSaveFileDialog : Command {
    public string Result { get; set; }
    public string DefaultName { get; set; }
    public string Extension { get; set; }
    public string Message { get; set; }
    public string Directory { get; set; }
}
public class ShowOpenFileDialog : Command {
    public string Directory { get; set; }
    public string[] Filters { get; set; }
    public string Result { get; set; }
    public string Extension { get; set; }
}
public class ShowOpenFolderDialog : Command {
    public string Result { get; set; }
    public string Folder { get; set; }
    public string DefaultName { get; set; }
}
public class ShowSaveFolderDialog : Command {
    public string Result { get; set; }
    public string Folder { get; set; }
    public string DefaultName { get; set; }
}