// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2007 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Chris Toshok (toshok@novell.com)
//	Brian O'Keefe (zer0keefie@gmail.com)
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QF.MVVM
{
#if DLL
    [Serializable]
    public class ModelCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
#else
    [Serializable]
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
#endif
  
		[Serializable]
		sealed class SimpleMonitor : IDisposable {
			private int _busyCount;

			public SimpleMonitor()
			{
			}

			public void Enter()
			{
				_busyCount++;
			}

			public void Dispose()
			{
				_busyCount--;
			}

			public bool Busy
			{
				get { return _busyCount > 0; }
			}
		}

		private SimpleMonitor _monitor = new SimpleMonitor ();

#if !DLL
        public ObservableCollection ()
		{
            
		}

		public ObservableCollection (IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException ("collection");

			foreach (var item in collection)
				Add (item);
		}

		public ObservableCollection (List<T> list)
			: base (list != null ? new List<T> (list) : null)
		{
		}
#endif

		[field:NonSerialized]
		public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
		[field:NonSerialized]
		public virtual event PropertyChangedEventHandler PropertyChanged;

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { this.PropertyChanged += value; }
			remove { this.PropertyChanged -= value; }
		}

		protected IDisposable BlockReentrancy ()
		{
			_monitor.Enter ();
			return _monitor;
		}

		protected void CheckReentrancy ()
		{
			NotifyCollectionChangedEventHandler eh = CollectionChanged;

			// Only have a problem if we have more than one event listener.
			if (_monitor.Busy && eh != null && eh.GetInvocationList ().Length > 1)
				throw new InvalidOperationException ("Cannot modify the collection while reentrancy is blocked.");
		}

		protected override void ClearItems ()
		{
			CheckReentrancy ();
			


			OnCollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
			OnPropertyChanged (new PropertyChangedEventArgs ("Count"));
			OnPropertyChanged (new PropertyChangedEventArgs ("Item[]"));
            base.ClearItems();
		}

		protected override void InsertItem (int index, T item)
		{
			CheckReentrancy ();

			base.InsertItem (index, item);

			OnCollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Add, item, index));
			OnPropertyChanged (new PropertyChangedEventArgs ("Count"));
			OnPropertyChanged (new PropertyChangedEventArgs ("Item[]"));
		}

		public void Move (int oldIndex, int newIndex)
		{
			MoveItem (oldIndex, newIndex);
		}

		protected virtual void MoveItem (int oldIndex, int newIndex)
		{
			CheckReentrancy ();

			T item = this [oldIndex];
			base.RemoveItem (oldIndex);
			base.InsertItem (newIndex, item);

			OnCollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
			OnPropertyChanged (new PropertyChangedEventArgs ("Item[]"));
		}

		protected virtual void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedEventHandler eh = CollectionChanged;

			if (eh != null) {
				// Make sure that the invocation is done before the collection changes,
				// Otherwise there's a chance of data corruption.
				using (BlockReentrancy ()) {
					eh (this, e);
				}
			}
		}

		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler eh = PropertyChanged;

			if (eh != null)
				eh (this, e);
		}

		protected override void RemoveItem (int index)
		{
			CheckReentrancy ();

			T item = this [index];

			base.RemoveItem (index);

			OnCollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Remove, item, index));
			OnPropertyChanged (new PropertyChangedEventArgs ("Count"));
			OnPropertyChanged (new PropertyChangedEventArgs ("Item[]"));
		}

		protected override void SetItem (int index, T item)
		{
			CheckReentrancy ();

			T oldItem = this [index];

			base.SetItem (index, item);

			OnCollectionChanged (this, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Replace, item, oldItem, index));
			OnPropertyChanged (new PropertyChangedEventArgs ("Item[]"));
		}
	}
}