using UnityEngine;
using System;
using Unity.Mathematics;

public class PlatformRemove : MonoBehaviour
{
    GameObject platformTop = new GameObject();
    GameObject platformDown = new GameObject();
    public bool playerInRange = false;
    GameObject currentNearbyButton = new GameObject();
    ButtonList buttonList;
    CharacterStun characterStun;
    
    public void Start()
    {
         platformTop = GameObject.Find("ButtonPlane1");
         platformDown = GameObject.Find("ButtonPlane2");
         buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
         
         

    }

  
    
    public void TopPLatformRemoveOnButtonPressed()
    {
        Destroy(platformTop);
    }
    public void DownPLatformRemoveOnButtonPressed()
    {
        Destroy(platformDown);
    }

    public void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "ButtonTop")
        {
            TopPLatformRemoveOnButtonPressed();
            System.Random random = new System.Random();
            int trueButtonNumber = random.Next(0, buttonList.knopk.Length);
            Debug.Log(trueButtonNumber);
            buttonList.knopk[trueButtonNumber].transform.Find("Button").tag = "TrueButton";
            Debug.Log(buttonList.knopk[trueButtonNumber].name);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "BottomButton")
        {
            DownPLatformRemoveOnButtonPressed();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "TrueButton")
        {
            
            //Win
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "Button")
        {
            GetComponent<CharacterStun>().ApplyStun(2f);

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ) playerInRange = true;
        currentNearbyButton = gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
        if (other.CompareTag("Player") && currentNearbyButton == gameObject)
        {
            currentNearbyButton = null;
        }
       

    }

}
