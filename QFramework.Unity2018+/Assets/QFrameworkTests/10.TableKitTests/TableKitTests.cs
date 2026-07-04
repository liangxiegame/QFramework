using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace QFramework.Tests
{
    public class TableTests
    {
        [Test]
        public void Add_Calls_OnAdd()
        {
            var school = new School();
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };

            school.Add(student);

            Assert.AreEqual(1, school.OnAddCount);
        }

        [Test]
        public void Remove_Calls_OnRemove()
        {
            var school = new School();
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };
            school.Add(student);

            school.Remove(student);

            Assert.AreEqual(1, school.OnRemoveCount);
        }

        [Test]
        public void Clear_Calls_OnClear()
        {
            var school = new School();

            school.Clear();

            Assert.AreEqual(1, school.OnClearCount);
        }

        [Test]
        public void GetEnumerator_Returns_All_Items()
        {
            var school = new School();
            var liangxie = new Student { Name = "liangxie", Age = 18, Level = 1 };
            var ava = new Student { Name = "ava", Age = 19, Level = 2 };
            school.Add(liangxie);
            school.Add(ava);

            var students = school.ToList();

            Assert.AreEqual(2, students.Count);
            Assert.IsTrue(students.Contains(liangxie));
            Assert.IsTrue(students.Contains(ava));
        }

        [Test]
        public void Dispose_Calls_OnDispose()
        {
            var school = new School();

            school.Dispose();

            Assert.AreEqual(1, school.OnDisposeCount);
        }
    }

    public class TableIndexTests
    {
        [Test]
        public void Add_Then_Get_Returns_Item()
        {
            var index = new TableIndex<int, Student>(student => student.Age);
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };

            index.Add(student);

            var students = index.Get(18).ToList();
            Assert.AreEqual(1, students.Count);
            Assert.AreSame(student, students[0]);
        }

        [Test]
        public void Get_Returns_Empty_When_No_Match()
        {
            var index = new TableIndex<int, Student>(student => student.Age);

            var students = index.Get(18).ToList();

            Assert.IsEmpty(students);
        }

        [Test]
        public void Multiple_Items_Same_Key()
        {
            var index = new TableIndex<int, Student>(student => student.Age);
            var liangxie = new Student { Name = "liangxie", Age = 18, Level = 1 };
            var ava = new Student { Name = "ava", Age = 18, Level = 2 };
            var luna = new Student { Name = "luna", Age = 18, Level = 3 };

            index.Add(liangxie);
            index.Add(ava);
            index.Add(luna);

            var students = index.Get(18).ToList();
            Assert.AreEqual(3, students.Count);
            Assert.IsTrue(students.Contains(liangxie));
            Assert.IsTrue(students.Contains(ava));
            Assert.IsTrue(students.Contains(luna));
        }

        [Test]
        public void Remove_Then_Get_Empty()
        {
            var index = new TableIndex<int, Student>(student => student.Age);
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };
            index.Add(student);

            index.Remove(student);

            Assert.IsEmpty(index.Get(18).ToList());
        }

        [Test]
        public void Clear_Removes_All()
        {
            var index = new TableIndex<int, Student>(student => student.Age);
            index.Add(new Student { Name = "liangxie", Age = 18, Level = 1 });
            index.Add(new Student { Name = "ava", Age = 19, Level = 2 });

            index.Clear();

            Assert.IsEmpty(index.Get(18).ToList());
            Assert.IsEmpty(index.Get(19).ToList());
            Assert.IsEmpty(index.Dictionary);
        }

        [Test]
        public void Dispose_Cleans_Up()
        {
            var index = new TableIndex<int, Student>(student => student.Age);
            index.Add(new Student { Name = "liangxie", Age = 18, Level = 1 });

            index.Dispose();

            Assert.IsNull(index.Dictionary);
        }
    }

    public class IntegratedTableTests
    {
        [Test]
        public void Add_Indexes_Correctly()
        {
            var school = new School();
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };

            school.Add(student);

            var students = school.AgeIndex.Get(18).ToList();

            Assert.AreEqual(1, students.Count);
            Assert.AreSame(student, students[0]);
        }

        [Test]
        public void Remove_De_Indexes()
        {
            var school = new School();
            var student = new Student { Name = "liangxie", Age = 18, Level = 1 };
            school.Add(student);

            school.Remove(student);

            Assert.IsEmpty(school.AgeIndex.Get(18).ToList());
        }

        [Test]
        public void Clear_De_Indexes_All()
        {
            var school = new School();
            school.Add(new Student { Name = "liangxie", Age = 18, Level = 1 });
            school.Add(new Student { Name = "ava", Age = 19, Level = 2 });

            school.Clear();

            Assert.IsEmpty(school.AgeIndex.Get(18).ToList());
            Assert.IsEmpty(school.AgeIndex.Get(19).ToList());
            Assert.IsEmpty(school.AgeIndex.Dictionary);
            Assert.IsEmpty(school.LevelIndex.Dictionary);
        }

        [Test]
        public void Multiple_Indexes()
        {
            var school = new School();
            var liangxie = new Student { Name = "liangxie", Age = 18, Level = 1 };
            var ava = new Student { Name = "ava", Age = 19, Level = 1 };
            school.Add(liangxie);
            school.Add(ava);

            var ageStudents = school.AgeIndex.Get(18).ToList();
            var levelStudents = school.LevelIndex.Get(1).ToList();

            Assert.AreEqual(1, ageStudents.Count);
            Assert.AreSame(liangxie, ageStudents[0]);
            Assert.AreEqual(2, levelStudents.Count);
            Assert.IsTrue(levelStudents.Contains(liangxie));
            Assert.IsTrue(levelStudents.Contains(ava));
        }
    }

    public class Student
    {
        public string Name;
        public int Age;
        public int Level;
    }

    public class School : Table<Student>
    {
        public TableIndex<int, Student> AgeIndex;
        public TableIndex<int, Student> LevelIndex;

        public int OnAddCount, OnRemoveCount, OnClearCount, OnDisposeCount;

        public School()
        {
            AgeIndex = new TableIndex<int, Student>(student => student.Age);
            LevelIndex = new TableIndex<int, Student>(student => student.Level);
        }

        protected override void OnAdd(Student item)
        {
            AgeIndex.Add(item);
            LevelIndex.Add(item);
            OnAddCount++;
        }

        protected override void OnRemove(Student item)
        {
            AgeIndex.Remove(item);
            LevelIndex.Remove(item);
            OnRemoveCount++;
        }

        protected override void OnClear()
        {
            AgeIndex.Clear();
            LevelIndex.Clear();
            OnClearCount++;
        }

        public override System.Collections.Generic.IEnumerator<Student> GetEnumerator()
        {
            return AgeIndex.Dictionary.Values.SelectMany(students => students).GetEnumerator();
        }

        protected override void OnDispose()
        {
            AgeIndex.Dispose();
            LevelIndex.Dispose();
            OnDisposeCount++;
        }
    }
}
