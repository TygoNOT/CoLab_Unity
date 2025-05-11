using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using System.Collections;

public enum Team
{
    None,
    Red,
    Blue
}

public class PlayerMovement : NetworkBehaviour
{
    [Header("Attributes")]
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] private float positionRange = 5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float crouchJumpHeight = 3f;

    private bool isOnLadder = false;
    private bool isCrouching = false;
    public float jumpHeight = 7f;
    float velocityY;
    bool isGrounded;
    float cameraCap;
    Vector2 currentMouseDelta, currentMouseDeltaVelocity, currentDir, currentDirVelocity, velocity;

    [Header("Reference")]
    CharacterController controller;
    [SerializeField] LayerMask ground;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform playerCamera;
    [SerializeField] private Camera cm;

    private Animator animator;
    public static PlayerMovement LocalInstance;
    public bool IsStunned { get; private set; } = false;

    [Header("Audio")]
    [SerializeField] AudioClip walkSound;
    private AudioSource audioSource;    
    public NetworkVariable<Team> playerTeam = new NetworkVariable<Team>(
        Team.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!IsOwner || !IsSpawned) return;
        UpdateMouse();
        UpdateMove();
        HandleCrouch();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        if (isOnLadder)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");
            Vector3 climbVelocity = new Vector3(0f, verticalInput * climbSpeed, 0f);
            controller.Move(climbVelocity * Time.deltaTime);
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        if (targetDir.magnitude > 0 && isGrounded && !audioSource.isPlaying)
        {
            PlayWalkSound();
        }
        else if (targetDir.magnitude == 0 || !isGrounded)
        {
            StopWalkSound();
        }
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        velocityY += gravity * 2f * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            float jumpPower = isCrouching ? crouchJumpHeight : jumpHeight;
            velocityY = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        SetupCamera(IsOwner);


        if (IsServer)
        {
            transform.position = new Vector3(Random.Range(-positionRange, positionRange), 1, Random.Range(-positionRange, positionRange));
        }
        if (IsClient)
        {
            transform.position = new Vector3(Random.Range(-positionRange, positionRange), 1, Random.Range(-positionRange, positionRange));
        }
    }
    private void SetupCamera(bool isOwner)
    {
        if (cm == null) return;

        cm.enabled = isOwner;

        AudioListener listener = cm.GetComponent<AudioListener>();
        if (listener != null)
        {
            listener.enabled = isOwner;
        }
    }

    public void SelectTeam(Team team)
    {
        SelectTeamServerRpc((int)team);
    }

    [ServerRpc]
    private void SelectTeamServerRpc(int teamInt)
    {
        Team team = (Team)teamInt;
        if (TeamManager.Instance != null)
        {
            playerTeam.Value = team;
            TeamManager.Instance.TryAddToTeamServerRpc(team);
            Debug.Log($"Player assigned to {team}");
        }
        else
        {
            Debug.Log("Team full or TeamManager not found");
        }
    }
    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : standingHeight;
            Speed = isCrouching ? crouchSpeed : 6f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            velocityY = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }

    void PlayWalkSound()
    {
        if (walkSound != null)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.loop = true;  
            audioSource.Play();
        }
    }

    void StopWalkSound()
    {
        if (audioSource.isPlaying && audioSource.clip == walkSound)
        {
            audioSource.Stop();
        }
    }

    public void ApplyStun(float duration)
    {
        if (!IsStunned)
            StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        IsStunned = true;
        controller.enabled = false;
        yield return new WaitForSeconds(duration);

        controller.enabled = true;
        IsStunned = false;
    }

    [ClientRpc]
    public void ApplyStunClientRpc(float duration)
    {
        if (IsOwner)
        {
            ApplyStun(duration);
        }
    }

}