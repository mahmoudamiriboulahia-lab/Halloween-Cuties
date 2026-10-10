using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MathsGameManger : MonoBehaviour
{  [Header("Text value In UI")]
   public Text TextValue1;
    public Text TextValue2;
     [Header("Button Answer")]
     public GameObject[] ButtonAnswer;
    public GameObject SymbolResulteTotal;
    public GameObject SymbolResulteV0;
    public GameObject SymbolResulteV1;

    public GameObject textQst;
   public Vector2 targetPosition;
    
    int  val1;
    int val2;
    //x  is the index of button answer
 int x;
  [Header("item  gred")]
 // add item to the gread list 
 public GameObject PanelView1;
 public GameObject PanelView2;
 public Image ImageItem;
 //Game over
 public GameObject PanelGameOver;
 public GameObject Partical;
    // Start is called before the first frame update
    void Start()
    {
        AdsControl.instance.ShowInterstitialAd();

        Operration();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Operration(){
        //
        //take range number 
        val1=Random.Range(1,9);
         val2=Random.Range(1,9);
         // show number in UI
         TextValue1.text=val1+"";
         TextValue2.text=val2+"";
         // give the correct number to one the button with random way 
          x=Random.Range(0,ButtonAnswer.Length);
         ButtonAnswer[x].GetComponentInChildren<Text>().text=(val1+val2)+"";
         // 
         for(int i=0;i<ButtonAnswer.Length;i++){
             if(x!=i){
                // the correct number doit diffrent from others
                int correctNumber=val1+val2;
                //Total
                for(int r=0; r< correctNumber; r++)
                {
                    SymbolResulteTotal.transform.GetChild(r).gameObject.SetActive(true);
                }

                //V0
                for (int r = 0; r < val1; r++)
                {
                    SymbolResulteV0.transform.GetChild(r).gameObject.SetActive(true);
                }
                //V1
                for (int r = 0; r < val2; r++)
                {
                    SymbolResulteV1.transform.GetChild(r).gameObject.SetActive(true);
                }
                int Rdm=Random.Range(1,20);
                    while(correctNumber==Rdm){
                        Rdm=Random.Range(1,20);
                        } 
                 //show number in the buttons
                      ButtonAnswer[i].GetComponentInChildren<Text>().text=Rdm+"";
                 
                
             }
         }
  

       


    }
    
    IEnumerator waiting(){
        yield return new WaitForSeconds(1);
    }
 public void CHeckButtonCklick(int i){
    if(i==x){



          // Get the button's RectTransform component
        //RectTransform rectTransform = ButtonAnswer[i].GetComponent<RectTransform>();

        // Set the button's position to the target position
       // rectTransform.anchoredPosition = targetPosition;
            /////
            ///
           textQst.GetComponent<Text>().text = ButtonAnswer[i].GetComponentInChildren<Text>().text;
           // textQst.SetActive(false);
       //show game over
       StartCoroutine(GameOver());

    }else{
        ButtonAnswer[i].SetActive(false);
    }

  }
        
IEnumerator GameOver(){
  yield return new WaitForSeconds(0.3f);
  
        //PanelGameOver.SetActive(true);

        //Rest scene 
        StartCoroutine(RestGame());
}
IEnumerator RestGame(){
  yield return new WaitForSeconds(1.1f);
   SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log(SceneManager.GetActiveScene().name);
        
}
}
