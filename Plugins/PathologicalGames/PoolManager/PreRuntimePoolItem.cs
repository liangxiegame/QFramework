/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
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
    /// Adds an instance which is already a part of a scene to a pool on game start
    /// 
    /// Online Docs: http://docs.poolmanager.path-o-logical.com
    /// </description>
    [AddComponentMenu("Path-o-logical/PoolManager/Pre-Runtime Pool Item")]
    public class PreRuntimePoolItem : MonoBehaviour
    {
        #region Public Properties
        /// <summary>
        /// The name of the pool this instance will be added to on game start
        /// </summary>
        public string poolName = "";


        /// <summary>
        /// The prefab this instance came from. Unfortunatly we can't use drag&drop 
        /// because Unity will create a self-reference instead of keeping a dependable
        /// reference to the actual prefab.
        /// </summary>
        public string prefabName = "";


        /// <summary>
        /// Start the game with this instance de-spawned (inactive)
        /// </summary>
        public bool despawnOnStart = true;


        /// <summary>
        /// If true, the instance will not be reparented to the SpawnPool's 
        /// "group" (empty game object used for organizational purposes)
        /// 
        /// Leave this set to false except for special cases where it would break
        /// something in the game.
        /// </summary>
        public bool doNotReparent = false;
        #endregion Public Properties


        /// <summary>
        /// Register this item with the pool at game start
        /// </summary>
        private void Start()
        {
            SpawnPool pool;
            if (!PoolManager.Pools.TryGetValue(this.poolName, out pool))
            {

                string msg = "PreRuntimePoolItem Error ('{0}'): " +
                        "No pool with the name '{1}' exists! Create one using the " +
                        "PoolManager Inspector interface or PoolManager.CreatePool()." +
                        "See the online docs for more information at " +
                        "http://docs.poolmanager.path-o-logical.com";

                Debug.LogError(string.Format(msg, this.name, this.poolName));
                return;
            }

            // Add this instance to a pool
            pool.Add(this.transform, this.prefabName,
                     this.despawnOnStart, !this.doNotReparent);
        }
    }

}

