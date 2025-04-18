using UnityEngine;
using Unity.Netcode;

public enum Team
{
    None,
    Red,
    Blue
}

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float positionRange = 5f;

    private Animator animator;
    public static PlayerMovement LocalInstance;

    public NetworkVariable<Team> playerTeam = new NetworkVariable<Team>(
        Team.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetFloat("run", movementDirection.magnitude);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        if (IsServer)
        {
            transform.position = new Vector3(Random.Range(-positionRange, positionRange), 1, Random.Range(-positionRange, positionRange));
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
}