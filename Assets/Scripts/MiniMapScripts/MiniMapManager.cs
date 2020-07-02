using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    // Start is called before the first frame update
    Transform miniMapRoot;
    bool state = false;
    private bool begining = true;
    private GameObject[] mapBtnImgs;
    private Brush brush;
    private Sprite brushImg;
    private Sprite brushWhiteImg;
    private SpriteRenderer Btn_Brush;

    void Start()
    {
        brushImg = Resources.Load("ItemIconImage/brush", typeof(Sprite)) as Sprite;
        brushWhiteImg = Resources.Load("ItemIconImage/brushWhite", typeof(Sprite)) as Sprite;

        brush = GameObject.Find("SpongeBrush").GetComponent<Brush>();
        miniMapRoot = this.transform.GetChild(1);
        mapBtnImgs = GameObject.FindGameObjectsWithTag("mapBtn_img");
        Btn_Brush = GameObject.Find("BrushImage").GetComponent<SpriteRenderer>();
        ShowMapOrNot();
    }
    public void ShowMapOrNot()
    {
        foreach (var image in mapBtnImgs)
        {
            image.GetComponent<Text>().enabled = state;
        }
        miniMapRoot.gameObject.GetComponent<Canvas>().enabled = state;
        state = !state;

    }
    public void changeBrushColor()
    {
        brush.paintState = brush.paintState.Equals(Brush.PaintState.clear) ? Brush.PaintState.white :Brush.PaintState.clear;
        Btn_Brush.sprite = brush.paintState.Equals(Brush.PaintState.clear) ? brushImg:brushWhiteImg;
    }
    // Update is called once per frame
    void Update()
    {
        if (begining)
        {
            begining = false;
            //miniMapRoot.gameObject.GetComponent<Canvas>().enabled = false;
        }
    }
    
}
