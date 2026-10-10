using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class inappreviw : MonoBehaviour
{

    public void RequestAppReview()
    {
#if UNITY_IPHONE
        Device.RequestStoreReview();
    
 
#endif
    }


    public void Privacy(string URL)
    {
        Application.OpenURL(URL);
    }
}
