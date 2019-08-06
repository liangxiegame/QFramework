using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using QF.GraphDesigner;
using Invert.Data;
using QF;

namespace QF.GraphDesigner
{
    public interface IGraphData : IItem, IDataRecord, IDataHeirarchy
	{
		string SystemPath {get;set;}
		string SystemDirectory {get;}
		//ElementDiagramSettings Settings {get;}
		IEnumerable<IGraphItem> AllGraphItems {get;}
		IEnumerable<IDiagramNode> NodeItems {get;}
		IGraphFilter CurrentFilter {get;}
		string Name {get;set;}
    	string Namespace {get;}


        int RefactorCount { get; set; }
    
        string Version { get; set; }

        // Filters
        IGraphFilter RootFilter { get; set; }

        bool Errors { get; set; }
        Exception Error { get; set; }

        bool Precompiled { get; set; }

        string Directory { get;  }
        IGraphFilter[] FilterStack { get; set; }
        bool IsDirty { get; set; }


        //IEnumerable<ConnectionData> Connections { get; }
        void AddConnection(IConnectable output, IConnectable input);
        void AddConnection(string output, string input);
        void RemoveConnection(IConnectable output, IConnectable input);
        void ClearOutput(IConnectable output);
        void ClearInput(IConnectable input); 
        IGraphFilter CreateDefaultFilter(string identifier = null);
        void CleanUpDuplicates();
     
        void PushFilter(IGraphFilter filter);
        void PopToFilter(IGraphFilter filter1);
        void PopToFilterById( string filterId);
        void PopFilter();
    }
}