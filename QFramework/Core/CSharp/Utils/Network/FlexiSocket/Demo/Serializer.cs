// *************************************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2016 Sean
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *************************************************************************************************
// Project source: https://github.com/theoxuan/FlexiSocket

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlexiFramework.Serializing;
using UnityEngine;

public class Serializer : MonoBehaviour, IFlexiSerializer<Quaternion>
{
    // Use this for initialization
    private void Start()
    {
        if (File.Exists("shit.bin"))
        {
            using (var stream = File.OpenRead("shit.bin"))
            {
                var serializer = new FlexiSerializer(this) {Layout = FlexiSerializationLayout.Mapping};
                var shit = serializer.Deserialize<Shit>(stream);
                Debug.Log(shit);
            }
        }
        else
        {
            var shit = new Shit(100, "Fuck this", true,
                new Ass(-99, null, true,
                    new[]
                    {
                        Quaternion.identity, Quaternion.Euler(90, 0, 0), Quaternion.Euler(0, 90, 0),
                        Quaternion.Euler(0, 0, 90)
                    }), new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0}, Vector3.one)
            {Bytes = new byte[] {128, 255}, List = new List<string>() {"lucas", "cindy", "moron"}};

            var serializer = new FlexiSerializer(this) {Layout = FlexiSerializationLayout.Mapping };

            using (var stream = File.OpenWrite("shit.bin"))
            {
                serializer.Serialize(shit, stream);
            }

            var sb = new StringBuilder();
            serializer.Encode(shit, sb);
            File.WriteAllText("shit.txt", sb.ToString());
        }
    }

    #region Implementation of IFlexiSerializer<Quaternion>

    void IFlexiSerializer<Quaternion>.Serialize(Quaternion value, BinaryWriter writer)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }

    Quaternion IFlexiSerializer<Quaternion>.Deserialize(BinaryReader reader)
    {
        return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    #endregion
}

public class Ass
{
    public int Index { get; private set; }
    public string Name { get; private set; }
    public bool Gender { get; private set; }
    public Quaternion[] Quaternions { get; set; }
    public Color[] Colors { get; set; }

    public Ass()
    {
    }

    public Ass(int index, string name, bool gender, Quaternion[] quaternions)
    {
        Index = index;
        Name = name;
        Gender = gender;
        Quaternions = quaternions;
    }

    public override string ToString()
    {
        return string.Format("{{ Index: {0}, Name: {1}, Gender: {2}, Quaternions: [{3}], Colors: [{4}] }}",
            Index,
            Name,
            Gender,
            Quaternions == null
                ? null
                : Quaternions.Aggregate(string.Empty,
                    (s, n) => string.IsNullOrEmpty(s) ? n.ToString() : s + ", " + n),
            Colors == null
                ? null
                : Colors.Aggregate(string.Empty,
                    (s, n) => string.IsNullOrEmpty(s) ? n.ToString() : s + ", " + n));
    }
}

public sealed class Shit
{
    public int ID { get; set; }
    public int Index { get; private set; }
    public string Name { get; set; }
    public bool Gender { get; set; }
    public Ass Ass { get; set; }
    public int[] Items { get; set; }
    public Rect Rect { get; set; }
    public Vector3 Position { get; set; }
    public byte[] Bytes { get; set; }
    public List<string> List { get; set; }

    public Shit(int index, string name, bool gender, Ass ass, int[] items, Vector3 position)
    {
        Index = index;
        Name = name;
        Gender = gender;
        Ass = ass;
        Items = items;
        Position = position;
    }

    public override string ToString()
    {
        return
            string.Format(
                "{{ ID: {0}, Index: {1}, Name: {2}, Gender: {3}, Ass: {4}, Position: {5}, Rect: {6} Items: [{7}], Bytes: [{8}]}}, List: [{9}]}}",
                ID,
                Index,
                Name,
                Gender,
                Ass,
                Position,
                Rect,
                Items == null
                    ? null
                    : Items.Aggregate(string.Empty, (s, n) => string.IsNullOrEmpty(s) ? n.ToString() : s + ", " + n),
                Bytes == null
                    ? null
                    : Bytes.Aggregate(string.Empty, (s, n) => string.IsNullOrEmpty(s) ? n.ToString() : s + ", " + n),
                List == null
                    ? null
                    : List.Aggregate(string.Empty, (s, n) => string.IsNullOrEmpty(s) ? n.ToString() : s + ", " + n)
                );
    }
}