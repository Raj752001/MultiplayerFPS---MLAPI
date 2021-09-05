using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MLAPI;
using MLAPI.SceneManagement;

public class JoinGame : MonoBehaviour
{
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    public void JoinRoom()
    {
        /* networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();*/
        status.text = "JOINING...";
        networkManager.StartClient();
        NetworkSceneManager.SwitchScene("MainLevel");
    }

    /*public void RefreshRoomList()
    {
        ClearRoomList();
        *//*networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);*//*
        status.text = "Loading...";
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> _matchInfo)
    {
        status.text = "";

        if(_matchInfo == null)
        {
            status.text = "Could't get room list.";
            return;
        }

        foreach(MatchInfoSnapshot _match in _matchInfo)
        {
            Debug.Log(_match.name);
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab, roomListParent);
            //_roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if(_roomListItem != null)
            {
                _roomListItem.Setup(_match, JoinRoom);
            }
           
            // as well as setting up a callback function that will join the game
            roomList.Add(_roomListItemGO);
        }

        if(roomList.Count == 0)
        {
            status.text = "No rooms found.";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }*/

}
