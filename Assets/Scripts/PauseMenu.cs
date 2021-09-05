using MLAPI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool IsOn = false;

    private NetworkManager networkManager;

    private void Start()
    {
        if (NetworkManager.Singleton == null)
            NetworkManager.Singleton.SetSingleton();
        networkManager = NetworkManager.Singleton;
    }

    public void LeaveRoom()
    {
        networkManager.StopClient();
        GameManager.instance.SetSceneCanvasActive(true);
    }
}
