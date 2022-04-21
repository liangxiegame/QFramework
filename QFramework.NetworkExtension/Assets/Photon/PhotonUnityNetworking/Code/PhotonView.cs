// ----------------------------------------------------------------------------
// <copyright file="PhotonView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Contains the PhotonView class.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;
    using System.Collections.Generic;
    using Photon.Realtime;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    /// <summary>
    /// A PhotonView identifies an object across the network (viewID) and configures how the controlling client updates remote instances.
    /// </summary>
    /// \ingroup publicApi
    [AddComponentMenu("Photon Networking/Photon View")]
    public class PhotonView : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void SetPhotonViewExecutionOrder()
        {
            int photonViewExecutionOrder = -16000;
            GameObject go = new GameObject();
            PhotonView pv = go.AddComponent<PhotonView>();
            MonoScript monoScript = MonoScript.FromMonoBehaviour(pv);

            if (photonViewExecutionOrder != MonoImporter.GetExecutionOrder(monoScript))
            {
                MonoImporter.SetExecutionOrder(monoScript, photonViewExecutionOrder); // very early but allows other scripts to run even earlier...
            }

            DestroyImmediate(go); 
        }
        #endif

        #if UNITY_EDITOR
        [ContextMenu("Open PUN Wizard")]
        void OpenPunWizard()
        {
            EditorApplication.ExecuteMenuItem("Window/Photon Unity Networking/PUN Wizard");
        }
        #endif

        #if UNITY_EDITOR
        // Suppressing compiler warning "this variable is never used". Only used in the CustomEditor, only in Editor
        #pragma warning disable 0414
        [SerializeField]
        bool ObservedComponentsFoldoutOpen = true;
        #pragma warning restore 0414
        #endif
        
        #if UNITY_EDITOR
        /// called by Editor to reset the component
        private void Reset()
        {
            observableSearch = ObservableSearch.AutoFindAll;
        }
        #endif



        [FormerlySerializedAs("group")]
        public byte Group = 0;

        // NOTE: this is now an integer because unity won't serialize short (needed for instantiation). we SEND only a short though!
        // NOTE: prefabs have a prefixField of -1. this is replaced with any currentLevelPrefix that's used at runtime. instantiated GOs get their prefix set pre-instantiation (so those are not -1 anymore)
        public int Prefix
        {
            get
            {
                if (this.prefixField == -1 && PhotonNetwork.NetworkingClient != null)
                {
                    this.prefixField = PhotonNetwork.currentLevelPrefix;
                }

                return this.prefixField;
            }
            set { this.prefixField = value; }
        }

        // this field is serialized by unity. that means it is copied when instantiating a persistent obj into the scene
        [FormerlySerializedAs("prefixBackup")]
        public int prefixField = -1;



        /// <summary>
        /// This is the InstantiationData that was passed when calling PhotonNetwork.Instantiate* (if that was used to spawn this prefab)
        /// </summary>
        public object[] InstantiationData
        {
            get { return this.instantiationDataField; }
            protected internal set { this.instantiationDataField = value; }
        }

        internal object[] instantiationDataField;

        /// <summary>
        /// For internal use only, don't use
        /// </summary>
        protected internal List<object> lastOnSerializeDataSent = null;
        protected internal List<object> syncValues;

        /// <summary>
        /// For internal use only, don't use
        /// </summary>
        protected internal object[] lastOnSerializeDataReceived = null;

        [FormerlySerializedAs("synchronization")]
        public ViewSynchronization Synchronization = ViewSynchronization.UnreliableOnChange;

        protected internal bool mixedModeIsReliable = false;

        /// <summary>Defines if ownership of this PhotonView is fixed, can be requested or simply taken.</summary>
        /// <remarks>
        /// Note that you can't edit this value at runtime.
        /// The options are described in enum OwnershipOption.
        /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        /// </remarks>
        [FormerlySerializedAs("ownershipTransfer")]
        public OwnershipOption OwnershipTransfer = OwnershipOption.Fixed;


        public enum ObservableSearch { Manual, AutoFindActive, AutoFindAll }

        /// Default to manual so existing PVs in projects default to same as before. Reset() changes this to AutoAll for new implementations.
        public ObservableSearch observableSearch = ObservableSearch.Manual;
        
        public List<Component> ObservedComponents;



        internal MonoBehaviour[] RpcMonoBehaviours;



        [Obsolete("Renamed. Use IsRoomView instead")]
        public bool IsSceneView
        {
            get { return this.IsRoomView; }
        }
        
        /// <summary>True if the PhotonView was loaded with the scene (game object) or instantiated with InstantiateRoomObject.</summary>
        /// <remarks>
        /// Room objects are not owned by a particular player but belong to the scene. Thus they don't get destroyed when their
        /// creator leaves the game and the current Master Client can control them (whoever that is).
        /// The ownerId is 0 (player IDs are 1 and up).
        /// </remarks>
        public bool IsRoomView
        {
            get { return this.CreatorActorNr == 0; }
        }

        public bool IsOwnerActive
        {
            get { return this.Owner != null && !this.Owner.IsInactive; }
        }

        /// <summary>
        /// True if the PhotonView is "mine" and can be controlled by this client.
        /// </summary>
        /// <remarks>
        /// PUN has an ownership concept that defines who can control and destroy each PhotonView.
        /// True in case the controller matches the local Player.
        /// True if this is a scene photonview (null owner and ownerId == 0) on the Master client.
        /// </remarks>
        public bool IsMine { get; private set; }
        public bool AmController
        {
            get { return this.IsMine; }
        }

        public Player Controller { get; private set; }
        
        public int CreatorActorNr { get; private set; }

        public bool AmOwner { get; private set; }

        
        /// <summary>
        /// The owner of a PhotonView is the creator of an object by default Ownership can be transferred and the owner may not be in the room anymore. Objects in the scene don't have an owner.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        ///
        /// Ownership can be transferred to another player with PhotonView.TransferOwnership or any player can request
        /// ownership by calling the PhotonView's RequestOwnership method.
        /// The current owner has to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        /// </remarks>
        public Player Owner { get; private set; }



        [NonSerialized]
        private int ownerActorNr;

        public int OwnerActorNr
        {
            get { return this.ownerActorNr; }
            set
            {
                if (value != 0 && this.ownerActorNr == value)
                {
                    return;
                }

                Player prevOwner = this.Owner;

                this.Owner = PhotonNetwork.CurrentRoom == null ? null : PhotonNetwork.CurrentRoom.GetPlayer(value, true);
                this.ownerActorNr = this.Owner != null ? this.Owner.ActorNumber : value;

                this.AmOwner = PhotonNetwork.LocalPlayer != null && this.ownerActorNr == PhotonNetwork.LocalPlayer.ActorNumber;

                this.UpdateCallbackLists();
                if (!ReferenceEquals(this.OnOwnerChangeCallbacks, null))
                {
                    for (int i = 0, cnt = this.OnOwnerChangeCallbacks.Count; i < cnt; ++i)
                    {
                        this.OnOwnerChangeCallbacks[i].OnOwnerChange(this.Owner, prevOwner);
                    }
                }
            }
        }

        
        [NonSerialized]
        private int controllerActorNr;

        public int ControllerActorNr
        {
            get { return this.controllerActorNr; }
            set
            {
                Player prevController = this.Controller;

                this.Controller = PhotonNetwork.CurrentRoom == null ? null : PhotonNetwork.CurrentRoom.GetPlayer(value, true);
                if (this.Controller != null && this.Controller.IsInactive)
                {
                    this.Controller = PhotonNetwork.MasterClient;
                }
                this.controllerActorNr = this.Controller != null ? this.Controller.ActorNumber : value;

                this.IsMine = PhotonNetwork.LocalPlayer != null && this.controllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber;

                if (!ReferenceEquals(this.Controller, prevController))
                {
                    this.UpdateCallbackLists();
                    if (!ReferenceEquals(this.OnControllerChangeCallbacks, null))
                    {
                        for (int i = 0, cnt = this.OnControllerChangeCallbacks.Count; i < cnt; ++i)
                        {
                            this.OnControllerChangeCallbacks[i].OnControllerChange(this.Controller, prevController);
                        }
                    }
                }
            }
        }


        /// This field is the Scene ViewID (0 if not used). loaded with the scene, used in Awake().
        [SerializeField]
        [FormerlySerializedAs("viewIdField")]
        [HideInInspector]
        public int sceneViewId = 0; // TODO: in best case, this is not public


        /// This field is the "runtime" ViewID as backup for the property.
        [NonSerialized]
        private int viewIdField = 0;
        
        /// <summary>
        /// The ID of the PhotonView. Identifies it in a networked game (per room).
        /// </summary>
        /// <remarks>See: [Network Instantiation](@ref instantiateManual)</remarks>
        public int ViewID
        {
            get
            {
                return this.viewIdField;
            }

            set
            {
                // TODO: Check if the isPlaying check is needed when the PhotonViewHandler is updated
                if (value != 0 && this.viewIdField != 0)
                {
                    Debug.LogWarning("Changing a ViewID while it's in use is not possible (except setting it to 0 (not being used). Current ViewID: " + this.viewIdField);
                    return;
                }
                
                if (value == 0 && this.viewIdField != 0)
                {
                    PhotonNetwork.LocalCleanPhotonView(this);
                }

                this.viewIdField = value;
                this.CreatorActorNr = value / PhotonNetwork.MAX_VIEW_IDS;   // the creator can be derived from the viewId. this is also the initial owner and creator.
                this.OwnerActorNr = this.CreatorActorNr;
                this.ControllerActorNr = this.CreatorActorNr;
                this.RebuildControllerCache();


                // if the viewID is set to a new, legit value, the view should register in the list of active PVs.
                if (value != 0)
                {
                    PhotonNetwork.RegisterPhotonView(this);
                }
            }
        }

        [FormerlySerializedAs("instantiationId")]
        public int InstantiationId; // if the view was instantiated with a GO, this GO has a instantiationID (first view's viewID)

        [SerializeField]
        [HideInInspector]
        public bool isRuntimeInstantiated;


        protected internal bool removedFromLocalViewList;

        
        /// <summary>Will FindObservables() and assign the sceneViewId, if that is != 0. This initializes the PhotonView if loaded with the scene. Called once by Unity, when this instance is created.</summary>
        protected internal void Awake()
        {
            if (this.ViewID != 0)
            {
                return;
            }
            
            if (this.sceneViewId != 0)
            {
                // PhotonNetwork.Instantiate will set a ViewID != 0 before the object awakes. So only objects loaded with the scene ever use the sceneViewId (even if the obj got pooled)
                this.ViewID = this.sceneViewId;
            }

            this.FindObservables();
        }


        /// called by PhotonNetwork.LocalCleanupAnythingInstantiated
        internal void ResetPhotonView(bool resetOwner)
        {
            //// If this was fired by this connection rejoining, reset the ownership cache to owner = creator.
            //// TODO: This reset may not be needed at all with the ownership being invalidated next.
            //if (resetOwner)
            //    ResetOwnership();

            //this.ownershipCacheIsValid = OwnershipCacheState.Invalid;

            // Reset the delta check to force a complete update of owned objects, to ensure joining connections get full updates.
            this.lastOnSerializeDataSent = null;
        }

        
        /// called by OnJoinedRoom, OnMasterClientSwitched, OnPlayerEnteredRoom and OnEvent for OwnershipUpdate
        /// OnPlayerLeftRoom will set a new controller directly, if the controller or owner left
        internal void RebuildControllerCache(bool ownerHasChanged = false)
        {
            //var prevController = this.controller;

            // objects without controller and room objects (ownerId 0) check if controller update is needed
            if (this.controllerActorNr == 0 || this.OwnerActorNr == 0 || this.Owner == null || this.Owner.IsInactive)
            {
                var masterclient = PhotonNetwork.MasterClient;
                this.ControllerActorNr = masterclient == null ? -1 : masterclient.ActorNumber;
            }
            else
            {
                this.ControllerActorNr = this.OwnerActorNr;
            }
        }


        public void OnPreNetDestroy(PhotonView rootView)
        {
            UpdateCallbackLists();

            if (!ReferenceEquals(OnPreNetDestroyCallbacks, null))
                for (int i = 0, cnt = OnPreNetDestroyCallbacks.Count; i < cnt; ++i)
                {
                    OnPreNetDestroyCallbacks[i].OnPreNetDestroy(rootView);
                }
        }

        protected internal void OnDestroy()
        {
            if (!this.removedFromLocalViewList)
            {
                bool wasInList = PhotonNetwork.LocalCleanPhotonView(this);

                if (wasInList && this.InstantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                {
                    Debug.Log("PUN-instantiated '" + this.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
                }
            }
        }


        /// <summary>
        /// Depending on the PhotonView's OwnershipTransfer setting, any client can request to become owner of the PhotonView.
        /// </summary>
        /// <remarks>
        /// Requesting ownership can give you control over a PhotonView, if the OwnershipTransfer setting allows that.
        /// The current owner might have to implement IPunCallbacks.OnOwnershipRequest to react to the ownership request.
        ///
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void RequestOwnership()
        {
            if (OwnershipTransfer != OwnershipOption.Fixed)
            {
                PhotonNetwork.RequestOwnership(this.ViewID, this.ownerActorNr);
            }
            else
            {
                if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                {
                    Debug.LogWarning("Attempting to RequestOwnership of GameObject '" + name + "' viewId: " + ViewID +
                        ", but PhotonView.OwnershipTransfer is set to Fixed.");
                }
            }
        }

        /// <summary>
        /// Transfers the ownership of this PhotonView (and GameObject) to another player.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void TransferOwnership(Player newOwner)
        {
            if (newOwner != null)
                TransferOwnership(newOwner.ActorNumber);
            else
            {
                if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                {
                    Debug.LogWarning("Attempting to TransferOwnership of GameObject '" + name + "' viewId: " + ViewID +
                   ", but provided Player newOwner is null.");
                }
            }
        }

        /// <summary>
        /// Transfers the ownership of this PhotonView (and GameObject) to another player.
        /// </summary>
        /// <remarks>
        /// The owner/controller of a PhotonView is also the client which sends position updates of the GameObject.
        /// </remarks>
        public void TransferOwnership(int newOwnerId)
        {
            if (OwnershipTransfer == OwnershipOption.Takeover || (OwnershipTransfer == OwnershipOption.Request && this.AmController))
            {
                PhotonNetwork.TransferOwnership(this.ViewID, newOwnerId);
            }
            else
            {
                if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                {
                    if (OwnershipTransfer == OwnershipOption.Fixed)
                        Debug.LogWarning("Attempting to TransferOwnership of GameObject '" + name + "' viewId: " + ViewID +
                            " without the authority to do so. TransferOwnership is not allowed if PhotonView.OwnershipTransfer is set to Fixed.");
                    else if (OwnershipTransfer == OwnershipOption.Request)
                        Debug.LogWarning("Attempting to TransferOwnership of GameObject '" + name + "' viewId: " + ViewID +
                           " without the authority to do so. PhotonView.OwnershipTransfer is set to Request, so only the controller of this object can TransferOwnership.");
                }
            }
        }

        /// <summary>
        /// Will find IPunObservable components on this GameObject and nested children and add them to the ObservedComponents list.
        /// </summary>
        /// <remarks>
        /// This is called via PhotonView.Awake(), which in turn is called immediately by the engine's AddComponent method.
        /// 
        /// Changing the ObservedComponents of a PhotonView at runtime can be problematic, if other clients are not also
        /// updating their list.
        /// </remarks>
        /// <param name="force">If true, FindObservables will work as if observableSearch is AutoFindActive.</param>
        public void FindObservables(bool force = false)
        {
            if (!force && this.observableSearch == ObservableSearch.Manual)
            {
                return;
            }

            if (this.ObservedComponents == null)
            {
                this.ObservedComponents = new List<Component>();
            }
            else
            {
                this.ObservedComponents.Clear();
            }

            this.transform.GetNestedComponentsInChildren<Component, IPunObservable, PhotonView>(force || this.observableSearch == ObservableSearch.AutoFindAll, this.ObservedComponents);
        }


        public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
            {
                for (int i = 0; i < this.ObservedComponents.Count; ++i)
                {
                    var component = this.ObservedComponents[i];
                    if (component != null)
                        SerializeComponent(this.ObservedComponents[i], stream, info);
                }
            }
        }

        public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.ObservedComponents != null && this.ObservedComponents.Count > 0)
            {
                for (int i = 0; i < this.ObservedComponents.Count; ++i)
                {
                    var component = this.ObservedComponents[i];
                    if (component != null)
                        DeserializeComponent(component, stream, info);
                }
            }
        }

        protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
        {
            IPunObservable observable = component as IPunObservable;
            if (observable != null)
            {
                observable.OnPhotonSerializeView(stream, info);
            }
            else
            {
                Debug.LogError("Observed scripts have to implement IPunObservable. " + component + " does not. It is Type: " + component.GetType(), component.gameObject);
            }
        }

        protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
        {
            IPunObservable observable = component as IPunObservable;
            if (observable != null)
            {
                observable.OnPhotonSerializeView(stream, info);
            }
            else
            {
                Debug.LogError("Observed scripts have to implement IPunObservable. " + component + " does not. It is Type: " + component.GetType(), component.gameObject);
            }
        }


        /// <summary>
        /// Can be used to refesh the list of MonoBehaviours on this GameObject while PhotonNetwork.UseRpcMonoBehaviourCache is true.
        /// </summary>
        /// <remarks>
        /// Set PhotonNetwork.UseRpcMonoBehaviourCache to true to enable the caching.
        /// Uses this.GetComponents<MonoBehaviour>() to get a list of MonoBehaviours to call RPCs on (potentially).
        ///
        /// While PhotonNetwork.UseRpcMonoBehaviourCache is false, this method has no effect,
        /// because the list is refreshed when a RPC gets called.
        /// </remarks>
        public void RefreshRpcMonoBehaviourCache()
        {
            this.RpcMonoBehaviours = this.GetComponents<MonoBehaviour>();
        }


        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, including this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// RPC calls can target "All" or the "Others".
        /// Usually, the target "All" gets executed locally immediately after sending the RPC.
        /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        /// <param name="target">The group of targets and the way the RPC gets sent.</param>
        /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RPC(string methodName, RpcTarget target, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, target, false, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, including this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// RPC calls can target "All" or the "Others".
        /// Usually, the target "All" gets executed locally immediately after sending the RPC.
        /// The "*ViaServer" options send the RPC to the server and execute it on this client when it's sent back.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        ///<param name="target">The group of targets and the way the RPC gets sent.</param>
        ///<param name="encrypt"> </param>
        ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RpcSecure(string methodName, RpcTarget target, bool encrypt, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, including this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// This method allows you to make an RPC calls on a specific player's client.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        /// <param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        /// <param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
        /// <param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RPC(string methodName, Player targetPlayer, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
        }

        /// <summary>
        /// Call a RPC method of this GameObject on remote clients of this room (or on all, including this client).
        /// </summary>
        /// <remarks>
        /// [Remote Procedure Calls](@ref rpcManual) are an essential tool in making multiplayer games with PUN.
        /// It enables you to make every client in a room call a specific method.
        ///
        /// This method allows you to make an RPC calls on a specific player's client.
        /// Of course, calls are affected by this client's lag and that of remote clients.
        ///
        /// Each call automatically is routed to the same PhotonView (and GameObject) that was used on the
        /// originating client.
        ///
        /// See: [Remote Procedure Calls](@ref rpcManual).
        /// </remarks>
        ///<param name="methodName">The name of a fitting method that was has the RPC attribute.</param>
        ///<param name="targetPlayer">The group of targets and the way the RPC gets sent.</param>
        ///<param name="encrypt"> </param>
        ///<param name="parameters">The parameters that the RPC method has (must fit this call!).</param>
        public void RpcSecure(string methodName, Player targetPlayer, bool encrypt, params object[] parameters)
        {
            PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
        }

        public static PhotonView Get(Component component)
        {
            return component.transform.GetParentComponent<PhotonView>();
        }

        public static PhotonView Get(GameObject gameObj)
        {
            return gameObj.transform.GetParentComponent<PhotonView>();
        }

        /// <summary>
        /// Finds the PhotonView Component with a viewID in the scene
        /// </summary>
        /// <param name="viewID"></param>
        /// <returns>The PhotonView with ViewID. Returns null if none found</returns>
        public static PhotonView Find(int viewID)
        {
            return PhotonNetwork.GetPhotonView(viewID);
        }

        
        #region Callback Interfaces


        private struct CallbackTargetChange
        {
            public IPhotonViewCallback obj;
            public Type type;
            public bool add;

            public CallbackTargetChange(IPhotonViewCallback obj, Type type, bool add)
            {
                this.obj = obj;
                this.type = type;
                this.add = add;
            }
        }

        private Queue<CallbackTargetChange> CallbackChangeQueue = new Queue<CallbackTargetChange>();

        private List<IOnPhotonViewPreNetDestroy> OnPreNetDestroyCallbacks;
        private List<IOnPhotonViewOwnerChange> OnOwnerChangeCallbacks;
        private List<IOnPhotonViewControllerChange> OnControllerChangeCallbacks;

        /// <summary>
        /// Add object to all applicable callback interfaces. Object must implement at least one IOnPhotonViewCallback derived interface.
        /// </summary>
        /// <param name="obj">An object that implements OnPhotonView callback interface(s).</param>
        public void AddCallbackTarget(IPhotonViewCallback obj)
        {
            CallbackChangeQueue.Enqueue(new CallbackTargetChange(obj, null, true));
        }

        /// <summary>
        /// Remove object from all applicable callback interfaces. Object must implement at least one IOnPhotonViewCallback derived interface.
        /// </summary>
        /// <param name="obj">An object that implements OnPhotonView callback interface(s).</param>
        public void RemoveCallbackTarget(IPhotonViewCallback obj)
        {
            CallbackChangeQueue.Enqueue(new CallbackTargetChange(obj, null, false));
        }

        /// <summary>
        /// Add object to this PhotonView's callback.
        /// T is the IOnPhotonViewCallback derived interface you want added to its associated callback list.
        /// Supplying IOnPhotonViewCallback (the interface base class) as T will add ALL implemented IOnPhotonViewCallback Interfaces found on the object.
        /// </summary>
        public void AddCallback<T>(IPhotonViewCallback obj) where T : class, IPhotonViewCallback
        {
            CallbackChangeQueue.Enqueue(new CallbackTargetChange(obj, typeof(T), true));
        }

        /// <summary>
        /// Remove object from this PhotonView's callback list for T.
        /// T is the IOnPhotonViewCallback derived interface you want removed from its associated callback list.
        /// Supplying IOnPhotonViewCallback (the interface base class) as T will remove ALL implemented IOnPhotonViewCallback Interfaces found on the object.
        /// </summary>
        public void RemoveCallback<T>(IPhotonViewCallback obj) where T : class, IPhotonViewCallback
        {
            CallbackChangeQueue.Enqueue(new CallbackTargetChange(obj, typeof(T), false));
        }

        /// <summary>
        /// Apply any queued add/remove of interfaces from the callback lists. Typically called before looping callback lists.
        /// </summary>
        private void UpdateCallbackLists()
        {
            while (CallbackChangeQueue.Count > 0)
            {
                var item = CallbackChangeQueue.Dequeue();
                var obj = item.obj;
                var type = item.type;
                var add = item.add;

                if (type == null)
                {
                    TryRegisterCallback(obj, ref OnPreNetDestroyCallbacks, add);
                    TryRegisterCallback(obj, ref OnOwnerChangeCallbacks, add);
                    TryRegisterCallback(obj, ref OnControllerChangeCallbacks, add);
                }
                else if (type == typeof(IOnPhotonViewPreNetDestroy))
                    RegisterCallback(obj as IOnPhotonViewPreNetDestroy, ref OnPreNetDestroyCallbacks, add);

                else if (type == typeof(IOnPhotonViewOwnerChange))
                    RegisterCallback(obj as IOnPhotonViewOwnerChange, ref OnOwnerChangeCallbacks, add);

                else if (type == typeof(IOnPhotonViewControllerChange))
                    RegisterCallback(obj as IOnPhotonViewControllerChange, ref OnControllerChangeCallbacks, add);
            }
        }

        private void TryRegisterCallback<T>(IPhotonViewCallback obj, ref List<T> list, bool add) where T : class, IPhotonViewCallback
        {
            T iobj = obj as T;
            if (iobj != null)
            {
                RegisterCallback(iobj, ref list, add);
            }
        }

        private void RegisterCallback<T>(T obj, ref List<T> list, bool add) where T : class, IPhotonViewCallback
        {
            if (ReferenceEquals(list, null))
                list = new List<T>();

            if (add)
            {
                if (!list.Contains(obj))
                    list.Add(obj);
            }
            else
            {
                if (list.Contains(obj))
                    list.Remove(obj);
            }
        }


        #endregion Callback Interfaces


        public override string ToString()
        {
            return string.Format("View {0}{3} on {1} {2}", this.ViewID, (this.gameObject != null) ? this.gameObject.name : "GO==null", (this.IsRoomView) ? "(scene)" : string.Empty, this.Prefix > 0 ? "lvl" + this.Prefix : "");
        }
    }
}