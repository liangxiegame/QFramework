using System;
using System.Collections;
using System.Collections.Generic;

namespace QF.MVVM
{
 #if !NETFX_CORE
    public class NotifyCollectionChangedEventArgs : EventArgs
    {

        #region " Attributes "
        private NotifyCollectionChangedAction _notifyAction;
        private IList _newItemList;
        private int _newStartingIndex;
        private IList _oldItemList;
        private int _oldStartingIndex;
        #endregion

        #region " Constructors "
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            this.InitializeAdd(action, null, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException("Must Be Reset Add Or Remove Action For Ctor", "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException("Reset Action Requires Null Item", "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changed Items");
                }
                this.InitializeAddOrRemove(action, changedItems, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object changedItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException("Must Be Reset Add Or Remove Action For Ctor", "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException("Reset Action Requires Null Item", "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
            }
        }


        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("new Items");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("old Items");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException("Must Be Reset Add Or Remove Action For Ctor", "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException("Reset Action Requires Null Item", "action");
                }
                if (startingIndex != -1)
                {
                    throw new ArgumentException("Reset Action Requires Index Minus 1", "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changed Items");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException("Index Cannot Be Negative", "startingIndex");
                }
                this.InitializeAddOrRemove(action, changedItems, startingIndex);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object changedItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException("Must Be Reset Add Or Remove Action For Ctor", "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException("Reset Action Requires Null Item", "action");
                }
                if (index != -1)
                {
                    throw new ArgumentException("Reset Action Requires Index Minus 1", "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object newItem, Object oldItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("new Items");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("old Items");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            if (index < 0)
            {
                throw new ArgumentException("Index Cannot Be Negative", "index");
            }
            this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object changedItem, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            if (index < 0)
            {
                throw new ArgumentException("Index Cannot Be Negative", "index");
            }
            object[] newItems = new object[] { changedItem };
            this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, Object newItem, Object oldItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("Wrong Action For Ctor", "action");
            }
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
        }

        #endregion

        #region " Methods "
        private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
        {
            this._notifyAction = action;
            this._newItemList = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            this._newStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                this.InitializeAdd(action, changedItems, startingIndex);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                this.InitializeRemove(action, changedItems, startingIndex);
            }
        }

        private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
        {
            
            this.InitializeAdd(action, newItems, startingIndex);

            this.InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
        {
            this._notifyAction = action;
            this._oldItemList = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            this._oldStartingIndex = oldStartingIndex;
        }

        #endregion

        #region " Properties "
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return this._notifyAction;
            }
        }

        public IList NewItems
        {
            get
            {
                return this._newItemList ?? new List<object>();
            }
        }

        public int NewStartingIndex
        {
            get
            {
                return this._newStartingIndex;
            }
        }

        public IList OldItems
        {
            get
            {
                return this._oldItemList ?? new List<object>();
            }
        }

        public int OldStartingIndex
        {
            get
            {
                return this._oldStartingIndex;
            }
        }


        #endregion
    }
#endif
}