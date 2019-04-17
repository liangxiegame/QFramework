using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class UICallback : MonoBehaviour 
{

	public LuaFunction DropCallback
	{
		get { return m_DropCb;}
		set { m_DropCb = value;}
	}

	private LuaFunction m_DropCb = null;
}
