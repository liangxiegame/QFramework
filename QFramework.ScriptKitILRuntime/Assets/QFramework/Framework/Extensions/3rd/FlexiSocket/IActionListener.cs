

namespace QFramework
{ 

	public interface IActionListener
	{


		void OnAction(PTSimulateAction action);
	}

	public enum PTSimulateAction
	{
		LEFT,
		RIGHT,
		UP,
		DOWN

	}
}