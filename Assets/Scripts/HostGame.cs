using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        /*  if(networkManager.matchMaker == null)
          {
              networkManager.StartMatchMaker();
          }*/
        
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void CreateRoom()
    {
        SceneManager.LoadSceneAsync("MainLevel");
        networkManager.StartHost();
        Debug.Log(roomName);
        /*if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room" + roomName + " with room for" + roomSize + " players");
            // Create room
           *//* networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0,networkManager.OnMatchCreate);*//*
        }*/
    }
}
