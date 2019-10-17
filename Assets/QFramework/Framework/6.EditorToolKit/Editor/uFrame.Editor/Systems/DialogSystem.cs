using QF.GraphDesigner;

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