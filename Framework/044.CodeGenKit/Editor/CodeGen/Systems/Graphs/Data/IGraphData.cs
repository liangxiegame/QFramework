using Invert.Data;

namespace QFramework.CodeGen
{
    public interface IGraphData : IItem, IDataHeirarchy
	{ 
		string Namespace {get;}
	}
}