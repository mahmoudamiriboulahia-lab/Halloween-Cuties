using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnAddContore : MonoBehaviour
{
    public int Contore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BtnAdd()
    {  
        FindObjectOfType<GameManagerNeon>().conotre = Contore;
    }

}
