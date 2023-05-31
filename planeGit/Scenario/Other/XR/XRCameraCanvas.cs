using UnityEngine;
using UnityEngine.UI;

public class XRCameraCanvas : MonoBehaviour
{
    [SerializeField]
    RawImage display;
    WebCamTexture camTexture;
    public void OnEnable()
    {
        if (camTexture != null)
        {
            display.texture = null;
            camTexture.Stop();
            camTexture = null;
        }
        
        WebCamDevice[] webcamDevices = WebCamTexture.devices;

        for (int index = 0; index < webcamDevices.Length; index++)
        {
            if (webcamDevices[index].name.Contains("VIVE"))
            {
                WebCamDevice device = WebCamTexture.devices[index];
                camTexture = new WebCamTexture(device.name);
                display.texture = camTexture;
                display.color = Color.white;
                camTexture.Play();
                return;
            }
        }
    }

    public void OnDisable()
    {
        if (camTexture != null)
        {
            display.texture = null;
            camTexture.Stop();
            camTexture = null;
        }
    }
}
