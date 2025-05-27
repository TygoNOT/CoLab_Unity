using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlatformRemove : NetworkBehaviour
{
    private GameObject platformTop;
    private GameObject platformDown;
    public bool playerInRange = false;
    private GameObject currentNearbyButton;
    private SymbolsManager symbolsManager;
    public Material[] allMaterials;
    public AudioClip pressSound;
    public AudioClip swagSound;
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

    [SerializeField]
    private GameObject topCube;

    [SerializeField]
    public PlatformVerticalMover platformMover;

    [SerializeField]
    private PlatformVerticalMover verticalMover;

    [SerializeField]
    private PlatformVerticalMoverReverse verticalMoverReverse;

    [SerializeField]
    private PlatformBackAndForth backAndForth;

    [SerializeField]
    private PlatformBackAndForthReverse backAndForthReverse;

    [SerializeField]
    private PlatformForwardBackwardMover platformForwardBackwardMover;

    private NetworkObject platformTopNetObj;
    private NetworkObject platformDownNetObj;
    private NetworkObject door1NetObj;
    private NetworkObject door2NetObj;

    void Start()
    {
        audioSource = buttonCube.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = buttonCube.gameObject.AddComponent<AudioSource>();
        }

        platformTop = GameObject.Find("ButtonPlane1");
        platformDown = GameObject.Find("ButtonPlane2");

        if (platformTop != null) platformTopNetObj = platformTop.GetComponent<NetworkObject>();
        if (platformDown != null) platformDownNetObj = platformDown.GetComponent<NetworkObject>();

        if (door1 != null) door1NetObj = door1.GetComponent<NetworkObject>();
        if (door2 != null) door2NetObj = door2.GetComponent<NetworkObject>();

        if (GameObject.FindObjectOfType<SymbolsManager>() != null)
        {
            symbolsManager = GameObject.FindObjectOfType<SymbolsManager>();
            allMaterials = symbolsManager.GetMaterials();
        }
        if(topCube != null) topCube.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || currentNearbyButton == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaySoundLocal(pressSound);

            string tag = currentNearbyButton.tag;

            switch (tag)
            {
                case "ButtonTop":
                    RequestDestroyPlatformTopServerRpc();
                    SetTrueButtonServerRpc();
                    break;

                case "BottomButton":
                    RequestDestroyPlatformDownServerRpc();
                    break;

                case "TrueButton":
                    RequestDestroyDoorServerRpc();
                    break;

                case "Button":
                    PlaySoundLocal(incorrectPressSound, 0.2f);
                    playerInstance.GetComponent<NetworkObject>().GetComponent<PlayerMovement>().ApplyStunServerRpc(1f);
                    break;

                case "DoorButton":
                    if (invisibleWall != null) invisibleWall.SetActive(false);
                    break;

                case "DoorButtonRand":
                    RequestDestroyDoorServerRpc();
                    ActivateRandomButtonServerRpc();
                    break;

                case "SymbolRotation":
                    SymbolCube cube = currentNearbyButton.GetComponentInParent<SymbolCube>();
                    if (cube == null) cube = currentNearbyButton.GetComponentInChildren<SymbolCube>();
                    if (cube != null) cube.NextSymbol();
                    break;

                case "SequenceCheckButton":
                    if (symbolsManager != null)
                    {
                        if (!symbolsManager.IsSequenceCorrect())
                        {
                            PlaySoundLocal(incorrectPressSound);
                        }
                        else
                        {
                            PlaySoundLocal(pressSound);
                            RequestDestroyDoorServerRpc();
                        }
                    }
                    break;

                case "StopMoveButton":
                    ActivateCubeServerRpc(2f);
                    RequestPausePlatformServerRpc(2f);
                    break;

                case "StopMoveButton3":
                    RequestPauseForwardPlatformsServerRpc(2f);
                    PlaySoundLocal(pressSound);
                    break;

                case "StopMoveButton2":
                    RequestPauseAllPlatformsServerRpc(2f);
                    break;

                case "NotBest":
                    PlaySoundLocal(incorrectPressSound, 10f);
                    break;

                case "BestGame":
                    PlaySoundLocal(swagSound);
                    RequestDestroyDoorServerRpc();
                    break;
            }
        }
    }

    private void PlaySoundLocal(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyPlatformTopServerRpc()
    {
        if (platformTopNetObj != null && platformTopNetObj.IsSpawned)
        {
            platformTopNetObj.Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyPlatformDownServerRpc()
    {
        if (platformDownNetObj != null && platformDownNetObj.IsSpawned)
        {
            platformDownNetObj.Despawn();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPausePlatformServerRpc(float duration)
    {
        PausePlatformClientRpc(duration);
    }

    [ClientRpc]
    private void PausePlatformClientRpc(float duration)
    {
        if (verticalMover != null) verticalMover.PauseMovement(duration);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPauseForwardPlatformsServerRpc(float duration)
    {
        PauseForwardPlatformsClientRpc(duration);
    }
    [ClientRpc]
    private void PauseForwardPlatformsClientRpc(float duration)
    {
        if (platformForwardBackwardMover != null) platformForwardBackwardMover.PauseMovement(duration);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestPauseAllPlatformsServerRpc(float duration)
    {
        PauseAllPlatformsClientRpc(duration);
    }

    [ClientRpc]
    private void PauseAllPlatformsClientRpc(float duration)
    {
        if (verticalMover != null) verticalMover.PauseMovement(duration);
        if (verticalMoverReverse != null) verticalMoverReverse.PauseMovement(duration);
        if (backAndForth != null) backAndForth.PauseMovement(duration);
        if (backAndForthReverse != null) backAndForthReverse.PauseMovement(duration);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyDoorServerRpc()
    {
        if (door1NetObj != null && door1NetObj.IsSpawned)
            door1NetObj.Despawn();

        if (door2NetObj != null && door2NetObj.IsSpawned)
            door2NetObj.Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTrueButtonServerRpc()
    {
        buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
        System.Random random = new System.Random();
        int trueButtonNumber = random.Next(0, buttonList.knopk.Length);

        var trueButton = buttonList.knopk[trueButtonNumber].transform.Find("Button");
        Debug.Log(buttonList.knopk[trueButtonNumber].name);
        if (trueButton != null)
        {
            trueButton.tag = "TrueButton";
        }

        SetTrueButtonClientRpc(trueButtonNumber);
    }

    [ClientRpc]
    private void SetTrueButtonClientRpc(int trueButtonNumber)
    {
        buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
        for (int i = 0; i < buttonList.knopk.Length; i++)
        {
            var buttonTransform = buttonList.knopk[i].transform.Find("Button");
            if (buttonTransform != null)
            {
                buttonTransform.tag = (i == trueButtonNumber) ? "TrueButton" : "Button";
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestShowFinishServerRpc()
    {
        ShowFinishClientRpc();
    }

    [ClientRpc]
    private void ShowFinishClientRpc()
    {
        if (finish != null)
        {
            finish.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateRandomButtonServerRpc()
    {
        buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
        System.Random random = new System.Random();
        for (int i = 0; i < buttonList.knopk.Length; i++)
        {
            buttonList.knopk[i].SetActive(false);
        }
        int randomButtonNumber = random.Next(0, buttonList.knopk.Length);
        buttonList.knopk[randomButtonNumber].SetActive(true);
        ActivateRandomButtonClientRpc(randomButtonNumber);
    }

    [ClientRpc]
    private void ActivateRandomButtonClientRpc(int randomButtonNumber)
    {
        buttonList = GameObject.Find("Buttons").GetComponent<ButtonList>();
        if (randomButtonNumber >= 0 && randomButtonNumber < buttonList.knopk.Length)
        {
            buttonList.knopk[randomButtonNumber].SetActive(true);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInstance = other.gameObject;
            currentNearbyButton = gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (currentNearbyButton == gameObject)
            {
                currentNearbyButton = null;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateCubeServerRpc(float duration)
    {
        ActivateCubeClientRpc(duration);
    }

    [ClientRpc]
    public void ActivateCubeClientRpc(float duration)
    {
        StartCoroutine(CubeCoroutine(duration));
    }

    private IEnumerator CubeCoroutine(float duration)
    {
        topCube.SetActive(true);
        yield return new WaitForSeconds(duration);
        topCube.SetActive(false);
    }
}