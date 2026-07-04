using NUnit.Framework;
using System.Collections.Generic;

namespace QFramework.Tests
{
    public class JSONObjectParseTests
    {
        [Test]
        public void Parse_Empty_Object()
        {
            var jsonObject = JSONObject.Create("{}");

            Assert.IsTrue(jsonObject.IsObject);
            Assert.AreEqual(0, jsonObject.Count);
        }

        [Test]
        public void Parse_Empty_Array()
        {
            var jsonObject = JSONObject.Create("[]");

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(0, jsonObject.Count);
        }

        [Test]
        public void Parse_String()
        {
            var jsonObject = JSONObject.Create("\"hello\"");

            Assert.IsTrue(jsonObject.IsString);
            Assert.AreEqual("hello", jsonObject.StringValue);
        }

        [Test]
        public void Parse_Int()
        {
            var jsonObject = JSONObject.Create("42");

            Assert.IsTrue(jsonObject.IsNumber);
            Assert.IsTrue(jsonObject.IsInteger);
            Assert.AreEqual(42, jsonObject.IntValue);
        }

        [Test]
        public void Parse_Float()
        {
            var jsonObject = JSONObject.Create("3.14");

            Assert.IsTrue(jsonObject.IsNumber);
            Assert.IsFalse(jsonObject.IsInteger);
            Assert.AreEqual(3.14f, jsonObject.FloatValue, 0.0001f);
        }

        [Test]
        public void Parse_Bool_True()
        {
            var jsonObject = JSONObject.Create("true");

            Assert.IsTrue(jsonObject.IsBool);
            Assert.IsTrue(jsonObject.BoolValue);
        }

        [Test]
        public void Parse_Bool_False()
        {
            var jsonObject = JSONObject.Create("false");

            Assert.IsTrue(jsonObject.IsBool);
            Assert.IsFalse(jsonObject.BoolValue);
        }

        [Test]
        public void Parse_Null()
        {
            var jsonObject = JSONObject.Create("null");

            Assert.IsTrue(jsonObject.IsNull);
        }

        [Test]
        public void Parse_Nested_Object()
        {
            var jsonObject = JSONObject.Create("{\"a\":{\"b\":1}}");

            Assert.IsTrue(jsonObject.IsObject);
            Assert.IsTrue(jsonObject["a"].IsObject);
            Assert.AreEqual(1, jsonObject["a"]["b"].IntValue);
        }

        [Test]
        public void Parse_Nested_Array()
        {
            var jsonObject = JSONObject.Create("[[1,2],[3,4]]");

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(1, jsonObject[0][0].IntValue);
            Assert.AreEqual(2, jsonObject[0][1].IntValue);
            Assert.AreEqual(3, jsonObject[1][0].IntValue);
            Assert.AreEqual(4, jsonObject[1][1].IntValue);
        }

        [Test]
        public void Parse_Object_With_Keys()
        {
            var jsonObject = JSONObject.Create("{\"name\":\"test\"}");

            Assert.IsNotNull(jsonObject.Keys);
            Assert.AreEqual("name", jsonObject.Keys[0]);
            Assert.AreEqual("test", jsonObject["name"].StringValue);
        }

        [Test]
        public void Parse_Whitespace_Handling()
        {
            var jsonObject = JSONObject.Create("  {  \"a\" : 1  }  ");

            Assert.IsTrue(jsonObject.IsObject);
            Assert.AreEqual(1, jsonObject["a"].IntValue);
        }

