 using UnityEngine;
 //using UnityEngine.Purchasing;


public class IAPManager : MonoBehaviour
{
    private string premium = "premium";
  
   // public GameObject restoreButton;



    public void Awake()
    {
       // if (Application.platform != RuntimePlatform.IPhonePlayer)
          //  restoreButton.SetActive(false);

    }


  /*  public void OnComplete(Product product)
    {

        if (product.definition.id == premium)
        {
            PlayerPrefs.SetString("GoPremium", "premium");
            PlayerPrefs.Save();
        }
       
        
    }
    public void OnFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(product.definition.id + "Failed because" + failureReason);
        Debug.Log("kayn gafi");
    }*/
    


}


