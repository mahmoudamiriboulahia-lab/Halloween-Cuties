using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerNeon : MonoBehaviour
{
    public Camera M_camera;
    public GameObject Brush;
    LineRenderer CurrentLineRenderer;
    Vector2 lastPosition;
    // Matriel 
    public Material[] matriel;
    public int conotre;
    // instite Color in The Ui
    public Button BtnColor;
    public GameObject PlaceButtton;
    // Set Wiethd 
    public Slider SliderWieth;
    int contoreBrush;

    //button index
    int index;
    //sprite
    public Sprite[] SpriteColoring;
    public SpriteRenderer BackgrouldColoing;
    // Start is called before the first frame update
    void Start()
    {
        BackgrouldColoing.sprite = SpriteColoring[CanvenceManager.Instance.contoreX];
        for (int i = 0; i < matriel.Length - 1; i++)
        {
            Button btn = (Button)Instantiate(BtnColor);
            //set button in Scroll View
            btn.transform.SetParent(PlaceButtton.transform, false);
            // change color button to matruiel image 
            btn.GetComponent<Image>().color = matriel[i].color;
            btn.GetComponent<BtnAddContore>().Contore = i;

        }
    }

    // Update is called once per frame
    void Update()
    {
        Draw();

    }
    //

    void Draw()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.transform.CompareTag("Bakround"))
                {
                    CreateBrush();
                }
            }


        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.transform.CompareTag("Bakround"))
                {

                    Vector2 mousPosition = M_camera.ScreenToWorldPoint(Input.mousePosition);
                    if (mousPosition != lastPosition)
                    {
                        AddPoint(mousPosition);
                        lastPosition = mousPosition;

                    }
                }
            }




        }
        else
        {
            CurrentLineRenderer = null;
        }
    }
    void CreateBrush()
    {

        contoreBrush++;
        GameObject brushInstance = Instantiate(Brush);
        CurrentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        CurrentLineRenderer.material = matriel[conotre];
        // setlayer

        CurrentLineRenderer.sortingOrder = contoreBrush;
        // set weith 
        CurrentLineRenderer.SetWidth(SliderWieth.value, SliderWieth.value);

        Vector2 mousePos = M_camera.ScreenToWorldPoint(Input.mousePosition);
        CurrentLineRenderer.SetPosition(0, mousePos);
        CurrentLineRenderer.SetPosition(1, mousePos);

    }
    void AddPoint(Vector2 pointPosition)
    {
        CurrentLineRenderer.positionCount++;
        int possitionIndx = CurrentLineRenderer.positionCount - 1;
        CurrentLineRenderer.SetPosition(possitionIndx, pointPosition);

    }
    //Chose btn
    public void BtnGoum()
    {
        conotre = matriel.Length - 1;
    }
    public void BtnBrush()
    {
        conotre = 0;
    }

}
