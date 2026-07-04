using NUnit.Framework;

namespace QFramework.Tests
{
    public class LanguageTests
    {
        [Test]
        public void Language_Has_English()
        {
            Assert.AreEqual(10, (int)Language.English);
        }

        [Test]
        public void Language_Has_ChineseSimplified()
        {
            Assert.AreEqual(40, (int)Language.ChineseSimplified);
        }

        [Test]
        public void Language_Has_ChineseTraditional()
        {
            Assert.AreEqual(41, (int)Language.ChineseTraditional);
        }

        [Test]
        public void Language_Has_Japanese()
        {
            Assert.AreEqual(22, (int)Language.Japanese);
        }

        [Test]
        public void Language_Has_Korean()
        {
            Assert.AreEqual(23, (int)Language.Korean);
        }
    }

    public class LanguageDefineTests
    {
        [Test]
        public void LanguageDefine_Has_Language_Field()
        {
            var define = new LanguageDefine();

            Assert.AreEqual(default(Language), define.Language);
        }

        [Test]
        public void LanguageDefine_Can_Set_Language()
        {
            var define = new LanguageDefine
            {
                Language = Language.Japanese
            };

            Assert.AreEqual(Language.Japanese, define.Language);
        }
    }

    public class LanguageDefineConfigTests
    {
        [Test]
        public void Default_Has_Two_Languages()
        {
            var config = new LanguageDefineConfig();

            Assert.AreEqual(2, config.LanguageDefines.Count);
        }

        [Test]
        public void Default_First_Is_English()
        {
            var config = new LanguageDefineConfig();

            Assert.AreEqual(Language.English, config.LanguageDefines[0].Language);
        }

        [Test]
        public void Default_Second_Is_ChineseSimplified()
        {
            var config = new LanguageDefineConfig();

            Assert.AreEqual(Language.ChineseSimplified, config.LanguageDefines[1].Language);
        }

        [Test]
        public void Can_Add_Language_Define()
        {
            var config = new LanguageDefineConfig();

            config.LanguageDefines.Add(new LanguageDefine
            {
                Language = Language.Korean
            });

            Assert.AreEqual(3, config.LanguageDefines.Count);
            Assert.AreEqual(Language.Korean, config.LanguageDefines[2].Language);
        }
    }
}
