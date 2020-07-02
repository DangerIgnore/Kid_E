
/*
 * Native dll invocation helper by Francis R. Griffiths-Keam
 * www.runningdimensions.com/blog
 * https://forum.unity.com/threads/unloading-native-plugins-in-the-unity-editor.198296/
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Native
{
    public static T Invoke<T, T2>(IntPtr library, params object[] pars)
    {
        IntPtr funcPtr = GetProcAddress(library, typeof(T2).Name);
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogWarning("Could not gain reference to method address.");
            return default(T);
        }

        var func = Marshal.GetDelegateForFunctionPointer(GetProcAddress(library, typeof(T2).Name), typeof(T2));
        return (T)func.DynamicInvoke(pars);
    }

    public static void Invoke<T>(IntPtr library, params object[] pars)
    {
        IntPtr funcPtr = GetProcAddress(library, typeof(T).Name);
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogWarning("Could not gain reference to method address.");
            return;
        }

        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
        func.DynamicInvoke(pars);
    }

    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
}

/*
 C# Usage:
 using System;
using UnityEngine;
 
public class Example : MonoBehaviour
{
    static IntPtr nativeLibraryPtr;
 
    delegate int MultiplyFloat(float number, float multiplyBy);
    delegate void DoSomething(string words);
 
    void Awake()
    {
        if (nativeLibraryPtr != IntPtr.Zero) return;
 
        nativeLibraryPtr = Native.LoadLibrary("MyNativeLibraryName");
        if (nativeLibraryPtr == IntPtr.Zero)
        {
            Debug.LogError("Failed to load native library");
        }
    }
 
    void Update()
    {
        Native.Invoke<DoSomething>(nativeLibraryPtr, "Hello, World!");
        int result = Native.Invoke<int, MultiplyFloat>(nativeLibraryPtr, 10, 5); // Should return the number 50.
    }
 
    void OnApplicationQuit()
    {
        if (nativeLibraryPtr == IntPtr.Zero) return;
 
        Debug.Log(Native.FreeLibrary(nativeLibraryPtr)
                      ? "Native library successfully unloaded."
                      : "Native library could not be unloaded.");
    }
}
 
     
     
     
     */
