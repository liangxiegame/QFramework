// using UnityEngine;
//
// #if DOTWEEN
// using System.Collections.Generic;
// using DG.Tweening;
// #endif
//
// namespace QFramework.Tests.DOTweenInvalidTween
// {
//     /// <summary>
//     /// Validates the ActionKit DOTween fix: one getter call and normal completion without an invalid-tween warning.
//     /// Warning counts observe DOTween itself; this component does not generate the warning.
//     /// </summary>
//     public class DOTweenInvalidTweenRepro : MonoBehaviour
//     {
//         [SerializeField] private int mCreationCount;
//         [SerializeField] private int mWarningCount;
//         [SerializeField] private bool mCompleted;
//
//         private UnityEngine.UI.Image mImage;
//         private Sprite mRuntimeSprite;
//
//         public int CreationCount { get { return mCreationCount; } }
//         public int WarningCount { get { return mWarningCount; } }
//         public bool Completed { get { return mCompleted; } }
//         public float FillAmount { get { return mImage == null ? 0.0f : mImage.fillAmount; } }
//
//         public bool DependencyAvailable
//         {
//             get
//             {
// #if DOTWEEN
//                 return true;
// #else
//                 return false;
// #endif
//             }
//         }
//
// #if DOTWEEN
//         private readonly List<Tween> mCreatedTweens = new List<Tween>();
//         private IActionController mController;
//         private LogBehaviour mPreviousLogBehaviour;
//         private bool mObservingWarnings;
// #endif
//
//         private void Start()
//         {
//             CreateImage();
//
// #if DOTWEEN
//             DG.Tweening.DOTween.Init();
//             mPreviousLogBehaviour = DG.Tweening.DOTween.logBehaviour;
//             DG.Tweening.DOTween.logBehaviour = LogBehaviour.Default;
//             Application.logMessageReceived += OnLogMessage;
//             mObservingWarnings = true;
//
//             Debug.Log("[DOTween Repro] 开始验证修复：填充量从 0 到 1，持续 5 秒。" +
//                       "预期 getter 调用 1 次，正常完成且无 invalid tween 警告。", this);
//
//             mController = ActionKit.Sequence()
//                 .DOTween(CreateTween)
//                 .Start(this, OnSequenceComplete);
// #else
//             Debug.LogWarning("[DOTween Repro] 尚未安装或启用 DOTween。请导入官方 DOTween 1.2.790，" +
//                              "打开 Tools > Demigiant > DOTween Utility Panel，执行 Setup DOTween，" +
//                              "启用 UI 模块并确认 Scripting Define Symbols 中有 DOTWEEN，再进入 Play。", this);
// #endif
//         }
//
//         private void CreateImage()
//         {
//             var canvasObject = new GameObject("ReproCanvas", typeof(RectTransform), typeof(Canvas),
//                 typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
//             canvasObject.transform.SetParent(transform, false);
//             canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
//             var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
//             scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
//             scaler.referenceResolution = new Vector2(1280, 720);
//             scaler.matchWidthOrHeight = 0.5f;
//
//             var texture = Texture2D.whiteTexture;
//             mRuntimeSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
//                 new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect);
//             mRuntimeSprite.name = "DOTweenReproWhiteSprite";
//
//             var backgroundObject = new GameObject("FillBackground", typeof(RectTransform),
//                 typeof(UnityEngine.UI.Image));
//             backgroundObject.transform.SetParent(canvasObject.transform, false);
//             var backgroundRect = backgroundObject.GetComponent<RectTransform>();
//             backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
//             backgroundRect.pivot = new Vector2(0.5f, 0.5f);
//             backgroundRect.anchoredPosition = Vector2.zero;
//             backgroundRect.sizeDelta = new Vector2(480, 40);
//             var background = backgroundObject.GetComponent<UnityEngine.UI.Image>();
//             background.sprite = mRuntimeSprite;
//             background.color = new Color(0.15f, 0.20f, 0.26f, 1.0f);
//             background.raycastTarget = false;
//
//             var imageObject = new GameObject("FillImage", typeof(RectTransform), typeof(UnityEngine.UI.Image));
//             imageObject.transform.SetParent(backgroundObject.transform, false);
//             var imageRect = imageObject.GetComponent<RectTransform>();
//             imageRect.anchorMin = Vector2.zero;
//             imageRect.anchorMax = Vector2.one;
//             imageRect.offsetMin = imageRect.offsetMax = Vector2.zero;
//             mImage = imageObject.GetComponent<UnityEngine.UI.Image>();
//             mImage.sprite = mRuntimeSprite;
//             mImage.color = new Color(0.20f, 0.85f, 0.58f, 1.0f);
//             mImage.raycastTarget = false;
//             mImage.type = UnityEngine.UI.Image.Type.Filled;
//             mImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
//             mImage.fillOrigin = (int) UnityEngine.UI.Image.OriginHorizontal.Left;
//             mImage.fillAmount = 0.0f;
//         }
//
// #if DOTWEEN
//         private Tween CreateTween()
//         {
//             mCreationCount++;
//             var tween = mImage.DOFillAmount(1.0f, 5.0f)
//                 .SetEase(Ease.OutCubic)
//                 .SetAutoKill(true)
//                 .SetRecyclable(false);
//             // Keep both references for observation/cleanup without changing the copied wrapper.
//             mCreatedTweens.Add(tween);
//             Debug.Log("[DOTween Repro] getter 调用次数 = " + mCreationCount, this);
//             return tween;
//         }
//
//         private void OnLogMessage(string condition, string stackTrace, LogType type)
//         {
//             if (type == LogType.Warning &&
//                 condition.Contains("This Tween has been killed and is now invalid"))
//             {
//                 mWarningCount++;
//             }
//         }
//
//         private void OnSequenceComplete()
//         {
//             mCompleted = true;
//             mController = null;
//             StopObservingWarnings();
//             Debug.Log("[DOTween Repro] 完成：CreationCount=" + mCreationCount +
//                       ", WarningCount=" + mWarningCount + ", FillAmount=" + mImage.fillAmount, this);
//         }
//
//         private void StopObservingWarnings()
//         {
//             if (!mObservingWarnings) return;
//             Application.logMessageReceived -= OnLogMessage;
//             DG.Tweening.DOTween.logBehaviour = mPreviousLogBehaviour;
//             mObservingWarnings = false;
//         }
// #endif
//
//         private void OnDestroy()
//         {
// #if DOTWEEN
//             StopObservingWarnings();
//             if (mController != null)
//             {
//                 mController.Deinit();
//                 mController = null;
//             }
//
//             foreach (var tween in mCreatedTweens)
//             {
//                 if (tween.IsActive()) tween.Kill();
//             }
//             mCreatedTweens.Clear();
// #endif
//             // whiteTexture is Unity-owned; only the runtime-created sprite belongs to this test.
//             if (mRuntimeSprite != null) Destroy(mRuntimeSprite);
//         }
//     }
// }
