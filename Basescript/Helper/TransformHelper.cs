using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper
{
    public static Transform DeepFind(this Transform parent,string targetName)
    {
        Transform res = null;
        foreach(Transform child in parent)
        {
            if(child.name == targetName) return child;
            else
            {
                res = DeepFind(child,targetName);
                if (res != null) return res;
            }
        }
        return null;
    }
}
