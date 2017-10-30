using UnityEngine;
using System.Linq;

using Unity.Linq; // using LINQ to GameObject

// This script is attached to Root.

namespace Unity.Linq.Sample
{
    public class SampleSceneScript : MonoBehaviour
    {
        GameObject[] array = new GameObject[0];

        void OnGUI()
        {
            var origin = GameObject.Find("Origin");

            if (GUILayout.Button("Parent"))
            {
                Debug.Log("------Parent");
                var parent = origin.Parent();
                Debug.Log(parent.name);
            }

            if (GUILayout.Button("Child"))
            {
                Debug.Log("------Child");
                var child = origin.Child("Sphere_B"); // can find deactive object
                Debug.Log(child.name);
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Ancestors"))
                {
                    Debug.Log("------Ancestors");
                    foreach (var item in origin.Ancestors())
                    {
                        Debug.Log(item.name);
                    }
                }

                if (GUILayout.Button("AncestorsAndSelf"))
                {
                    Debug.Log("------AncestorsAndSelf");
                    foreach (var item in origin.AncestorsAndSelf())
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Children"))
                {
                    Debug.Log("------Children");
                    foreach (var item in origin.Children())
                    {
                        Debug.Log(item.name);
                    }
                }

                if (GUILayout.Button("ChildrenAndSelf"))
                {
                    Debug.Log("------ChildrenAndSelf");
                    foreach (var item in origin.ChildrenAndSelf())
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Descendants"))
                {
                    Debug.Log("------Descendants");
                    foreach (var item in origin.Descendants())
                    {
                        Debug.Log(item.name);
                    }
                }

                if (GUILayout.Button("DescendantsAndSelf"))
                {
                    Debug.Log("------DescendantsAndSelf");
                    foreach (var item in origin.DescendantsAndSelf())
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Descendants:DescendIntoChildren"))
                {
                    Debug.Log("------Descendants:DescendIntoChildren");
                    foreach (var item in origin.Descendants(x => x.name != "Group"))
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("BeforeSelf"))
                {
                    Debug.Log("------BeforeSelf");
                    foreach (var item in origin.BeforeSelf())
                    {
                        Debug.Log(item.name);
                    }
                }

                if (GUILayout.Button("BeforeSelfAndSelf"))
                {
                    Debug.Log("------BeforeSelfAndSelf");
                    foreach (var item in origin.BeforeSelfAndSelf())
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("AfterSelf"))
                {
                    Debug.Log("------AfterSelf");
                    foreach (var item in origin.AfterSelf())
                    {
                        Debug.Log(item.name);
                    }
                }

                if (GUILayout.Button("AfterSelf"))
                {
                    Debug.Log("------AfterSelfAndSelf");
                    foreach (var item in origin.AfterSelfAndSelf())
                    {
                        Debug.Log(item.name);
                    }
                }
            }

            if (GUILayout.Button("OfComponent"))
            {
                Debug.Log("------OfComponent");
                // get only SphereCollider
                var sphere = origin.Descendants().OfComponent<SphereCollider>();
                foreach (var item in sphere)
                {
                    Debug.Log("Sphere:" + item.name + " Radius:" + item.radius);
                }
            }

            if (GUILayout.Button("LINQ"))
            {
                Debug.Log("------LINQ");
                // get children only ends with B
                var filter = origin.Children().Where(x => x.name.EndsWith("B"));
                foreach (var item in filter)
                {
                    Debug.Log(item.name);
                }
            }

            if (GUILayout.Button("Add"))
            {
                // Adds the GameObject/Component as children of this GameObject. Target is cloned.
                Debug.Log("------Add(see around of origin)");

                origin.Add(new GameObject("lastChild0"));
                origin.AddRange(new[] { new GameObject("lastChild1"), new GameObject("lastChild2"), new GameObject("lastChild3") });

                origin.AddFirstRange(new[] { new GameObject("firstChild1"), new GameObject("firstChild2"), new GameObject("firstChild3") });
                origin.AddFirst(new GameObject("firstChild0"));

                origin.AddBeforeSelf(new GameObject("beforeSelf0"));
                origin.AddBeforeSelfRange(new[] { new GameObject("beforeSelf1"), new GameObject("beforeSelf2"), new GameObject("beforeSelf3") });

                origin.AddAfterSelfRange(new[] { new GameObject("afterSelf1"), new GameObject("afterSelf2"), new GameObject("afterSelf3") });
                origin.AddAfterSelf(new GameObject("afterSelf0"));

                // Note, Cloned object around origin but original object is placed top of hierarchy.
                Resources.FindObjectsOfTypeAll<GameObject>()
                    .Cast<GameObject>()
                    .Where(x => x.Parent() == null && !x.name.Contains("Camera") && x.name != "Root" && x.name != "" && x.name != "HandlesGO" && !x.name.StartsWith("Scene") && !x.name.Contains("Light") && !x.name.Contains("Materials"))
                    .Destroy();
            }

            if (GUILayout.Button("MoveTo"))
            {
                // Move the GameObject/Component as children of this GameObject.
                Debug.Log("------MoveTo(see around of origin)");

                origin.MoveToLast(new GameObject("lastChild0(Original)"));
                origin.MoveToLastRange(new[] { new GameObject("lastChild1(Original)"), new GameObject("lastChild2(Original)"), new GameObject("lastChild3(Original)") });

                origin.MoveToFirstRange(new[] { new GameObject("firstChild1(Original)"), new GameObject("firstChild2(Original)"), new GameObject("firstChild3(Original)") });
                origin.MoveToFirst(new GameObject("firstChild0(Original)"));

                origin.MoveToBeforeSelf(new GameObject("beforeSelf0(Original)"));
                origin.MoveToBeforeSelfRange(new[] { new GameObject("beforeSelf1(Original)"), new GameObject("beforeSelf2(Original)"), new GameObject("beforeSelf3(Original)") });

                origin.MoveToAfterSelfRange(new[] { new GameObject("afterSelf1(Original)"), new GameObject("afterSelf2(Original)"), new GameObject("afterSelf3(Original)") });
                origin.MoveToAfterSelf(new GameObject("afterSelf0(Original)"));
            }

            if (GUILayout.Button("Destroy"))
            {
                Debug.Log("------Destroy(see around of origin)");

                // Destroy Cloned Object. Press button after Add Button.
                origin.transform.root.gameObject
                    .Descendants()
                    .Where(x => x.name.EndsWith("(Clone)") || x.name.EndsWith("(Original)"))
                    .Destroy();
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("ToArrayNonAlloc"))
                {
                    Debug.Log("------ToArrayNonAlloc");
                    var size = origin.Children().ToArrayNonAlloc(ref array);
                    for (int i = 0; i < size; i++)
                    {
                        Debug.Log(array[i].name);
                    }
                }

                if (GUILayout.Button("ToArrayNonAlloc(with filter)"))
                {
                    Debug.Log("------ToArrayNonAllocWithFilter");
                    // ToArray and ToArrayNonAlloc has five overloads
                    // (), (selector), (filter), (filter, selector), (let, filter, selector)
                    var size = origin.Children().ToArrayNonAlloc(x => x.name.EndsWith("B"), ref array);
                    for (int i = 0; i < size; i++)
                    {
                        Debug.Log(array[i].name);
                    }
                }
            }
        }
    }
}