using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class uiz_small : MonoBehaviour
{

    public static string Scene_Name;
    public bool Show_Ads;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

  

    public void btn_restrt()
    {
        _btn_pindah(SceneManager.GetActiveScene().name);

    }
    public void _btn_pindah(string kk){
        this.gameObject.active = true ;
        Music_Singleton.Instance.s_play(0);
        Scene_Name = kk;
        GetComponent<Animator>().Play("end");
    }

    public void _btn_resume(){
        Music_Singleton.Instance.s_play(0);
        GetComponent<Animator>().Play("end2");
    }


    public void _anim_pindah(){
        SceneManager.LoadScene(Scene_Name);
    }

    public void _anim_close(){
        this.gameObject.SetActive(false);
    }
}
