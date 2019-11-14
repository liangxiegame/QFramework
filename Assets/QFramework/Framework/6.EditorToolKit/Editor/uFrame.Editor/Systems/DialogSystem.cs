using QF.GraphDesigner;

public class ShowSaveFileDialog : Command {


    public string Result { get; set; }
    public string DefaultName { get; set; }
    public string Extension { get; set; }
}
public class ShowOpenFileDialog : Command {
    public string Result { get; set; }
    public string Extension { get; set; }
}