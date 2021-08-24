using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class OpenURL : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenWindow(string url);

    public void OpenIt(string url)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenWindow(url);
#endif
    }
}