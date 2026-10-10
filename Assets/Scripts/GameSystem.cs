using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    int roboot = 0;
    private static GameSystem _instance = null;
	public static GameSystem Instance
	{
		get {return _instance ; }
	}
    public static int IDGame;
    public int ID_Answer = 0 ;
    public bool GameActive ;
    public bool GameFinish;

    [Serializable]
    public class GameImage{
        public Sprite[] Image;
    }

    
    public GameImage[] Game_Image;
    public Sprite Blank_Image;
    
    [Header("Game Collect")]
    public float Data_Timer ;
    public int Data_Score ;
   
    [Header("GUI & Text UI Here")]
    public TextMeshProUGUI Text_Score;
    public TextMeshProUGUI Text_Timer;
    public GameObject[] Uinya;
    public GameObject Ui_Tutorial,Ui_Timer;
    [Header("Game Position Setup")]
     public GameObject item_Drag;

    public List<GameObject> Pos_ID ;
    public List<GameObject> Container_ID ;
    public List<GameObject> Container_ID_DraggedItem ;
    int y;
   [HideInInspector]public List<int> Save_ID = new List<int>();
    [HideInInspector]public List<int> Save_Spawn_ID = new List<int>();
    // Start is called before the first frame update

    int rand;

    void Awake(){
		if(_instance == null){
			_instance = this;
		}
	}

    void Start()
    {      
        GameActive = false ;
        GameFinish = false;
        Data_Timer = 60;
        Data_Score = 0 ;
        //Begin();

         y = SceneManager.GetActiveScene().buildIndex;


        Begin();
        

    }


    public void Begin(){
        
       if(SceneManager.GetActiveScene().name != "Game1")
        {
            StartCoroutine(Game_Begin());
        }
        else
        {
            StartCoroutine(Game_Begin_1());
        }


       
    }

    IEnumerator Game_Begin(){
        yield return new WaitForSeconds(0.5f);
        GameActive = true ;
        ID_Answer = 0;
        Save_ID = new List<int>(new int[Container_ID.Count]);
        Save_Spawn_ID = new List<int>(new int[Container_ID.Count]);
        for (int i = 0; i < Container_ID.Count; i++)
        {
            rand = UnityEngine.Random.Range(1,Game_Image[IDGame].Image.Length);
            while(Save_ID.Contains(rand)){
                  rand = UnityEngine.Random.Range(1,Game_Image[IDGame].Image.Length);
            }
            Save_ID[i] = rand;
            Save_Spawn_ID[i] = rand - 1;
            Pos_ID[i].GetComponent<Image>().sprite = Game_Image[IDGame].Image[i];
            //Pos_ID[i].GetComponent<slotid>().idslot = Save_Spawn_ID[i];
        }
        StartCoroutine(Anim_Begin());
    }

    IEnumerator Game_Begin_1()
    {
        yield return new WaitForSeconds(0.5f);
        //GameActive = true;
        ID_Answer = 0;
        Save_ID = new List<int>(new int[Container_ID.Count]);
        Save_Spawn_ID = new List<int>(new int[Container_ID.Count]);
        for (int i = 0; i < Container_ID.Count; i++)
        {
            rand = UnityEngine.Random.Range(1, Game_Image[IDGame].Image.Length);
            while (Save_ID.Contains(rand))
            {
                rand = UnityEngine.Random.Range(1, Game_Image[IDGame].Image.Length);
            }
            Save_ID[i] = rand;
            Save_Spawn_ID[i] = rand - 1;
            Pos_ID[i].GetComponent<Image>().sprite = Game_Image[IDGame].Image[Save_Spawn_ID[i]];
            Pos_ID[i].GetComponent<slotid>().idslot = Save_Spawn_ID[i];
        }
        StartCoroutine(Anim_Begin());
        Ui_Timer.SetActive(true);
        yield return new WaitForSeconds(7f);
        Game1_Blank(); 
    }

    public void Game1_Blank()
    {
        for (int i = 0; i < Container_ID_DraggedItem.Count; i++)
        {
            GameObject gg = Container_ID_DraggedItem[i].GetComponentInChildren<idobject>().gameObject;
            gg.GetComponent<Image>().sprite = Blank_Image;
            gg.GetComponent<Animation>().Play("slot_anim_start");
        }
        GameActive = true;
        Ui_Timer.SetActive(false);
    }

    int ads = 0;
    IEnumerator Anim_Begin(){
        List<int> SaveRandom = new List<int>(new int[Container_ID.Count]);
        int Begin = Container_ID.Count + 1 ;
        for (int i = 0; i < SaveRandom.Count; i++)
        {
            rand = UnityEngine.Random.Range(1,Begin);
            while(SaveRandom.Contains(rand)){
                  rand = UnityEngine.Random.Range(1,Begin);
            }
            SaveRandom[i] = rand;
            
            Music_Singleton.Instance.s_play(5);
            Pos_ID[rand-1].GetComponent<Animation>().Play("slot_anim_start");
            StartCoroutine(Spawn_Item(i));
            yield return new WaitForSeconds(0.2f);
        }
        if(SceneManager.GetActiveScene().name == "Game2")
        {
            yield return new WaitForSeconds(0.25f);
            for (int i = 0; i < Container_ID_DraggedItem.Count; i++)
            {
                GameObject gg = Container_ID_DraggedItem[i].GetComponentInChildren<idobject>().gameObject;
                //gg.GetComponent<Image>().sprite = Blank_Image;
                gg.GetComponent<Animation>().Play("ChangeAnim");
            }
        }
        AdsControl.instance.ShowInterstitalRandomCurrent();

        // StartCoroutine(Spawn_Item());
    }


    public int deleted;
    public IEnumerator Spawn_Item(int id){
        yield return new WaitForSeconds(0);
        Music_Singleton.Instance.s_play(5);
        GameObject gg = Instantiate(item_Drag);
        
        
        gg.transform.SetParent(Container_ID_DraggedItem[id].transform,false);
        gg.transform.position = Container_ID_DraggedItem[id].transform.position;
        int range= UnityEngine.Random.Range(0,Save_Spawn_ID.Count);
        deleted = range;
        gg.GetComponent<Image>().sprite =  Game_Image[IDGame].Image[Save_Spawn_ID[range]];
        gg.GetComponent<idobject>().idnya = Save_Spawn_ID[range];
        gg.GetComponent<Animation>().Play("slot_anim_start");  
        Save_Spawn_ID.RemoveAt(deleted);     
        gg.transform.SetAsLastSibling();

    }

    public void Show_UI(int i){
        Uinya[i].active = true;
    }

    public void btn_Paused(){
        Uinya[1].active = true ;
        GameActive = false ;
    }

    public void btn_Resumed(){
        Uinya[1].active =false ;
        GameActive = true ;
    }

    public void btn_mute(){
        Music_Singleton.Instance.btn_muted();
    }

    public IEnumerator ResetGame(){
 
        yield return new WaitForSeconds(0.05f);
        {

            if(SceneManager.GetActiveScene().name == "G0")
                SceneManager.LoadScene("G1");
            if (SceneManager.GetActiveScene().name == "G1")
                SceneManager.LoadScene("G2");
            if (SceneManager.GetActiveScene().name == "G3")
                SceneManager.LoadScene("G3");
            if (SceneManager.GetActiveScene().name == "G3")
                SceneManager.LoadScene("SelectGame");
            /* if(y == 14)
             {
                 SceneManager.LoadScene("SelectGame");
             }
             else
             {

                 if (SceneManager.GetActiveScene().name != "Game1")
                 {

                     SceneManager.LoadScene(y);
                 }
                 else
                 {

                     SceneManager.LoadScene("Game1");
                 }

             }*/

        }
        
           
       
      


        /* Save_ID.Clear();
         Save_Spawn_ID.Clear();
         GameObject[] Item = GameObject.FindGameObjectsWithTag("item"); 
        for (int i = 0; i < Pos_ID.Count; i++)
        {

            Pos_ID[i].GetComponent<Animation>().Play("slot_anim_End");

            Pos_ID[i].GetComponent<slotid>().terisi = false ;
        } 
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < Pos_ID.Count; i++)
        {
            Pos_ID[i].transform.localScale = new Vector2(0,0);
            Destroy(Item[i]);
        }


        Begin();*/

    }

    void TextSetupUi(){
        int min = Mathf.FloorToInt(Data_Timer / 60);
		int sec = Mathf.FloorToInt(Data_Timer % 60);
		Text_Timer.GetComponent<TextMeshProUGUI>().text = min.ToString("00") + ":" + sec.ToString("00");
        Text_Score.GetComponent<TextMeshProUGUI>().text = Data_Score.ToString() ;
    }

    float t ;
    void TimerSetting(){
		if(GameActive && !GameFinish){
			t += Time.deltaTime ;
			if(t >= 1){
				Data_Timer -= 1;
				t = 0;
			}
		}
	}

    void Update(){
        TimerSetting();
        TextSetupUi();

        if(Data_Timer <= 0 && !GameFinish){
            GameFinish = true ;
            uiz_small.Scene_Name = "Home";
            Uinya[0].active = true ;
            Music_Singleton.Instance.s_play(4);

            int data = PlayerPrefs.GetInt("score"+SceneManager.GetActiveScene().name);
            if(Data_Score > data){
                PlayerPrefs.SetInt("score" + SceneManager.GetActiveScene().name, Data_Score);
            }

           // StartCoroutine(BackToMainMenu());
        }
    }

    IEnumerator BackToMainMenu(){
        uiz_small.Scene_Name = "Home";
        yield return new WaitForSeconds(3f);
//        Uinya[2].GetComponent<uiz_small>()._btn_pindah("Home");

    }
}
