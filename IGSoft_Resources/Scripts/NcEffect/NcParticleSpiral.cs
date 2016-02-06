// ----------------------------------------------------------------------------------
//
// FXMaker
// Modify by ismoon - 2012 - ismoonto@gmail.com
//
// reference source - http://wiki.unity3d.com/index.php/Particle_Spiral_Effect
//
// ----------------------------------------------------------------------------------


using UnityEngine; 
using System.Collections; 
 
/* -----------------------------------------------------------------------------------------------------
	-- ParticleSpiralEffect --
 
	This script spawns "spiral" particle effects, such as might be used for portals,
	whirlpools, galaxies, etc.  Note that the script can be adjusted to spawn any number of 
	particle systems for repeating spiral effects. Use this feature carefully so as not to 
	adversely impact framerate.  
 
	Using the script:
 
	- Assign this script to a transform or game object at the location where the spiral effect will
	be centered.  The spiral will be generated in the plane specified by the script transform's
	red axis (right, x) and blue axis (forward, z)  and centered around the green axis (up, y).  
 
	- Create a prefab that contains a particle system (emitter, animator, renderer).  Configure the
	prefab with your desired particle material, size and energy settings.  This script will override
	the Min/Max Emission settings based on your setting of m_nNumberOfArms and m_nParticlesPerArm.
 
	- Drag the prefab to the Particle Effect field of this script in the Inspector.
   ------------------------------------------------------------------------------------------------------ */
 
// OPTIONAL: 
// This structure mirrors the inspector settings for the effect and can be used in 
// combination with the resetEffect and getSettings methods to alter the effect
// appearance programatically.  Use of this structure is optional.
public class NcParticleSpiral : NcEffectBehaviour 
{ 
	// -- particleEffect --
	// This must be set to reference the prefab that contains the particle system components.
	// Those components can be adjusted as usual to achieve the desired appearance of the 
	// particles.  The parameters of the spiral effect itself are adjusted in the inspector using
	// public variables declared below.  Some of the spiral parameters override the particle
	// component values.  These are noted below.
	// Here are some recommendations for particle component settings to achieve desirable effects
	// in combination with this script:
	//	- Starting with default component settings
	//	- Particle emitter:
	//		Emit: OFF
	//		Simulate in Worldspace: OFF
	//		One Shot: ON
	//		Min/Max Emission: Overridden by script
	//	- Particle Renderer:
	//		Materials: Your favorite particle material 8^)
	//	- Particle Animator:
	//		Autodestruct: Overridden - ON (Prevents accumulation of used-up particle systems!)
	public		float		m_fDelayTime			= 0;
	protected	float		m_fStartTime;
	public		GameObject	m_ParticlePrefab;
 
	// The number of arms for the spiral effect.  
	public		int			m_nNumberOfArms			= 2;
 
	// The number of particles per spiral arm. 
	// Total number of particles in effect = m_nNumberOfArms * numberParticlesPerArm  
	public		int			m_nParticlesPerArm		= 100;
 
	// The separation between particles.
	public		float		m_fParticleSeparation	= 0.05f;
 
 	// The distance between the turns of the spiral.
	public		float		m_fTurnDistance			= 0.5f;
 
	// The vertical distance between turns of the spiral.
	// Useful to create a 3D spiral like a whirlpool or a
	// ummm.... corkscrew.  ;^)
	public		float		m_fVerticalTurnDistance = 0;
 
	// Creates a "hole" in the center of the spiral.  
	//	0 => spiral arms meet at center point.
	//	>0 => increase center hole size.
	// <0 => arms will start before the center and cross there.
	public		float		m_fOriginOffset			= 0.0f;
 
	// Rotation speed for the effect.
	// Spiral rotation only works when "Simulate in Worldspace" is
	// turned off for the emitter.
	public		float		m_fTurnSpeed			= 0;
 
	// Vary the particle lifetime along the spiral arms.  
	// A positive value will cause the particles to fade to the origin.
	// A negative value will cause the particles to fade to their tips.
	// CAUTION:  Use negative values carefully as they can cause
	// particle system accumulation depending on the Spawn Rate and
	// Min/Max Energy settings.
	public		float		m_fFadeValue			= 0;
 
