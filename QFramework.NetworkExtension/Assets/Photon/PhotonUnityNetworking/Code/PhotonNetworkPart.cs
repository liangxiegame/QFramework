// ----------------------------------------------------------------------------
// <copyright file="PhotonNetworkPart.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// PhotonNetwork is the central class of the PUN package.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System;
    using System.Linq;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using ExitGames.Client.Photon;
    using Photon.Realtime;

    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClassPun = ExitGames.Client.Photon.SupportClass;

    public static partial class PhotonNetwork
    {
        private static HashSet<byte> allowedReceivingGroups = new HashSet<byte>();

        private static HashSet<byte> blockedSendingGroups = new HashSet<byte>();

        private static HashSet<PhotonView> reusablePVHashset = new HashSet<PhotonView>();


        /// <summary>
        /// The photon view list.
        /// </summary>
        private static NonAllocDictionary<int, PhotonView> photonViewList = new NonAllocDictionary<int, PhotonView>();

        /// <summary>
        /// Gets the photon views.
        /// </summary>
        /// <remarks>
        /// This is an expensive operation as it returns a copy of the internal list.
        /// </remarks>
        /// <value>The photon views.</value>
        [System.Obsolete("Use PhotonViewCollection instead for an iterable collection of current photonViews.")]
        public static PhotonView[] PhotonViews
        {
            get
            {
                var views = new PhotonView[photonViewList.Count];
                int idx = 0;
                foreach (var v in photonViewList.Values)
                {
                    views[idx] = v;
                    idx++;
                }
                return views;
            }
        }

        /// <summary>
        /// Returns a new iterable collection of current photon views.
        /// </summary>
        /// <remarks>
        /// You can iterate over all PhotonViews in a simple foreach loop.
        /// To use this in a while-loop, assign the new iterator to a variable and then call MoveNext on that.
        /// </remarks>
        public static NonAllocDictionary<int, PhotonView>.ValueIterator PhotonViewCollection
        {
            get
            {
                return photonViewList.Values;
            }
        }

        public static int ViewCount
        {
            get { return photonViewList.Count; }
        }

        /// <summary>Parameters: PhotonView for which ownership changed, previous owner of the view.</summary>
        private static event Action<PhotonView, Player> OnOwnershipRequestEv;
        /// <summary>Parameters: PhotonView for which ownership was requested, player who requests ownership.</summary>
        private static event Action<PhotonView, Player> OnOwnershipTransferedEv;
        /// <summary>Parameters: PhotonView for which ownership was requested, player who requested (but didn't get) ownership.</summary>
        private static event Action<PhotonView, Player> OnOwnershipTransferFailedEv;

        /// <summary>
        /// Registers an object for callbacks for the implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks">.Net Callbacks</a>
        /// </remarks>
        /// <param name="target">The object that registers to get callbacks from PUN's LoadBalancingClient.</param>
        public static void AddCallbackTarget(object target)
        {
            if (target is PhotonView)
            {
                return;
            }

            IPunOwnershipCallbacks punOwnershipCallback = target as IPunOwnershipCallbacks;
            if (punOwnershipCallback != null)
            {
                OnOwnershipRequestEv += punOwnershipCallback.OnOwnershipRequest;
                OnOwnershipTransferedEv += punOwnershipCallback.OnOwnershipTransfered;
                OnOwnershipTransferFailedEv += punOwnershipCallback.OnOwnershipTransferFailed;
            }

            NetworkingClient.AddCallbackTarget(target);
        }


        /// <summary>
        /// Removes the target object from callbacks for its implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks">.Net Callbacks</a>
        /// </remarks>
        /// <param name="target">The object that unregisters from getting callbacks.</param>
        public static void RemoveCallbackTarget(object target)
        {
            if (target is PhotonView || NetworkingClient == null)
            {
                return;
            }

            IPunOwnershipCallbacks punOwnershipCallback = target as IPunOwnershipCallbacks;
            if (punOwnershipCallback != null)
            {
                OnOwnershipRequestEv -= punOwnershipCallback.OnOwnershipRequest;
                OnOwnershipTransferedEv -= punOwnershipCallback.OnOwnershipTransfered;
                OnOwnershipTransferFailedEv -= punOwnershipCallback.OnOwnershipTransferFailed;
            }

            NetworkingClient.RemoveCallbackTarget(target);
        }

        internal static string CallbacksToString()
        {
            var x = NetworkingClient.ConnectionCallbackTargets.Select(m => m.ToString()).ToArray();
            return string.Join(", ", x);
        }

        internal static byte currentLevelPrefix = 0;

        /// <summary>Internally used to flag if the message queue was disabled by a "scene sync" situation (to re-enable it).</summary>
        internal static bool loadingLevelAndPausedNetwork = false;

        /// <summary>For automatic scene syncing, the loaded scene is put into a room property. This is the name of said prop.</summary>
        internal const string CurrentSceneProperty = "curScn";
        internal const string CurrentScenePropertyLoadAsync = "curScnLa";


        /// <summary>
        /// An Object Pool can be used to keep and reuse instantiated object instances. Replaces Unity's default Instantiate and Destroy methods.
        /// </summary>
        /// <remarks>
        /// Defaults to the DefaultPool type.
        /// To use a GameObject pool, implement IPunPrefabPool and assign it here.
        /// Prefabs are identified by name.
        /// </remarks>
        public static IPunPrefabPool PrefabPool
        {
            get
            {
                return prefabPool;
            }
            set
            {
                if (value == null)
                {
                    Debug.LogWarning("PhotonNetwork.PrefabPool cannot be set to null. It will default back to using the 'DefaultPool' Pool");
                    prefabPool = new DefaultPool();
                }
                else
                {
                    prefabPool = value;
                }
            }
        }

        private static IPunPrefabPool prefabPool;

        /// <summary>
        /// While enabled, the MonoBehaviours on which we call RPCs are cached, avoiding costly GetComponents&lt;MonoBehaviour&gt;() calls.
        /// </summary>
        /// <remarks>
        /// RPCs are called on the MonoBehaviours of a target PhotonView. Those have to be found via GetComponents.
        ///
        /// When set this to true, the list of MonoBehaviours gets cached in each PhotonView.
        /// You can use photonView.RefreshRpcMonoBehaviourCache() to manually refresh a PhotonView's
        /// list of MonoBehaviours on demand (when a new MonoBehaviour gets added to a networked GameObject, e.g.).
        /// </remarks>
        public static bool UseRpcMonoBehaviourCache;

        private static readonly Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

        private static Dictionary<string, int> rpcShortcuts;  // lookup "table" for the index (shortcut) of an RPC name

        /// <summary>
        /// If an RPC method is implemented as coroutine, it gets started, unless this value is false.
        /// </summary>
        /// <remarks>
        /// As starting coroutines causes a little memnory garbage, you may want to disable this option but it is
        /// also good enough to not return IEnumerable from methods with the attribite PunRPC.
        /// </remarks>
        public static bool RunRpcCoroutines = true;


        // for asynchronous network synched loading.
        private static AsyncOperation _AsyncLevelLoadingOperation;

        private static float _levelLoadingProgress = 0f;

        /// <summary>
        /// Represents the scene loading progress when using LoadLevel().
        /// </summary>
        /// <remarks>
        /// The value is 0 if the app never loaded a scene with LoadLevel().</br>
        /// During async scene loading, the value is between 0 and 1.</br>
        /// Once any scene completed loading, it stays at 1 (signaling "done").</br>
        /// </remarks>
        /// <value>The level loading progress. Ranges from 0 to 1.</value>
        public static float LevelLoadingProgress
        {
            get
            {
                if (_AsyncLevelLoadingOperation != null)
                {
                    _levelLoadingProgress = _AsyncLevelLoadingOperation.progress;
                }
                else if (_levelLoadingProgress > 0f)
                {
                    _levelLoadingProgress = 1f;
                }

                return _levelLoadingProgress;
            }
        }

        /// <summary>
        /// Called when "this client" left a room to clean up.
        /// </summary>
        /// <remarks>
        /// if (Server == ServerConnection.GameServer && (state == ClientState.Disconnecting || state == ClientState.DisconnectingFromGameServer))
        /// </remarks>
        private static void LeftRoomCleanup()
        {
            // Clean up if we were loading asynchronously.
            if (_AsyncLevelLoadingOperation != null)
            {
                _AsyncLevelLoadingOperation.allowSceneActivation = false;
                _AsyncLevelLoadingOperation = null;
            }


            bool wasInRoom = NetworkingClient.CurrentRoom != null;
            // when leaving a room, we clean up depending on that room's settings.
            bool autoCleanupSettingOfRoom = wasInRoom && CurrentRoom.AutoCleanUp;

            allowedReceivingGroups = new HashSet<byte>();
            blockedSendingGroups = new HashSet<byte>();

            // Cleanup all network objects (all spawned PhotonViews, local and remote)
            if (autoCleanupSettingOfRoom || offlineModeRoom != null)
            {
                LocalCleanupAnythingInstantiated(true);
            }
        }


        /// <summary>
        /// Cleans up anything that was instantiated in-game (not loaded with the scene). Resets views that are not destroyed.
        /// </summary>
        // TODO: This method name no longer matches is function. It also resets room object's views.
        internal static void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
        {
            //if (tempInstantiationData.Count > 0)
            //{
            //    Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite ");
            //}

            // Destroy GO's (if we should)
            if (destroyInstantiatedGameObjects)
            {
                // Fill list with Instantiated objects
                HashSet<GameObject> instantiatedGos = new HashSet<GameObject>();
                foreach (PhotonView view in photonViewList.Values)
                {
                    if (view.isRuntimeInstantiated)
                    {
                        instantiatedGos.Add(view.gameObject); // HashSet keeps each object only once
                    }
                    // For non-instantiated objects (scene objects) - reset the view
                    else
                    {
                        view.ResetPhotonView(true);
                    }
                }

                foreach (GameObject go in instantiatedGos)
                {
                    RemoveInstantiatedGO(go, true);
                }
            }

            // photonViewList is cleared of anything instantiated (so scene items are left inside)
            // any other lists can be
            PhotonNetwork.lastUsedViewSubId = 0;
            PhotonNetwork.lastUsedViewSubIdStatic = 0;
        }


        /// <summary>
        /// Resets the PhotonView "lastOnSerializeDataSent" so that "OnReliable" synched PhotonViews send a complete state to new clients (if the state doesnt change, no messages would be send otherwise!).
        /// Note that due to this reset, ALL other players will receive the full OnSerialize.
        /// </summary>
        private static void ResetPhotonViewsOnSerialize()
        {
            foreach (PhotonView photonView in photonViewList.Values)
            {
                photonView.lastOnSerializeDataSent = null;
            }
        }

        // PHOTONVIEW/RPC related
#pragma warning disable 0414
        private static readonly Type typePunRPC = typeof(PunRPC);
        private static readonly Type typePhotonMessageInfo = typeof(PhotonMessageInfo);
        private static readonly object keyByteZero = (byte)0;
        private static readonly object keyByteOne = (byte)1;
        private static readonly object keyByteTwo = (byte)2;
        private static readonly object keyByteThree = (byte)3;
        private static readonly object keyByteFour = (byte)4;
        private static readonly object keyByteFive = (byte)5;
        private static readonly object keyByteSix = (byte)6;
        private static readonly object keyByteSeven = (byte)7;
        private static readonly object keyByteEight = (byte)8;
        private static readonly object[] emptyObjectArray = new object[0];
        private static readonly Type[] emptyTypeArray = new Type[0];
#pragma warning restore 0414

        /// <summary>
        /// Executes a received RPC event
        /// </summary>
        internal static void ExecuteRpc(Hashtable rpcData, Player sender)
        {
            if (rpcData == null || !rpcData.ContainsKey(keyByteZero))
            {
                Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
                return;
            }

            // ts: updated with "flat" event data
            int netViewID = (int)rpcData[keyByteZero]; // LIMITS PHOTONVIEWS&PLAYERS
            int otherSidePrefix = 0;    // by default, the prefix is 0 (and this is not being sent)
            if (rpcData.ContainsKey(keyByteOne))
            {
                otherSidePrefix = (short)rpcData[keyByteOne];
            }


            string inMethodName;
            if (rpcData.ContainsKey(keyByteFive))
            {
                int rpcIndex = (byte)rpcData[keyByteFive];  // LIMITS RPC COUNT
                if (rpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
                {
                    Debug.LogError("Could not find RPC with index: " + rpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
                    return;
                }
                else
                {
                    inMethodName = PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex];
                }
            }
            else
            {
                inMethodName = (string)rpcData[keyByteThree];
            }

            object[] arguments = null;
            if (rpcData.ContainsKey(keyByteFour))
            {
                arguments = (object[])rpcData[keyByteFour];
            }

            PhotonView photonNetview = GetPhotonView(netViewID);
            if (photonNetview == null)
            {
                int viewOwnerId = netViewID / PhotonNetwork.MAX_VIEW_IDS;
                bool owningPv = (viewOwnerId == NetworkingClient.LocalPlayer.ActorNumber);
                bool ownerSent = sender != null && viewOwnerId == sender.ActorNumber;

                if (owningPv)
                {
                    Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! View was/is ours." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender);
                }
                else
                {
                    Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! Was remote PV." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender + " Maybe GO was destroyed but RPC not cleaned up.");
                }
                return;
            }

            if (photonNetview.Prefix != otherSidePrefix)
            {
                Debug.LogError("Received RPC \"" + inMethodName + "\" on viewID " + netViewID + " with a prefix of " + otherSidePrefix + ", our prefix is " + photonNetview.Prefix + ". The RPC has been ignored.");
                return;
            }

            // Get method name
            if (string.IsNullOrEmpty(inMethodName))
            {
                Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
                return;
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Received RPC: " + inMethodName);
            }


            // SetReceiving filtering
            if (photonNetview.Group != 0 && !allowedReceivingGroups.Contains(photonNetview.Group))
            {
                return; // Ignore group
            }

            Type[] argumentsTypes = null;
            if (arguments != null && arguments.Length > 0)
            {
                argumentsTypes = new Type[arguments.Length];
                int i = 0;
                for (int index = 0; index < arguments.Length; index++)
                {
                    object objX = arguments[index];
                    if (objX == null)
                    {
                        argumentsTypes[i] = null;
                    }
                    else
                    {
                        argumentsTypes[i] = objX.GetType();
                    }

                    i++;
                }
            }


            int receivers = 0;
            int foundMethods = 0;
            if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonNetview.RpcMonoBehaviours == null || photonNetview.RpcMonoBehaviours.Length == 0)
            {
                photonNetview.RefreshRpcMonoBehaviourCache();
            }

            for (int componentsIndex = 0; componentsIndex < photonNetview.RpcMonoBehaviours.Length; componentsIndex++)
            {
                MonoBehaviour monob = photonNetview.RpcMonoBehaviours[componentsIndex];
                if (monob == null)
                {
                    Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                    continue;
                }

                Type type = monob.GetType();

                // Get [PunRPC] methods from cache
                List<MethodInfo> cachedRPCMethods = null;
                bool methodsOfTypeInCache = monoRPCMethodsCache.TryGetValue(type, out cachedRPCMethods);

                if (!methodsOfTypeInCache)
                {
                    List<MethodInfo> entries = SupportClassPun.GetMethods(type, typePunRPC);

                    monoRPCMethodsCache[type] = entries;
                    cachedRPCMethods = entries;
                }

                if (cachedRPCMethods == null)
                {
                    continue;
                }

                // Check cache for valid methodname+arguments
                for (int index = 0; index < cachedRPCMethods.Count; index++)
                {
                    MethodInfo mInfo = cachedRPCMethods[index];
                    if (!mInfo.Name.Equals(inMethodName))
                    {
                        continue;
                    }

                    ParameterInfo[] parameters = mInfo.GetCachedParemeters();
                    foundMethods++;


                    // if we got no arguments:
                    if (arguments == null)
                    {
                        if (parameters.Length == 0)
                        {
                            receivers++;
                            object o = mInfo.Invoke((object)monob, null);
                            if (PhotonNetwork.RunRpcCoroutines)
                            {
                                IEnumerator ie = null;//o as IEnumerator;
                                if ((ie = o as IEnumerator) != null)
                                {
                                    PhotonHandler.Instance.StartCoroutine(ie);
                                }
                            }
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PhotonMessageInfo))
                        {
                            int sendTime = (int)rpcData[keyByteTwo];

                            receivers++;
                            object o = mInfo.Invoke((object)monob, new object[] { new PhotonMessageInfo(sender, sendTime, photonNetview) });
                            if (PhotonNetwork.RunRpcCoroutines)
                            {
                                IEnumerator ie = null;//o as IEnumerator;
                                if ((ie = o as IEnumerator) != null)
                                {
                                    PhotonHandler.Instance.StartCoroutine(ie);
                                }
                            }
                        }
                        continue;
                    }


                    // if there are any arguments (in the incoming call check if the method is compatible
                    if (parameters.Length == arguments.Length)
                    {
                        // Normal, PhotonNetworkMessage left out
                        if (CheckTypeMatch(parameters, argumentsTypes))
                        {
                            receivers++;
                            object o = mInfo.Invoke((object)monob, arguments);
                            if (PhotonNetwork.RunRpcCoroutines)
                            {
                                IEnumerator ie = null;//o as IEnumerator;
                                if ((ie = o as IEnumerator) != null)
                                {
                                    PhotonHandler.Instance.StartCoroutine(ie);
                                }
                            }
                        }
                        continue;
                    }

                    if (parameters.Length == arguments.Length + 1)
                    {
                        // Check for PhotonNetworkMessage being the last
                        if (parameters[parameters.Length - 1].ParameterType == typeof(PhotonMessageInfo) && CheckTypeMatch(parameters, argumentsTypes))
                        {
                            int sendTime = (int)rpcData[keyByteTwo];
                            object[] argumentsWithInfo = new object[arguments.Length + 1];
                            arguments.CopyTo(argumentsWithInfo, 0);
                            argumentsWithInfo[argumentsWithInfo.Length - 1] = new PhotonMessageInfo(sender, sendTime, photonNetview);

                            receivers++;
                            object o = mInfo.Invoke((object)monob, argumentsWithInfo);
                            if (PhotonNetwork.RunRpcCoroutines)
                            {
                                IEnumerator ie = null;//o as IEnumerator;
                                if ((ie = o as IEnumerator) != null)
                                {
                                    PhotonHandler.Instance.StartCoroutine(ie);
                                }
                            }
                        }
                        continue;
                    }

                    if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
                    {
                        receivers++;
                        object o = mInfo.Invoke((object)monob, new object[] { arguments });
                        if (PhotonNetwork.RunRpcCoroutines)
                        {
                            IEnumerator ie = null;//o as IEnumerator;
                            if ((ie = o as IEnumerator) != null)
                            {
                                PhotonHandler.Instance.StartCoroutine(ie);
                            }
                        }
                        continue;
                    }
                }
            }

            // Error handling
            if (receivers != 1)
            {
                string argsString = string.Empty;
                int argsLength = 0;
                if (argumentsTypes != null)
                {
                    argsLength = argumentsTypes.Length;
                    for (int index = 0; index < argumentsTypes.Length; index++)
                    {
                        Type ty = argumentsTypes[index];
                        if (argsString != string.Empty)
                        {
                            argsString += ", ";
                        }

                        if (ty == null)
                        {
                            argsString += "null";
                        }
                        else
                        {
                            argsString += ty.Name;
                        }
                    }
                }

                GameObject context = photonNetview != null ? photonNetview.gameObject : null;
                if (receivers == 0)
                {
                    if (foundMethods == 0)
                    {
                        // found no method that matches
                        Debug.LogErrorFormat(context, "RPC method '{0}({2})' not found on object with PhotonView {1}. Implement as non-static. Apply [PunRPC]. Components on children are not found. " +
                            "Return type must be void or IEnumerator (if you enable RunRpcCoroutines). RPCs are a one-way message.", inMethodName, netViewID, argsString);
                    }
                    else
                    {
                        // found a method but not the right arguments
                        Debug.LogErrorFormat(context, "RPC method '{0}' found on object with PhotonView {1} but has wrong parameters. Implement as '{0}({2})'. PhotonMessageInfo is optional as final parameter." +
                            "Return type must be void or IEnumerator (if you enable RunRpcCoroutines).", inMethodName, netViewID, argsString);
                    }
                }
                else
                {
                    // multiple components have the same method
                    Debug.LogErrorFormat(context, "RPC method '{0}({2})' found {3}x on object with PhotonView {1}. Only one component should implement it." +
                            "Return type must be void or IEnumerator (if you enable RunRpcCoroutines).", inMethodName, netViewID, argsString, foundMethods);
                }
            }
        }

        /// <summary>
        /// Check if all types match with parameters. We can have more paramters then types (allow last RPC type to be different).
        /// </summary>
        /// <param name="methodParameters"></param>
        /// <param name="callParameterTypes"></param>
        /// <returns>If the types-array has matching parameters (of method) in the parameters array (which may be longer).</returns>
        private static bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
        {
            if (methodParameters.Length < callParameterTypes.Length)
            {
                return false;
            }

            for (int index = 0; index < callParameterTypes.Length; index++)
            {
#if NETFX_CORE
                TypeInfo methodParamTI = methodParameters[index].ParameterType.GetTypeInfo();
                TypeInfo callParamTI = callParameterTypes[index].GetTypeInfo();

                if (callParameterTypes[index] != null && !methodParamTI.IsAssignableFrom(callParamTI) && !(callParamTI.IsEnum && System.Enum.GetUnderlyingType(methodParamTI.AsType()).GetTypeInfo().IsAssignableFrom(callParamTI)))
                {
                    return false;
                }
#else
                Type type = methodParameters[index].ParameterType;
                if (callParameterTypes[index] != null && !type.IsAssignableFrom(callParameterTypes[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameterTypes[index])))
                {
                    return false;
                }
#endif
            }

            return true;
        }


        /// <summary>
        /// Destroys all Instantiates and RPCs locally and (if not localOnly) sends EvDestroy(player) and clears related events in the server buffer.
        /// </summary>
        public static void DestroyPlayerObjects(int playerId, bool localOnly)
        {
            if (playerId <= 0)
            {
                Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
                return;
            }

            if (!localOnly)
            {
                // clean server's Instantiate and RPC buffers
                OpRemoveFromServerInstantiationsOfPlayer(playerId);
                OpCleanActorRpcBuffer(playerId);

                // send Destroy(player) to anyone else
                SendDestroyOfPlayer(playerId);
            }

            // locally cleaning up that player's objects
            HashSet<GameObject> playersGameObjects = new HashSet<GameObject>();

            // with ownership transfer, some objects might lose their owner.
            // in that case, the creator becomes the owner again. every client can apply  done below.
            foreach (PhotonView view in photonViewList.Values)
            {
                if (view == null)
                {
                    Debug.LogError("Null view");
                    continue;
                }

                // Mark player created objects for destruction
                if (view.CreatorActorNr == playerId)
                {
                    playersGameObjects.Add(view.gameObject);
                    continue;
                }

                if (view.OwnerActorNr == playerId)
                {
                    var previousOwner = view.Owner;
                    view.OwnerActorNr = view.CreatorActorNr;
                    view.ControllerActorNr = view.CreatorActorNr;

                    // This callback was not originally here. Added with the IsMine caching changes.
                    if (PhotonNetwork.OnOwnershipTransferedEv != null)
                    {
                        PhotonNetwork.OnOwnershipTransferedEv(view, previousOwner);
                    }
                }
            }

            // any non-local work is already done, so with the list of that player's objects, we can clean up (locally only)
            foreach (GameObject gameObject in playersGameObjects)
            {
                RemoveInstantiatedGO(gameObject, true);
            }
        }

        public static void DestroyAll(bool localOnly)
        {
            if (!localOnly)
            {
                OpRemoveCompleteCache();
                SendDestroyOfAll();
            }

            LocalCleanupAnythingInstantiated(true);
        }

        internal static List<PhotonView> foundPVs = new List<PhotonView>();

        /// <summary>Removes GameObject and the PhotonViews on it from local lists and optionally updates remotes. GameObject gets destroyed at end.</summary>
        /// <remarks>
        /// This method might fail and quit early due to several tests.
        /// </remarks>
        /// <param name="go">GameObject to cleanup.</param>
        /// <param name="localOnly">For localOnly, tests of control are skipped and the server is not updated.</param>
        internal static void RemoveInstantiatedGO(GameObject go, bool localOnly)
        {
            // Avoid cleanup if we are quitting.
            if (ConnectionHandler.AppQuits)
                return;

            if (go == null)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
                return;
            }

            // Don't remove the GO if it doesn't have any PhotonView
            go.GetComponentsInChildren<PhotonView>(true, foundPVs);
            if (foundPVs.Count <= 0)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
                return;
            }

            PhotonView viewZero = foundPVs[0];

            // Don't remove GOs that are owned by others (unless this is the master and the remote player left)
            if (!localOnly)
            {
                //Debug.LogWarning("Destroy " + instantiationId + " creator " + creatorId, go);
                if (!viewZero.IsMine)
                {
                    Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor MasterClient taking over for owner who left: " + viewZero);
                    foundPVs.Clear();   // as foundPVs is re-used, clean it to avoid lingering references
                    return;
                }
            }

            // cleanup instantiation (event and local list)
            if (!localOnly)
            {
                ServerCleanInstantiateAndDestroy(viewZero); // server cleaning
            }

            int creatorActorNr = viewZero.CreatorActorNr;

            // cleanup PhotonViews and their RPCs events (if not localOnly)
            for (int j = foundPVs.Count - 1; j >= 0; j--)
            {
                PhotonView view = foundPVs[j];
                if (view == null)
                {
                    continue;
                }

                // TODO: Probably should have a enum that defines when auto-detachment should occur.
                // Check nested PVs for different creator. Detach if different, to avoid destroying reparanted objects.
                if (j != 0)
                {
                    // view does not belong to the same object as the root PV - unparent this nested PV to avoid destruction.
                    if (view.CreatorActorNr != creatorActorNr)
                    {
                        view.transform.SetParent(null, true);
                        continue;
                    }
                }

                // Notify all children PVs of impending destruction. Send the root PV (the actual object getting destroyed) to the callbacks.
                view.OnPreNetDestroy(viewZero);

                // we only destroy/clean PhotonViews that were created by PhotonNetwork.Instantiate (and those have an instantiationId!)
                if (view.InstantiationId >= 1)
                {
                    LocalCleanPhotonView(view);
                }
                if (!localOnly)
                {
                    OpCleanRpcBuffer(view);
                }
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Network destroy Instantiated GO: " + go.name);
            }
            
            foundPVs.Clear();           // as foundPVs is re-used, clean it to avoid lingering references

            go.SetActive(false);        // PUN 2 disables objects before the return to the pool
            prefabPool.Destroy(go);     // PUN 2 always uses a PrefabPool (even for the default implementation)
        }


        private static readonly ExitGames.Client.Photon.Hashtable removeFilter = new ExitGames.Client.Photon.Hashtable();
        private static readonly ExitGames.Client.Photon.Hashtable ServerCleanDestroyEvent = new ExitGames.Client.Photon.Hashtable();
        private static readonly RaiseEventOptions ServerCleanOptions = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };

        internal static RaiseEventOptions SendToAllOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
        internal static RaiseEventOptions SendToOthersOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.Others };
        internal static RaiseEventOptions SendToSingleOptions = new RaiseEventOptions() { TargetActors = new int[1] };

        /// <summary>
        /// Removes an instantiation event from the server's cache. Needs id and actorNr of player who instantiated.
        /// </summary>
        private static void ServerCleanInstantiateAndDestroy(PhotonView photonView)
        {
            int filterId;
            if (photonView.isRuntimeInstantiated)
            {
                filterId = photonView.InstantiationId; // actual, live InstantiationIds start with 1 and go up
                // remove the Instantiate-event from the server cache:
                removeFilter[keyByteSeven] = filterId;
                ServerCleanOptions.CachingOption = EventCaching.RemoveFromRoomCache;
                PhotonNetwork.RaiseEventInternal(PunEvent.Instantiation, removeFilter, ServerCleanOptions, SendOptions.SendReliable);
            }
            // Don't remove the Instantiation from the server, if it doesn't have a proper ID
            else
            {
                filterId = photonView.ViewID;
            }

            // send a Destroy-event to everyone (removing an event from the cache, doesn't send this to anyone else):
            ServerCleanDestroyEvent[keyByteZero] = filterId;
            ServerCleanOptions.CachingOption = photonView.isRuntimeInstantiated ? EventCaching.DoNotCache : EventCaching.AddToRoomCacheGlobal;   // if the view got loaded with the scene, cache EvDestroy for anyone (re)joining later

            PhotonNetwork.RaiseEventInternal(PunEvent.Destroy, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendReliable);
        }

        private static void SendDestroyOfPlayer(int actorNr)
        {
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData[keyByteZero] = actorNr;

            PhotonNetwork.RaiseEventInternal(PunEvent.DestroyPlayer, evData, null, SendOptions.SendReliable);
        }

        private static void SendDestroyOfAll()
        {
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData[keyByteZero] = -1;

            PhotonNetwork.RaiseEventInternal(PunEvent.DestroyPlayer, evData, null, SendOptions.SendReliable);
        }

        private static void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
        {
            // removes all "Instantiation" events of player actorNr. this is not an event for anyone else
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } };
            PhotonNetwork.RaiseEventInternal(PunEvent.Instantiation, null, options, SendOptions.SendReliable);
        }

        internal static void RequestOwnership(int viewID, int fromOwner)
        {
            //Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
            PhotonNetwork.RaiseEventInternal(PunEvent.OwnershipRequest, new int[] { viewID, fromOwner }, SendToAllOptions, SendOptions.SendReliable);
        }

        internal static void TransferOwnership(int viewID, int playerID)
        {
            //Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
            PhotonNetwork.RaiseEventInternal(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, SendToAllOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Call this on the Master to reassert ownership on clients. viewOwnerPairs are [viewId][viewOwnerActorNr] pairs. targetActor of -1 indicates send to all others.
        /// </summary>
        internal static void OwnershipUpdate(int[] viewOwnerPairs, int targetActor = -1)
        {
            RaiseEventOptions opts;
            if (targetActor == -1)
            {
                opts = SendToOthersOptions;
            }
            else
            {
                SendToSingleOptions.TargetActors[0] = targetActor;
                opts = SendToSingleOptions;
            }
            PhotonNetwork.RaiseEventInternal(PunEvent.OwnershipUpdate, viewOwnerPairs, opts, SendOptions.SendReliable);
        }

        public static bool LocalCleanPhotonView(PhotonView view)
        {
            view.removedFromLocalViewList = true;
            return photonViewList.Remove(view.ViewID);
        }

        public static PhotonView GetPhotonView(int viewID)
        {
            PhotonView result = null;
            photonViewList.TryGetValue(viewID, out result);

            /// Removed aggressive find that likely had no real use case, and was expensive.
            //if (result == null)
            //{
            //    PhotonView[] views = GameObject.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];

            //    for (int i = 0; i < views.Length; i++)
            //    {
            //        PhotonView view = views[i];
            //        if (view.ViewID == viewID)
            //        {
            //            if (view.didAwake)
            //            {
            //                Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + view);
            //            }
            //            return view;
            //        }
            //    }
            //}

            return result;
        }

        public static void RegisterPhotonView(PhotonView netView)
        {
            if (!Application.isPlaying)
            {
                photonViewList = new NonAllocDictionary<int, PhotonView>();
                return;
            }

            if (netView.ViewID == 0)
            {
                // don't register views with ID 0 (not initialized). they register when a ID is assigned later on
                Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
                return;
            }

            PhotonView listedView = null;
            bool isViewListed = photonViewList.TryGetValue(netView.ViewID, out listedView);
            if (isViewListed)
            {
                // if some other view is in the list already, we got a problem. it might be indestructible. print out error
                if (netView != listedView)
                {
                    Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.ViewID, netView, listedView));
                }
                else
                {
                    return;
                }

                RemoveInstantiatedGO(listedView.gameObject, true);
            }

            // Debug.Log("adding view to known list: " + netView);
            photonViewList.Add(netView.ViewID, netView);
            netView.removedFromLocalViewList = false;

            //Debug.LogError("view being added. " + netView);	// Exit Games internal log

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Registered PhotonView: " + netView.ViewID);
            }
        }


        /// <summary>
        /// Removes the RPCs of someone else (to be used as master).
        /// This won't clean any local caches. It just tells the server to forget a player's RPCs and instantiates.
        /// </summary>
        /// <param name="actorNumber"></param>
        public static void OpCleanActorRpcBuffer(int actorNumber)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
            PhotonNetwork.RaiseEventInternal(PunEvent.RPC, null, options, SendOptions.SendReliable);
        }

        /// <summary>
        /// Instead removing RPCs or Instantiates, this removed everything cached by the actor.
        /// </summary>
        /// <param name="actorNumber"></param>
        public static void OpRemoveCompleteCacheOfPlayer(int actorNumber)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
            PhotonNetwork.RaiseEventInternal(0, null, options, SendOptions.SendReliable);
        }


        public static void OpRemoveCompleteCache()
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.MasterClient };
            PhotonNetwork.RaiseEventInternal(0, null, options, SendOptions.SendReliable);
        }

        /// This clears the cache of any player/actor who's no longer in the room (making it a simple clean-up option for a new master)
        private static void RemoveCacheOfLeftPlayers()
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[ParameterCode.Code] = (byte)0;		// any event
            opParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;    // option to clear the room cache of all events of players who left

            NetworkingClient.LoadBalancingPeer.SendOperation((byte)OperationCode.RaiseEvent, opParameters, SendOptions.SendReliable);   // TODO: Check if this is the best implementation possible
        }

        // Remove RPCs of view (if they are local player's RPCs)
        public static void CleanRpcBufferIfMine(PhotonView view)
        {
            if (view.OwnerActorNr != NetworkingClient.LocalPlayer.ActorNumber && !NetworkingClient.LocalPlayer.IsMasterClient)
            {
                Debug.LogError("Cannot remove cached RPCs on a PhotonView thats not ours! " + view.Owner + " scene: " + view.IsRoomView);
                return;
            }

            OpCleanRpcBuffer(view);
        }


        private static readonly Hashtable rpcFilterByViewId = new ExitGames.Client.Photon.Hashtable();
        private static readonly RaiseEventOptions OpCleanRpcBufferOptions = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };

        /// <summary>Cleans server RPCs for PhotonView (without any further checks).</summary>
        public static void OpCleanRpcBuffer(PhotonView view)
        {
            rpcFilterByViewId[keyByteZero] = view.ViewID;
            PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcFilterByViewId, OpCleanRpcBufferOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Remove all buffered RPCs from server that were sent in the targetGroup, if this is the Master Client or if this controls the individual PhotonView.
        /// </summary>
        /// <remarks>
        /// This method requires either:
        /// - This client is the Master Client (can remove any RPCs per group).
        /// - Any other client: each PhotonView is checked if it is under this client's control. Only those RPCs are removed.
        /// </remarks>
        /// <param name="group">Interest group that gets all RPCs removed.</param>
        public static void RemoveRPCsInGroup(int group)
        {
            foreach (PhotonView view in photonViewList.Values)
            {
                if (view.Group == group)
                {
                    CleanRpcBufferIfMine(view);
                }
            }
        }

        /// <summary>
        /// Clear buffered RPCs based on filter parameters.
        /// </summary>
        /// <param name="viewId">The viewID of the PhotonView where the RPC has been called on. We actually need its ViewID. If 0 (default) is provided, all PhotonViews/ViewIDs are considered.</param>
        /// <param name="methodName">The RPC method name, if possible we will use its hash shortcut for efficiency. If none (null or empty string) is provided all RPC method names are considered.</param>
        /// <param name="callersActorNumbers">The actor numbers of the players who called/buffered the RPC. For example if two players buffered the same RPC you can clear the buffered RPC of one and keep the other. If none (null or empty array) is provided all senders are considered.</param>
        /// <returns>If the operation could be sent to the server.</returns>
        public static bool RemoveBufferedRPCs(int viewId = 0, string methodName = null, int[] callersActorNumbers = null/*, params object[] parameters*/)
        {
            Hashtable filter = new Hashtable(2);
            if (viewId != 0)
            {
                filter[keyByteZero] = viewId;
            }
            if (!string.IsNullOrEmpty(methodName))
            {
                // send name or shortcut (if available)
                int shortcut;
                if (rpcShortcuts.TryGetValue(methodName, out shortcut))
                {
                    filter[keyByteFive] = (byte)shortcut; // LIMITS RPC COUNT
                }
                else
                {
                    filter[keyByteThree] = methodName;
                }
            }
            //if (parameters != null && parameters.Length > 0)
            //{
            //    filter[keyByteFour] = parameters;
            //}
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
            if (callersActorNumbers != null)
            {
                raiseEventOptions.TargetActors = callersActorNumbers;
            }
            return RaiseEventInternal(PunEvent.RPC, filter, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Sets level prefix for PhotonViews instantiated later on. Don't set it if you need only one!
        /// </summary>
        /// <remarks>
        /// Important: If you don't use multiple level prefixes, simply don't set this value. The
        /// default value is optimized out of the traffic.
        ///
        /// This won't affect existing PhotonViews (they can't be changed yet for existing PhotonViews).
        ///
        /// Messages sent with a different level prefix will be received but not executed. This affects
        /// RPCs, Instantiates and synchronization.
        ///
        /// Be aware that PUN never resets this value, you'll have to do so yourself.
        /// </remarks>
        /// <param name="prefix">Max value is short.MaxValue = 255</param>
        public static void SetLevelPrefix(byte prefix)
        {
            // TODO: check can use network

            currentLevelPrefix = prefix;
            // TODO: should we really change the prefix for existing PVs?! better keep it!
            //foreach (PhotonView view in photonViewList.Values)
            //{
            //    view.prefix = prefix;
            //}
        }


        /// RPC Hashtable Structure
        /// (byte)0 -> (int) ViewId (combined from actorNr and actor-unique-id)
        /// (byte)1 -> (short) prefix (level)
        /// (byte)2 -> (int) server timestamp
        /// (byte)3 -> (string) methodname
        /// (byte)4 -> (object[]) parameters
        /// (byte)5 -> (byte) method shortcut (alternative to name)
        ///
        /// This is sent as event (code: 200) which will contain a sender (origin of this RPC).

        static ExitGames.Client.Photon.Hashtable rpcEvent = new ExitGames.Client.Photon.Hashtable();
        static RaiseEventOptions RpcOptionsToAll = new RaiseEventOptions();


        internal static void RPC(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
        {
            if (blockedSendingGroups.Contains(view.Group))
            {
                return; // Block sending on this group
            }

            if (view.ViewID < 1)
            {
                Debug.LogError("Illegal view ID:" + view.ViewID + " method: " + methodName + " GO:" + view.gameObject.name);
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Sending RPC \"" + methodName + "\" to target: " + target + " or player:" + player + ".");
            }


            //ts: changed RPCs to a one-level hashtable as described in internal.txt
            rpcEvent.Clear();

            rpcEvent[keyByteZero] = (int)view.ViewID; // LIMITS NETWORKVIEWS&PLAYERS
            if (view.Prefix > 0)
            {
                rpcEvent[keyByteOne] = (short)view.Prefix;
            }
            rpcEvent[keyByteTwo] = PhotonNetwork.ServerTimestamp;


            // send name or shortcut (if available)
            int shortcut = 0;
            if (rpcShortcuts.TryGetValue(methodName, out shortcut))
            {
                rpcEvent[keyByteFive] = (byte)shortcut; // LIMITS RPC COUNT
            }
            else
            {
                rpcEvent[keyByteThree] = methodName;
            }

            if (parameters != null && parameters.Length > 0)
            {
                rpcEvent[keyByteFour] = (object[])parameters;
            }

            SendOptions sendOptions = new SendOptions() { Reliability = true, Encrypt = encrypt };

            // if sent to target player, this overrides the target
            if (player != null)
            {
                if (NetworkingClient.LocalPlayer.ActorNumber == player.ActorNumber)
                {
                    ExecuteRpc(rpcEvent, player);
                }
                else
                {
                    RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { player.ActorNumber } };
                    PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                    // NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
                }

                return;
            }

            switch (target)
            {
                // send to a specific set of players
                case RpcTarget.All:
                    RpcOptionsToAll.InterestGroup = (byte)view.Group;   // NOTE: Test-wise, this is static and re-used to avoid memory garbage
                    PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, RpcOptionsToAll, sendOptions);

                    // Execute local
                    ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                    break;
                case RpcTarget.Others:
                    {
                        RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group };
                        PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                        break;
                    }
                case RpcTarget.AllBuffered:
                    {
                        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache };
                        PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);

                        // Execute local
                        ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                        break;
                    }
                case RpcTarget.OthersBuffered:
                    {
                        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache };
                        PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                        break;
                    }
                case RpcTarget.MasterClient:
                    {
                        if (NetworkingClient.LocalPlayer.IsMasterClient)
                        {
                            ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                        }
                        else
                        {
                            RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
                            PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                        }

                        break;
                    }
                case RpcTarget.AllViaServer:
                    {
                        RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group, Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                        if (PhotonNetwork.OfflineMode)
                        {
                            ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                        }

                        break;
                    }
                case RpcTarget.AllBufferedViaServer:
                    {
                        RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group, Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
                        PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, sendOptions);
                        if (PhotonNetwork.OfflineMode)
                        {
                            ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                        }

                        break;
                    }
                default:
                    Debug.LogError("Unsupported target enum: " + target);
                    break;
            }
        }


        /// <summary>Enable/disable receiving on given Interest Groups (applied to PhotonViews).</summary>
        /// <remarks>
        /// A client can tell the server which Interest Groups it's interested in.
        /// The server will only forward events for those Interest Groups to that client (saving bandwidth and performance).
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/gameplay/interestgroups
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/package-demos/culling-demo
        /// </remarks>
        /// <param name="disableGroups">The interest groups to disable (or null).</param>
        /// <param name="enableGroups">The interest groups to enable (or null).</param>
        public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
        {
            // TODO: check can use network

            if (disableGroups != null)
            {
                if (disableGroups.Length == 0)
                {
                    // a byte[0] should disable ALL groups in one step and before any groups are enabled. we do this locally, too.
                    allowedReceivingGroups.Clear();
                }
                else
                {
                    for (int index = 0; index < disableGroups.Length; index++)
                    {
                        byte g = disableGroups[index];
                        if (g <= 0)
                        {
                            Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The Group number should be at least 1.");
                            continue;
                        }

                        if (allowedReceivingGroups.Contains(g))
                        {
                            allowedReceivingGroups.Remove(g);
                        }
                    }
                }
            }

            if (enableGroups != null)
            {
                if (enableGroups.Length == 0)
                {
                    // a byte[0] should enable ALL groups in one step. we do this locally, too.
                    for (byte index = 0; index < byte.MaxValue; index++)
                    {
                        allowedReceivingGroups.Add(index);
                    }

                    allowedReceivingGroups.Add(byte.MaxValue);
                }
                else
                {
                    for (int index = 0; index < enableGroups.Length; index++)
                    {
                        byte g = enableGroups[index];
                        if (g <= 0)
                        {
                            Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The Group number should be at least 1.");
                            continue;
                        }

                        allowedReceivingGroups.Add(g);
                    }
                }
            }

            if (!PhotonNetwork.offlineMode)
            {
                NetworkingClient.OpChangeGroups(disableGroups, enableGroups);
            }
        }


        /// <summary>Enable/disable sending on given group (applied to PhotonViews)</summary>
        /// <remarks>
        /// This does not interact with the Photon server-side.
        /// It's just a client-side setting to suppress updates, should they be sent to one of the blocked groups.
        ///
        /// This setting is not particularly useful, as it means that updates literally never reach the server or anyone else.
        /// Use with care.
        /// </remarks>
        /// <param name="group">The interest group to affect.</param>
        /// <param name="enabled">Sets if sending to group is enabled (or not).</param>
        public static void SetSendingEnabled(byte group, bool enabled)
        {
            // TODO: check can use network

            if (!enabled)
            {
                blockedSendingGroups.Add(group); // can be added to HashSet no matter if already in it
            }
            else
            {
                blockedSendingGroups.Remove(group);
            }
        }



        /// <summary>Enable/disable sending on given groups (applied to PhotonViews)</summary>
        /// <remarks>
        /// This does not interact with the Photon server-side.
        /// It's just a client-side setting to suppress updates, should they be sent to one of the blocked groups.
        ///
        /// This setting is not particularly useful, as it means that updates literally never reach the server or anyone else.
        /// Use with care.
        /// <param name="enableGroups">The interest groups to enable sending on (or null).</param>
        /// <param name="disableGroups">The interest groups to disable sending on (or null).</param>
        public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
        {
            // TODO: check can use network

            if (disableGroups != null)
            {
                for (int index = 0; index < disableGroups.Length; index++)
                {
                    byte g = disableGroups[index];
                    blockedSendingGroups.Add(g);
                }
            }

            if (enableGroups != null)
            {
                for (int index = 0; index < enableGroups.Length; index++)
                {
                    byte g = enableGroups[index];
                    blockedSendingGroups.Remove(g);
                }
            }
        }


        internal static void NewSceneLoaded()
        {
            if (loadingLevelAndPausedNetwork)
            {
                _AsyncLevelLoadingOperation = null;
                loadingLevelAndPausedNetwork = false;
                PhotonNetwork.IsMessageQueueRunning = true;
            }
            else
            {
                PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
            }

            // Debug.Log("OnLevelWasLoaded photonViewList.Count: " + photonViewList.Count); // Exit Games internal log

            List<int> removeKeys = new List<int>();
            foreach (KeyValuePair<int, PhotonView> kvp in photonViewList)
            {
                PhotonView view = kvp.Value;
                if (view == null)
                {
                    removeKeys.Add(kvp.Key);
                }
            }

            for (int index = 0; index < removeKeys.Count; index++)
            {

                int key = removeKeys[index];
                photonViewList.Remove(key);
            }

            if (removeKeys.Count > 0)
            {
                if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                    Debug.Log("New level loaded. Removed " + removeKeys.Count + " scene view IDs from last level.");
            }
        }


        /// <summary>
        /// Defines how many updated produced by OnPhotonSerialize() are batched into one message.
        /// </summary>
        /// <remarks>
        /// A low number increases overhead, a high number might lead to fragmented messages.
        /// </remarks>
        public static int ObjectsInOneUpdate = 20;


        private static readonly PhotonStream serializeStreamOut = new PhotonStream(true, null);
        private static readonly PhotonStream serializeStreamIn = new PhotonStream(false, null);


        ///<summary> cache the RaiseEventOptions to prevent redundant Memory Allocation</summary>
        private static RaiseEventOptions serializeRaiseEvOptions = new RaiseEventOptions();

        private struct RaiseEventBatch : IEquatable<RaiseEventBatch>
        {
            public byte Group;
            public bool Reliable;

            public override int GetHashCode()
            {
                return (this.Group << 1) + (this.Reliable ? 1 : 0);
            }

            public bool Equals(RaiseEventBatch other)
            {
                return this.Reliable == other.Reliable && this.Group == other.Group;
            }
        }


        private class SerializeViewBatch : IEquatable<SerializeViewBatch>, IEquatable<RaiseEventBatch>
        {
            public readonly RaiseEventBatch Batch;
            public List<object> ObjectUpdates;
            private int defaultSize = PhotonNetwork.ObjectsInOneUpdate;
            private int offset;


            // the offset enables us to skip the first X entries in the ObjectUpdate(s), leaving room for (e.g.) timestamp of sending and level prefix
            public SerializeViewBatch(RaiseEventBatch batch, int offset)
            {
                this.Batch = batch;
                this.ObjectUpdates = new List<object>(this.defaultSize);
                this.offset = offset;
                for (int i = 0; i < offset; i++) this.ObjectUpdates.Add(null);
            }

            public override int GetHashCode()
            {
                return (this.Batch.Group << 1) + (this.Batch.Reliable ? 1 : 0);
            }

            public bool Equals(SerializeViewBatch other)
            {
                return this.Equals(other.Batch);
            }

            public bool Equals(RaiseEventBatch other)
            {
                return this.Batch.Reliable == other.Reliable && this.Batch.Group == other.Group;
            }

            public override bool Equals(object obj)
            {
                SerializeViewBatch other = obj as SerializeViewBatch;
                return other != null && this.Batch.Equals(other.Batch);
            }

            public void Clear()
            {
                this.ObjectUpdates.Clear();
                for (int i = 0; i < offset; i++) this.ObjectUpdates.Add(null);
            }

            public void Add(List<object> viewData)
            {
                if (this.ObjectUpdates.Count >= this.ObjectUpdates.Capacity)
                {
                    // NOTE: we could also trim to new size
                    throw new Exception("Can't add. Size exceeded.");
                }

                this.ObjectUpdates.Add(viewData);
            }
        }


        private static readonly Dictionary<RaiseEventBatch, SerializeViewBatch> serializeViewBatches = new Dictionary<RaiseEventBatch, SerializeViewBatch>();


        /// <summary>Calls all locally controlled PhotonViews to write their updates in OnPhotonSerializeView. Called by a PhotonHandler.</summary>
        internal static void RunViewUpdate()
        {
            if (PhotonNetwork.OfflineMode || CurrentRoom == null || CurrentRoom.Players == null)
            {
                return;
            }


            // no need to send OnSerialize messages while being alone (these are not buffered anyway)
#if !PHOTON_DEVELOP
            if (CurrentRoom.Players.Count <= 1)
            {
                return;
            }
#else
            serializeRaiseEvOptions.Receivers = (CurrentRoom.Players.Count == 1) ? ReceiverGroup.All : ReceiverGroup.Others;
#endif



            /* Format of the event's data object[]:
             *  [0] = PhotonNetwork.ServerTimestamp;
             *  [1] = currentLevelPrefix;  OPTIONAL!
             *  [2] = object[] of PhotonView x
             *  [3] = object[] of PhotonView y or NULL
             *  [...]
             *
             *  We only combine updates for XY objects into one RaiseEvent to avoid fragmentation.
             *  The Reliability and Interest Group are only used for RaiseEvent and not contained in the event/data that reaches the other clients.
             *  This is read in OnEvent().
             */


            var enumerator = photonViewList.GetEnumerator();   // replacing foreach (PhotonView view in this.photonViewList.Values) for memory allocation improvement
            while (enumerator.MoveNext())
            {
                PhotonView view = enumerator.Current.Value;

                // a client only sends updates for active, synchronized PhotonViews that are under it's control (isMine)
                if (view.Synchronization == ViewSynchronization.Off || view.IsMine == false || view.isActiveAndEnabled == false)
                {
                    continue;
                }

                if (blockedSendingGroups.Contains(view.Group))
                {
                    continue; // Block sending on this group
                }


                // call the PhotonView's serialize method(s)
                List<object> evData = OnSerializeWrite(view);
                if (evData == null)
                {
                    continue;
                }

                RaiseEventBatch eventBatch = new RaiseEventBatch();
                eventBatch.Reliable = view.Synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable;
                eventBatch.Group = view.Group;

                SerializeViewBatch svBatch = null;
                bool found = serializeViewBatches.TryGetValue(eventBatch, out svBatch);
                if (!found)
                {
                    svBatch = new SerializeViewBatch(eventBatch, 2);    // NOTE: the 2 first entries are kept empty for timestamp and level prefix
                    serializeViewBatches.Add(eventBatch, svBatch);
                }

                svBatch.Add(evData);
                if (svBatch.ObjectUpdates.Count == svBatch.ObjectUpdates.Capacity)
                {
                    SendSerializeViewBatch(svBatch);
                }
            }

            var enumeratorB = serializeViewBatches.GetEnumerator();
            while (enumeratorB.MoveNext())
            {
                SendSerializeViewBatch(enumeratorB.Current.Value);
            }
        }


        private static void SendSerializeViewBatch(SerializeViewBatch batch)
        {
            if (batch == null || batch.ObjectUpdates.Count <= 2)
            {
                return;
            }

            serializeRaiseEvOptions.InterestGroup = batch.Batch.Group;
            batch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp;
            batch.ObjectUpdates[1] = (currentLevelPrefix != 0) ? (object)currentLevelPrefix : null;
            byte code = batch.Batch.Reliable ? PunEvent.SendSerializeReliable : PunEvent.SendSerialize;

            PhotonNetwork.RaiseEventInternal(code, batch.ObjectUpdates, serializeRaiseEvOptions, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            batch.Clear();
        }


        // calls OnPhotonSerializeView (through ExecuteOnSerialize)
        // the content created here is consumed by receivers in: ReadOnSerialize
        private static List<object> OnSerializeWrite(PhotonView view)
        {
            if (view.Synchronization == ViewSynchronization.Off)
            {
                return null;
            }


            // each view creates a list of values that should be sent
            PhotonMessageInfo info = new PhotonMessageInfo(NetworkingClient.LocalPlayer, PhotonNetwork.ServerTimestamp, view);

            if (view.syncValues == null) view.syncValues = new List<object>();
            view.syncValues.Clear();
            serializeStreamOut.SetWriteStream(view.syncValues);
            serializeStreamOut.SendNext(null);  //to become: viewID,
            serializeStreamOut.SendNext(null);  //to become: is compressed
            serializeStreamOut.SendNext(null);  //to become: null-values (for compression) followed by: values for this object's update


            view.SerializeView(serializeStreamOut, info);

            // check if there are actual values to be sent (after the "header" of viewId, (bool)compressed and (int[])nullValues)
            if (serializeStreamOut.Count <= SyncFirstValue)
            {
                return null;
            }


            List<object> currentValues = serializeStreamOut.GetWriteStream();
            currentValues[SyncViewId] = view.ViewID;
            currentValues[SyncCompressed] = false;      // (bool) compression was used.
            currentValues[SyncNullValues] = null;       // if reliable compressed, this is non-null.
                                                        // next: sequence of values in this object's update.

            if (view.Synchronization == ViewSynchronization.Unreliable)
            {
                return currentValues;
            }


            // ViewSynchronization: Off, Unreliable, UnreliableOnChange, ReliableDeltaCompressed
            if (view.Synchronization == ViewSynchronization.UnreliableOnChange)
            {
                if (AlmostEquals(currentValues, view.lastOnSerializeDataSent))
                {
                    if (view.mixedModeIsReliable)
                    {
                        return null;
                    }

                    view.mixedModeIsReliable = true;
                    List<object> temp = view.lastOnSerializeDataSent;   // TODO: extract "exchange" into method in PV
                    view.lastOnSerializeDataSent = currentValues;
                    view.syncValues = temp;
                }
                else
                {
                    view.mixedModeIsReliable = false;
                    List<object> temp = view.lastOnSerializeDataSent;   // TODO: extract "exchange" into method in PV
                    view.lastOnSerializeDataSent = currentValues;
                    view.syncValues = temp;
                }


                return currentValues;
            }

            if (view.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
            {
                // TODO: fix delta compression / comparison

                // compress content of data set (by comparing to view.lastOnSerializeDataSent)
                // the "original" dataArray is NOT modified by DeltaCompressionWrite
                List<object> dataToSend = DeltaCompressionWrite(view.lastOnSerializeDataSent, currentValues);

                // cache the values that were written this time (not the compressed values)
                List<object> temp = view.lastOnSerializeDataSent;   // TODO: extract "exchange" into method in PV
                view.lastOnSerializeDataSent = currentValues;
                view.syncValues = temp;

                return dataToSend;
            }

            return null;
        }

        /// <summary>
        /// Reads updates created by OnSerializeWrite
        /// </summary>
        private static void OnSerializeRead(object[] data, Player sender, int networkTime, short correctPrefix)
        {
            // read view ID from key (byte)0: a int-array (PUN 1.17++)
            int viewID = (int)data[SyncViewId];


            // debug:
            //LogObjectArray(data);

            PhotonView view = GetPhotonView(viewID);
            if (view == null)
            {
                Debug.LogWarning("Received OnSerialization for view ID " + viewID + ". We have no such PhotonView! Ignore this if you're joining or leaving a room. State: " + NetworkingClient.State);
                return;
            }

            if (view.Prefix > 0 && correctPrefix != view.Prefix)
            {
                Debug.LogError("Received OnSerialization for view ID " + viewID + " with prefix " + correctPrefix + ". Our prefix is " + view.Prefix);
                return;
            }

            // SetReceiving filtering
            if (view.Group != 0 && !allowedReceivingGroups.Contains(view.Group))
            {
                return; // Ignore group
            }




            if (view.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
            {
                object[] uncompressed = DeltaCompressionRead(view.lastOnSerializeDataReceived, data);
                //LogObjectArray(uncompressed,"uncompressed ");
                if (uncompressed == null)
                {
                    // Skip this packet as we haven't got received complete-copy of this view yet.
                    if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                    {
                        Debug.Log("Skipping packet for " + view.name + " [" + view.ViewID +
                                  "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game.");
                    }
                    return;
                }

                // store last received values (uncompressed) for delta-compression usage
                view.lastOnSerializeDataReceived = uncompressed;
                data = uncompressed;
            }

            // TODO: re-check if ownership needs to be adjusted based on updates.
            // most likely, only the PhotonView.Controller should be affected, if anything at all.
            // TODO: find a way to sync the owner of a PV for late joiners.

            //// This is when joining late to assign ownership to the sender
            //// this has nothing to do with reading the actual synchronization update.
            //// We don't do anything if OwnerShip Was Touched, which means we got the infos already. We only possibly act if ownership was never transfered.
            //// We do override OwnershipWasTransfered if owner is the masterClient.
            //if (sender.ID != view.OwnerActorNr && (!view.OwnershipWasTransfered || view.OwnerActorNr == 0) && view.currentMasterID == -1)
            //{
            //    // obviously the owner changed and we didn't yet notice.
            //    //Debug.Log("Adjusting owner to sender of updates. From: " + view.OwnerActorNr + " to: " + sender.ID);
            //    view.OwnerActorNr = sender.ID;
            //}

            serializeStreamIn.SetReadStream(data, 3);
            PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, view);

            view.DeserializeView(serializeStreamIn, info);
        }


        // compresses currentContent by using NULL as value if currentContent equals previousContent
        // skips initial indexes, as defined by SyncFirstValue
        // to conserve memory, the previousContent is re-used as buffer for the result! duplicate the values before using this, if needed
        // returns null, if nothing must be sent (current content might be null, which also returns null)
        // SyncFirstValue should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
        public const int SyncViewId = 0;
        public const int SyncCompressed = 1;
        public const int SyncNullValues = 2;
        public const int SyncFirstValue = 3;

        private static List<object> DeltaCompressionWrite(List<object> previousContent, List<object> currentContent)
        {
            if (currentContent == null || previousContent == null || previousContent.Count != currentContent.Count)
            {
                return currentContent; // the current data needs to be sent (which might be null)
            }

            if (currentContent.Count <= SyncFirstValue)
            {
                return null; // this send doesn't contain values (except the "headers"), so it's not being sent
            }

            List<object> compressedContent = previousContent; // the previous content is no longer needed, once we compared the values!
            compressedContent[SyncCompressed] = false;
            int compressedValues = 0;

            Queue<int> valuesThatAreChangedToNull = null;
            for (int index = SyncFirstValue; index < currentContent.Count; index++)
            {
                object newObj = currentContent[index];
                object oldObj = previousContent[index];
                if (AlmostEquals(newObj, oldObj))
                {
                    // compress (by using null, instead of value, which is same as before)
                    compressedValues++;
                    compressedContent[index] = null;
                }
                else
                {
                    compressedContent[index] = newObj;

                    // value changed, we don't replace it with null
                    // new value is null (like a compressed value): we have to mark it so it STAYS null instead of being replaced with previous value
                    if (newObj == null)
                    {
                        if (valuesThatAreChangedToNull == null)
                        {
                            valuesThatAreChangedToNull = new Queue<int>(currentContent.Count);
                        }
                        valuesThatAreChangedToNull.Enqueue(index);
                    }
                }
            }

            // Only send the list of compressed fields if we actually compressed 1 or more fields.
            if (compressedValues > 0)
            {
                if (compressedValues == currentContent.Count - SyncFirstValue)
                {
                    // all values are compressed to null, we have nothing to send
                    return null;
                }

                compressedContent[SyncCompressed] = true;
                if (valuesThatAreChangedToNull != null)
                {
                    compressedContent[SyncNullValues] = valuesThatAreChangedToNull.ToArray(); // data that is actually null (not just cause we didn't want to send it)
                }
            }

            compressedContent[SyncViewId] = currentContent[SyncViewId];
            return compressedContent; // some data was compressed but we need to send something
        }


        private static object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
        {
            if ((bool)incomingData[SyncCompressed] == false)
            {
                // index 1 marks "compressed" as being true.
                return incomingData;
            }

            // Compression was applied (as data[1] == true)
            // we need a previous "full" list of values to restore values that are null in this msg. else, ignore this
            if (lastOnSerializeDataReceived == null)
            {
                return null;
            }


            int[] indexesThatAreChangedToNull = incomingData[2] as int[];
            for (int index = SyncFirstValue; index < incomingData.Length; index++)
            {
                if (indexesThatAreChangedToNull != null && indexesThatAreChangedToNull.Contains(index))
                {
                    continue; // if a value was set to null in this update, we don't need to fetch it from an earlier update
                }
                if (incomingData[index] == null)
                {
                    // we replace null values in this received msg unless a index is in the "changed to null" list
                    object lastValue = lastOnSerializeDataReceived[index];
                    incomingData[index] = lastValue;
                }
            }

            return incomingData;
        }


        // startIndex should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
        // returns the incomingData with modified content. any object being null (means: value unchanged) gets replaced with a previously sent value. incomingData is being modified


        private static bool AlmostEquals(IList<object> lastData, IList<object> currentContent)
        {
            if (lastData == null && currentContent == null)
            {
                return true;
            }

            if (lastData == null || currentContent == null || (lastData.Count != currentContent.Count))
            {
                return false;
            }

            for (int index = 0; index < currentContent.Count; index++)
            {
                object newObj = currentContent[index];
                object oldObj = lastData[index];
                if (!AlmostEquals(newObj, oldObj))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if both objects are almost identical.
        /// Used to check whether two objects are similar enough to skip an update.
        /// </summary>
        static bool AlmostEquals(object one, object two)
        {
            if (one == null || two == null)
            {
                return one == null && two == null;
            }

            if (!one.Equals(two))
            {
                // if A is not B, lets check if A is almost B
                if (one is Vector3)
                {
                    Vector3 a = (Vector3)one;
                    Vector3 b = (Vector3)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForVectorSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is Vector2)
                {
                    Vector2 a = (Vector2)one;
                    Vector2 b = (Vector2)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForVectorSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is Quaternion)
                {
                    Quaternion a = (Quaternion)one;
                    Quaternion b = (Quaternion)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForQuaternionSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is float)
                {
                    float a = (float)one;
                    float b = (float)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForFloatSynchronization))
                    {
                        return true;
                    }
                }

                // one does not equal two
                return false;
            }

            return true;
        }

        // NOTE: Might be used as replacement for the equivalent method in SupportClass.
        internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
        {
            mi = null;

            if (monob == null || string.IsNullOrEmpty(methodType))
            {
                return false;
            }

            List<MethodInfo> methods = SupportClassPun.GetMethods(monob.GetType(), null);
            for (int index = 0; index < methods.Count; index++)
            {
                MethodInfo methodInfo = methods[index];
                if (methodInfo.Name.Equals(methodType))
                {
                    mi = methodInfo;
                    return true;
                }
            }

            return false;
        }


        /// <summary>Internally used to detect the current scene and load it if PhotonNetwork.AutomaticallySyncScene is enabled.</summary>
        internal static void LoadLevelIfSynced()
        {
            if (!PhotonNetwork.AutomaticallySyncScene || PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            // check if "current level" is set in props
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {
                return;
            }

            // if loaded level is not the one defined by master in props, load that level
            object sceneId = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];
            if (sceneId is int)
            {
                if (SceneManagerHelper.ActiveSceneBuildIndex != (int)sceneId)
                {
                    PhotonNetwork.LoadLevel((int)sceneId);
                }
            }
            else if (sceneId is string)
            {
                if (SceneManagerHelper.ActiveSceneName != (string)sceneId)
                {
                    PhotonNetwork.LoadLevel((string)sceneId);
                }
            }
        }


        internal static void SetLevelInPropsIfSynced(object levelId)
        {
            if (!PhotonNetwork.AutomaticallySyncScene || !PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }
            if (levelId == null)
            {
                Debug.LogError("Parameter levelId can't be null!");
                return;
            }


            // check if "current level" is already set in the room properties (then we don't set it again)
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {
                object levelIdInProps = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];
                //Debug.Log("levelId (to set): "+ levelId + " levelIdInProps: " + levelIdInProps + " SceneManagerHelper.ActiveSceneName: "+ SceneManagerHelper.ActiveSceneName);

                if (levelId.Equals(levelIdInProps))
                {
                    //Debug.LogWarning("The levelId equals levelIdInProps. Don't set property again.");
                    return;
                }
                else
                {
                    // if the new levelId does not equal the level in properties, there is a chance that build index and scene name refer to the same scene.
                    // as Unity does not provide all scenes with build index, we only check for the currently loaded scene (with a high chance this is the correct one).
                    int scnIndex = SceneManagerHelper.ActiveSceneBuildIndex;
                    string scnName = SceneManagerHelper.ActiveSceneName;

                    if ((levelId.Equals(scnIndex) && levelIdInProps.Equals(scnName)) || (levelId.Equals(scnName) && levelIdInProps.Equals(scnIndex)))
                    {
                        //Debug.LogWarning("The levelId and levelIdInProps refer to the same scene. Don't set property for it.");
                        return;
                    }
                }
            }


            // if the new levelId does not match the current room-property, we can cancel existing loading (as we start a new one)
            if (_AsyncLevelLoadingOperation != null)
            {
                if (!_AsyncLevelLoadingOperation.isDone)
                {
                    Debug.LogWarning("PUN cancels an ongoing async level load, as another scene should be loaded. Next scene to load: " + levelId);
                }

                _AsyncLevelLoadingOperation.allowSceneActivation = false;
                _AsyncLevelLoadingOperation = null;
            }


            // current level is not yet in props, or different, so this client has to set it
            Hashtable setScene = new Hashtable();
            if (levelId is int) setScene[CurrentSceneProperty] = (int)levelId;
            else if (levelId is string) setScene[CurrentSceneProperty] = (string)levelId;
            else Debug.LogError("Parameter levelId must be int or string!");

            PhotonNetwork.CurrentRoom.SetCustomProperties(setScene);
            SendAllOutgoingCommands(); // send immediately! because: in most cases the client will begin to load and pause sending anything for a while
        }


        private static void OnEvent(EventData photonEvent)
        {
            int actorNr = photonEvent.Sender;
            Player originatingPlayer = null;
            if (actorNr > 0 && NetworkingClient.CurrentRoom != null)
            {
                originatingPlayer = NetworkingClient.CurrentRoom.GetPlayer(actorNr);
            }

            switch (photonEvent.Code)
            {
                case EventCode.Join:
                    ResetPhotonViewsOnSerialize();
                    break;

                case PunEvent.RPC:
                    ExecuteRpc(photonEvent.CustomData as Hashtable, originatingPlayer);
                    break;

                case PunEvent.SendSerialize:
                case PunEvent.SendSerializeReliable:
                    // Debug.Log(photonEvent.ToStringFull());

                    /* This case must match definition in RunViewUpdate() and OnSerializeWrite().
                     * Format of the event's data object[]:
                     *  [0] = PhotonNetwork.ServerTimestamp;
                     *  [1] = currentLevelPrefix;  OPTIONAL!
                     *  [2] = object[] of PhotonView x
                     *  [3] = object[] of PhotonView y or NULL
                     *  [...]
                     *
                     *  We only combine updates for XY objects into one RaiseEvent to avoid fragmentation.
                     *  The Reliability and Interest Group are only used for RaiseEvent and not contained in the event/data that reaches the other clients.
                     *  This is read in OnEvent().
                     */

                    object[] pvUpdates = (object[])photonEvent[ParameterCode.Data];
                    int remoteUpdateServerTimestamp = (int)pvUpdates[0];
                    short remoteLevelPrefix = (pvUpdates[1] != null) ? (byte)pvUpdates[1] : (short)0;

                    object[] viewUpdate = null;
                    for (int i = 2; i < pvUpdates.Length; i++)
                    {
                        viewUpdate = pvUpdates[i] as object[];
                        if (viewUpdate == null)
                        {
                            break;
                        }
                        OnSerializeRead(viewUpdate, originatingPlayer, remoteUpdateServerTimestamp, remoteLevelPrefix);
                    }
                    break;

                case PunEvent.Instantiation:
                    NetworkInstantiate((Hashtable)photonEvent.CustomData, originatingPlayer);
                    break;

                case PunEvent.CloseConnection:

                    // MasterClient "requests" a disconnection from us
                    if (PhotonNetwork.EnableCloseConnection == false)
                    {
                        Debug.LogWarning("CloseConnection received from " + originatingPlayer + ". PhotonNetwork.EnableCloseConnection is false. Ignoring the request (this client stays in the room).");
                    }
                    else if (originatingPlayer == null || !originatingPlayer.IsMasterClient)
                    {
                        Debug.LogWarning("CloseConnection received from " + originatingPlayer + ". That player is not the Master Client. " + PhotonNetwork.MasterClient + " is.");
                    }
                    else if (PhotonNetwork.EnableCloseConnection)
                    {
                        PhotonNetwork.LeaveRoom(false);
                    }

                    break;

                case PunEvent.DestroyPlayer:
                    Hashtable evData = (Hashtable)photonEvent.CustomData;
                    int targetPlayerId = (int)evData[keyByteZero];
                    if (targetPlayerId >= 0)
                    {
                        DestroyPlayerObjects(targetPlayerId, true);
                    }
                    else
                    {
                        DestroyAll(true);
                    }
                    break;

                case EventCode.Leave:

                    // destroy objects & buffered messages
                    if (CurrentRoom != null && CurrentRoom.AutoCleanUp && (originatingPlayer == null || !originatingPlayer.IsInactive))
                    {
                        DestroyPlayerObjects(actorNr, true);
                    }
                    break;

                case PunEvent.Destroy:
                    evData = (Hashtable)photonEvent.CustomData;
                    int instantiationId = (int)evData[keyByteZero];
                    // Debug.Log("Ev Destroy for viewId: " + instantiationId + " sent by owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == actorNr) + " this client is owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == this.LocalPlayer.ID));


                    PhotonView pvToDestroy = null;
                    if (photonViewList.TryGetValue(instantiationId, out pvToDestroy))
                    {
                        RemoveInstantiatedGO(pvToDestroy.gameObject, true);
                    }
                    else
                    {
                        Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + instantiationId + ". Sent by actorNr: " + actorNr);
                    }

                    break;

                case PunEvent.OwnershipRequest:
                    {
                        int[] requestValues = (int[])photonEvent.CustomData;
                        int requestedViewId = requestValues[0];
                        int requestedFromOwnerId = requestValues[1];


                        PhotonView requestedView = GetPhotonView(requestedViewId);
                        if (requestedView == null)
                        {
                            Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + requestedViewId);
                            break;
                        }

                        if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                        {
                            Debug.Log(string.Format("OwnershipRequest. actorNr {0} requests view {1} from {2}. current pv owner: {3} is {4}. isMine: {6} master client: {5}", actorNr, requestedViewId, requestedFromOwnerId, requestedView.OwnerActorNr, requestedView.IsOwnerActive ? "active" : "inactive", MasterClient.ActorNumber, requestedView.IsMine));
                        }

                        switch (requestedView.OwnershipTransfer)
                        {
                            case OwnershipOption.Takeover:
                                int currentPvOwnerId = requestedView.OwnerActorNr;
                                if (requestedFromOwnerId == currentPvOwnerId || (requestedFromOwnerId == 0 && currentPvOwnerId == MasterClient.ActorNumber) || currentPvOwnerId == 0)
                                {
                                    // a takeover is successful automatically, if taken from current owner
                                    Player prevOwner = requestedView.Owner;

                                    requestedView.OwnerActorNr = actorNr;
                                    requestedView.ControllerActorNr = actorNr;

                                    if (PhotonNetwork.OnOwnershipTransferedEv != null)
                                    {
                                        PhotonNetwork.OnOwnershipTransferedEv(requestedView, prevOwner);
                                    }
                                }
                                else
                                {

                                    if (PhotonNetwork.OnOwnershipTransferFailedEv != null)
                                    {
                                        PhotonNetwork.OnOwnershipTransferFailedEv(requestedView, originatingPlayer);
                                    }
                                    //Debug.LogWarning("requestedView.OwnershipTransfer was ignored! ");
                                }
                                break;

                            case OwnershipOption.Request:
                                if (PhotonNetwork.OnOwnershipRequestEv != null)
                                {
                                    PhotonNetwork.OnOwnershipRequestEv(requestedView, originatingPlayer);
                                }
                                break;

                            default:
                                Debug.LogWarning("Ownership mode == " + (requestedView.OwnershipTransfer) + ". Ignoring request.");
                                break;
                        }
                    }
                    break;

                case PunEvent.OwnershipTransfer:
                    {
                        int[] transferViewToUserID = (int[])photonEvent.CustomData;
                        int requestedViewId = transferViewToUserID[0];
                        int newOwnerId = transferViewToUserID[1];

                        if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                        {
                            Debug.Log("Ev OwnershipTransfer. ViewID " + requestedViewId + " to: " + newOwnerId + " Time: " + Environment.TickCount % 1000);
                        }

                        PhotonView requestedView = GetPhotonView(requestedViewId);
                        if (requestedView != null)
                        {
                            // Only apply this if pv allows Takeover, or allows Request and this message originates from the controller or owner.
                            if (requestedView.OwnershipTransfer == OwnershipOption.Takeover ||
                                (requestedView.OwnershipTransfer == OwnershipOption.Request && (originatingPlayer == requestedView.Controller || originatingPlayer == requestedView.Owner)))
                            {
                                Player prevOwner = requestedView.Owner;

                                requestedView.OwnerActorNr= newOwnerId;
                                requestedView.ControllerActorNr = newOwnerId;

                                if (PhotonNetwork.OnOwnershipTransferedEv != null)
                                {
                                    PhotonNetwork.OnOwnershipTransferedEv(requestedView, prevOwner);
                                }
                            }
                            else if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                            {
                                if (requestedView.OwnershipTransfer == OwnershipOption.Request)
                                    Debug.Log("Failed incoming OwnershipTransfer attempt for '" + requestedView.name + "; " + requestedViewId +
                                              " - photonView has OwnershipTransfer set to OwnershipOption.Request, but Player attempting to change owner is not the current owner/controller.");
                                else
                                    Debug.Log("Failed incoming OwnershipTransfer attempt for '" + requestedView.name + "; " + requestedViewId +
                                              " - photonView has OwnershipTransfer set to OwnershipOption.Fixed.");
                            }
                        }
                        else if (PhotonNetwork.LogLevel >= PunLogLevel.ErrorsOnly)
                        {
                            Debug.LogErrorFormat("Failed to find a PhotonView with ID={0} for incoming OwnershipTransfer event (newOwnerActorNumber={1}), sender={2}",
                                                 requestedViewId, newOwnerId, actorNr);
                        }

                        break;
                    }

                case PunEvent.OwnershipUpdate:
                    {
                        reusablePVHashset.Clear();

                        // Deserialize the list of exceptions, these are views on the master who's Owner and Creator didn't match.
                        int[] viewOwnerPair = (int[])photonEvent.CustomData;

                        for (int i = 0, cnt = viewOwnerPair.Length; i < cnt; i++)
                        {
                            int viewId = viewOwnerPair[i];
                            i++;
                            int newOwnerId = viewOwnerPair[i];

                            PhotonView view = GetPhotonView(viewId);
                            if (view == null)
                            {
                                if (PhotonNetwork.LogLevel >= PunLogLevel.ErrorsOnly)
                                {
                                    Debug.LogErrorFormat("Failed to find a PhotonView with ID={0} for incoming OwnershipUpdate event (newOwnerActorNumber={1}), sender={2}. If you load scenes, make sure to pause the message queue.", viewId, newOwnerId, actorNr);
                                }

                                continue;
                            }

                            Player prevOwner = view.Owner;
                            Player newOwner = CurrentRoom.GetPlayer(newOwnerId, true);

                            view.OwnerActorNr= newOwnerId;
                            view.ControllerActorNr = newOwnerId;

                            reusablePVHashset.Add(view);
                            // If this produces an owner change locally, fire the OnOwnershipTransfered callbacks
                            if (PhotonNetwork.OnOwnershipTransferedEv != null && newOwner != prevOwner)
                            {
                                PhotonNetwork.OnOwnershipTransferedEv(view, prevOwner);
                            }
                        }

                        // Initialize all views. Typically this is just fired on a new client after it joins a room and gets the first OwnershipUpdate from the Master.
                        // This was moved from PhotonHandler OnJoinedRoom to here, to allow objects to retain controller = -1 until an controller is actually known.
                        foreach (var view in PhotonViewCollection)
                        {
                            if (!reusablePVHashset.Contains(view))
                                view.RebuildControllerCache();
                        }

                        break;
                    }


            }
        }

        private static void OnOperation(OperationResponse opResponse)
        {
            switch (opResponse.OperationCode)
            {
                case OperationCode.GetRegions:
                    if (opResponse.ReturnCode != 0)
                    {
                        if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
                        {
                            Debug.Log("OpGetRegions failed. Will not ping any. ReturnCode: " + opResponse.ReturnCode);
                        }
                        return;
                    }
                    if (ConnectMethod == ConnectMethod.ConnectToBest)
                    {
                        string previousBestRegionSummary = PhotonNetwork.BestRegionSummaryInPreferences;

                        if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                        {
                            Debug.Log("PUN got region list. Going to ping minimum regions, based on this previous result summary: " + previousBestRegionSummary);
                        }
                        NetworkingClient.RegionHandler.PingMinimumOfRegions(OnRegionsPinged, previousBestRegionSummary);
                    }
                    break;
                case OperationCode.JoinGame:
                    if (Server == ServerConnection.GameServer)
                    {
                        PhotonNetwork.LoadLevelIfSynced();
                    }
                    break;
            }
        }

        private static void OnClientStateChanged(ClientState previousState, ClientState state)
        {
            if (
                (previousState == ClientState.Joined && state == ClientState.Disconnected) ||
                (Server == ServerConnection.GameServer && (state == ClientState.Disconnecting || state == ClientState.DisconnectingFromGameServer))
            )
            {
                LeftRoomCleanup();
            }

            if (state == ClientState.ConnectedToMasterServer && _cachedRegionHandler != null)
            {
                BestRegionSummaryInPreferences = _cachedRegionHandler.SummaryToCache;
                _cachedRegionHandler = null;
            }
        }

        // to be used in the main thread. as OnRegionsPinged is called in a separate thread and so we can't use some of the Unity methods (like saving playerPrefs)
        private static RegionHandler _cachedRegionHandler;

        private static void OnRegionsPinged(RegionHandler regionHandler)
        {
            if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
            {
                Debug.Log(regionHandler.GetResults());
            }

            _cachedRegionHandler = regionHandler;
            //PhotonNetwork.BestRegionSummaryInPreferences = regionHandler.SummaryToCache; // can not be called here, as it's not in the main thread


            // the dev region overrides the best region selection in "development" builds (unless it was set but is empty).

#if UNITY_EDITOR
            if (!PhotonServerSettings.DevRegionSetOnce)
            {
                // if no dev region was defined before or if the dev region is unavailable, set a new dev region
                PhotonServerSettings.DevRegionSetOnce = true;
                PhotonServerSettings.DevRegion = _cachedRegionHandler.BestRegion.Code;
            }
#endif

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!string.IsNullOrEmpty(PhotonServerSettings.DevRegion) && ConnectMethod == ConnectMethod.ConnectToBest)
            {
                Debug.LogWarning("PUN is in development mode (development build). As the 'dev region' is not empty (" + PhotonServerSettings.DevRegion + ") it overrides the found best region. See PhotonServerSettings.");

                string _finalDevRegion = PhotonServerSettings.DevRegion;
                if (!_cachedRegionHandler.EnabledRegions.Any(p => p.Code == PhotonServerSettings.DevRegion))
                {
                    _finalDevRegion = _cachedRegionHandler.EnabledRegions[0].Code;

                    Debug.LogWarning("The 'dev region' (" + PhotonServerSettings.DevRegion + ") was not found in the enabled regions, the first enabled region is picked (" + _finalDevRegion + ")");
                }

                PhotonNetwork.NetworkingClient.ConnectToRegionMaster(_finalDevRegion);
                return;
            }
#endif

            if (NetworkClientState == ClientState.ConnectedToNameServer)
            {
                PhotonNetwork.NetworkingClient.ConnectToRegionMaster(regionHandler.BestRegion.Code);
            }
        }
    }
}
