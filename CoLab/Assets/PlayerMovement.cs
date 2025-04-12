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
    public int currentPlayersTeamRed, currentPlayersTeamBlue = 0;
    private Animator animator;

    public Team playerTeam = Team.None;

    public static PlayerMovement LocalInstance;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
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

    public void SelectTeam(Team team)
    {
        SelectTeamServerRpc((int)team);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        if (IsServer)
        {
            UpdatePositionServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]

    private void UpdatePositionServerRpc()
    {
        transform.position = new Vector3(Random.Range(positionRange, -positionRange), 1, Random.Range(positionRange, -positionRange));
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }

    [ServerRpc(RequireOwnership = false)]

    private void SelectTeamServerRpc(int teamInt)
    {
        Team team = (Team)teamInt;

        if (team == Team.Red)
        {
            if (currentPlayersTeamRed < 2)
            {
                playerTeam = team;
                currentPlayersTeamRed++;
            }
            else
            {
                Debug.Log("Team Red is full");
            }
        }
        else if (team == Team.Blue)
        {
            if (currentPlayersTeamBlue < 2)
            {
                playerTeam = team;
                currentPlayersTeamBlue++;
            }
            else
            {
                Debug.Log("Team Blue is full");
            }
        }
    }
}