        [Test]
        public void Parse_Invalid_JSON_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => JSONObject.Create("garbage input"));
        }
    }

    public class JSONObjectCreateTests
    {
        [Test]
        public void Create_Empty_Object()
        {
            var jsonObject = JSONObject.EmptyObject;

            Assert.IsTrue(jsonObject.IsObject);
            Assert.AreEqual(0, jsonObject.Count);
        }

        [Test]
        public void Create_Empty_Array()
        {
            var jsonObject = JSONObject.EmptyArray;

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(0, jsonObject.Count);
        }

        [Test]
        public void Create_Null_Object()
        {
            var jsonObject = JSONObject.NullObject;

            Assert.IsTrue(jsonObject.IsNull);
        }

        [Test]
        public void Create_String_Object()
        {
            var jsonObject = JSONObject.CreateStringObject("x");

            Assert.IsTrue(jsonObject.IsString);
            Assert.AreEqual("x", jsonObject.StringValue);
        }

        [Test]
        public void Create_From_Dictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                { "name", "test" },
                { "value", "42" }
            };

            var jsonObject = JSONObject.Create(dictionary);

            Assert.IsTrue(jsonObject.IsObject);
            Assert.AreEqual(2, jsonObject.Count);
            Assert.AreEqual("test", jsonObject["name"].StringValue);
            Assert.AreEqual("42", jsonObject["value"].StringValue);
        }

        [Test]
        public void Create_From_JSON_Array()
        {
            var jsonObject = JSONObject.Create(new[]
            {
                JSONObject.Create(1),
                JSONObject.CreateStringObject("two")
            });

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(2, jsonObject.Count);
            Assert.AreEqual(1, jsonObject[0].IntValue);
            Assert.AreEqual("two", jsonObject[1].StringValue);
        }
    }

    public class JSONObjectFieldTests
    {
        [Test]
        public void AddField_And_HasField()
        {
            var jsonObject = JSONObject.EmptyObject;

            jsonObject.AddField("key", "value");

            Assert.IsTrue(jsonObject.HasField("key"));
        }

        [Test]
        public void AddField_Multiple_Types()
        {
            var jsonObject = JSONObject.EmptyObject;

            jsonObject.AddField("int", 1);
            jsonObject.AddField("string", "value");
            jsonObject.AddField("bool", true);

            Assert.AreEqual(1, jsonObject["int"].IntValue);
            Assert.AreEqual("value", jsonObject["string"].StringValue);
            Assert.IsTrue(jsonObject["bool"].BoolValue);
        }

        [Test]
        public void SetField_Updates_Existing()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("score", 1);

            jsonObject.SetField("score", 2);

            Assert.AreEqual(2, jsonObject["score"].IntValue);
            Assert.AreEqual(1, jsonObject.Count);
        }

        [Test]
        public void SetField_Adds_When_Not_Exist()
        {
            var jsonObject = JSONObject.EmptyObject;

            jsonObject.SetField("score", 2);

            Assert.IsTrue(jsonObject.HasField("score"));
            Assert.AreEqual(2, jsonObject["score"].IntValue);
        }

        [Test]
        public void RemoveField_Removes()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("key", "value");

            jsonObject.RemoveField("key");
            Assert.IsFalse(jsonObject.HasField("key"));
        }

        [Test]
        public void GetField_Returns_Correct_Object()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("key", "value");

            var field = jsonObject.GetField("key");

            Assert.IsNotNull(field);
            Assert.AreEqual("value", field.StringValue);
        }

        [Test]
        public void GetField_Returns_Null_When_Missing()
        {
            var jsonObject = JSONObject.EmptyObject;

            Assert.IsNull(jsonObject.GetField("missing"));
        }

        [Test]
        public void Indexer_String_Access()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("key", "value");

            Assert.AreEqual("value", jsonObject["key"].StringValue);
        }

        [Test]
        public void Indexer_Int_Access()
        {
            var jsonObject = JSONObject.EmptyArray;
            jsonObject.Add(JSONObject.Create(7));

            Assert.AreEqual(7, jsonObject[0].IntValue);
        }

        [Test]
        public void HasFields_Checks_Multiple()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("a", 1);
            jsonObject.AddField("b", 2);

            Assert.IsTrue(jsonObject.HasFields(new[] { "a", "b" }));
            Assert.IsFalse(jsonObject.HasFields(new[] { "a", "c" }));
        }
    }

    public class JSONObjectPrintTests
    {
        [Test]
        public void Print_Roundtrip_Object()
        {
            var original = JSONObject.Create("{\"name\":\"test\",\"count\":3}");

            var printed = original.Print();
            var parsed = JSONObject.Create(printed);

            Assert.IsTrue(parsed.IsObject);
            Assert.AreEqual("test", parsed["name"].StringValue);
            Assert.AreEqual(3, parsed["count"].IntValue);
        }

        [Test]
        public void Print_Roundtrip_Array()
        {
            var original = JSONObject.Create("[1,\"two\",true]");

            var printed = original.Print();
            var parsed = JSONObject.Create(printed);

            Assert.IsTrue(parsed.IsArray);
            Assert.AreEqual(1, parsed[0].IntValue);
            Assert.AreEqual("two", parsed[1].StringValue);
            Assert.IsTrue(parsed[2].BoolValue);
        }

        [Test]
        public void Print_Bool()
        {
            Assert.AreEqual("true", JSONObject.Create(true).Print());
            Assert.AreEqual("false", JSONObject.Create(false).Print());
        }

        [Test]
        public void Print_Null()
        {
            Assert.AreEqual("null", JSONObject.NullObject.Print());
        }

        [Test]
        public void ToString_Returns_Print()
        {
            var jsonObject = JSONObject.EmptyObject;
            jsonObject.AddField("key", "value");

            Assert.AreEqual(jsonObject.Print(), jsonObject.ToString());
        }
    }

    public class JSONObjectMergeAndCopyTests
    {
        [Test]
        public void Copy_Creates_Independent_Copy()
        {
            var original = JSONObject.EmptyObject;
            original.AddField("score", 1);
            original.AddField("nested", JSONObject.EmptyObject);
            original["nested"].AddField("value", "original");

            var copy = original.Copy();
            copy.SetField("score", 2);
            copy["nested"].SetField("value", "copy");

            Assert.AreEqual(1, original["score"].IntValue);
            Assert.AreEqual("original", original["nested"]["value"].StringValue);
            Assert.AreEqual(2, copy["score"].IntValue);
            Assert.AreEqual("copy", copy["nested"]["value"].StringValue);
        }

        [Test]
        public void Merge_Objects_Merges_Fields()
        {
            var left = JSONObject.EmptyObject;
            left.AddField("a", 1);

            var right = JSONObject.EmptyObject;
            right.AddField("b", 2);

            left.Merge(right);

            Assert.AreEqual(1, left["a"].IntValue);
            Assert.AreEqual(2, left["b"].IntValue);
        }

        [Test]
        public void Merge_Null_Absorbs()
        {
            var left = JSONObject.NullObject;
            var right = JSONObject.EmptyObject;
            right.AddField("a", 1);

            left.Merge(right);

            Assert.IsTrue(left.IsObject);
            Assert.AreEqual(1, left["a"].IntValue);
        }
    }

    public class JSONObjectOperatorTests
    {
        [Test]
        public void Implicit_Bool_True()
        {
            var jsonObject = JSONObject.EmptyObject;

            Assert.IsTrue((bool)jsonObject);
        }

        [Test]
        public void Implicit_Bool_False()
        {
            JSONObject nullObj = null;

            Assert.IsFalse((bool)nullObj);
        }
    }
}
