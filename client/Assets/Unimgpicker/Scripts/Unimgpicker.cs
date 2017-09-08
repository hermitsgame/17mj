﻿using UnityEngine;
using System.Collections;

public class Unimgpicker : MonoBehaviour
{
    public delegate void ImageDelegate(string path);

    public delegate void ErrorDelegate(string message);

    public event ImageDelegate Completed;

    public event ErrorDelegate Failed;

    private IPicker picker =
#if UNITY_IOS && !UNITY_EDITOR
            new PickeriOS();
#elif UNITY_ANDROID && !UNITY_EDITOR
            new PickerAndroid();
#else
            new PickerUnsupported();
#endif

    public void Show(string title, string outputFileName, string objName, int maxSize)
    {
        picker.Show(title, outputFileName, objName, maxSize);
    }

    public void OnComplete(string path)
    {
        var handler = Completed;
        if (handler != null)
        {
            handler(path);
        }
    }

    public void OnFailure(string message)
    {
        var handler = Failed;
        if (handler != null)
        {
            handler(message);
        }
    }
}