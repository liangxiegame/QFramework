using NUnit.Framework;

namespace QFramework.Tests
{
    public class BindableListTests
    {
        [Test]
        public void OnAdd_Triggers_On_Insert()
        {
            var list = new BindableList<string>();
            var triggered = false;
            var addedIndex = -1;
            var addedItem = string.Empty;

            list.OnAdd.Register((index, item) =>
            {
                triggered = true;
                addedIndex = index;
                addedItem = item;
            });

            list.Insert(0, "item");

            Assert.IsTrue(triggered);
            Assert.AreEqual(0, addedIndex);
            Assert.AreEqual("item", addedItem);
        }

        [Test]
        public void OnRemove_Triggers_On_RemoveAt()
        {
            var list = new BindableList<string> { "first", "second" };
            var triggered = false;
            var removedIndex = -1;
            var removedItem = string.Empty;

            list.OnRemove.Register((index, item) =>
            {
                triggered = true;
                removedIndex = index;
                removedItem = item;
            });

            list.RemoveAt(1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(1, removedIndex);
            Assert.AreEqual("second", removedItem);
        }

        [Test]
        public void OnReplace_Triggers_On_Set_Item()
        {
            var list = new BindableList<string> { "old" };
            var triggered = false;

            list.OnReplace.Register((index, oldItem, newItem) => { triggered = true; });

            list[0] = "new";

            Assert.IsTrue(triggered);
        }

        [Test]
        public void OnReplace_Passes_Old_And_New()
        {
            var list = new BindableList<string> { "old" };
            var replacedIndex = -1;
            var oldValue = string.Empty;
            var newValue = string.Empty;

            list.OnReplace.Register((index, oldItem, newItem) =>
            {
                replacedIndex = index;
                oldValue = oldItem;
                newValue = newItem;
            });

            list[0] = "new";

            Assert.AreEqual(0, replacedIndex);
            Assert.AreEqual("old", oldValue);
            Assert.AreEqual("new", newValue);
        }

        [Test]
        public void OnClear_Triggers_On_Clear()
        {
            var list = new BindableList<int> { 1 };
            var triggered = false;

            list.OnClear.Register(() => { triggered = true; });

            list.Clear();

            Assert.IsTrue(triggered);
        }

        [Test]
        public void OnCountChanged_Triggers_On_Add()
        {
            var list = new BindableList<int>();
            var changedCount = -1;

            list.OnCountChanged.Register(count => { changedCount = count; });

            list.Add(1);

            Assert.AreEqual(1, changedCount);
        }

        [Test]
        public void OnCountChanged_Triggers_On_Remove()
        {
            var list = new BindableList<int> { 1, 2 };
            var changedCount = -1;

            list.OnCountChanged.Register(count => { changedCount = count; });

            list.Remove(1);

            Assert.AreEqual(1, changedCount);
        }

        [Test]
        public void OnCountChanged_Does_Not_Trigger_On_Clear_Empty()
        {
            var list = new BindableList<int>();
            var triggered = false;

            list.OnCountChanged.Register(count => { triggered = true; });

            list.Clear();

            Assert.IsFalse(triggered);
        }

        [Test]
        public void OnMove_Triggers_On_Move()
        {
            var list = new BindableList<string> { "first", "second", "third" };
            var triggered = false;
            var oldIndexValue = -1;
            var newIndexValue = -1;
            var movedItem = string.Empty;

            list.OnMove.Register((oldIndex, newIndex, item) =>
            {
                triggered = true;
                oldIndexValue = oldIndex;
                newIndexValue = newIndex;
                movedItem = item;
            });

            list.Move(0, 2);

            Assert.IsTrue(triggered);
            Assert.AreEqual(0, oldIndexValue);
            Assert.AreEqual(2, newIndexValue);
            Assert.AreEqual("first", movedItem);
        }

        [Test]
        public void Move_Preserves_Item()
        {
            var list = new BindableList<string> { "first", "second", "third" };

            list.Move(0, 2);

            Assert.AreEqual("second", list[0]);
            Assert.AreEqual("third", list[1]);
            Assert.AreEqual("first", list[2]);
        }

        [Test]
        public void Constructor_With_Enumerable()
        {
            var values = new[] { 1, 2, 3 };

            var list = new BindableList<int>(values);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        [Test]
        public void Constructor_With_List()
        {
            var values = new System.Collections.Generic.List<int> { 1, 2, 3 };

            var list = new BindableList<int>(values);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }
    }

    public class BindableDictionaryTests
    {
        [Test]
        public void OnAdd_Triggers_On_Add()
        {
            var dictionary = new BindableDictionary<string, int>();
            var triggered = false;
            var addedKey = string.Empty;
            var addedValue = -1;

            dictionary.OnAdd.Register((key, value) =>
            {
                triggered = true;
                addedKey = key;
                addedValue = value;
            });

            dictionary.Add("one", 1);

            Assert.IsTrue(triggered);
            Assert.AreEqual("one", addedKey);
            Assert.AreEqual(1, addedValue);
        }

        [Test]
        public void OnRemove_Triggers_On_Remove()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);
            var triggered = false;
            var removedKey = string.Empty;
            var removedValue = -1;

            dictionary.OnRemove.Register((key, value) =>
            {
                triggered = true;
                removedKey = key;
                removedValue = value;
            });

            dictionary.Remove("one");

            Assert.IsTrue(triggered);
            Assert.AreEqual("one", removedKey);
            Assert.AreEqual(1, removedValue);
        }

