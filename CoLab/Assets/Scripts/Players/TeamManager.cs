using Unity.Netcode;

public class TeamManager : NetworkBehaviour
{
    public static TeamManager Instance;

    public NetworkVariable<int> redCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> blueCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void Awake()
    {
        if (Instance != null)
            Instance = this;
    }

    [ServerRpc]
    public void TryAddToTeamServerRpc(Team team)
    {
        if (team == Team.Red && redCount.Value < 2)
        {
            redCount.Value++;
        }
        else if (team == Team.Blue && blueCount.Value < 2)
        {
            blueCount.Value++;
        }
    }
}