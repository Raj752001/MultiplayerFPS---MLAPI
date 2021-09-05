using MLAPI;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;

    public string remoteLayername = "RemotePlayer";

    public string dontDrawLayerName = "DontDraw";
    public GameObject playerGraphics;

    public GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    private void Start()
    {
        if(!IsLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            // Disable player graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            // Configuer PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab.");
            ui.SetController(GetComponent<PlayerController>());

            GetComponent<Player>().SetupPlayer();
        }
    }

    public override void NetworkStart()
    {
        base.NetworkStart();

        string _netID = GetComponent<NetworkObject>().NetworkObjectId.ToString();
        Debug.LogWarning(_netID);
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayername);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if(IsLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);

       GameManager.UnRegisterPlayer(transform.name);
    }
}