        [Test]
        public void OnRemove_Returns_True_When_Exists()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);

            var removed = dictionary.Remove("one");

            Assert.IsTrue(removed);
        }

        [Test]
        public void OnRemove_Returns_False_When_Not_Exists()
        {
            var dictionary = new BindableDictionary<string, int>();

            var removed = dictionary.Remove("missing");

            Assert.IsFalse(removed);
        }

        [Test]
        public void OnReplace_Triggers_On_Set_Existing_Key()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);
            var triggered = false;

            dictionary.OnReplace.Register((key, oldValue, newValue) => { triggered = true; });

            dictionary["one"] = 2;

            Assert.IsTrue(triggered);
        }

        [Test]
        public void OnReplace_Passes_Old_And_New()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);
            var replacedKey = string.Empty;
            var oldValue = -1;
            var newValue = -1;

            dictionary.OnReplace.Register((key, oldItem, newItem) =>
            {
                replacedKey = key;
                oldValue = oldItem;
                newValue = newItem;
            });

            dictionary["one"] = 2;

            Assert.AreEqual("one", replacedKey);
            Assert.AreEqual(1, oldValue);
            Assert.AreEqual(2, newValue);
        }

        [Test]
        public void OnClear_Triggers_On_Clear()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);
            var triggered = false;

            dictionary.OnClear.Register(() => { triggered = true; });

            dictionary.Clear();

            Assert.IsTrue(triggered);
        }

        [Test]
        public void OnCountChanged_Triggers_On_Add()
        {
            var dictionary = new BindableDictionary<string, int>();
            var changedCount = -1;

            dictionary.OnCountChanged.Register(count => { changedCount = count; });

            dictionary.Add("one", 1);

            Assert.AreEqual(1, changedCount);
        }

        [Test]
        public void OnCountChanged_Does_Not_Trigger_On_Clear_Empty()
        {
            var dictionary = new BindableDictionary<string, int>();
            var triggered = false;

            dictionary.OnCountChanged.Register(count => { triggered = true; });

            dictionary.Clear();

            Assert.IsFalse(triggered);
        }

        [Test]
        public void TryGetValue_Works()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);

            int value;
            var found = dictionary.TryGetValue("one", out value);

            Assert.IsTrue(found);
            Assert.AreEqual(1, value);
        }

        [Test]
        public void ContainsKey_Works()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);

            Assert.IsTrue(dictionary.ContainsKey("one"));
            Assert.IsFalse(dictionary.ContainsKey("missing"));
        }

        [Test]
        public void Indexer_New_Key_Triggers_OnAdd()
        {
            var dictionary = new BindableDictionary<string, int>();
            var triggered = false;
            var addedKey = string.Empty;
            var addedValue = -1;

            dictionary.OnAdd.Register((key, value) =>
            {
                triggered = true;
                addedKey = key;
                addedValue = value;
            });

            dictionary["one"] = 1;

            Assert.IsTrue(triggered);
            Assert.AreEqual("one", addedKey);
            Assert.AreEqual(1, addedValue);
        }

        [Test]
        public void Indexer_Existing_Key_Triggers_OnReplace()
        {
            var dictionary = new BindableDictionary<string, int>();
            dictionary.Add("one", 1);
            var triggered = false;
            var replacedKey = string.Empty;
            var oldValue = -1;
            var newValue = -1;

            dictionary.OnReplace.Register((key, oldItem, newItem) =>
            {
                triggered = true;
                replacedKey = key;
                oldValue = oldItem;
                newValue = newItem;
            });

            dictionary["one"] = 2;

            Assert.IsTrue(triggered);
            Assert.AreEqual("one", replacedKey);
            Assert.AreEqual(1, oldValue);
            Assert.AreEqual(2, newValue);
        }
    }
}
