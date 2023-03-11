/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    internal class MDLayout
    {
        MDContext mContext;
        MDBlockContainer mDocument;

        public MDLayout(MDContext context, MDBlockContainer doc)
        {
            mContext = context;
            mDocument = doc;
        }

        public float Height
        {
            get { return mDocument.Rect.height; }
        }

        public MDBlock Find(string id)
        {
            return mDocument.Find(id);
        }

        public void Arrange(float maxWidth)
        {
            mContext.Reset();
            mDocument.Arrange(mContext, MDViewer.Margin, maxWidth);
        }

        public void Draw()
        {
            mContext.Reset();
            mDocument.Draw(mContext);
        }
    }
}
#endif