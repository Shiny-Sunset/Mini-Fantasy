using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    public string fileName = "screenshot.png";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string path = Application.dataPath + "/" + fileName;
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("スクリーンショットを保存しました: " + path);
        }
    }
}
