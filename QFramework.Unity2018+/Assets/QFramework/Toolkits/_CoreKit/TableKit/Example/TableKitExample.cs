using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class TableKitExample : MonoBehaviour
    {
        public class Student
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int Level { get; set; }
        }
        public class School : Table<Student>
        {
            public TableIndex<int, Student> AgeIndex = new TableIndex<int, Student>((student) => student.Age);
            public TableIndex<int, Student> LevelIndex = new TableIndex<int, Student>((student) => student.Level);
            
            protected override void OnAdd(Student item)
            {
                AgeIndex.Add(item);
                LevelIndex.Add(item);
            }

            protected override void OnRemove(Student item)
            {
                AgeIndex.Remove(item);
                LevelIndex.Remove(item);
            }

            protected override void OnClear()
            {
                AgeIndex.Clear();
                LevelIndex.Clear();
            }

            public override IEnumerator<Student> GetEnumerator()
            {
                return AgeIndex.Dictionary.Values.SelectMany(s=>s).GetEnumerator();
            }

            protected override void OnDispose()
            {
                AgeIndex.Dispose();
                LevelIndex.Dispose();
            }
        }


        private void Start()
        {
            var school = new School();
            school.Add(new Student(){Age = 1,Level = 2,Name = "liangxie"});
            school.Add(new Student(){Age = 2,Level = 2,Name = "ava"});
            school.Add(new Student(){Age = 3,Level = 2,Name = "abc"});
            school.Add(new Student(){Age = 3,Level = 3,Name = "efg"});
            
            foreach (var student in school.LevelIndex.Get(2).Where(s=>s.Age < 3))
            {
                Debug.Log(student.Age + ":" + student.Level + ":" + student.Name);
            }
        }
    }
}
// 1:2:liangxie
// 2:2:ava