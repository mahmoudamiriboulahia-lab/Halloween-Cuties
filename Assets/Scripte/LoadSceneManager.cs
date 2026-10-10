using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static string Scene_Name;
    public static LoadSceneManager USE;
    private void Awake()
    {
        USE = this;
    }
    public void btn_restrt()
    {
        _btn_pindah(SceneManager.GetActiveScene().name);

    }

    public void _btn_pindah(string kk)
    {
        this.gameObject.active = true;
        // Music_Singleton.Instance.s_play(0);
        Scene_Name = kk;
        GetComponent<Animator>().Play("end");
    }
    public void Ads_btn_pindah(string kk)
    {
        this.gameObject.active = true;
        // Music_Singleton.Instance.s_play(0);
        Scene_Name = kk;
        GetComponent<Animator>().Play("end");
    }
    public void _btn_pindahSave(string kk)
    {
        this.gameObject.active = true;
        // Music_Singleton.Instance.s_play(0);
        Scene_Name = kk;
        GetComponent<Animator>().Play("end");
        GameObject cL = new GameObject();
        cL = GameObject.Find("PaintingBoard");
        Destroy(cL);
    }

    public void _btn_resume()
    {
        // Music_Singleton.Instance.s_play(0);
        GetComponent<Animator>().Play("end2");
    }


    public void _anim_pindah()
    {
        SceneManager.LoadScene(Scene_Name);
    }


    public void privacy(string Privacy)
    {
        Application.OpenURL(Privacy);
    }

    public void _anim_close()
    {
        this.gameObject.SetActive(false);
    }
    public void LoadA(string scenename)
    {
        
        SceneManager.LoadScene(scenename);
    }



}
