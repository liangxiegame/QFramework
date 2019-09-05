using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using QF.GraphDesigner;
using Invert.Data;
using QF;
using Invert.uFrame.Editor;
using UnityEditor;
using UnityEngine;

public class UnityGraphData: ScriptableObject, IGraphData,  IItem
    
{
    [SerializeField, HideInInspector]
    public string _jsonData;

	public IEnumerable<IDataRecord> ChildRecords {
		get {
			yield break;
		}}
    public IGraphData Graph
    {
        get
        {
            return _graph ?? (_graph = new InvertGraph()
            {

                Name = this.name
            });
        }
        set { _graph = value; }
    }

    public UnityGraphData()
    {

    }

  
    [NonSerialized]
    private IGraphData _graph;



    public bool Errors
    {
        get { return Graph.Errors; }
        set { Graph.Errors = value; }
    }

    public Exception Error
    {
        get { return Graph.Error; }
        set { Graph.Error = value; }
    }

    //public string AssetPath
    //{
    //    get { return Graph.AssetPath; }
    //}

    //public string AssetDirectory
    //{
    //    get { return Graph.AssetDirectory; }
    //}

    public string AssetPath { get; private set; }
    public string AssetDirectory { get; private set; }

    public string SystemDirectory
    {
        get { return Graph.SystemDirectory; }
    }

    public string SystemPath
    {
        get
        {
            return Graph.SystemPath;
            //return InvertGraphEditor.Platform.GetAssetPath(this);
        }
        set { Graph.SystemPath = value; }
    }

    public string Directory
    {
        get { return System.IO.Path.GetDirectoryName(SystemPath); }

    }

    public IGraphFilter[] FilterStack { get; set; }
    public bool IsDirty { get; set; }

    public IEnumerable<IGraphItem> AllGraphItems
    {
        get { return Graph.AllGraphItems; }
    }


    public string Namespace { get; set; }


    public void Save()
    {
        //Graph.Save();
    }

    public void PushFilter(IGraphFilter filter)
    {
        
    }

    public void PopToFilter(IGraphFilter filter1)
    {
      
    }

    public void PopToFilterById(string filterId)
    {
  
    }

    public void PopFilter()
    {
        
    }

    public IGraphFilter CurrentFilter
    {
        get { return Graph.CurrentFilter; }
    }



    public IRepository Repository { get; set; }

    public string Identifier
    {
        set { Graph.Identifier = value; }
        get { return Graph.Identifier; }
    }

    public bool Changed { get; set; }
    public IEnumerable<string> ForeignKeys
    {
        get { yield break; }
    }


    //public ElementDiagramSettings Settings
    //{
    //    //set { Graph.Settings = value; }
    //    get { return Graph.Settings; }
    //}

    private string _name;
    private List<IGraphItemEvents> _listeners;

    public virtual string Name
    {
        get
        {
#if UNITY_EDITOR
            
            return this.name;
#endif

            return (_name = Regex.Replace(this.name, "[^a-zA-Z0-9_.]+", "")); ;
        }
        set
        {
#if UNITY_EDITOR
            this.name = value;
            EditorUtility.SetDirty(this);
#endif
            _name = value;
        }
    }

    public string Version
    {
        set { Graph.Version = value; }
        get { return Graph.Version; }
    }

    public int RefactorCount
    {
        set { Graph.RefactorCount = value; }
        get { return Graph.RefactorCount; }
    }

    public virtual IEnumerable<IDiagramNode> NodeItems
    {
        get { return Graph.NodeItems; }
    }


    public virtual IGraphFilter RootFilter
    {
        set { Graph.RootFilter = value; }
        get { return Graph.RootFilter; }
    }

    public virtual IGraphFilter CreateDefaultFilter(string identifier = null)
    {
        return Graph.CreateDefaultFilter();
    }


    public void CleanUpDuplicates()
    {
        Graph.CleanUpDuplicates();
    }

    public List<ErrorInfo> Validate()
    {
        return null;
    }


    




    public bool Precompiled
    {
        get { return Graph.Precompiled; }
        set { Graph.Precompiled = value; }
    }



    public void AddConnection(IConnectable output, IConnectable input)
    {
        Graph.AddConnection(output, input);
    }

    public void AddConnection(string output, string input)
    {
        Graph.AddConnection(output,input);
    }

    public void RemoveConnection(IConnectable output, IConnectable input)
    {
        Graph.RemoveConnection(output, input);
    }

    public void ClearOutput(IConnectable output)
    {
        Graph.ClearOutput(output);
    }

    public void ClearInput(IConnectable input)
    {
        Graph.ClearInput(input);
    }

    public string Title
    {
        get { return this.name; }
    }

    public string Group
    {
        get { return "Graphs"; }
    }

    public string SearchTag
    {
        get { return this.name; }
    }

    public string Description { get; set; }





}