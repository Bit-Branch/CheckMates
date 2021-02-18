using Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ChessGame{
public class PhotonLobby : MonoBehaviourPunCallbacks
{
    private static PhotonLobby instance;
    private RoomInfo[] rooms;
    public Text connectionLog;
    private bool joinedTheRoom;

    
    private void Awake() 
    {
        instance = this;
    }

    public static PhotonLobby GetInstance()
    {
        return instance;
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

   public override void OnJoinRandomFailed(short returnCode, string message)
   {
       CreateRoom(); 
   }

   public override void OnCreateRoomFailed(short returnCode, string message)
   {
       CreateRoom();
   }

   public override void OnJoinedRoom()
   {
       UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
   }

   public void TryToJoinRandomRoom()
   {
        PhotonNetwork.JoinRandomRoom();
   }
    
    private void CreateRoom()
    {
       int randomRoomName = Random.Range(0,10000);
       RoomOptions roomOptions = new RoomOptions(){ IsVisible = true, IsOpen = true, MaxPlayers = 2};
       PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOptions);   
    }

    private void AddMessageToLog(string message)
    {
        connectionLog.text += message + "\n";
    }

    public bool isJoinedTheRoom()
    {
        return joinedTheRoom;
    }

    public override void OnConnectedToMaster() 
    {
        //TODO
    }
 
}
}
