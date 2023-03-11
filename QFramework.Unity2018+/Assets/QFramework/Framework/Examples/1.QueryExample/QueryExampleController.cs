using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
    public class QueryExampleController : MonoBehaviour,IController
    {
        public class StudentModel : AbstractModel
        {

            public List<string> StudentNames = new List<string>()
            {
                "张三",
                "李四"
            };
            
            protected override void OnInit()
            {
                
            }
        }
        
        public class TeacherModel : AbstractModel
        {
            public List<string> TeacherNames = new List<string>()
            {
                "王五",
                "赵六"
            };
                
            protected override void OnInit()
            {
                
            }
        }

        // Architecture
        public class QueryExampleApp : Architecture<QueryExampleApp>
        {
            protected override void Init()
            {
                this.RegisterModel(new StudentModel());
                this.RegisterModel(new TeacherModel());
            }
        }
        
        
        /// <summary>
        /// 获取学校的全部人数
        /// </summary>
        public class SchoolAllPersonCountQuery : AbstractQuery<int>
        {
            protected override int OnDo()
            {
                return this.GetModel<StudentModel>().StudentNames.Count +
                       this.GetModel<TeacherModel>().TeacherNames.Count;
            }
        }

        private int mAllPersonCount = 0;

        private void OnGUI()
        {
            GUILayout.Label(mAllPersonCount.ToString());

            if (GUILayout.Button("查询学校总人数"))
            {
                mAllPersonCount = this.SendQuery(new SchoolAllPersonCountQuery());
            }
        }

        public IArchitecture GetArchitecture()
        {
            return QueryExampleApp.Interface;
        }
    }
}