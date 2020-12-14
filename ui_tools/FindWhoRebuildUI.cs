using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
public class FindWhoRebuildUI : MonoBehaviour
{

    IList<ICanvasElement> m_LayoutRebuildQueue;
    IList<ICanvasElement> m_GraphicRebuildQueue;

    private void Awake()
    {
        System.Type type = typeof(CanvasUpdateRegistry);
        FieldInfo field = type.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        m_LayoutRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
        field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        m_GraphicRebuildQueue = (IList<ICanvasElement>)field.GetValue(CanvasUpdateRegistry.instance);
    }

    private string getHierarchy(Transform tf)
    {
        List<string> result = new List<string>();
        while (tf != null)
        {
            result.Add(tf.gameObject.name);
            tf = tf.parent;
        }
        string s = "";
        for (int i = result.Count-1; i >=0 ; i--)
        {
            s += "/"+ result[i];
        }
        return s;
    }
    private void Update()
    {
        for (int j = 0; j < m_LayoutRebuildQueue.Count; j++)
        {
            var rebuild = m_LayoutRebuildQueue[j];
            if (ObjectValidForUpdate(rebuild))
            {
                // Debug.LogError("rebuild.transform.name" + rebuild.transform.gameObject.name);
                var g = rebuild.transform.GetComponent<Graphic>();
                if (g == null)
                {
                    Debug.LogFormat("{0} LayoutChange引起网格重建", getHierarchy(rebuild.transform));
                }
                else
                {
                    var can = g.canvas;
                    if (can == null)
                    {
                        Debug.LogFormat("{0} LayoutChange引起网格重建", getHierarchy(rebuild.transform));
                    }
                    else
                    {
                        Debug.LogFormat("{0} LayoutChange引起{1}网格重建", getHierarchy(rebuild.transform), can.name);
                    }
                }
                
               
            }
        }

        for (int j = 0; j < m_GraphicRebuildQueue.Count; j++)
        {
            var element = m_GraphicRebuildQueue[j];
            if (ObjectValidForUpdate(element))
            {
                // Debug.LogError("rebuild.transform.name" + rebuild.transform.gameObject.name);
                var g = element.transform.GetComponent<Graphic>();
                if (g == null)
                {
                    Debug.LogFormat("{0} GraphicChange引起网格重建" , getHierarchy(element.transform));
                }
                else
                {
                    var can = g.canvas;
                    if (can == null)
                    {
                        Debug.LogFormat("{0} GraphicChange引起网格重建", getHierarchy(element.transform));
                    }
                    else
                    {
                        Debug.LogFormat("{0} GraphicChange引起{1}网格重建", getHierarchy(element.transform), can.name);
                    }
                }
            }
               
        }
    }
    private bool ObjectValidForUpdate(ICanvasElement element)
    {
        var valid = element != null;

        var isUnityObject = element is Object;
        if (isUnityObject)
            valid = (element as Object) != null; //Here we make use of the overloaded UnityEngine.Object == null, that checks if the native object is alive.

        return valid;
    }
}

#endif