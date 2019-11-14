using System;

using QF.DVA;
using Unidux;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
	public class DVAInstanceExample : MonoBehaviour
	{
		DVAInstanceModel mModel = new DVAInstanceModel();

		
		
		void Start()
		{
			var btnAdd = transform.Find("BtnAdd").GetComponent<Button>();
			var btnSub = transform.Find("BtnSub").GetComponent<Button>();
			var number = transform.Find("Number").GetComponent<Text>();
			
			btnAdd.onClick.AddListener(() =>
			{
				mModel.Dispatch("INCREASE_COUNT");
				Debug.Log("Increase");
			});
			
			btnSub.onClick.AddListener(() =>
			{
				mModel.Dispatch("DECREASE_COUNT"); 
				
			});
			
			mModel.Subject.StartWith(mModel.Store.State).Subscribe(state =>
			{
				number.text = state.Count.ToString();
			}).AddTo(this);
			
		}

		void Update()
		{
			mModel.Store.Update();
		}
	}

	[Serializable]
	public class DVAInstanceState : DvaState
	{
		public int Count;
	}


	public class DVAInstanceModel : ReducerBase<DVAInstanceState, DvaAction>, IStoreAccessor,IDisposable
	{
		public DVAInstanceModel()
		{
			Store = new Store<DVAInstanceState>(new DVAInstanceState(),this);
		}
		
		
		public Store<DVAInstanceState> Store { get;set; }

		public Subject<DVAInstanceState> Subject
		{
			get { return Store.Subject; }
		}
		
		public object Dispatch(string path,object payload = null)
		{
			return Store.Dispatch(new DvaAction()
			{
				Type = path,
				Payload = payload
			});
		}
		
		public override DVAInstanceState Reduce(DVAInstanceState state, DvaAction action)
		{
			if (action.Type == "INCREASE_COUNT")
			{
				state.Count++;
			}
			else if (action.Type == "DECREASE_COUNT")
			{
				state.Count--;
			}

			return state;
		}

		public IStoreObject StoreObject
		{
			get { return Store; }
		}

		public void Dispose()
		{

		}
	}
}