	// Vary the particle size along the spiral arms.
	// A positive value will cause the particles to be larger at the origin, smaller at the tips.
	// A negative value will cause the particles to be smaller at the origin, larger at the tips.
	public		float		m_fSizeValue			= 0;
 
	// How many particle systems to spawn.  "Infinite" by default.
	public		int			m_nNumberOfSpawns	= 9999999;
 
	// How often a new effect should be spawned.  In seconds.  This setting
	// works closely with the particle emitter Min/Max Energy settings to 
	// achieve your desired effect.
	public		float		m_fSpawnRate		= 5.0f;
 
	// These constants define the min and max values used for the randomizeEffect method.
	protected	const int	Min_numArms			= 1;
	protected	const int	Max_numArms			= 10;
	protected	const int	Min_numPPA			= 20;
	protected	const int	Max_numPPA			= 60;
	protected	const float	Min_partSep			= -0.3f;
	protected	const float	Max_partSep			= 0.3f;
	protected	const float	Min_turnDist		= -1.5f;
	protected	const float	Max_turnDist		= 1.5f;
	protected	const float	Min_vertDist		= 0.0f;
	protected	const float	Max_vertDist		= 0.5f;
	protected	const float	Min_originOffset	= -3.0f;
	protected	const float	Max_originOffset	= 3.0f;
	protected	const float	Min_turnSpeed		= -180.0f;
	protected	const float	Max_turnSpeed		= 180.0f;
	protected	const float	Min_fade			= -1.0f;
	protected	const float	Max_fade			= 1.0f;
	protected	const float	Min_size			= -2.0f;
	protected	const float	Max_size			= 2.0f;
 
 	public struct SpiralSettings
	{
		public int		numArms;  			// number of spiral arms
		public int		numPPA;  			// number of particles per arm
		public float	partSep;			// separation between particles
		public float	turnDist;			// distance between spiral turns
		public float	vertDist;			// vertical turn distance
		public float	originOffset;		// size of hole in middle of spiral
		public float	turnSpeed;			// speed that spiral rotates.
		public float	fade;				// fade particles along the arms
		public float	size;				// change particle size along arms
	}	

	/* ------------------------------------------------------------------------------------------------------*/
 
	// Time at which the last spawn occurred.  Defaults to a "smallish" number
	// so the first effect appears more or less immediately.  
	private float timeOfLastSpawn = -1000.0f;
 
	// Count of effects spawned so far.
	private int spawnCount = 0;
 
	// Total number of particles.
	private int totParticles;
 
	// The settings for the effect as set when the effect is first created,
	// i.e. the default settings.
	private SpiralSettings defaultSettings;
 
	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if ((m_ParticlePrefab == null || m_ParticlePrefab.GetComponent<ParticleEmitter>() == null) && (GetComponent<ParticleEmitter>() == null))
			return "SCRIPT_EMPTY_LEGACYPARTICLEPREFAB";
		return "";	// no error
	}
