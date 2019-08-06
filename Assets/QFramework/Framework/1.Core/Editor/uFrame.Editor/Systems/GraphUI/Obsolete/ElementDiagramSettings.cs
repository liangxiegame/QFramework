using QF.GraphDesigner;
using QF;
using QF.Json;
using UnityEngine;

public class ElementDiagramSettings : IJsonObject
{
    public void Serialize(JSONClass cls)
    {
        cls.Add("CodeGenDisabled", new JSONData(CodeGenDisabled));
        cls.Add("SnapSize", new JSONData(_snapSize));
        cls.Add("Snap", new JSONData(_snap));
        cls.Add("CodePathStrategyName", new JSONData(_codePathStrategyName));
        cls.Add("GridLinesColor", SerializeColor(GridLinesColor));
        cls.Add("GridLinesColorSecondary", SerializeColor(GridLinesColorSecondary));
        cls.Add("AssociationLinkColor", SerializeColor(_associationLinkColor));
        cls.Add("DefinitionLinkColor", SerializeColor(_definitionLinkColor));
        cls.Add("InheritanceLinkColor", SerializeColor(_inheritanceLinkColor));
        cls.Add("SceneManagerLinkColor", SerializeColor(_sceneManagerLinkColor));
        cls.Add("SubSystemLinkColor", SerializeColor(_subSystemLinkColor));
        cls.Add("TransitionLinkColor", SerializeColor(_transitionLinkColor));
        cls.Add("ViewLinkColor", SerializeColor(_viewLinkColor));
        
        cls.Add("RootNamespace", new JSONData(RootNamespace));
    }

    public bool CodeGenDisabled { get; set; }

    public JSONNode SerializeColor(Color color)
    {
        var cls = new JSONClass
        {
            {"r", new JSONData(color.r)},
            {"g", new JSONData(color.g)},
            {"b", new JSONData(color.b)},
            {"a", new JSONData(color.a)}
        };
        return cls;
    }

    public string RootNamespace
    {
        get { return _rootNamespace; }
        set { _rootNamespace = value; }
    }

    public Color DeserializeColor(JSONNode color, Color def)
    {
        if (color == null) return def;
        return new Color(color["r"].AsFloat, color["g"].AsFloat, color["b"].AsFloat, color["a"].AsFloat);
    }
    public Color DeserializeColor(JSONNode color)
    {
        if (color == null) return Color.gray;
        return new Color(color["r"].AsFloat, color["g"].AsFloat, color["b"].AsFloat, color["a"].AsFloat);
    }
    public void Deserialize(JSONClass cls)
    {
        if (cls["CodeGenDisabled"] != null)
        {
            CodeGenDisabled = cls["CodeGenDisabled"].AsBool;
        }
        CodePathStrategyName = cls["CodePathStrategyName"];
        AssociationLinkColor = DeserializeColor(cls["AssociationLinkColor"]);
        DefinitionLinkColor = DeserializeColor(cls["DefinitionLinkColor"]);
        InheritanceLinkColor = DeserializeColor(cls["InheritanceLinkColor"]);
        SceneManagerLinkColor = DeserializeColor(cls["SceneManagerLinkColor"]);
        SubSystemLinkColor = DeserializeColor(cls["SubSystemLinkColor"]);
        TransitionLinkColor = DeserializeColor(cls["TransitionLinkColor"]);
        ViewLinkColor = DeserializeColor(cls["ViewLinkColor"]);
        SnapSize = cls["SnapSize"].AsInt;
        Snap = cls["Snap"].AsBool;
        GridLinesColor = DeserializeColor(cls["GridLinesColor"], new Color(0.271f, 0.271f, 0.271f));
        GridLinesColorSecondary = DeserializeColor(cls["GridLinesColorSecondary"], new Color(0.169f, 0.169f, 0.169f));

        if (cls["RootNamespace"] != null)
        {
            RootNamespace = cls["RootNamespace"].Value;
        }
    }

    public Color GridLinesColorSecondary
    {
        get { return _gridLinesColorSecondary; }
        set { _gridLinesColorSecondary = value; }
    }


    private Color _associationLinkColor = Color.white;

    private Color _definitionLinkColor = Color.cyan;

    private Color _inheritanceLinkColor = Color.green;

    private Color _sceneManagerLinkColor = Color.gray;

    private Color _subSystemLinkColor = Color.grey;


    private string _codePathStrategyName = "Default";

    private Color _transitionLinkColor = Color.yellow;

    private Color _viewLinkColor = Color.blue;

    private int _snapSize = 10;

    private ICodePathStrategy _codePathStrategy;

    private Color _gridLinesColor = new Color(0.271f, 0.271f, 0.271f);
    private Color _gridLinesColorSecondary = new Color(0.169f, 0.169f, 0.169f);
    private bool _generateDefaultBindings = true;
    private string _rootNamespace = string.Empty;
    private bool _snap = true;

    public Color AssociationLinkColor
    {
        get { return _associationLinkColor; }
        set { _associationLinkColor = value; }
    }

    public Color DefinitionLinkColor
    {
        get { return _definitionLinkColor; }
        set { _definitionLinkColor = value; }
    }

    public Color InheritanceLinkColor
    {
        get { return _inheritanceLinkColor; }
        set { _inheritanceLinkColor = value; }
    }

    public Color SceneManagerLinkColor
    {
        get { return _sceneManagerLinkColor; }
        set { _sceneManagerLinkColor = value; }
    }

    public Color SubSystemLinkColor
    {
        get { return _subSystemLinkColor; }
        set { _subSystemLinkColor = value; }
    }

    public Color TransitionLinkColor
    {
        get { return _transitionLinkColor; }
        set { _transitionLinkColor = value; }
    }

    public Color ViewLinkColor
    {
        get { return _viewLinkColor; }
        set { _viewLinkColor = value; }
    }

    public int SnapSize
    {
        get { return 12; }
        set { _snapSize = value; }
    }

    public bool Snap
    {
        get { return _snap; }
        set { _snap = value; }
    }

    public string CodePathStrategyName
    {
        get { return string.IsNullOrEmpty(_codePathStrategyName) ? "Default" : _codePathStrategyName; }
        set { _codePathStrategyName = value;
            _codePathStrategy = null;
        }
    }

   
    public Color GridLinesColor
    {
        get { return _gridLinesColor; }
        set { _gridLinesColor = value; }
    }

    


    public JSONNode Serialize()
    {
        return new JSONClass();
    }


}