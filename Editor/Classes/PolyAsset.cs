using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Super class for every Polybrush Assets implementing Reset() function
/// so that when you reset them from the inspector they don't loose their name
/// </summary>
public class PolyAsset : ScriptableObject
{
    /// <summary>
    /// Reset() function so that when you reset Polybrush Assets
    /// from the inspector they don't loose their name
    /// </summary>
    protected virtual void Reset()
    {
#if UNITY_6000_6_OR_NEWER
        // Unity 6.6 obsoletes GetInstanceID() and AssetDatabase.GetAssetPath(int).
        // GetAssetPath(Object) is still valid and is the direct replacement.
        string path = AssetDatabase.GetAssetPath(this);
#else
#pragma warning disable 618
        string path = AssetDatabase.GetAssetPath(this.GetInstanceID());
#pragma warning restore 618
#endif
        this.name = Path.GetFileNameWithoutExtension(path);
    }
}
