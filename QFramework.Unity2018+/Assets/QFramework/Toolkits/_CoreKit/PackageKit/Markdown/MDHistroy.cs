/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{
    public class MDHistory
    {
        private int          mIndex   = -1;
        private List<string> mHistory = new List<string>();

        public bool     IsEmpty    { get { return mHistory.Count == 0; } }
        public int      Count      { get { return mHistory.Count; } }
        public string   Current    { get { return mIndex >= 0 ? mHistory[ mIndex ] : null; } }
        public bool     CanBack    { get { return mIndex > 0; } }
        public bool     CanForward { get { return mIndex != mHistory.Count - 1; } }

        public void Clear()
        {
            mHistory.Clear();
            mIndex = -1;
        }

        public string Join()
        {
            return string.Join( ";", mHistory.GetRange( 0, mIndex + 1 ).ToArray() );
        }

        public string Forward()
        {
            if( CanForward )
            {
                mIndex++;
            }

            return Current;
        }

        public string Back()
        {
            if( CanBack )
            {
                mIndex--;
            }

            return Current;
        }

        public void Add( string url )
        {
            if( Current == url )
            {
                return;
            }

            if( mIndex + 1 < mHistory.Count )
            {
                mHistory.RemoveRange( mIndex + 1, mHistory.Count - mIndex - 1 );
            }

            mHistory.Add( url );
            mIndex++;
        }

        public void OnOpen( string url )
        {
            if( Current != url )
            {
                Clear();
                Add( url );
            }
        }
    }
}