// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public	class NgEnum
{
	// 3D ------------------------------------------------------------------
	public	enum AXIS				{X=0, Y, Z};
	public	enum TRANSFORM			{POSITION=0, ROTATION, SCALE};
	public	enum PREFAB_TYPE		{All, ParticleSystem, LegacyParticle, NcSpriteFactory};		// SHURIKEN

	public	static	string[]		m_TextureSizeStrings	= {"32", "64", "128", "256", "512", "1024", "2048", "4096"};
	public	static	int[]			m_TextureSizeIntters	= {32, 64, 128, 256, 512, 1024, 2048, 4096};
}
