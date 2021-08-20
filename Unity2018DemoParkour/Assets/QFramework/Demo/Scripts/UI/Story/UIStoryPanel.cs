

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFramework.PlatformRunner
{
	internal enum PlotState
	{
		Idle,
		Show,
		Wait,
		Hide,
	}

	public class UIStoryPanelData : UIPanelData
	{
		// TODO: Query
	}

	public partial class UIStoryPanel : UIPanel,IPointerDownHandler
	{
		protected override void OnClose()
		{
			
		}

		protected override void OnInit(IUIData uiData = null)
		{
			StartCoroutine(ShowPlot());
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException();
		}

		private Texture mLastTexture = null;
		private Texture mCurTexture  = null;

		private PlotState mCurState = PlotState.Idle;

		private const int PlotNum       = 4;
		private       int mCurPlotIndex = 1;

		[SerializeField] private float mFadeOutTime = 1.0f;
		[SerializeField] private float mFadeInTime  = 1.0f;
		[SerializeField] private float mWaitTime    = 1.0f;

		private IEnumerator ShowPlot()
		{
			mCurState = PlotState.Show;

			mLastTexture = mCurTexture;

			if (null != mLastTexture)
			{
				Resources.UnloadAsset(mLastTexture);
			}

			mCurTexture = Resources.Load<Texture>("Image/Plot/plot_picture_" + mCurPlotIndex);

			PlotImage.texture = mCurTexture;

			yield return FadeOut();
			yield return WaitingForHide();
		}


		private IEnumerator WaitingForHide()
		{
			mCurState = PlotState.Wait;

			yield return new WaitForSeconds(mWaitTime);

			if (mCurState != PlotState.Wait)
			{
				yield break;
			}

			yield return HidePlot();
		}


		private IEnumerator HidePlot()
		{

			mCurState = PlotState.Hide;

			yield return FadeIn();

			if (mCurPlotIndex >= PlotNum)
			{
//				SceneManager.Instance.EnterHomeScene();
			}
			else
			{
				mCurPlotIndex++;

				yield return ShowPlot();
			}
		}


		/// <inheritdoc />
		/// <summary>
		/// 加收点击事件
		/// </summary>
		public void OnPointerDown(PointerEventData eventData)
		{
			Debug.LogWarning("click?");

			if (mCurState == PlotState.Wait)
			{
				mCurState = PlotState.Idle;

				StopAllCoroutines();

				StartCoroutine(HidePlot());
			}
		}

		private IEnumerator FadeOut()
		{
			// 初始化
			var color = FadeBlack.color;
			color.a = 1.0f;
			FadeBlack.color = color;

			// FadeBlack.DOFade(0.0f, mFadeOutTime);

			yield return new WaitForSeconds(mFadeOutTime);
		}

		private IEnumerator FadeIn()
		{
			// 初始化
			var color = FadeBlack.color;
			color.a = 0.0f;
			FadeBlack.color = color;
			// FadeBlack.DOFade(1.0f, mFadeInTime);

			yield return new WaitForSeconds(mFadeInTime);
		}
	}
}