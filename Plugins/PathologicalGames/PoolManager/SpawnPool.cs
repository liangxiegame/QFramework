/// <Licensing>
/// � 2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathologicalGames
{
    
    /// <description>
    /// Online Docs: 
    ///     http://docs.poolmanager2.path-o-logical.com/code-reference/spawnpool
    ///     
    ///	A special List class that manages object pools and keeps the scene 
    ///	organized.
    ///	
    ///  * Only active/spawned instances are iterable. Inactive/despawned
    ///    instances in the pool are kept internally only.
    /// 
    ///	 * Instanciated objects can optionally be made a child of this GameObject
    ///	   (reffered to as a 'group') to keep the scene hierachy organized.
    ///		 
    ///	 * Instances will get a number appended to the end of their name. E.g. 
    ///	   an "Enemy" prefab will be called "Enemy(Clone)001", "Enemy(Clone)002", 
    ///	   "Enemy(Clone)003", etc. Unity names all clones the same which can be
    ///	   confusing to work with.
    ///		   
    ///	 * Objects aren't destroyed by the Despawn() method. Instead, they are
    ///	   deactivated and stored to an internal queue to be used again. This
    ///	   avoids the time it takes unity to destroy gameobjects and helps  
    ///	   performance by reusing GameObjects. 
    ///		   
    ///  * Two events are implimented to enable objects to handle their own reset needs. 
    ///    Both are optional.
    ///      1) When objects are Despawned BroadcastMessage("OnDespawned()") is sent.
    ///		 2) When reactivated, a BroadcastMessage("OnRespawned()") is sent. 
    ///		    This 
    /// </description>
    [AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
    public sealed class SpawnPool : MonoBehaviour, IList<Transform>
    {
        #region Inspector Parameters
        /// <summary>
        /// Returns the name of this pool used by PoolManager. This will always be the
        /// same as the name in Unity, unless the name contains the work "Pool", which
        /// PoolManager will strip out. This is done so you can name a prefab or
        /// GameObject in a way that is development friendly. For example, "EnemiesPool" 
        /// is easier to understand than just "Enemies" when looking through a project.
        /// </summary>
        public string poolName = "";

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's scale.
        /// </summary>
        public bool matchPoolScale = false;

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's layer.
        /// </summary>
        public bool matchPoolLayer = false;

        /// <summary>
        /// If True, do not reparent instances under the SpawnPool's Transform.
        /// </summary>
        public bool dontReparent = false;
		
		/// <summary>
        /// If true, the Pool's group, GameObject, will be set to Unity's 
        /// Object.DontDestroyOnLoad()
        /// </summary>
        public bool dontDestroyOnLoad
		{
			get
			{
				return this._dontDestroyOnLoad;
			}
			
			set
			{
				this._dontDestroyOnLoad = value;
				
				if (this.group != null)
					Object.DontDestroyOnLoad(this.group.gameObject);
			}
		}
        public bool _dontDestroyOnLoad = false;  // Property backer and used by GUI.
		
        /// <summary>
        /// Print information to the Unity Console
        /// </summary>
        public bool logMessages = false;

        /// <summary>
        /// A list of PreloadDef options objects used by the inspector for user input
        /// </summary>
        public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

        /// <summary>
        /// Used by the inspector to store this instances foldout states.
        /// </summary>
        public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
        #endregion Inspector Parameters



        #region Public Code-only Parameters
        /// <summary>
        /// The time in seconds to stop waiting for particles to die.
        /// A warning will be logged if this is triggered.
        /// </summary>
        public float maxParticleDespawnTime = 300;

        /// <summary>
        /// The group is an empty game object which will be the parent of all
        /// instances in the pool. This helps keep the scene easy to work with.
        /// </summary>
        public Transform group { get; private set; }

        /// <summary>
        /// Returns the prefab of the given name (dictionary key)
        /// </summary>
        public PrefabsDict prefabs = new PrefabsDict();

        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        /// <summary>
        /// Readonly access to prefab pools via a dictionary<string, PrefabPool>.
        /// </summary>
        public Dictionary<string, PrefabPool> prefabPools
        {
            get
            {
                var dict = new Dictionary<string, PrefabPool>();

                for (int i = 0; i < this._prefabPools.Count; i++)
                    dict[this._prefabPools[i].prefabGO.name] = this._prefabPools[i];

                return dict;
            }
        }
        #endregion Public Code-only Parameters



        #region Private Properties
        private List<PrefabPool> _prefabPools = new List<PrefabPool>();
        internal List<Transform> _spawned = new List<Transform>();
        #endregion Private Properties



        #region Constructor and Init
        private void Awake()
        {
            // Make this GameObject immortal if the user requests it.
            if (this._dontDestroyOnLoad) Object.DontDestroyOnLoad(this.gameObject);

            this.group = this.transform;

            // Default name behavior will use the GameObject's name without "Pool" (if found)
            if (this.poolName == "")
            {
                // Automatically Remove "Pool" from names to allow users to name prefabs in a 
                //   more development-friendly way. E.g. "EnemiesPool" becomes just "Enemies".
                //   Notes: This will return the original string if "Pool" isn't found.
                //          Do this once here, rather than a getter, to avoide string work
                this.poolName = this.group.name.Replace("Pool", "");
                this.poolName = this.poolName.Replace("(Clone)", "");
            }


            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));

            // Only used on items defined in the Inspector
            for (int i = 0; i < this._perPrefabPoolOptions.Count; i++)
            {
                if (this._perPrefabPoolOptions[i].prefab == null)
                {
                    Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' " +
                              "contains a PrefabPool with no prefab reference. Skipping.",
                               this.poolName));
                    continue;
                }

                // Init the PrefabPool's GameObject cache because it can't do it.
                //   This is only needed when created by the inspector because the constructor
                //   won't be run.
                this._perPrefabPoolOptions[i].inspectorInstanceConstructor();
                this.CreatePrefabPool(this._perPrefabPoolOptions[i]);
            }

            // Add this SpawnPool to PoolManager for use. This is done last to lower the 
            //   possibility of adding a badly init pool.
            PoolManager.Pools.Add(this);
        }


        /// <summary>
        /// Runs when this group GameObject is destroyed and executes clean-up
        /// </summary>
        private void OnDestroy()
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));

            PoolManager.Pools.Remove(this);

            this.StopAllCoroutines();

            // We don't need the references to spawns which are about to be destroyed
            this._spawned.Clear();

            // Clean-up
            foreach (PrefabPool pool in this._prefabPools) pool.SelfDestruct();

            // Probably overkill, and may not do anything at all, but...
            this._prefabPools.Clear();
            this.prefabs._Clear();
        }


        /// <summary>
        /// Creates a new PrefabPool in this Pool and instances the requested 
        /// number of instances (set by PrefabPool.preloadAmount). If preload 
        /// amount is 0, nothing will be spawned and the return list will be empty.
        /// 
        /// It is rare this function is needed during regular usage.
        /// This function should only be used if you need to set the preferences
        /// of a new PrefabPool, such as culling or pre-loading, before use. Otherwise, 
        /// just use Spawn() and if the prefab is used for the first time a PrefabPool 
        /// will automatically be created with defaults anyway.
        /// 
        /// Note: Instances with ParticleEmitters can be preloaded too because 
        ///       it won't trigger the emmiter or the coroutine which waits for 
        ///       particles to die, which Spawn() does.
        ///       
        /// Usage Example:
        ///     // Creates a prefab pool and sets culling options but doesn't
        ///     //   need to spawn any instances (this is fine)
        ///     PrefabPool prefabPool = new PrefabPool()
        ///     prefabPool.prefab = myPrefabReference;
        ///     prefabPool.preloadAmount = 0;
        ///     prefabPool.cullDespawned = True;
        ///     prefabPool.cullAbove = 50;
        ///     prefabPool.cullDelay = 30;
        ///     
        ///     // Enemies is just an example. Any pool is fine.
        ///     PoolManager.Pools["Enemies"].CreatePrefabPool(prefabPool);
        ///     
        ///     // Then, just use as normal...
        ///     PoolManager.Pools["Enemies"].Spawn(myPrefabReference);
        /// </summary>
        /// <param name="prefabPool">A PrefabPool object</param>
        /// <returns>A List of instances spawned or an empty List</returns>
        public void CreatePrefabPool(PrefabPool prefabPool)
        {
            // Only add a PrefabPool once. Uses a GameObject comparison on the prefabs
            //   This will rarely be needed and will almost Always run at game start, 
            //   even if user-executed. This really only fails If a user tries to create 
            //   a PrefabPool using a prefab which already has a PrefabPool in the same
            //   SpawnPool. Either user created twice or PoolManager went first or even 
            //   second in cases where a user-script beats out PoolManager's init during 
            //   Awake();
            bool isAlreadyPool = this.GetPrefabPool(prefabPool.prefab) == null ? false : true;
            if (!isAlreadyPool)
            {
                // Used internally to reference back to this spawnPool for things 
                //   like anchoring co-routines.
                prefabPool.spawnPool = this;

                this._prefabPools.Add(prefabPool);

                // Add to the prefabs dict for convenience
                this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
            }

            // Preloading (uses a singleton bool to be sure this is only done once)
            if (prefabPool.preloaded != true)
            {
                if (this.logMessages)
                    Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}",
                                               this.poolName,
                                               prefabPool.preloadAmount,
                                               prefabPool.prefab.name));

                prefabPool.PreloadInstances();
            }
        }


        /// <summary>
        /// Add an existing instance to this pool. This is used during game start 
        /// to pool objects which are not instanciated at runtime
        /// </summary>
        /// <param name="instance">The instance to add</param>
        /// <param name="prefabName">
        /// The name of the prefab used to create this instance
        /// </param>
        /// <param name="despawn">True to depawn on start</param>
        /// <param name="parent">True to make a child of the pool's group</param>
        public void Add(Transform instance, string prefabName, bool despawn, bool parent)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i].prefabGO == null)
                {
                    Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
                    return;
                }

                if (this._prefabPools[i].prefabGO.name == prefabName)
                {
                    this._prefabPools[i].AddUnpooled(instance, despawn);

                    if (this.logMessages)
                        Debug.Log(string.Format(
                                "SpawnPool {0}: Adding previously unpooled instance {1}",
                                                this.poolName,
                                                instance.name));

                    if (parent) instance.parent = this.group;

                    // New instances are active and must be added to the internal list 
                    if (!despawn) this._spawned.Add(instance);

                    return;
                }
            }

            // Log an error if a PrefabPool with the given name was not found
            Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.",
                                         this.poolName,
                                         prefabName));

        }
        #endregion Constructor and Init



        #region List Overrides
        /// <summary>
        /// Not Implimented. Use Spawn() to properly add items to the pool.
        /// This is required because the prefab needs to be stored in the internal
        /// data structure in order for the pool to function properly. Items can
        /// only be added by instencing them using SpawnInstance().
        /// </summary>
        /// <param name="item"></param>
        public void Add(Transform item)
        {
            string msg = "Use SpawnPool.Spawn() to properly add items to the pool.";
            throw new System.NotImplementedException(msg);
        }


        /// <summary>
        /// Not Implimented. Use Despawn() to properly manage items that should remain 
        /// in the Queue but be deactivated. There is currently no way to safetly
        /// remove items from the pool permentantly. Destroying Objects would
        /// defeat the purpose of the pool.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(Transform item)
        {
            string msg = "Use Despawn() to properly manage items that should " +
                         "remain in the pool but be deactivated.";
            throw new System.NotImplementedException(msg);
        }

        #endregion List Overrides



        #region Pool Functionality
        /// <description>
        ///	Spawns an instance or creates a new instance if none are available.
        ///	Either way, an instance will be set to the passed position and 
        ///	rotation.
        /// 
        /// Detailed Information:
        /// Checks the Data structure for an instance that was already created
        /// using the prefab. If the prefab has been used before, such as by
        /// setting it in the Unity Editor to preload instances, or just used
        /// before via this function, one of its instances will be used if one
        /// is available, or a new one will be created.
        /// 
        /// If the prefab has never been used a new PrefabPool will be started 
        /// with default options. 
        /// 
        /// To alter the options on a prefab pool, use the Unity Editor or see
        /// the documentation for the PrefabPool class and 
        /// SpawnPool.SpawnPrefabPool()
        ///		
        /// Broadcasts "OnSpawned" to the instance. Use this to manage states.
        ///		
        /// An overload of this function has the same initial signature as Unity's 
        /// Instantiate() that takes position and rotation. The return Type is different 
        /// though. Unity uses and returns a GameObject reference. PoolManager 
        /// uses and returns a Transform reference (or other supported type, such 
        /// as AudioSource and ParticleSystem)
        /// </description>
        /// <param name="prefab">
        /// The prefab used to spawn an instance. Only used for reference if an 
        /// instance is already in the pool and available for respawn. 
        /// NOTE: Type = Transform
        /// </param>
        /// <param name="pos">The position to set the instance to</param>
        /// <param name="rot">The rotation to set the instance to</param>
        /// <param name="parent">An optional parent for the instance</param>
        /// <returns>
        /// The instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached. You DO NOT need to test for null return values unless you 
        /// used the limit option.
        /// </returns>
        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            Transform inst;

            #region Use from Pool
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                // Determine if the prefab was ever used as explained in the docs
                //   I believe a comparison of two references is processor-cheap.
                if (this._prefabPools[i].prefabGO == prefab.gameObject)
                {
                    // Now we know the prefabPool for this prefab exists. 
                    // Ask the prefab pool to setup and activate an instance.
                    // If there is no instance to spawn, a new one is instanced
                    inst = this._prefabPools[i].SpawnInstance(pos, rot);

                    // This only happens if the limit option was used for this
                    //   Prefab Pool.
                    if (inst == null) return null;
					
					if (parent != null)  // User explicitly provided a parent
					{
						inst.parent = parent;
					}
                    else if (!this.dontReparent && inst.parent != this.group)  // Auto organize?
					{
						// If a new instance was created, it won't be grouped
                        inst.parent = this.group;
					}

                    // Add to internal list - holds only active instances in the pool
                    // 	 This isn't needed for Pool functionality. It is just done 
                    //	 as a user-friendly feature which has been needed before.
                    this._spawned.Add(inst);
					
	                // Notify instance it was spawned so it can manage it's state
	                inst.gameObject.BroadcastMessage(
						"OnSpawned",
						this,
						SendMessageOptions.DontRequireReceiver
					);

                    // Done!
                    return inst;
                }
            }
            #endregion Use from Pool


            #region New PrefabPool
            // The prefab wasn't found in any PrefabPools above. Make a new one
            PrefabPool newPrefabPool = new PrefabPool(prefab);
            this.CreatePrefabPool(newPrefabPool);

            // Spawn the new instance (Note: prefab already set in PrefabPool)
            inst = newPrefabPool.SpawnInstance(pos, rot);
			
			if (parent != null)  // User explicitly provided a parent
			{
				inst.parent = parent;
			}
            else  // Auto organize
			{
            	inst.parent = this.group;  
			}


            // New instances are active and must be added to the internal list 
            this._spawned.Add(inst);
            #endregion New PrefabPool

            // Notify instance it was spawned so it can manage it's state
            inst.gameObject.BroadcastMessage(
				"OnSpawned",
				this,
				SendMessageOptions.DontRequireReceiver
			);

            // Done!
            return inst;
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// </summary>
        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
            Transform inst = this.Spawn(prefab, pos, rot, null);

            // Can happen if limit was used
            if (inst == null) return null;

            return inst;
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab and instance using an 'empty' 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(Transform prefab)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab  and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn(Transform prefab, Transform parent)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
        }
		
		
		#region GameObject Overloads
		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			return Spawn(prefab.transform, pos, rot, parent);
		}
		
		public Transform Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			return Spawn(prefab.transform, pos, rot);
		}
		
		public Transform Spawn(GameObject prefab)
		{
			return Spawn(prefab.transform);
		}
		
		public Transform Spawn(GameObject prefab, Transform parent)
		{
			return Spawn(prefab.transform, parent);
		}
		#endregion GameObject Overloads
		
		
        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled  
        /// from the SpawnPool.prefabs dictionary.
        /// </summary>
        public Transform Spawn(string prefabName)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn(string prefabName, Transform parent)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, parent);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled from 
        /// the SpawnPool.prefabs dictionary. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, pos, rot);
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, 
                               Transform parent)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, pos, rot, parent);
        }


        public AudioSource Spawn(AudioSource prefab,
                            Vector3 pos, Quaternion rot)
        {
            return this.Spawn(prefab, pos, rot, null);  // parent = null
        }


        public AudioSource Spawn(AudioSource prefab)
        {
            return this.Spawn
            (
                prefab, 
                Vector3.zero, Quaternion.identity,
                null  // parent = null
            );
        }
		
	 	
		public AudioSource Spawn(AudioSource prefab, Transform parent)
        {
            return this.Spawn
            (
                prefab, 
                Vector3.zero, 
				Quaternion.identity,
                parent
            );
        }
		
		
        public AudioSource Spawn(AudioSource prefab,
                            	 Vector3 pos, Quaternion rot,
                            	 Transform parent)
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = Spawn(prefab.transform, pos, rot, parent);

            // Can happen if limit was used
            if (inst == null) return null;

            // Get the emitter and start it
            var src = inst.GetComponent<AudioSource>();
            src.Play();

            this.StartCoroutine(this.ListForAudioStop(src));

            return src;
        }


        /// <summary>
        ///	See docs for SpawnInstance(Transform prefab, Vector3 pos, Quaternion rot)
        ///	for basic functionalty information.
        ///		
        /// Pass a ParticleSystem component of a prefab to instantiate, trigger 
        /// emit, then listen for when all particles have died to "auto-destruct", 
        /// but instead of destroying the game object it will be deactivated and 
        /// added to the pool to be reused.
        /// 
        /// IMPORTANT: 
        ///     * You must pass a ParticleSystem next time as well, or the emitter
        ///       will be treated as a regular prefab and simply activate, but emit
        ///       will not be triggered!
        ///     * The listner that waits for the death of all particles will 
        ///       time-out after a set number of seconds and log a warning. 
        ///       This is done to keep the developer aware of any unexpected 
        ///       usage cases. Change the public property "maxParticleDespawnTime"
        ///       to adjust this length of time.
        /// 
        /// Broadcasts "OnSpawned" to the instance. Use this instead of Awake()
        ///		
        /// This function has the same initial signature as Unity's Instantiate() 
        /// that takes position and rotation. The return Type is different though.
        /// </summary>
        public ParticleSystem Spawn(ParticleSystem prefab,
                                    Vector3 pos, Quaternion rot)
        {
            return Spawn(prefab, pos, rot, null);  // parent = null

        }

        /// <summary>
        /// See primary Spawn ParticleSystem method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public ParticleSystem Spawn(ParticleSystem prefab,
                                    Vector3 pos, Quaternion rot,
                                    Transform parent)
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = this.Spawn(prefab.transform, pos, rot, parent);

            // Can happen if limit was used
            if (inst == null) return null;

            // Get the emitter and start it
            var emitter = inst.GetComponent<ParticleSystem>();
            //emitter.Play(true);  // Seems to auto-play on activation so this may not be needed

            this.StartCoroutine(this.ListenForEmitDespawn(emitter));

            return emitter;
        }


        /// <summary>
        ///	See docs for SpawnInstance(ParticleSystems prefab, Vector3 pos, Quaternion rot)
        ///	This is the same but for ParticleEmitters
        ///	
        /// IMPORTANT: 
        ///     * This function turns off Unity's ParticleAnimator autodestruct if
        ///       one is found.
        /// </summary>
        public ParticleEmitter Spawn(ParticleEmitter prefab,
                                     Vector3 pos, Quaternion rot)
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = this.Spawn(prefab.transform, pos, rot);

            // Can happen if limit was used
            if (inst == null) return null;

            // Make sure autodestrouct is OFF as it will cause null references
            var animator = inst.GetComponent<ParticleAnimator>();
            if (animator != null) animator.autodestruct = false;

            // Get the emitter
            var emitter = inst.GetComponent<ParticleEmitter>();
            emitter.emit = true;

            this.StartCoroutine(this.ListenForEmitDespawn(emitter));

            return emitter;
        }


        /// <summary>
        /// This will not be supported for Shuriken particles. This will eventually 
        /// be depricated.
        /// </summary>
        /// <param name="prefab">
        /// The prefab to instance. Not used if an instance already exists in 
        /// the scene that is queued for reuse. Type = ParticleEmitter
        /// </param>
        /// <param name="pos">The position to set the instance to</param>
        /// <param name="rot">The rotation to set the instance to</param>
        /// <param name="colorPropertyName">Same as Material.SetColor()</param>
        /// <param name="color">a Color object. Same as Material.SetColor()</param>
        /// <returns>The instance's ParticleEmitter</returns>
        public ParticleEmitter Spawn(ParticleEmitter prefab,
                                     Vector3 pos, Quaternion rot,
                                     string colorPropertyName, Color color)
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = this.Spawn(prefab.transform, pos, rot);

            // Can happen if limit was used
            if (inst == null) return null;

            // Make sure autodestrouct is OFF as it will cause null references
            var animator = inst.GetComponent<ParticleAnimator>();
            if (animator != null) animator.autodestruct = false;

            // Get the emitter
            var emitter = inst.GetComponent<ParticleEmitter>();

            // Set the color of the particles, then emit
            emitter.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
            emitter.emit = true;

            this.StartCoroutine(ListenForEmitDespawn(emitter));

            return emitter;
        }


        /// <summary>
        ///	If the passed object is managed by the SpawnPool, it will be 
        ///	deactivated and made available to be spawned again.
        ///		
        /// Despawned instances are removed from the primary list.
        /// </summary>
        /// <param name="item">The transform of the gameobject to process</param>
        public void Despawn(Transform instance)
        {
            // Find the item and despawn it
            bool despawned = false;
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i]._spawned.Contains(instance))
                {
                    despawned = this._prefabPools[i].DespawnInstance(instance);
                    break;
                }  // Protection - Already despawned?
                else if (this._prefabPools[i]._despawned.Contains(instance))
                {
                    Debug.LogError(
                        string.Format("SpawnPool {0}: {1} has already been despawned. " +
                                       "You cannot despawn something more than once!",
                                        this.poolName,
                                        instance.name));
                    return;
                }
            }

            // If still false, then the instance wasn't found anywhere in the pool
            if (!despawned)
            {
                Debug.LogError(string.Format("SpawnPool {0}: {1} not found in SpawnPool",
                               this.poolName,
                               instance.name));
                return;
            }

            // Remove from the internal list. Only active instances are kept. 
            // 	 This isn't needed for Pool functionality. It is just done 
            //	 as a user-friendly feature which has been needed before.
            this._spawned.Remove(instance);
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn(Transform instance, Transform parent)
        {
            instance.parent = parent;
            this.Despawn(instance);
        }


        /// <description>
        /// See docs for Despawn(Transform instance). This expands that functionality.
        ///   If the passed object is managed by this SpawnPool, it will be 
        ///   deactivated and made available to be spawned again.
        /// </description>
        /// <param name="item">The transform of the instance to process</param>
        /// <param name="seconds">The time in seconds to wait before despawning</param>
        public void Despawn(Transform instance, float seconds)
        {
            this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, false, null));
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn(Transform instance, float seconds, Transform parent)
        {
            this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, true, parent));
        }


        /// <summary>
        /// Waits X seconds before despawning. See the docs for DespawnAfterSeconds()
        /// the argument useParent is used because a null parent is valid in Unity. It will 
        /// make the scene root the parent
        /// </summary>
        private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
        {
            GameObject go = instance.gameObject;
            while (seconds > 0)
            {
                yield return null;

                // If the instance was deactivated while waiting here, just quit
                if (!go.activeInHierarchy)
                    yield break;
                
                seconds -= Time.deltaTime;
            }

            if (useParent)
                this.Despawn(instance, parent);
            else
                this.Despawn(instance);
        }


        /// <description>
        /// Despawns all active instances in this SpawnPool
        /// </description>
        public void DespawnAll()
        {
            var spawned = new List<Transform>(this._spawned);
            for (int i = 0; i < spawned.Count; i++)
                this.Despawn(spawned[i]);
        }


        /// <description>
        ///	Returns true if the passed transform is currently spawned.
        /// </description>
        /// <param name="item">The transform of the gameobject to test</param>
        public bool IsSpawned(Transform instance)
        {
            return this._spawned.Contains(instance);
        }

        #endregion Pool Functionality



        #region Utility Functions
        /// <summary>
        /// Returns the prefab pool for a given prefab.
        /// </summary>
        /// <param name="prefab">The Transform of an instance</param>
        /// <returns>PrefabPool</returns>
        public PrefabPool GetPrefabPool(Transform prefab)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i].prefabGO == null)
                    Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                                 this.poolName));

                if (this._prefabPools[i].prefabGO == prefab.gameObject)
                    return this._prefabPools[i];
            }

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab pool for a given prefab.
        /// </summary>
        /// <param name="prefab">The GameObject of an instance</param>
        /// <returns>PrefabPool</returns>
        public PrefabPool GetPrefabPool(GameObject prefab)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
            {
                if (this._prefabPools[i].prefabGO == null)
                    Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                                 this.poolName));

                if (this._prefabPools[i].prefabGO == prefab)
                    return this._prefabPools[i];
            }

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab used to create the passed instance. 
        /// This is provided for convienince as Unity doesn't offer this feature.
        /// </summary>
        /// <param name="instance">The Transform of an instance</param>
        /// <returns>Transform</returns>
        public Transform GetPrefab(Transform instance)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
                if (this._prefabPools[i].Contains(instance))
                    return this._prefabPools[i].prefab;

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab used to create the passed instance. 
        /// This is provided for convienince as Unity doesn't offer this feature.
        /// </summary>
        /// <param name="instance">The GameObject of an instance</param>
        /// <returns>GameObject</returns>
        public GameObject GetPrefab(GameObject instance)
        {
            for (int i = 0; i < this._prefabPools.Count; i++)
                if (this._prefabPools[i].Contains(instance.transform))
                    return this._prefabPools[i].prefabGO;

            // Nothing found
            return null;
        }


        private IEnumerator ListForAudioStop(AudioSource src)
        {
            // Safer to wait a frame before testing if playing.
            yield return null;

            while (src.isPlaying)
                yield return null;

            this.Despawn(src.transform);
        }


        /// <summary>
        /// Used to determine when a particle emiter should be despawned
        /// </summary>
        /// <param name="emitter">ParticleEmitter to process</param>
        /// <returns></returns>
        private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
        {
            // This will wait for the particles to emit. Without this, there will
            //   be no particles in the while test below. I don't know why the extra 
            //   frame is required but should never be noticable. No particles can
            //   fade out that fast and still be seen to change over time.
            yield return null;
            yield return new WaitForEndOfFrame();

            // Do nothing until all particles die or the safecount hits a max value
            float safetimer = 0;   // Just in case! See Spawn() for more info
            while (emitter.particleCount > 0)
            {
                safetimer += Time.deltaTime;
                if (safetimer > this.maxParticleDespawnTime)
                    Debug.LogWarning
                    (
                        string.Format
                        (
                            "SpawnPool {0}: " +
                                "Timed out while listening for all particles to die. " +
                                "Waited for {1}sec.",
                            this.poolName,
                            this.maxParticleDespawnTime
                        )
                    );

                yield return null;
            }

            // Turn off emit before despawning
            emitter.emit = false;
            this.Despawn(emitter.transform);
        }

        // ParticleSystem (Shuriken) Version...
        private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
        {
            // Wait for the delay time to complete
            // Waiting the extra frame seems to be more stable and means at least one 
            //  frame will always pass
            yield return new WaitForSeconds(emitter.startDelay + 0.25f);

            // Do nothing until all particles die or the safecount hits a max value
            float safetimer = 0;   // Just in case! See Spawn() for more info
            while (emitter.IsAlive(true))
            {
                if (!PoolManagerUtils.activeInHierarchy(emitter.gameObject))
                {
                    emitter.Clear(true);
                    yield break;  // Do nothing, already despawned. Quit.
                }

                safetimer += Time.deltaTime;
                if (safetimer > this.maxParticleDespawnTime)
                    Debug.LogWarning
                    (
                        string.Format
                        (
                            "SpawnPool {0}: " +
                                "Timed out while listening for all particles to die. " +
                                "Waited for {1}sec.",
                            this.poolName,
                            this.maxParticleDespawnTime
                        )
                    );

                yield return null;
            }

            // Turn off emit before despawning
            //emitter.Clear(true);
            this.Despawn(emitter.transform);
        }

        #endregion Utility Functions



        /// <summary>
        /// Returns a formatted string showing all the spawned member names
        /// </summary>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var name_list = new List<string>();
            foreach (Transform item in this._spawned)
                name_list.Add(item.name);

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return System.String.Join(", ", name_list.ToArray());
        }


        /// <summary>
        /// Read-only index access. You can still modify the instance at the given index.
        /// Read-only reffers to setting an index to a new instance reference, which would
        /// change the list. Setting via index is never needed to work with index access.
        /// </summary>
        /// <param name="index">int address of the item to get</param>
        /// <returns></returns>
        public Transform this[int index]
        {
            get { return this._spawned[index]; }
            set { throw new System.NotImplementedException("Read-only."); }
        }

        /// <summary>
        /// The name "Contains" is misleading so IsSpawned was implimented instead.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Transform item)
        {
            string message = "Use IsSpawned(Transform instance) instead.";
            throw new System.NotImplementedException(message);
        }


        /// <summary>
        /// Used by OTHERList.AddRange()
        /// This adds this list to the passed list
        /// </summary>
        /// <param name="array">The list AddRange is being called on</param>
        /// <param name="arrayIndex">
        /// The starting index for the copy operation. AddRange seems to pass the last index.
        /// </param>
        public void CopyTo(Transform[] array, int arrayIndex)
        {
            this._spawned.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// Returns the number of items in this (the collection). Readonly.
        /// </summary>
        public int Count
        {
            get { return this._spawned.Count; }
        }


        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Transform> GetEnumerator()
        {
            for (int i = 0; i < this._spawned.Count; i++)
                yield return this._spawned[i];
        }

        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this._spawned.Count; i++)
                yield return this._spawned[i];
        }

        // Not implemented
        public int IndexOf(Transform item) { throw new System.NotImplementedException(); }
        public void Insert(int index, Transform item) { throw new System.NotImplementedException(); }
        public void RemoveAt(int index) { throw new System.NotImplementedException(); }
        public void Clear() { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        bool ICollection<Transform>.Remove(Transform item) { throw new System.NotImplementedException(); }

    }



    /// <summary>
    /// This class is used to display a more complex user entry interface in the 
    /// Unity Editor so we can collect more options related to each Prefab.
    /// 
    /// See this class documentation for Editor Options.
    /// 
    /// This class is also the primary pool functionality for SpawnPool. SpawnPool
    /// manages the Pool using these settings and methods. See the SpawnPool 
    /// documentation for user documentation and usage.
    /// </summary>
    [System.Serializable]
    public class PrefabPool
    {

        #region Public Properties Available in the Editor
        /// <summary>
        /// The prefab to preload
        /// </summary>
        public Transform prefab;

        /// <summary>
        /// A reference of the prefab's GameObject stored for performance reasons
        /// </summary>
        internal GameObject prefabGO;  // Hidden in inspector, but not Debug tab

        /// <summary>
        /// The number of instances to preload
        /// </summary>
        public int preloadAmount = 1;

        /// <summary>
        /// Displays the 'preload over time' options
        /// </summary>
        public bool preloadTime = false;

        /// <summary>
        /// The number of frames it will take to preload all requested instances
        /// </summary>
        public int preloadFrames = 2;

        /// <summary>
        /// The number of seconds to wait before preloading any instances
        /// </summary>
        public float preloadDelay = 0;

        /// <summary>
        /// Limits the number of instances allowed in the game. Turning this ON
        ///	means when 'Limit Amount' is hit, no more instances will be created.
        /// CALLS TO SpawnPool.Spawn() WILL BE IGNORED, and return null!
        ///
        /// This can be good for non-critical objects like bullets or explosion
        ///	Flares. You would never want to use this for enemies unless it makes
        ///	sense to begin ignoring enemy spawns in the context of your game.
        /// </summary>
        public bool limitInstances = false;

        /// <summary>
        /// This is the max number of instances allowed if 'limitInstances' is ON.
        /// </summary>
        public int limitAmount = 100;

        /// <summary>
        /// FIFO stands for "first-in-first-out". Normally, limiting instances will
        /// stop spawning and return null. If this is turned on (set to true) the
        /// first spawned instance will be despawned and reused instead, keeping the
        /// total spawned instances limited but still spawning new instances.
        /// </summary>
        public bool limitFIFO = false;  // Keep after limitAmount for auto-inspector

        /// <summary>
        /// Turn this ON to activate the culling feature for this Pool. 
        /// Use this feature to remove despawned (inactive) instances from the pool
        /// if the size of the pool grows too large. 
        ///	
        /// DO NOT USE THIS UNLESS YOU NEED TO MANAGE MEMORY ISSUES!
        /// This should only be used in extreme cases for memory management. 
        /// For most pools (or games for that matter), it is better to leave this 
        /// off as memory is more plentiful than performance. If you do need this
        /// you can fine tune how often this is triggered to target extreme events.
        /// 
        /// A good example of when to use this would be if you you are Pooling 
        /// projectiles and usually never need more than 10 at a time, but then
        /// there is a big one-off fire-fight where 50 projectiles are needed. 
        /// Rather than keep the extra 40 around in memory from then on, set the 
        /// 'Cull Above' property to 15 (well above the expected max) and the Pool 
        /// will Destroy() the extra instances from the game to free up the memory. 
        /// 
        /// This won't be done immediately, because you wouldn't want this culling 
        /// feature to be fighting the Pool and causing extra Instantiate() and 
        /// Destroy() calls while the fire-fight is still going on. See 
        /// "Cull Delay" for more information about how to fine tune this.
        /// </summary>
        public bool cullDespawned = false;

        /// <summary>
        /// The number of TOTAL (spawned + despawned) instances to keep. 
        /// </summary>
        public int cullAbove = 50;

        /// <summary>
        /// The amount of time, in seconds, to wait before culling. This is timed 
        /// from the moment when the Queue's TOTAL count (spawned + despawned) 
        /// becomes greater than 'Cull Above'. Once triggered, the timer is repeated 
        /// until the count falls below 'Cull Above'.
        /// </summary>
        public int cullDelay = 60;

        /// <summary>
        /// The maximum number of instances to destroy per this.cullDelay
        /// </summary>
        public int cullMaxPerPass = 5;

        /// <summary>
        /// Prints information during run-time to make debugging easier. This will 
        /// be set to true if the owner SpawnPool is true, otherwise the user's setting
        /// here will be used
        /// </summary>
        public bool _logMessages = false;  // Used by the inspector
        public bool logMessages            // Read-only
        {
            get
            {
                if (forceLoggingSilent) return false;

                if (this.spawnPool.logMessages)
                    return this.spawnPool.logMessages;
                else
                    return this._logMessages;
            }
        }

        // Forces logging to be silent regardless of user settings.
        private bool forceLoggingSilent = false;


        /// <summary>
        /// Used internally to reference back to the owner spawnPool for things like
        /// anchoring co-routines.
        /// </summary>
        public SpawnPool spawnPool;
        #endregion Public Properties Available in the Editor


        #region Constructor and Self-Destruction
        /// <description>
        ///	Constructor to require a prefab Transform
        /// </description>
        public PrefabPool(Transform prefab)
        {
            this.prefab = prefab;
            this.prefabGO = prefab.gameObject;
        }

        /// <description>
        ///	Constructor for Serializable inspector use only
        /// </description>
        public PrefabPool() { }

        /// <description>
        ///	A pseudo constructor to init stuff not init by the serialized inspector-created
        ///	instance of this class.
        /// </description>
        internal void inspectorInstanceConstructor()
        {
            this.prefabGO = this.prefab.gameObject;
            this._spawned = new List<Transform>();
            this._despawned = new List<Transform>();
        }


        /// <summary>
        /// Run by a SpawnPool when it is destroyed
        /// </summary>
        internal void SelfDestruct()
        {
            // Probably overkill but no harm done
            this.prefab = null;
            this.prefabGO = null;
            this.spawnPool = null;

            // Go through both lists and destroy everything
            foreach (Transform inst in this._despawned)
                if (inst != null)
                    Object.Destroy(inst.gameObject);

            foreach (Transform inst in this._spawned)
                if (inst != null)
                    Object.Destroy(inst.gameObject);

            this._spawned.Clear();
            this._despawned.Clear();
        }
        #endregion Constructor and Self-Destruction


        #region Pool Functionality
        /// <summary>
        /// Is set to true when the culling coroutine is started so another one
        /// won't be
        /// </summary>
        private bool cullingActive = false;


        /// <summary>
        /// The active instances associated with this prefab. This is the pool of
        /// instances created by this prefab.
        /// 
        /// Managed by a SpawnPool
        /// </summary>
        internal List<Transform> _spawned = new List<Transform>();
        public List<Transform> spawned { get { return new List<Transform>(this._spawned); } }

        /// <summary>
        /// The deactive instances associated with this prefab. This is the pool of
        /// instances created by this prefab.
        /// 
        /// Managed by a SpawnPool
        /// </summary>
        internal List<Transform> _despawned = new List<Transform>();
        public List<Transform> despawned { get { return new List<Transform>(this._despawned); } }


        /// <summary>
        /// Returns the total count of instances in the PrefabPool
        /// </summary>
        public int totalCount
        {
            get
            {
                // Add all the items in the pool to get the total count
                int count = 0;
                count += this._spawned.Count;
                count += this._despawned.Count;
                return count;
            }
        }


        /// <summary>
        /// Used to make PreloadInstances() a one-time event. Read-only.
        /// </summary>
        private bool _preloaded = false;
        internal bool preloaded
        {
            get { return this._preloaded; }
            private set { this._preloaded = value; }
        }


        /// <summary>
        /// Move an instance from despawned to spawned, set the position and 
        /// rotation, activate it and all children and return the transform
        /// </summary>
        /// <returns>
        /// True if successfull, false if xform isn't in the spawned list
        /// </returns>
        internal bool DespawnInstance(Transform xform)
        {
            return DespawnInstance(xform, true);
        }

        internal bool DespawnInstance(Transform xform, bool sendEventMessage)
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'",
                                       this.spawnPool.poolName,
                                       this.prefab.name,
                                       xform.name));

            // Switch to the despawned list
            this._spawned.Remove(xform);
            this._despawned.Add(xform);

            // Notify instance of event OnDespawned for custom code additions.
            //   This is done before handling the deactivate and enqueue incase 
            //   there the user introduces an unforseen issue.
            if (sendEventMessage)
                xform.gameObject.BroadcastMessage(
					"OnDespawned",
					this.spawnPool,
                    SendMessageOptions.DontRequireReceiver
				);

            // Deactivate the instance and all children
            PoolManagerUtils.SetActive(xform.gameObject, false);

            // Trigger culling if the feature is ON and the size  of the 
            //   overall pool is over the Cull Above threashold.
            //   This is triggered here because Despawn has to occur before
            //   it is worth culling anyway, and it is run fairly often.
            if (!this.cullingActive &&   // Cheap & Singleton. Only trigger once!
                this.cullDespawned &&    // Is the feature even on? Cheap too.
                this.totalCount > this.cullAbove)   // Criteria met?
            {
                this.cullingActive = true;
                this.spawnPool.StartCoroutine(CullDespawned());
            }
            return true;
        }



        /// <summary>
        /// Waits for 'cullDelay' in seconds and culls the 'despawned' list if 
        /// above 'cullingAbove' amount. 
        /// 
        /// Triggered by DespawnInstance()
        /// </summary>
        internal IEnumerator CullDespawned()
        {
            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING TRIGGERED! " +
                                          "Waiting {2}sec to begin checking for despawns...",
                                        this.spawnPool.poolName,
                                        this.prefab.name,
                                        this.cullDelay));

            // First time always pause, then check to see if the condition is
            //   still true before attempting to cull.
            yield return new WaitForSeconds(this.cullDelay);

            while (this.totalCount > this.cullAbove)
            {
                // Attempt to delete an amount == this.cullMaxPerPass
                for (int i = 0; i < this.cullMaxPerPass; i++)
                {
                    // Break if this.cullMaxPerPass would go past this.cullAbove
                    if (this.totalCount <= this.cullAbove)
                        break;  // The while loop will stop as well independently

                    // Destroy the last item in the list
                    if (this._despawned.Count > 0)
                    {
                        Transform inst = this._despawned[0];
                        this._despawned.RemoveAt(0);
                        MonoBehaviour.Destroy(inst.gameObject);

                        if (this.logMessages)
                            Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                                    "CULLING to {2} instances. Now at {3}.",
                                                this.spawnPool.poolName,
                                                this.prefab.name,
                                                this.cullAbove,
                                                this.totalCount));
                    }
                    else if (this.logMessages)
                    {
                        Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                                    "CULLING waiting for despawn. " +
                                                    "Checking again in {2}sec",
                                                this.spawnPool.poolName,
                                                this.prefab.name,
                                                this.cullDelay));

                        break;
                    }
                }

                // Check again later
                yield return new WaitForSeconds(this.cullDelay);
            }

            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING FINISHED! Stopping",
                                        this.spawnPool.poolName,
                                        this.prefab.name));

            // Reset the singleton so the feature can be used again if needed.
            this.cullingActive = false;
            yield return null;
        }



        /// <summary>
        /// Move an instance from despawned to spawned, set the position and 
        /// rotation, activate it and all children and return the transform.
        /// 
        /// If there isn't an instance available, a new one is made.
        /// </summary>
        /// <returns>
        /// The new instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached.
        /// </returns>    
        internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
        {
            // Handle FIFO limiting if the limit was used and reached.
            //   If first-in-first-out, despawn item zero and continue on to respawn it
            if (this.limitInstances && this.limitFIFO &&
                this._spawned.Count >= this.limitAmount)
            {
                Transform firstIn = this._spawned[0];

                if (this.logMessages)
                {
                    Debug.Log(string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                            "LIMIT REACHED! FIFO=True. Calling despawning for {2}...",
                        this.spawnPool.poolName,
                        this.prefab.name,
                        firstIn
                    ));
                }

                this.DespawnInstance(firstIn);

                // Because this is an internal despawn, we need to re-sync the SpawnPool's
                //  internal list to reflect this
                this.spawnPool._spawned.Remove(firstIn);
            }

            Transform inst;

            // If nothing is available, create a new instance
            if (this._despawned.Count == 0)
            {
                // This will also handle limiting the number of NEW instances
                inst = this.SpawnNew(pos, rot);
            }
            else
            {
                // Switch the instance we are using to the spawned list
                // Use the first item in the list for ease
                inst = this._despawned[0];
                this._despawned.RemoveAt(0);
                this._spawned.Add(inst);

                // This came up for a user so this was added to throw a user-friendly error
                if (inst == null)
                {
                    var msg = "Make sure you didn't delete a despawned instance directly.";
                    throw new MissingReferenceException(msg);
                }

                if (this.logMessages)
                    Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.",
                                            this.spawnPool.poolName,
                                            this.prefab.name,
                                            inst.name));

                // Get an instance and set position, rotation and then 
                //   Reactivate the instance and all children
                inst.position = pos;
                inst.rotation = rot;
                PoolManagerUtils.SetActive(inst.gameObject, true);

            }
			
			//
			// NOTE: OnSpawned message broadcast was moved to main Spawn() to ensure it runs last
			//
			
            return inst;
        }



        /// <summary>
        /// Spawns a NEW instance of this prefab and adds it to the spawned list.
        /// The new instance is placed at the passed position and rotation
        /// </summary>
        /// <param name="pos">Vector3</param>
        /// <param name="rot">Quaternion</param>
        /// <returns>
        /// The new instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached.
        /// </returns>
        public Transform SpawnNew() { return this.SpawnNew(Vector3.zero, Quaternion.identity); }
        public Transform SpawnNew(Vector3 pos, Quaternion rot)
        {
            // Handle limiting if the limit was used and reached.
            if (this.limitInstances && this.totalCount >= this.limitAmount)
            {
                if (this.logMessages)
                {
                    Debug.Log(string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                                "LIMIT REACHED! Not creating new instances! (Returning null)",
                            this.spawnPool.poolName,
                            this.prefab.name
                    ));
                }

                return null;
            }

            // Use the SpawnPool group as the default position and rotation
            if (pos == Vector3.zero) pos = this.spawnPool.group.position;
            if (rot == Quaternion.identity) rot = this.spawnPool.group.rotation;

            var inst = (Transform)Object.Instantiate(this.prefab, pos, rot);
            this.nameInstance(inst);  // Adds the number to the end

            if (!this.spawnPool.dontReparent)
                inst.parent = this.spawnPool.group;  // The group is the parent by default

            if (this.spawnPool.matchPoolScale)
                inst.localScale = Vector3.one;

            if (this.spawnPool.matchPoolLayer)
                this.SetRecursively(inst, this.spawnPool.gameObject.layer);

            // Start tracking the new instance
            this._spawned.Add(inst);

            if (this.logMessages)
                Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.",
                                        this.spawnPool.poolName,
                                        this.prefab.name,
                                        inst.name));

            return inst;
        }


        /// <summary>
        /// Sets the layer of the passed transform and all of its children
        /// </summary>
        /// <param name="xform">The transform to process</param>
        /// <param name="layer">The new layer</param>
        private void SetRecursively(Transform xform, int layer)
        {
            xform.gameObject.layer = layer;
            foreach (Transform child in xform)
                SetRecursively(child, layer);
        }


        /// <summary>
        /// Used by a SpawnPool to add an existing instance to this PrefabPool.
        /// This is used during game start to pool objects which are not 
        /// instantiated at runtime
        /// </summary>
        /// <param name="inst">The instance to add</param>
        /// <param name="despawn">True to despawn on add</param>
        internal void AddUnpooled(Transform inst, bool despawn)
        {
            this.nameInstance(inst);   // Adds the number to the end

            if (despawn)
            {
                // Deactivate the instance and all children
                PoolManagerUtils.SetActive(inst.gameObject, false);

                // Start Tracking as despawned
                this._despawned.Add(inst);
            }
            else
                this._spawned.Add(inst);
        }


        /// <summary>
        /// Preload PrefabPool.preloadAmount instances if they don't already exist. In 
        /// otherwords, if there are 7 and 10 should be preloaded, this only creates 3.
        /// This is to allow asynchronous Spawn() usage in Awake() at game start
        /// </summary>
        /// <returns></returns>
        internal void PreloadInstances()
        {
            // If this has already been run for this PrefabPool, there is something
            //   wrong!
            if (this.preloaded)
            {
                Debug.Log(string.Format("SpawnPool {0} ({1}): " +
                                          "Already preloaded! You cannot preload twice. " +
                                          "If you are running this through code, make sure " +
                                          "it isn't also defined in the Inspector.",
                                        this.spawnPool.poolName,
                                        this.prefab.name));

                return;
            }

            if (this.prefab == null)
            {
                Debug.LogError(string.Format("SpawnPool {0} ({1}): Prefab cannot be null.",
                                             this.spawnPool.poolName,
                                             this.prefab.name));

                return;
            }

            // Protect against preloading more than the limit amount setting
            //   This prevents an infinite loop on load if FIFO is used.
            if (this.limitInstances && this.preloadAmount > this.limitAmount)
            {
                Debug.LogWarning
                (
                    string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                            "You turned ON 'Limit Instances' and entered a " +
                            "'Limit Amount' greater than the 'Preload Amount'! " +
                            "Setting preload amount to limit amount.",
                         this.spawnPool.poolName,
                         this.prefab.name
                    )
                );

                this.preloadAmount = this.limitAmount;
            }

            // Notify the user if they made a mistake using Culling
            //   (First check is cheap)
            if (this.cullDespawned && this.preloadAmount > this.cullAbove)
            {
                Debug.LogWarning(string.Format("SpawnPool {0} ({1}): " +
                    "You turned ON Culling and entered a 'Cull Above' threshold " +
                    "greater than the 'Preload Amount'! This will cause the " +
                    "culling feature to trigger immediatly, which is wrong " +
                    "conceptually. Only use culling for extreme situations. " +
                    "See the docs.",
                    this.spawnPool.poolName,
                    this.prefab.name
                ));
            }

            if (this.preloadTime)
            {
                if (this.preloadFrames > this.preloadAmount)
                {
                    Debug.LogWarning(string.Format("SpawnPool {0} ({1}): " +
                        "Preloading over-time is on but the frame duration is greater " +
                        "than the number of instances to preload. The minimum spawned " +
                        "per frame is 1, so the maximum time is the same as the number " +
                        "of instances. Changing the preloadFrames value...",
                        this.spawnPool.poolName,
                        this.prefab.name
                    ));

                    this.preloadFrames = this.preloadAmount;
                }

                this.spawnPool.StartCoroutine(this.PreloadOverTime());
            }
            else
            {
                // Reduce debug spam: Turn off this.logMessages then set it back when done.
                this.forceLoggingSilent = true;

                Transform inst;
                while (this.totalCount < this.preloadAmount) // Total count will update
                {
                    // Preload...
                    // This will parent, position and orient the instance
                    //   under the SpawnPool.group
                    inst = this.SpawnNew();
                    this.DespawnInstance(inst, false);
                }

                // Restore the previous setting
                this.forceLoggingSilent = false;
            }
        }

        private IEnumerator PreloadOverTime()
        {
            yield return new WaitForSeconds(this.preloadDelay);

            Transform inst;

            // subtract anything spawned by other scripts, just in case
            int amount = this.preloadAmount - this.totalCount;
            if (amount <= 0)
                yield break;

            // Doesn't work for Windows8...
            //  This does the division and sets the remainder as an out value.
            //int numPerFrame = System.Math.DivRem(amount, this.preloadFrames, out remainder);
            int remainder = amount % this.preloadFrames;
            int numPerFrame = amount / this.preloadFrames;

            // Reduce debug spam: Turn off this.logMessages then set it back when done.
            this.forceLoggingSilent = true;

            int numThisFrame;
            for (int i = 0; i < this.preloadFrames; i++)
            {
                // Add the remainder to the *last* frame
                numThisFrame = numPerFrame;
                if (i == this.preloadFrames - 1)
                {
                    numThisFrame += remainder;
                }

                for (int n = 0; n < numThisFrame; n++)
                {
                    // Preload...
                    // This will parent, position and orient the instance
                    //   under the SpawnPool.group
                    inst = this.SpawnNew();
                    if (inst != null)
                        this.DespawnInstance(inst, false);

                    yield return null;
                }

                // Safety check in case something else is making instances. 
                //   Quit early if done early
                if (this.totalCount > this.preloadAmount)
                    break;
            }

            // Restore the previous setting
            this.forceLoggingSilent = false;
        }

        #endregion Pool Functionality


        #region Utilities
        /// <summary>
        /// If this PrefabPool spawned or despawned lists contain the given 
        /// transform, true is returned. Othrewise, false is returned
        /// </summary>
        /// <param name="transform">A transform to test.</param>
        /// <returns>bool</returns>
        public bool Contains(Transform transform)
        {
            if (this.prefabGO == null)
                Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                             this.spawnPool.poolName));

            bool contains;

            contains = this.spawned.Contains(transform);
            if (contains)
                return true;

            contains = this.despawned.Contains(transform);
            if (contains)
                return true;

            return false;
        }
        
        /// <summary>
        /// Appends a number to the end of the passed transform. The number
        /// will be one more than the total objects in this PrefabPool, so 
        /// name the object BEFORE adding it to the spawn or depsawn lists.
        /// </summary>
        /// <param name="instance"></param>
        private void nameInstance(Transform instance)
        {
            // Rename by appending a number to make debugging easier
            //   ToString() used to pad the number to 3 digits. Hopefully
            //   no one has 1,000+ objects.
            instance.name += (this.totalCount + 1).ToString("#000");
        }
        #endregion Utilities

    }



    public class PrefabsDict : IDictionary<string, Transform>
    {
        #region Public Custom Memebers
        /// <summary>
        /// Returns a formatted string showing all the prefab names
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var keysArray = new string[this._prefabs.Count];
            this._prefabs.Keys.CopyTo(keysArray, 0);

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return string.Format("[{0}]", System.String.Join(", ", keysArray));
        }
        #endregion Public Custom Memebers


        #region Internal Dict Functionality
        // Internal Add and Remove...
        internal void _Add(string prefabName, Transform prefab)
        {
            this._prefabs.Add(prefabName, prefab);
        }

        internal bool _Remove(string prefabName)
        {
            return this._prefabs.Remove(prefabName);
        }

        internal void _Clear()
        {
            this._prefabs.Clear();
        }
        #endregion Internal Dict Functionality


        #region Dict Functionality
        // Internal (wrapped) dictionary
        private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

        /// <summary>
        /// Get the number of SpawnPools in PoolManager
        /// </summary>
        public int Count { get { return this._prefabs.Count; } }

        /// <summary>
        /// Returns true if a prefab exists with the passed prefab name.
        /// </summary>
        /// <param name="prefabName">The name to look for</param>
        /// <returns>True if the prefab exists, otherwise, false.</returns>
        public bool ContainsKey(string prefabName)
        {
            return this._prefabs.ContainsKey(prefabName);
        }

        /// <summary>
        /// Used to get a prefab when the user is not sure if the prefabName is used.
        /// This is faster than checking Contains(prefabName) and then accessing the dict
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string prefabName, out Transform prefab)
        {
            return this._prefabs.TryGetValue(prefabName, out prefab);
        }

        #region Not Implimented

        public void Add(string key, Transform value)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Remove(string prefabName)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Contains(KeyValuePair<string, Transform> item)
        {
            string msg = "Use Contains(string prefabName) instead.";
            throw new System.NotImplementedException(msg);
        }

        public Transform this[string key]
        {
            get
            {
                Transform prefab;
                try
                {
                    prefab = this._prefabs[key];
                }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A Prefab with the name '{0}' not found. " +
                                                "\nPrefabs={1}",
                                                key, this.ToString());
                    throw new KeyNotFoundException(msg);
                }

                return prefab;
            }
            set
            {
                throw new System.NotImplementedException("Read-only.");
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this._prefabs.Keys;
            }
        }


        public ICollection<Transform> Values
        {
            get
            {
                return this._prefabs.Values;
            }
        }


        #region ICollection<KeyValuePair<string, Transform>> Members
        private bool IsReadOnly { get { return true; } }
        bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly { get { return true; } }

        public void Add(KeyValuePair<string, Transform> item)
        {
            throw new System.NotImplementedException("Read-only");
        }

        public void Clear() { throw new System.NotImplementedException(); }

        private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            string msg = "Cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        void ICollection<KeyValuePair<string, Transform>>.CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            string msg = "Cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        public bool Remove(KeyValuePair<string, Transform> item)
        {
            throw new System.NotImplementedException("Read-only");
        }
        #endregion ICollection<KeyValuePair<string, Transform>> Members
        #endregion Not Implimented




        #region IEnumerable<KeyValuePair<string, Transform>> Members
        public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion



        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion

        #endregion Dict Functionality

    }

}