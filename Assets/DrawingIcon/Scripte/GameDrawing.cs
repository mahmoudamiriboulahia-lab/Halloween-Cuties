using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDrawing : MonoBehaviour
{
    public GameObject ObjectInstite;
    public GameObject ObjectPopCorn;
    public Camera MainCamera;
    //sprite object
    public  Sprite[] SpriteObject;
    //time destroy object 
    public float TimeDestroy;
    public  float  TimeInstite;
    private float lastTime;
    int contore;
    // Start is called before the first frame update
    void Start()
    {
        contore = 0;

    }

    // Update is called once per frame
    void Update()
    {
        InstitieObject();
    }
   public void InstitieObject()
    {
        if (Input.GetMouseButton(0) && lastTime<=Time.time)
        {
            lastTime = Time.time + TimeInstite;
            Vector3 mouseposition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (contore==1)
            {
                GameObject oo = Instantiate(ObjectPopCorn, new Vector3(mouseposition.x, mouseposition.y, 0), Quaternion.identity);
              
                Destroy(oo, TimeDestroy);
            }
            else
            {
                GameObject ob = Instantiate(ObjectInstite, new Vector3(mouseposition.x, mouseposition.y, 0), Quaternion.identity);
                ob.GetComponent<SpriteRenderer>().sprite = SpriteObject[contore];
                Destroy(ob, TimeDestroy);
            }
           
        

        }
       
    }
    public void CHosePref(int i)
    {
        contore = i;
    }
}
