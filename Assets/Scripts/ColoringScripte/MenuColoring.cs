using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuColoring : MonoBehaviour
{
    public GameObject BtnColor ,PanelColor;
    public GameObject BtnStickers ,PanelStickers;
    public GameObject BtnPattem, PanelPattem;
    public GameObject BtnGlitters, PanelGlitters;

    public GameObject ButtonAnimation;



    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void _PanelToolsColor()
    {
            PanelColor.SetActive(true);
            PanelStickers.SetActive(false);
            PanelPattem.SetActive(false);
        PanelGlitters.SetActive(false);
    }
    public void _PanelToolsStickers()
    {
        PanelColor.SetActive(false);
        PanelStickers.SetActive(true);
        PanelPattem.SetActive(false);
        PanelGlitters.SetActive(false);
    }
    public void _PanelToolsPattem()
    {
        PanelColor.SetActive(false);
        PanelStickers.SetActive(false);
        PanelPattem.SetActive(true);
        PanelGlitters.SetActive(false);
    }
    public void _PanelToolsGlitters()
    {
        PanelColor.SetActive(false);
        PanelStickers.SetActive(false);
        PanelPattem.SetActive(false);
        PanelGlitters.SetActive(true);
    }

    public void Pressed()
    {
        this.GetComponent<Animator>().SetBool("Pressed", true);
    }
}
