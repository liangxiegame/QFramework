using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unidux.Example.MultipleState
{
    [Serializable]
    public class State : DvaState
    {
        public int IntValue = 0;
        public uint UintValue = 0;
        public long LongValue = 0;
        public ulong UlongValue = 0;
        public float FloatValue = 0;
        public double DoubleValue = 0;
        public bool BoolValue = false;
        public Friend AnimalFriend = Friend.ServalCat;
        
        public int? NullableIntValue = null;
        public uint? NullableUintValue = null;
        public long? NullableLongValue = null;
        public ulong? NullableUlongValue = null;
        public float? NullableFloatValue = null;
        public double? NullableDoubleValue = null;
        public bool? NullableBoolValue = null;
        public Friend? NullableAnimalFriend = null;

        public string StringValue = null;
        public Color ColorValue = Color.black;
        public Vector2 Vector2Value = Vector2.zero;
        public Vector3 Vector3Value = Vector3.zero;
        public Vector4 Vector4Value = Vector4.zero;

        public List<string> ListValue = new List<string>() {"one", "two", "three"};
        public Dictionary<string, int> DictionaryValue = new Dictionary<string, int>() {{"one", 1}, {"two", 2}};
        public string[] StringArray = {"one", "two", "three"};

        public IdNameState IdNameState = new IdNameState();
        public EmptyState EmptyState = new EmptyState();

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is State))
            {
                return false;
            }
            State other = (State) obj;

            var equal =
                other.IntValue == this.IntValue &&
                other.UintValue == this.UintValue &&
                other.LongValue == this.LongValue &&
                other.UlongValue == this.UlongValue &&
                other.FloatValue == this.FloatValue &&
                other.DoubleValue == this.DoubleValue &&
                other.BoolValue == this.BoolValue &&
                other.StringValue == this.StringValue &&
                other.AnimalFriend == this.AnimalFriend &&
                other.ColorValue == this.ColorValue &&
                other.Vector2Value == this.Vector2Value &&
                other.Vector3Value == this.Vector3Value &&
                other.Vector4Value == this.Vector4Value &&
                other.ListValue.SequenceEqual(this.ListValue) &&
                other.DictionaryValue.Keys.SequenceEqual(this.DictionaryValue.Keys) &&
                other.DictionaryValue.Values.SequenceEqual(this.DictionaryValue.Values) &&
                other.StringArray.SequenceEqual(this.StringArray) &&
                other.IdNameState.Equals(this.IdNameState) &&
                other.EmptyState.Equals(this.EmptyState);

            return equal;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class IdNameState : StateElement
    {
        public int Id = 0;
        public string Name = "";

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            IdNameState state = (IdNameState) obj;
            return obj != null && this.Id == state.Id && this.Name == state.Name;
        }

        public override string ToString()
        {
            return new StringBuilder().Append("Id:").Append(Id).Append(", Name:").Append(Name).ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class EmptyState : StateElement
    {
        public override bool Equals(object obj)
        {
            return obj != null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}