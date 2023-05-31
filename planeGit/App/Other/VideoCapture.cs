using System;
using System.IO;
using RenderHeads.Media.AVProMovieCapture;
using UnityEngine;

[RequireComponent(typeof(CaptureFromCamera))]
public class VideoCapture : MonoBehaviour
{
    #region

    [SerializeField] CaptureFromCamera captureFromCamera;

    #endregion

    #region Public Method
    
    public void StartCapture(string folderName, string fileName)
    {
        string folderPath = $"C:/share/Replay/{folderName}";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        captureFromCamera.OutputFolderPath = folderPath;
        captureFromCamera.FilenamePrefix = fileName;
        captureFromCamera.StartCapture();
    }
    
    public void EndCapture()
    {
        captureFromCamera.StopCapture();
    }
    
    #endregion
}
