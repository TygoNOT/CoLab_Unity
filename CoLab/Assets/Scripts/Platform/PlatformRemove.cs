using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public class PlatformRemove : NetworkBehaviour
{
    private GameObject platformTop;
    private GameObject platformDown;
    public bool playerInRange = false;
    private GameObject currentNearbyButton;
    private SymbolsManager symbolsManager;
    public Material[] allMaterials;
    public AudioClip pressSound;
    private AudioSource audioSource;
    public AudioClip incorrectPressSound;
    ButtonList buttonList;
    public Transform buttonCube;
    private GameObject playerInstance;
    [SerializeField]
    private GameObject finish;
    [SerializeField]
    private GameObject invisibleWall;
    [SerializeField]
    private GameObject door1;
    [SerializeField]
    private GameObject door2;
    public void Start()
    {
        audioSource = buttonCube.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = buttonCube.gameObject.AddComponent<AudioSource>();
        }

        if (pressSound != null)
        {
            audioSource.clip = pressSound;
        }
        platformTop = GameObject.Find("ButtonPlane1");
        platformDown = GameObject.Find("ButtonPlane2");

        if (GameObject.FindObjectOfType<SymbolsManager>() != null)
        {
            symbolsManager = GameObject.FindObjectOfType<SymbolsManager>();
            allMaterials = symbolsManager.GetMaterials();
        }
        
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
            if (pressSound != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
            TopPLatformRemoveOnButtonPressed();
            System.Random random = new System.Random();
            int trueButtonNumber = random.Next(0, buttonList.knopk.Length);
            Debug.Log(trueButtonNumber);
            buttonList.knopk[trueButtonNumber].transform.Find("Button").tag = "TrueButton";
            Debug.Log(buttonList.knopk[trueButtonNumber].name);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "BottomButton")
        {
            if (pressSound != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            DownPLatformRemoveOnButtonPressed();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "TrueButton")
        {
            if (pressSound != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            finish.SetActive(true);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "Button")
        {
            if (incorrectPressSound != null)
            {
                audioSource.clip = incorrectPressSound;
                audioSource.volume = 0.2f;
                audioSource.Play();
            }
            playerInstance.GetComponent<NetworkObject>().GetComponent<PlayerMovement>().ApplyStunClientRpc(1f);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "DoorButton")
        {
            if(invisibleWall!=null) invisibleWall.SetActive(false);
            if (pressSound != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "SymbolRotation")
        {
            if (pressSound != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            SymbolCube cube = currentNearbyButton.GetComponentInParent<SymbolCube>();
            if (cube == null) cube = currentNearbyButton.GetComponentInChildren<SymbolCube>();

            if (cube != null)
            {
                cube.NextSymbol();
            }
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && currentNearbyButton.tag == "SequenceCheckButton")
        {
            SymbolsManager symbolsManager = GameObject.FindObjectOfType<SymbolsManager>();
            if (!symbolsManager.IsSequenceCorrect())
            {
                audioSource.clip = incorrectPressSound;
                audioSource.Play();
            }
            if (pressSound != null && symbolsManager.IsSequenceCorrect())
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            if (symbolsManager != null && symbolsManager.IsSequenceCorrect())
            {
                door1.SetActive(false);
                door2.SetActive(false);
            }
            else
            {
                Debug.Log("Неправильная последовательность!");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ) playerInRange = true;
        currentNearbyButton = gameObject;
        playerInstance = other.gameObject;
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
