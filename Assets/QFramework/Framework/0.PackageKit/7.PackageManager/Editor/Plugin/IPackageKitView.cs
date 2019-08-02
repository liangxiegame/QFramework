

namespace QF.Editor
{
	public interface IPackageKitView 
	{
		IQFrameworkContainer Container { get; set; }
		/// <summary>
		/// 1 after 0
		/// </summary>
		int RenderOrder { get;}
		
		bool Ignore { get; }
		
		bool Enabled { get;}
		
		void Init(IQFrameworkContainer container);

		void OnUpdate();
		void OnGUI();

		void OnDispose();
	}
	
}