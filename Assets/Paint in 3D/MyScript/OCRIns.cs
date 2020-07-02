using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.baidu.ai;
using System.Web;
using System.IO;
///summary
///
///summary
public class OCRIns : MonoBehaviour
{
    public Color filterColor;
    public Camera eyes;
    private string token;
    private string access_token;
    private void Start()
    {
        token = AccessToken.getAccessToken();
        access_token = AccessToken.GetJosn(token,"access_token");
    }
    private void Update()
    {
        
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 10, 100, 30), "screenShot"))
        {
            //获取截图
            Texture2D picture = CaptureCamera.getScreenShot(eyes, new Rect(Screen.width * 0f, Screen.height * 0f, Screen.width * 1f, Screen.height * 1f));
            var result = DiscernPic.discernNumber(picture.EncodeToPNG());
            var handresult = DiscernPic.handWriteNumber(picture.EncodeToPNG());
            Debug.Log(" pre "+ result);
            Debug.Log("pre hand" + handresult);

            //将图像全局二值化处理
            Texture2D picture2Gray_Global = CaptureCamera.Otsu(picture);
            CaptureCamera.savePic(picture2Gray_Global, "picture2Gray_Global");
            result = DiscernPic.discernNumber(picture2Gray_Global.EncodeToPNG());
            handresult = DiscernPic.handWriteNumber(picture.EncodeToPNG());
            Debug.Log(" after 2gray " + result);
            Debug.Log("after 2gray hand" + handresult);
            //将图像局部二值化处理
            Texture2D pic2Gray_Local = CaptureCamera.Csauvola(picture,Screen.width, Screen.height,1,2);
            CaptureCamera.savePic(pic2Gray_Local, "pic2Gray_Local");
            //Debug.Log(" after 2gray " + result);
            //将图片根据固定颜色过滤
            Texture2D picFilter = CaptureCamera.colorFilter(filterColor,picture);
            CaptureCamera.savePic(picFilter, "picFilter");
        }
    }
}
