using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPiano : MonoBehaviour
{
    public GameObject[] ToolsMusic;
    // Start is called before the first frame update
    void Start()
    {
        ChangeToolsMusic(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToolsMusic(int tool)
    {
        for(int i =  0;i< ToolsMusic.Length; i++)
        {
            ToolsMusic[i].SetActive(false);
        }
        ToolsMusic[tool].SetActive(true);
    }
}
