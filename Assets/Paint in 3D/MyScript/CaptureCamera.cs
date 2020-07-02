using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/// <summary>
/// 对相机截图。 
/// </summary>
/// <returns>The screenshot2.</returns>
/// <param name="camera">Camera.要被截屏的相机</param>
/// <param name="rect">Rect.截屏的区域</param>

public static class CaptureCamera
{
    /// <summary>
    /// 获得主相机的截屏
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Texture2D getScreenShot(Camera camera, Rect rect)
    {
        // 创建一个RenderTexture对象
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //ps: camera2.targetTexture = rt;
        //ps: camera2.Render();
        //ps: -------------------------------------------------------------------

        // 激活这个rt, 并从中中读取像素。
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        return screenShot;
    }
    public static Texture2D colorFilter(Color color,Texture2D capturePic)
    {
        int width = capturePic.width;
        int height = capturePic.height;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(capturePic.GetPixel(i,j)!=color)
                {
                    capturePic.SetPixel(i, j, Color.black);
                }
            }
        }
        return capturePic;
    }
    /// <summary>
    /// 图像全局二值化
    /// </summary>
    /// <param name="texTemp"></param>
    /// <returns></returns>
    public static Texture2D Otsu(Texture2D texTemp)
    {
        int FinalValue;
        float finalValueFloat = 0;
        int width = texTemp.width;
        int height = texTemp.height;
        float[] nHistogram = new float[256];//灰度直方图  
        float[] dVariance = new float[256];//类间方差
        int N = width * height;//总像素数
        for (int i = 0; i < 256; i++)
        {
            nHistogram[i] = 0.0f;
            dVariance[i] = 0.0f;
        }
        float g = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                g = texTemp.GetPixel(i, j).grayscale;
                int temp = (int)Math.Round(g * 255);
                nHistogram[temp]++;//建立直方图
            }
        }
        float Pa = 0.0f;      //背景出现概率  
        float Pb = 0.0f;      //目标出现概率  
        float Wa = 0.0f;      //背景平均灰度值  
        float Wb = 0.0f;      //目标平均灰度值  
        float W0 = 0.0f;      //全局平均灰度值  
        float dData1 = 0.0f;
        float dData2 = 0.0f;
        //计算全局平均灰度 
        for (int i = 0; i < 256; i++)
        {
            nHistogram[i] /= N;
            W0 += i * nHistogram[i];
        }
        float scale = -0.008f * W0 + 2.5f;
        //对每个灰度值计算类间方差  
        for (int i = 0; i < 256; i++)
        {
            Pa += nHistogram[i];
            Pb = 1 - Pa;
            dData1 += i * nHistogram[i];
            dData2 = W0 - dData1;
            Wa = dData1 / Pa;
            Wb = dData2 / Pb;
            dVariance[i] = (Pa * Pb * Mathf.Pow((Wb - Wa), 2));
        }
        //遍历每个方差，求取类间最大方差所对应的灰度值 
        float temp2 = 0f;
        for (int i = 0; i < 256; i++)
        {
            if (dVariance[i] > temp2)
            {
                temp2 = dVariance[i];
                FinalValue = i;
                finalValueFloat = FinalValue / 255f;
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (texTemp.GetPixel(i, j).grayscale < finalValueFloat+0.02f) { texTemp.SetPixel(i, j, Color.white * 0); }
                else
                {
                    texTemp.SetPixel(i, j, Color.white);
                }
            }
        }
        return texTemp;
    }

    public static Texture2D Csauvola(Texture2D trueTex, int w, int h, float k, int windowSize)
    {
        float[] grayImage = new float[trueTex.width * trueTex.height];
        int tw = trueTex.width;
        int th = trueTex.height;
        for (int m = 0; m < tw; m++)
        {
            for (int n = 0; n < th; n++)
            {
                grayImage[n * tw + m] = trueTex.GetPixel(m, n).grayscale;
            }
        }  

        int whalf = windowSize >> 1;
        //windowSize = 1;
        int i, j;
        int IMAGE_WIDTH = w;
        int IMAGE_HEIGHT = h;
        // create the integral image
        float[] integralImg = new float[IMAGE_WIDTH * IMAGE_HEIGHT];
        float[] integralImgSqrt = new float[IMAGE_WIDTH * IMAGE_HEIGHT];
        float sum = 0;
        float sqrtsum = 0;
        int index;
        StringBuilder sbs = new StringBuilder();
        int ids = 0;
        for (i = 0; i < IMAGE_HEIGHT; i++)
        {
            sum = 0;
            sqrtsum = 0;
            for (j = 0; j < IMAGE_WIDTH; j++)
            {
                //index = (IMAGE_HEIGHT - 1 - i) * IMAGE_WIDTH + j;
                index = i * IMAGE_WIDTH + j;
                sum += grayImage[index];
                sqrtsum += grayImage[index] * grayImage[index];
                ids += 1;
                sbs.AppendLine("g=" + grayImage[index] + "sum=" + sum + "sqrtsum=" + sqrtsum + "index=" + ids);
                if (i == 0)
                {
                    integralImg[index] = sum;
                    integralImgSqrt[index] = sqrtsum;
                }
                else
                {
                    integralImgSqrt[index] = integralImgSqrt[(i - 1) * IMAGE_WIDTH + j] + sqrtsum;
                    integralImg[index] = integralImg[(i - 1) * IMAGE_WIDTH + j] + sum;
                }
            }
        }
        int xmin, ymin, xmax, ymax;
        float mean, std, threshold;
        float diagsum, idiagsum, diff, sqdiagsum, sqidiagsum, sqdiff, area;
        for (i = 0; i < IMAGE_WIDTH; i++)
        {
            for (j = 0; j < IMAGE_HEIGHT; j++)
            {
                xmin = Mathf.Max(0, i - whalf);
                ymin = Mathf.Max(0, j - whalf);
                xmax = Mathf.Min(IMAGE_WIDTH - 1, i + whalf);
                ymax = Mathf.Min(IMAGE_HEIGHT - 1, j + whalf);
                area = (xmax - xmin + 1) * (ymax - ymin + 1);
                if (area <= 0)
                {
                    trueTex.SetPixel(i,j, Color.clear);//255;
                    continue;
                }
                if (xmin == 0 && ymin == 0)
                {
                    diff = integralImg[ymax * IMAGE_WIDTH + xmax];
                    sqdiff = integralImgSqrt[ymax * IMAGE_WIDTH + xmax];
                }
                else if (xmin > 0 && ymin == 0)
                {
                    diff = integralImg[ymax * IMAGE_WIDTH + xmax] - integralImg[ymax * IMAGE_WIDTH + xmin - 1];
                    sqdiff = integralImgSqrt[ymax * IMAGE_WIDTH + xmax] - integralImgSqrt[ymax * IMAGE_WIDTH + xmin - 1];
                }
                else if (xmin == 0 && ymin > 0)
                {
                    diff = integralImg[ymax * IMAGE_WIDTH + xmax] - integralImg[(ymin - 1) * IMAGE_WIDTH + xmax];
                    sqdiff = integralImgSqrt[ymax * IMAGE_WIDTH + xmax] - integralImgSqrt[(ymin - 1) * IMAGE_WIDTH + xmax]; ;
                }
                else
                {
                    diagsum = integralImg[ymax * IMAGE_WIDTH + xmax] + integralImg[(ymin - 1) * IMAGE_WIDTH + xmin - 1];
                    idiagsum = integralImg[(ymin - 1) * IMAGE_WIDTH + xmax] + integralImg[ymax * IMAGE_WIDTH + xmin - 1];
                    diff = diagsum - idiagsum;
                    sqdiagsum = integralImgSqrt[ymax * IMAGE_WIDTH + xmax] + integralImgSqrt[(ymin - 1) * IMAGE_WIDTH + xmin - 1];
                    sqidiagsum = integralImgSqrt[(ymin - 1) * IMAGE_WIDTH + xmax] + integralImgSqrt[ymax * IMAGE_WIDTH + xmin - 1];
                    sqdiff = sqdiagsum - sqidiagsum;
                }
                //灰度和平均值
                mean = diff / area;
                //标准方差。
                std = Mathf.Sqrt((sqdiff - diff * diff / area) / (area - 1));
                //阈值
                threshold = mean * (1 + k * ((std / 255) - 1));
                //当前像素灰度
                float golys = grayImage[j * IMAGE_WIDTH + i];
                //用当前像素灰度和当前阈值做比较
                if (grayImage[j * IMAGE_WIDTH + i] < threshold)
                {
                    trueTex.SetPixel(i,j, Color.white * 0);//0;
                }
                else
                {
                    trueTex.SetPixel(i,j,Color.white);//255;
                }
                //索贝尔算出图片边缘
                int iw = IMAGE_WIDTH;
                float gx = 0;
                float gy = 0;
                int addline = 1;
                if (j - addline < 0 || j + addline >= IMAGE_HEIGHT || i - addline < 0 || i + addline >= iw)
                {
                    trueTex.SetPixel(i,j, Color.clear);
                    continue;
                }
                gx = grayImage[(j - addline) * iw + i + addline] + 2 * grayImage[(j) * iw + i + addline] + grayImage[(j + addline) * iw + i + addline] - (grayImage[(j - addline) * iw + i - addline] + 2 * grayImage[(j) * iw + i - addline] + grayImage[(j + 1) * iw + i - addline]);
                gy = grayImage[(j - addline) * iw + i - addline] + 2 * grayImage[(j - addline) * iw + i] + grayImage[(j - addline) * iw + i + addline] - (grayImage[(j + addline) * iw + i - addline] + 2 * grayImage[(j + addline) * iw + i] + grayImage[(j + addline) * iw + i + addline]);
                float g = Mathf.Sqrt(Mathf.Pow(gx, 2) + Mathf.Pow(gy, 2));
                if (golys < g)
                {
                    trueTex.SetPixel(i,j, Color.clear);
                }
            }
        }
        return trueTex;
    }

    public static Texture2D NumberCutBaseColor(Texture2D texTemp)
    {
        int width = texTemp.width;
        int height = texTemp.height;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(texTemp.GetPixel(i,j).r>75&& texTemp.GetPixel(i, j).g > 75
                    && texTemp.GetPixel(i, j).b > 75)
                {
                    texTemp.SetPixel(i, j, Color.white);
                }
                else
                {
                    texTemp.SetPixel(i, j, Color.black);
                }
            }
        }
        return texTemp;
    }
    public static void savePic(Texture2D texTemp,string fileName)
    {
        // 最后将这些纹理数据，成一个png图片文件
        byte[] bytes = texTemp.EncodeToPNG();
        string filename = Application.dataPath + "/"+fileName+".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));
    }
}


