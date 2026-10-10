using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParentPanel: MonoBehaviour
{
  [Header("Text value In UI")]
    public Text TextValue1;
    public Text TextValue2;
    [Header("Button Answer")]
    public GameObject[] ButtonAnswer;
    //
    public GameObject textQst;

    int val1;
    int val2;
    //x  is the index of button answer
    int x;


    public GameObject PanelPurchase;
    public GameObject PanelParent;
    // Start is called before the first frame update

    public Sprite NoCor;
    public Sprite CorlorSprit;
    void Start()
    {
        Operration();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Operration()
    {
        //
        //take range number 
        val1 = Random.Range(1, 9);
        val2 = Random.Range(1, 9);

        // show number in UI
        TextValue1.text = val1 + "";
        TextValue2.text = val2 + "";

        // give the correct number to one the button with random way 
         x = Random.Range(0, ButtonAnswer.Length);
        //    x = Random.Range(0, 9);

        ButtonAnswer[x].GetComponentInChildren<Text>().text = (val1 * val2) + "";
        //


        for (int i = 0; i < ButtonAnswer.Length; i++)
        {
            ButtonAnswer[i].GetComponent<Image>().sprite = CorlorSprit;
            if (x != i)
            {
                // the correct number doit diffrent from others
                int correctNumber = val1 * val2;
                int Rdm = Random.Range(1, 20);
                while (correctNumber == Rdm)
                {
                    Rdm = Random.Range(1, 20);
                }
                //show number in the buttons
                ButtonAnswer[i].GetComponentInChildren<Text>().text = Rdm + "";


            }
        }
      




    }

    IEnumerator waiting()
    {
        yield return new WaitForSeconds(1);
    }
    public void CHeckButtonCklick(int i)
    {
        if (i == x)
        {
            textQst.GetComponent<Text>().text = val1 * val2 + "";

            //show 
            StartCoroutine(GoPurchasePanel());

        }
        else
        {
            ButtonAnswer[i].GetComponent<Image>().sprite=NoCor;
            StartCoroutine(HidPanelParent());
            

        }

    }

    IEnumerator GoPurchasePanel()
    {
        yield return new WaitForSeconds(0.3f);
        PanelParent.SetActive(false);
        PanelPurchase.SetActive(true);
        Operration();


        //Rest scene 
        //  StartCoroutine(RestGame());
    }
    IEnumerator HidPanelParent()
    {
        yield return new WaitForSeconds(1.1f);
         PanelParent.SetActive(false);
        Operration();



    }
}
