using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
[Serializable]
public class UnityEngineObjectArrayStorage : SerializableDictionary.Storage<UnityEngine.Object[]> { }

[Serializable]
public class StringObjectArrayDictionary : SerializableDictionary<string, UnityEngine.Object[], UnityEngineObjectArrayStorage> { }
public class UIManifest : MonoBehaviour
{
    public StringObjectDictionary Datas= new StringObjectDictionary();
    public StringObjectArrayDictionary ArrayData = new StringObjectArrayDictionary();

    public UnityEngine.Object GetData(string key)
    {
        UnityEngine.Object result = null;
        if (Datas.TryGetValue(key, out result))
        {
            return result;
        }
        return result;
    }
    public UnityEngine.Object[] GetArrayData(string key)
    {
        UnityEngine.Object[] result = null;
        if (ArrayData.TryGetValue(key, out result))
        {
            return result;
        }
        return result;
    }
    
}
