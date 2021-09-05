using UnityEngine;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using System;
using MLAPI.Transports.UNET;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    public GameObject sceneCamera;
    public GameObject sceneCanvas;

    public Text hostName;
    public Text joinName;
    public string baseUrl = "local";

    public UNetTransport uNetTransport;

    public Transform[] spawnPositions; 

    NetworkVariable<int> nextSpawnPosition = new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0);

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one game manager in this scene");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void StartHost() {

        Transform spawnPostion = GetSpawnPosition();

        byte[] encoded = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(hostName.text));
        uint value = BitConverter.ToUInt32(encoded, 0) % 65531;

        uNetTransport.ConnectPort = (int)value;
        Debug.LogWarning(value);
        uNetTransport.ServerListenPort = (int)value;


        NetworkManager.Singleton.StartHost(spawnPostion.position, spawnPostion.rotation, true);
        SetSceneCanvasActive(false);
    }

    public void JoinGame() {

        /*byte[] encoded = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(joinName.text));
        uint value = BitConverter.ToUInt32(encoded, 0) % 65531;

        uNetTransport.ConnectPort = (int)value;
        Debug.LogWarning(value);
        uNetTransport.ServerListenPort = (int)value;*/

        uNetTransport.ConnectAddress = baseUrl + joinName.text;

        NetworkManager.Singleton.StartClient();
        SetSceneCanvasActive(false);
    }

    public Transform GetSpawnPosition() {

        Transform spawnPostion = spawnPositions[nextSpawnPosition.Value];
        nextSpawnPosition.Value = Random.Range(0, spawnPositions.Length);

        return spawnPostion;
    }

    public void SetSceneCanvasActive(bool isActive) {
        if (sceneCanvas == null)
            return;

        sceneCanvas.SetActive(isActive);

    }

    public void SetSceneCameraActive(bool isActive)
    {
        if(sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }

    #region Player tracking

    private static string PLAYER_ID_PREFIX = "Player ";
    static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer( string _playerID )
    {
        return players[_playerID];
    }

    /*private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string _playerID in players.Keys)
        {
            GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }*/
    #endregion
}
