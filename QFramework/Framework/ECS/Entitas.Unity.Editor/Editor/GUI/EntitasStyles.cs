using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static class EntitasStyles
    {
        static GUIStyle mSectionHeader;

        public static GUIStyle sectionHeader
        {
            get
            {
                if (mSectionHeader == null)
                {
                    mSectionHeader = new GUIStyle("OL Title");
                }

                return mSectionHeader;
            }
        }

        static GUIStyle mSectionContent;

        public static GUIStyle sectionContent
        {
            get
            {
                if (mSectionContent == null)
                {
                    mSectionContent = new GUIStyle("OL Box");
                    mSectionContent.stretchHeight = false;
                }

                return mSectionContent;
            }
        }
    }
}