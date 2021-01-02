using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UI.Xml.CustomAttributes;

namespace UI.Xml
{
    [Serializable]
    public partial class XmlElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
    {
        [SerializeField]
        protected string _tagType = null;

        public string tagType
        {
            get
            {
                return _tagType;
            }
        }

        [NonSerialized]
        protected ElementTagHandler _tagHandler;
        public ElementTagHandler tagHandler
        {
            get
            {
                if (_tagHandler == null)
                {
                    if (tagType != null) _tagHandler = XmlLayoutUtilities.GetXmlTagHandler(tagType);
                }

                return _tagHandler;
            }
        }

        [SerializeField]
        protected RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                return _rectTransform;
            }
        }

        [SerializeField]
        public AttributeDictionary attributes = new AttributeDictionary();

        [SerializeField]
        protected XmlLayout xmlLayout;

        [SerializeField]
        public List<XmlElement> childElements = new List<XmlElement>();

        [SerializeField]
        public List<string> classes = new List<string>();

        [SerializeField]
        public XmlElement parentElement = null;

        [SerializeField]
        protected string m_id = "";
        public string id
        {
            get
            {
                return m_id;
            }
        }

        [SerializeField]
        protected string m_internalId = "";
        public string internalId
        {
            get
            {
                return m_internalId;
            }
        }

        #region Animation
        public ShowAnimation ShowAnimation = ShowAnimation.None;
        public HideAnimation HideAnimation = HideAnimation.None;
        public float AnimationDuration = 0.25f;
        public float ShowAnimationDelay = 0f;
        public float HideAnimationDelay = 0f;

        public bool _IsAnimating { get; protected set; }
        public bool IsAnimating
        {
            get
            {
                return _IsAnimating || GetCleansedChildElements().Any(c => c.IsAnimating);
            }
        }
        #endregion

        [NonSerialized]
        public bool Visible = false;

        public float DefaultOpacity = 1f;

        #region Audio
        public AudioClip OnClickSound = null;
        public AudioClip OnMouseEnterSound = null;
        public AudioClip OnMouseExitSound = null;
        public float AudioVolume = 1f;
        // For now, we are skipping AudioMixerGroup as it does not seem to be possible to locate one from scratch using just a string
        // a manual step is required in the editor
        //public UnityEngine.Audio.AudioMixerGroup AudioMixerGroup = null;

        private AudioSource m_AudioSource = null;
        protected AudioSource AudioSource
        {
            get
            {
                if (m_AudioSource == null)
                {
                    m_AudioSource = this.GetComponent<AudioSource>();
                    if (m_AudioSource == null) m_AudioSource = this.gameObject.AddComponent<AudioSource>();
                }

                return m_AudioSource;
            }
        }
        #endregion

        #region Dragging and Dropping
        // Can this XmlElement be dragged?
        public bool AllowDragging = false;
        // If AllowDragging is true, should this XmlElement be able to be dragged beyond the confines of its parent?
        public bool RestrictDraggingToParentBounds = true;

        public bool ReturnToOriginalPositionWhenReleased = true;

        // This is static - as only one element may be dragged at a time, this will record which element it is for Drag & Drop functionality
        public static XmlElement ElementCurrentlyBeingDragged = null;
        // Can this XmlElement receive drop events?
        public bool IsDropReceiver = false;

        public bool IsBeingDragged { get; set; }
        private Vector2 OriginalPositionOnDragStart { get; set; }
        private Vector2 OriginalPivotOnDragStart { get; set; }

        #endregion

        #region Tooltip
        public string Tooltip;
        public XmlLayoutTooltip.TooltipPosition TooltipPosition = XmlLayoutTooltip.TooltipPosition.Right;
        public bool TooltipFollowMouse = false;
        public float TooltipOffset = 8f;
        public Color TooltipBackgroundColor;
        public Color TooltipBorderColor;
        public Color TooltipTextColor;
        public Color TooltipTextOutlineColor;
        public Sprite TooltipBackgroundImage;
        public Sprite TooltipBorderImage;
        public int TooltipFontSize;
        public Font TooltipFont;
        public RectOffset TooltipPadding;
        #endregion

        #region Event-Handling
        private EventTrigger m_EventTrigger = null;
        public EventTrigger EventTrigger
        {
            get
            {
                if (m_EventTrigger == null)
                {
                    m_EventTrigger = this.GetComponent<EventTrigger>();

                    if (m_EventTrigger == null) m_EventTrigger = this.gameObject.AddComponent<EventTrigger>();
                }

                return m_EventTrigger;
            }
        }

        [SerializeField]
        protected List<Action> m_onClickEvents = new List<Action>();
        [SerializeField]
        protected List<Action> m_onMouseEnterEvents = new List<Action>();
        [SerializeField]
        protected List<Action> m_onMouseExitEvents = new List<Action>();
        [SerializeField]
        public List<Action<XmlElement, XmlElement>> m_onElementDroppedEvents = new List<Action<XmlElement, XmlElement>>();
        [SerializeField]
        public List<Action> m_onBeginDragEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onEndDragEvents = new List<Action>();
        [SerializeField]
        public List<Action> m_onDragEvents = new List<Action>();
        #endregion

        void Start()
        {
        }

        public void Initialise(XmlLayout xmlLayout, RectTransform rectTransform, ElementTagHandler tagHandler)
        {
            this.xmlLayout = xmlLayout;
            this._rectTransform = rectTransform;
            this._tagHandler = tagHandler;

            if (this.tagHandler != null)
            {
                this._tagType = tagHandler.tagType;
            }
        }

        public void SetAttribute(string attribute, string value)
        {
            if (HasAttribute(attribute))
            {
                attributes[attribute] = value;
            }
            else
            {
                attributes.Add(attribute, value);
            }
        }

        public void RemoveAttribute(string name)
        {
            if (HasAttribute(name))
            {
                attributes.Remove(name);
            }
        }

        public string GetAttribute(string name)
        {
            if (HasAttribute(name))
            {
                return attributes[name];
            }

            return null;
        }

        public bool HasAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        /// <summary>
        /// Apply the provided attributes to this XmlElement. If no attributes are provided, then this function will use this XmlElement's attribute collection instead.
        /// </summary>
        /// <param name="_attributes"></param>
        public void ApplyAttributes(AttributeDictionary _attributes = null)
        {
            if (_attributes != null)
            {
                this.attributes = _attributes;
            }
            else
            {
                _attributes = this.attributes;
            }

            if (_attributes.ContainsKey("internalid"))
            {
                this.m_internalId = _attributes["internalid"];
            }

            if (_attributes.ContainsKey("id"))
            {
                this.m_id = _attributes["id"];
            }

            tagHandler.SetInstance(rectTransform, xmlLayout);
            tagHandler.ApplyAttributes(_attributes);
        }

        /// <summary>
        /// Get the value of this XmlElement.
        /// Returns null if this element doesn't have a GetValue implementation.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (tagHandler is Tags.IHasXmlFormValue)
            {
                tagHandler.SetInstance(this.rectTransform, this.xmlLayout);

                return ((Tags.IHasXmlFormValue)tagHandler).GetValue(this);
            }

            return null;
        }

        /// <summary>
        /// Does this XmlElement have a specific class? (as set in Xml)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool HasClass(string c)
        {
            return classes.Contains(c, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieve a child XmlElement by its internalId.
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns></returns>
        public XmlElement GetElementByInternalId(string internalId)
        {
            if (!childElements.Any()) return null;

            var topLevelChild = childElements.FirstOrDefault(c => String.Equals(c.internalId, internalId, StringComparison.OrdinalIgnoreCase));

            if (topLevelChild != null) return topLevelChild;

            foreach (var child in childElements)
            {
                var depthSearch = child.GetElementByInternalId(internalId);

                if (depthSearch != null) return depthSearch;
            }

            return null;
        }

        /// <summary>
        /// Get a child XmlElement by its internalId (and call GetComponent to retrieve the desired component)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="internalId"></param>
        /// <returns></returns>
        public T GetElementByInternalId<T>(string internalId)
            where T : MonoBehaviour
        {
            var element = GetElementByInternalId(internalId);

            if (element != null)
            {
                return element.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// Add a child XmlElement to this XmlElement
        /// </summary>
        /// <param name="child"></param>
        public void AddChildElement(XmlElement child, bool adjustRectTransform = true)
        {
            if (adjustRectTransform)
            {
                child.transform.SetParent(this.transform);
                child.transform.localScale = Vector3.one;
                child.transform.position = new Vector3(child.transform.position.x, child.transform.position.y, 0);
                child.rectTransform.anchoredPosition3D = new Vector3(child.rectTransform.anchoredPosition3D.x, child.rectTransform.anchoredPosition3D.y, 0);
                child.transform.localRotation = new Quaternion();
                child.transform.SetAsLastSibling();
            }

            child.parentElement = this;

            if (!childElements.Contains(child)) childElements.Add(child);
        }

        /// <summary>
        /// Break the link between a child element and this element
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChildElement(XmlElement child)
        {
            if (childElements.Contains(child))
            {
                childElements.Remove(child);
            }
        }

        void OnDestroy()
        {
            if (parentElement != null)
            {
                parentElement.childElements.Remove(this);
            }
        }

        /// <summary>
        /// Show this XmlElement and trigger its show animation (if it has one set)        
        /// </summary>
        /// <param name="recursiveCall">Internal - should always be left as the default value of (false)</param>
        public void Show(bool recursiveCall = false)
        {
            if (Visible) return;

            Visible = true;

            if (!recursiveCall) // if this is a recursive call, then there's no need to mark this object as active (and besides, we may not want to)
            {
                gameObject.SetActive(true);
            }

            foreach (var childElement in GetCleansedChildElements())
            {
                childElement.Show(true);
            }

            if (!Application.isPlaying) return;

            if (this.gameObject.activeInHierarchy && ShowAnimation != ShowAnimation.None)
            {
                StartCoroutine(PlayShowAnimation(ShowAnimation));
            }
        }

        /// <summary>
        /// Hide this XmlElement and trigger the hide animation if necessary.
        /// </summary>
        /// <param name="recursiveCall">Internal - should always be left as the default value of (false)</param>
        /// <param name="onCompleteCallback">Specifies an Action to be called after this XmlElement is hidden (after any animation is completed).</param>
        public void Hide(bool recursiveCall = false, Action onCompleteCallback = null)
        {
            if (!gameObject.activeInHierarchy || !Visible)
            {
                Visible = false;

                if (onCompleteCallback != null) onCompleteCallback();

                return;
            }

            foreach (var childElement in GetCleansedChildElements())
            {
                childElement.Hide(true);
            }

            if (Application.isPlaying && HideAnimation != HideAnimation.None)
            {
                StartCoroutine(PlayHideAnimation(HideAnimation));
            }

            // if Hide() was called by the parent XmlElement, then don't actually disable the gameobject (there's no need, as the parent will itself be disabled)
            // additionally, hiding this element before the parent has finished whatever animation it is playing (if any) will cause it to disappear too early
            if (!recursiveCall)
            {
                if (!Application.isPlaying)
                {
                    gameObject.SetActive(false);
                    return;
                }

                // hide this XmlElement, but only once it and all of its children have finished their hide animations
                StartCoroutine(HideWhenAllAnimationIsComplete(onCompleteCallback));
            }
            else
            {
                Visible = false;
            }
        }

        protected IEnumerator PlayShowAnimation(ShowAnimation animation)
        {
            // Don't start animating if we're already animating (wait for the animation to finish first)            
            while (_IsAnimating) yield return new WaitForEndOfFrame();

            _IsAnimating = true;

            if (!xmlLayout.IsReady) yield return new WaitForEndOfFrame();

            if (ShowAnimationDelay > 0)
            {
                CanvasGroup.alpha = 0;
                yield return new WaitForSeconds(ShowAnimationDelay);
            }

            CanvasGroup.alpha = DefaultOpacity;
            CanvasGroup.blocksRaycasts = true;

            if (animation.IsSlideAnimation())
            {
                m_Animator.enabled = false;
                yield return PlaySlideInAnimation(animation);
            }
            else
            {
                m_Animator.enabled = true;
                m_Animator.speed = 0.25f / AnimationDuration;
                m_Animator.Play(animation.ToString());
                yield return new WaitForSeconds(m_Animator.GetCurrentAnimatorStateInfo(0).length / m_Animator.speed);
                m_Animator.enabled = false;
            }

            _IsAnimating = false;
        }

        protected IEnumerator PlayHideAnimation(HideAnimation animation)
        {
            // Don't start animating if we're already animating (wait for the animation to finish first)            
            while (_IsAnimating) yield return new WaitForEndOfFrame();

            _IsAnimating = true;

            if (HideAnimationDelay > 0)
            {
                yield return new WaitForSeconds(HideAnimationDelay);
            }

            CanvasGroup.blocksRaycasts = false;

            if (animation.IsSlideAnimation())
            {
                m_Animator.enabled = false;
                yield return PlaySlideOutAnimation(animation);
            }
            else
            {
                m_Animator.enabled = true;
                m_Animator.speed = 0.25f / AnimationDuration;
                m_Animator.Play(animation.ToString());
                yield return new WaitForSeconds(m_Animator.GetCurrentAnimatorStateInfo(0).length / m_Animator.speed);
                m_Animator.enabled = false;
            }

            _IsAnimating = false;
        }

        protected Vector2 GetDistanceForSlideAnimation(SlideDirection direction)
        {
            float desiredXChange = 0, desiredYChange = 0;

            Vector3[] parentCorners = new Vector3[4], elementCorners = new Vector3[4];
            ((RectTransform)rectTransform.parent).GetWorldCorners(parentCorners);
            rectTransform.GetWorldCorners(elementCorners);

            switch (direction)
            {
                case SlideDirection.Top:
                    {
                        // distance from the element's bottom edge to the parent's top edge
                        var parentTopEdge = parentCorners[2].y;
                        var elementBottomEdge = elementCorners[0].y;

                        desiredYChange = parentTopEdge - elementBottomEdge;
                    }
                    break;

                case SlideDirection.Bottom:
                    {
                        // distance from the element's top edge to the parent's bottom edge
                        var parentBottomEdge = parentCorners[3].y;
                        var elementTopEdge = elementCorners[1].y;

                        desiredYChange = parentBottomEdge - elementTopEdge;
                    }
                    break;

                case SlideDirection.Left:
                    {
                        // distance from the element's right edge to the parent's left edge
                        var parentLeftEdge = parentCorners[0].x;
                        var elementRightEdge = elementCorners[3].x;

                        desiredXChange = parentLeftEdge - elementRightEdge;
                    }
                    break;

                case SlideDirection.Right:
                    {
                        // distance from the element's left edge to the parent's right edge
                        var parentRightEdge = parentCorners[3].x;
                        var elementLeftEdge = elementCorners[0].x;

                        desiredXChange = parentRightEdge - elementLeftEdge;
                    }
                    break;
            }

            return new Vector2(desiredXChange, desiredYChange);
        }

        protected IEnumerator PlaySlideInAnimation(ShowAnimation animation)
        {
            var distance = GetDistanceForSlideAnimation(animation.ToSlideDirection());

            if (distance.x != 0)
            {
                yield return MoveDistanceX(distance.x, 0);
                yield return MoveDistanceX(-distance.x, AnimationDuration);
            }
            else if (distance.y != 0)
            {
                yield return MoveDistanceY(distance.y, 0);
                yield return MoveDistanceY(-distance.y, AnimationDuration);
            }
        }

        protected IEnumerator PlaySlideOutAnimation(HideAnimation animation)
        {
            var distance = GetDistanceForSlideAnimation(animation.ToSlideDirection());

            if (distance.x != 0)
            {
                yield return MoveDistanceX(distance.x, AnimationDuration);
                CanvasGroup.alpha = 0;

                yield return null;

                yield return MoveDistanceX(-distance.x, 0);
            }
            else if (distance.y != 0)
            {
                yield return MoveDistanceY(distance.y, AnimationDuration);
                CanvasGroup.alpha = 0;

                yield return null;

                yield return MoveDistanceY(-distance.y, 0);
            }
        }

        protected IEnumerator MoveDistanceX(float distance, float animationDuration = 0.25f)
        {
            float initialX = transform.localPosition.x;
            float destinationX = initialX + distance;

            if (animationDuration == 0)
            {
                transform.localPosition = new Vector2(destinationX, transform.localPosition.y);

                yield break;
            }

            float rate = 1.0f / animationDuration;
            float index = 0f;

            while (index < 1)
            {
                transform.localPosition = new Vector2(Mathf.Lerp(initialX, destinationX, index), transform.localPosition.y);
                index += rate * Time.deltaTime;

                yield return null;
            }

            transform.localPosition = new Vector2(destinationX, transform.localPosition.y);
        }

        protected IEnumerator MoveDistanceY(float distance, float animationDuration = 0.25f)
        {
            float initialY = transform.localPosition.y;
            float destinationY = initialY + distance;

            if (animationDuration == 0)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, destinationY);

                yield break;
            }

            float rate = 1.0f / animationDuration;
            float index = 0f;

            while (index < 1)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(initialY, destinationY, index));
                index += rate * Time.deltaTime;

                yield return null;
            }

            transform.localPosition = new Vector2(transform.localPosition.x, destinationY);
        }

        protected IEnumerator HideWhenAllAnimationIsComplete(Action onCompleteCallback)
        {
            while (IsAnimating) yield return new WaitForEndOfFrame();

            gameObject.SetActive(false);

            Visible = false;

            if (onCompleteCallback != null) onCompleteCallback();

            yield return new WaitForEndOfFrame();
        }

        protected Animator m_Animator
        {
            get
            {
                var animator = this.GetComponent<Animator>();
                if (animator == null) animator = this.gameObject.AddComponent<Animator>();

                var animatorController = "Animation/XmlLayoutAnimationController".ToRuntimeAnimatorController();
                animator.runtimeAnimatorController = animatorController;
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;

                // Some animations require a canvas group in order to function correctly
                GetCanvasGroup();

                return animator;
            }
        }

        public CanvasGroup CanvasGroup
        {
            get
            {
                return GetCanvasGroup();
            }
        }

        private CanvasGroup GetCanvasGroup()
        {
            var canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = this.gameObject.AddComponent<CanvasGroup>();

            return canvasGroup;
        }

        private void DelayCallUntilEndOfFrame(Action call)
        {
            StartCoroutine(_DelayCallUntilEndOfFrame(call));
        }

        private IEnumerator _DelayCallUntilEndOfFrame(Action call)
        {
            yield return new WaitForEndOfFrame();

            call();
        }

        private List<XmlElement> GetCleansedChildElements()
        {
            childElements = childElements.Where(c => c != null).ToList();

            return childElements;
        }

        /// <summary>
        /// Get the values of this element and any child elements by either 'id' or 'internalId'
        /// Please note: duplicate id/internalId values will be ignored (only the first located value will be used)
        /// </summary>
        /// <param name="onlyFormElements"></param>
        /// <param name="locateElementsBy"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetFormData(eLocateElementsBy locateElementsBy = eLocateElementsBy.InternalId)
        {
            var formData = new Dictionary<string, string>();

            // if this element's tag handler does not use the IHasXmlFormValue interface, then it does not implement GetValue(), therefore we do not have a value
            if (this.tagHandler is Tags.IHasXmlFormValue)
            {
                switch (locateElementsBy)
                {
                    case eLocateElementsBy.Id:
                        if (!String.IsNullOrEmpty(this.id))
                        {
                            formData.AddIfKeyNotExists(this.id, this.GetValue());
                        }
                        break;
                    case eLocateElementsBy.InternalId:
                        if (!String.IsNullOrEmpty(this.internalId))
                        {
                            formData.AddIfKeyNotExists(this.internalId, this.GetValue());
                        }
                        break;
                }
            }

            var cleansedChildElements = GetCleansedChildElements();

            if (!cleansedChildElements.Any()) return formData;

            foreach (var childElement in cleansedChildElements)
            {
                var childResults = childElement.GetFormData(locateElementsBy);

                if (childResults != null && childResults.Any())
                {
                    foreach (var result in childResults)
                    {
                        formData.AddIfKeyNotExists(result.Key, result.Value);
                    }
                }
            }

            return formData;
        }

        public enum eLocateElementsBy
        {
            Id,
            InternalId
        }

        #region Internal Events
        public void OnPointerClick(PointerEventData eventData)
        {
            PlaySound(OnClickSound);

            if (m_onClickEvents != null && m_onClickEvents.Any())
            {
                m_onClickEvents.ForEach(a => a.Invoke());
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlaySound(OnMouseEnterSound);

            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.ShowTooltip(this, Tooltip);
            }

            if (m_onMouseEnterEvents != null && m_onMouseEnterEvents.Any())
            {
                m_onMouseEnterEvents.ForEach(a => a.Invoke());
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlaySound(OnMouseExitSound);

            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.HideTooltip(this);
            }

            if (m_onMouseExitEvents != null && m_onMouseExitEvents.Any())
            {
                m_onMouseExitEvents.ForEach(a => a.Invoke());
            }
        }

        public void AddOnClickEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onClickEvents.Clear();

            m_onClickEvents.Add(action);
        }

        public void AddOnMouseEnterEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onClickEvents.Clear();

            m_onMouseEnterEvents.Add(action);
        }

        public void AddOnMouseExitEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onMouseExitEvents.Clear();

            m_onMouseExitEvents.Add(action);
        }

        public void AddOnElementDroppedEvent(Action<XmlElement, XmlElement> action, bool clearExisting = false)
        {
            if (clearExisting) m_onElementDroppedEvents.Clear();

            m_onElementDroppedEvents.Add(action);
        }

        public void AddOnBeginDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onBeginDragEvents.Clear();

            m_onBeginDragEvents.Add(action);
        }

        public void AddOnEndDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onEndDragEvents.Clear();

            m_onEndDragEvents.Add(action);
        }

        public void AddOnDragEvent(Action action, bool clearExisting = false)
        {
            if (clearExisting) m_onDragEvents.Clear();

            m_onDragEvents.Add(action);
        }
        #endregion

        #region Audio Methods
        protected void PlaySound(AudioClip sound)
        {
            if (sound == null) return;

            AudioSource.volume = AudioVolume;
            //AudioSource.outputAudioMixerGroup = AudioMixerGroup;
            AudioSource.clip = sound;
            AudioSource.Play();
        }
        #endregion

        #region Dragging
        void IDragHandler.OnDrag(PointerEventData pointerData)
        {
            if (!AllowDragging || pointerData == null) return;

            // this is the element being dragged
            XmlElement.ElementCurrentlyBeingDragged = this;

            Vector2 currentPosition = this.rectTransform.position;

            if (!IsBeingDragged)
            {
                OriginalPivotOnDragStart = rectTransform.pivot;
                OriginalPositionOnDragStart = currentPosition;

                // Stop blocking raycasts; if we don't do this, OnDrop will not be called
                CanvasGroup.blocksRaycasts = false;
            }

            if (RestrictDraggingToParentBounds)
            {
                var parentRectTransform = (RectTransform)this.rectTransform.parent;

                var parentRect = parentRectTransform.rect;
                var thisRect = rectTransform.rect;

                var parentXY = (Vector2)parentRectTransform.TransformPoint(parentRect.x, parentRect.y, 0);

                var thisPivot = rectTransform.pivot;

                var minX = parentXY.x + (thisPivot.x * thisRect.width);
                var minY = parentXY.y + (thisPivot.y * thisRect.height);

                var maxX = minX + parentRect.width - thisRect.width;
                var maxY = minY + parentRect.height - thisRect.height;

                currentPosition = rectTransform.position;

                currentPosition.x = Mathf.Clamp(currentPosition.x + pointerData.delta.x, minX, maxX);
                currentPosition.y = Mathf.Clamp(currentPosition.y + pointerData.delta.y, minY, maxY);
            }
            else
            {
                if (!IsBeingDragged)
                {
                    // initialise
                    // remove from parent rectTransform and set parent to the XmlLayout itself
                    this.rectTransform.SetParent(xmlLayout.XmlElement.rectTransform);
                }

                currentPosition.x += pointerData.delta.x;
                currentPosition.y += pointerData.delta.y;
            }

            rectTransform.position = currentPosition;

            this.IsBeingDragged = true;

            if (m_onDragEvents.Any())
            {
                m_onDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!this.IsBeingDragged) return;
            if (this == XmlElement.ElementCurrentlyBeingDragged) XmlElement.ElementCurrentlyBeingDragged = null;

            this.IsBeingDragged = false;
            this.rectTransform.SetParent(parentElement.rectTransform);

            if (ReturnToOriginalPositionWhenReleased)
            {
                this.rectTransform.pivot = OriginalPivotOnDragStart;
                this.rectTransform.position = OriginalPositionOnDragStart;
            }

            // Resume blocking raycasts
            CanvasGroup.blocksRaycasts = true;

            if (m_onEndDragEvents.Any())
            {
                m_onEndDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_onBeginDragEvents.Any())
            {
                m_onBeginDragEvents.ForEach(e => e.Invoke());
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!IsDropReceiver || eventData == null) return;
            if (XmlElement.ElementCurrentlyBeingDragged == null) return;

            if (m_onElementDroppedEvents != null && m_onElementDroppedEvents.Any())
            {
                m_onElementDroppedEvents.ForEach(a => a.Invoke(XmlElement.ElementCurrentlyBeingDragged, this));
            }
        }

        // Thank you jmorhart: http://answers.unity3d.com/questions/976201/set-a-recttranforms-pivot-without-changing-its-pos.html
        public void SetPivot(Vector2 pivot, RectTransform rectTransform = null)
        {
            if (rectTransform == null) rectTransform = this.rectTransform;
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
        #endregion

    }
}