#endif

 	public override int GetAnimationState()
	{
		if ((enabled && IsActive(gameObject)))
		{
			if (GetEngineTime() < m_fStartTime + m_fDelayTime + 0.1f)
				return 1;
			return -1;
		}
		return -1;
	}

	public void RandomizeEditor()
	{
		m_nNumberOfArms			= Random.Range(Min_numArms, Max_numArms);
		m_nParticlesPerArm		= Random.Range(Min_numPPA, Max_numPPA);
		m_fParticleSeparation	= Random.Range(Min_partSep, Max_partSep);
		m_fTurnDistance			= Random.Range(Min_turnDist, Max_turnDist);
		m_fVerticalTurnDistance = Random.Range(Min_vertDist, Max_vertDist);
		m_fOriginOffset			= Random.Range(Min_originOffset, Max_originOffset);
		m_fTurnSpeed			= Random.Range(Min_turnSpeed, Max_turnSpeed);
		m_fFadeValue			= Random.Range(Min_fade, Max_fade);
		m_fSizeValue			= Random.Range(Min_size, Max_size);
	}

	// Loop Function --------------------------------------------------------------------
	void Start()
	{
		m_fStartTime = GetEngineTime();

		if (m_ParticlePrefab == null)
		{
			ParticleEmitter emitter = GetComponent<ParticleEmitter>();
			if (emitter == null)
				return;
			emitter.emit = false;
		}

		// Record the current Inspector settings as the defaults.
		defaultSettings = getSettings();
	}

	/* ------------------------------------------------------------------------------------------------------
		-- SpawnEffect --
 
		This function spawns a new particle effect system each time it's called.  The system 
		spawned is the prefab referenced by the public particleEffect variable.
	   ------------------------------------------------------------------------------------------------------- */
	void SpawnEffect()
	{
		// Instantiate the effect prefab.
		GameObject effectObject;
		if (m_ParticlePrefab != null)
		{
			effectObject = CreateGameObject(m_ParticlePrefab);
			if (effectObject == null)
				return;
			ChangeParent(transform, effectObject.transform, true, null);
		} else {
			effectObject = gameObject;
		}
 
		// Get the particle emitter from the new effect object.
		ParticleEmitter emitter = effectObject.GetComponent<ParticleEmitter>();
		if (emitter == null)
			return;
 
		emitter.emit			= false;
		emitter.useWorldSpace	= false;

		// Make sure autodestruct is on so that dead particles systems get destroyed.
		ParticleAnimator animator = emitter.transform.GetComponent<ParticleAnimator>();
		if (animator != null)
			animator.autodestruct = true;
 
		// Generate the particles.
		emitter.Emit(m_nNumberOfArms * m_nParticlesPerArm);
 
		// Extract the particles from the created emitter. 
		Particle[] p = emitter.particles; 
 
		// The rotation angle, in radians, between the arms of the spiral.
		float armSpacing = (2 * Mathf.PI) / m_nNumberOfArms;
 
		// Loop thru the arms...
		for (int j=0; j<m_nNumberOfArms; j++)
		{
			float r = 0;
			float theta = 0;
			float armRotation = j*armSpacing;
 
			// Loop thru the particles for this arm and place them.
			for (int i = 0;  i < m_nParticlesPerArm;  i++)
			{
				// This particle's index.
				int pX = j * m_nParticlesPerArm + i;
 
				// This is the equation for our spiral in polar coords....
				r = m_fOriginOffset + m_fTurnDistance * theta;
 
				// All particles are positioned wrt the local transform origin.
				Vector3 newPos = effectObject.transform.localPosition;
				//Vector3 newPos = effectObject.position;
 
				// Convert to Cartesian coords...
				newPos.x = newPos.x + r * Mathf.Cos(theta);
				newPos.z = newPos.z + r * Mathf.Sin(theta);
 
				// Rotate the particle about the origin to our desired position for this arm.
				float x = newPos.x * Mathf.Cos(armRotation) + newPos.z * Mathf.Sin(armRotation);
				float z = -newPos.x * Mathf.Sin(armRotation) + newPos.z * Mathf.Cos(armRotation);
				newPos.x = x;
				newPos.z = z;
 
				// Introduce the desired vertical offset.
				newPos.y = newPos.y + i * m_fVerticalTurnDistance;
 
				// Update the particle position.
				if (emitter.useWorldSpace)
					newPos = transform.TransformPoint(newPos);
				p[pX].position = newPos;
 
				// Bump to our next point on the curve.
				theta += m_fParticleSeparation;
 
				// Set particle lifetime to account for specified fade.
//				p[pX].energy = p[pX].energy - i * m_fFadeValue;
				if (m_fFadeValue != 0)
					p[pX].energy = p[pX].energy * (1 - Mathf.Abs(m_fFadeValue)) + p[pX].energy * Mathf.Abs(m_fFadeValue) * (m_fFadeValue < 0 ? m_nParticlesPerArm-i : i+1) / m_nParticlesPerArm;
 
				// Set particle size to account for specified variation.
//				p[pX].size = p[pX].size - i * m_fSizeValue;
				if (m_fSizeValue != 0)
					p[pX].size += Mathf.Abs(m_fSizeValue) * (m_fSizeValue < 0 ? m_nParticlesPerArm-i : i+1) / m_nParticlesPerArm;
 
			}		
		}
		// Update the actual particles.
		emitter.particles = p;
	}
 
	void Update()
	{
		if (GetEngineTime() < m_fStartTime + m_fDelayTime)
			return;

		// Spin the entire particle effect.
		if (m_fTurnSpeed != 0)
			this.transform.Rotate(this.transform.up * GetEngineDeltaTime() * (m_fTurnSpeed), Space.World);
	}
  
	void LateUpdate() 
	{
		if (GetEngineTime() < m_fStartTime + m_fDelayTime)
			return;

		// Check to see if it's time to spawn a new particle system.
		float timeSinceLastSpawn = GetEngineTime() - timeOfLastSpawn;
		if (m_fSpawnRate <= timeSinceLastSpawn && spawnCount < m_nNumberOfSpawns)
		{
			SpawnEffect();
			timeOfLastSpawn = GetEngineTime();
			spawnCount++;
		}
	}
 
	// Return the current settings for the effect.
	public SpiralSettings getSettings()
	{
		SpiralSettings result;
 
		result.numArms = m_nNumberOfArms;
		result.numPPA = m_nParticlesPerArm;
		result.partSep = m_fParticleSeparation;
		result.turnDist = m_fTurnDistance;
		result.vertDist = m_fVerticalTurnDistance;
		result.originOffset = m_fOriginOffset;
		result.turnSpeed = m_fTurnSpeed;
		result.fade = m_fFadeValue;
		result.size = m_fSizeValue;
 
		return result;
	}
 
	// Reset the effect to use the specified settings.
	// Except for the killCurrent option, this will only effect the
	// appearance of future spawns.
	public SpiralSettings resetEffect(bool killCurrent, SpiralSettings settings)
	{
		// If requested, destroy all current spawns of this effect.
		if (killCurrent)
			killCurrentEffects();
 
		// Assign the new settings and then spawn a new effect with these settings.
		m_nNumberOfArms = settings.numArms;  		
		m_nParticlesPerArm = settings.numPPA;  		
		m_fParticleSeparation = settings.partSep;		
		m_fTurnDistance = settings.turnDist;		
		m_fVerticalTurnDistance = settings.vertDist;		
		m_fOriginOffset = settings.originOffset;	
		m_fTurnSpeed = settings.turnSpeed;	
		m_fFadeValue = settings.fade;			
		m_fSizeValue = settings.size;				
 
		SpawnEffect();
		timeOfLastSpawn = GetEngineTime();
		spawnCount++;
 
		return getSettings();
	}
 
	// Reset the particle effect to its Inspector established defaults.
	public SpiralSettings resetEffectToDefaults(bool killCurrent)
	{
		return resetEffect(killCurrent, defaultSettings);
	}
 
	// Randomize the settings and return the new values.
	public SpiralSettings randomizeEffect(bool killCurrent)
	{
		// If requested, destroy all current spawns of this effect.
		if (killCurrent)
			killCurrentEffects();
 
		// Assign the new random settings and then spawn a new effect with these settings.
		RandomizeEditor();
 
		SpawnEffect();
		timeOfLastSpawn = GetEngineTime();
		spawnCount++;
 
		return getSettings();
	}
 
	// Kill all current spawns of the effect.
	private void killCurrentEffects()
	{
		// Loop thru the particle emitter children of this object.  Each one is a particle effect system
		// we want to destroy.
		ParticleEmitter[] emitters = this.transform.GetComponentsInChildren<ParticleEmitter>();
		foreach (ParticleEmitter emitter in emitters)
		{
			Debug.Log("resetEffect killing: " + emitter.name);
			// Make sure autodestruct is on.
			ParticleAnimator animator = emitter.transform.GetComponent<ParticleAnimator>();
			if (animator != null)
				animator.autodestruct = true;
			// Now loop thru the particles and set their energies to a small number.  The effect will
			// subsequently autodestruct.  I originally tried setting the energy to zero, but in that
			// case they did *not* autodestruct.
			// I originally tried simply doing a Destroy on the emitter, but got threatening runtime messages.
			Particle[] p  = emitter.particles;
			for (int i=0; i < p.Length;  i++)
				p[i].energy = 0.1f;
			emitter.particles = p;
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime		/= fSpeedRate;
		m_fTurnSpeed		*= fSpeedRate;
	}

}
