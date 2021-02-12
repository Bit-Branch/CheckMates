using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ChessGame{
    public class ChessGameManager : MonoBehaviourPunCallbacks
    {
        public GameObject MainCamera;
        public static ChessGameManager Instance = null;
        public Text CurrentGameLog;

        private PhotonView photonView;

            public void Awake()
            {
                Instance = this;
                photonView = GetComponent<PhotonView>();
            }

            public void Start()
            {
                //TODO
            }
            
            [PunRPC]
            private void AddMessageToLog(string text)
            {
                CurrentGameLog.text += text + "\n";
            }

            public override void OnPlayerEnteredRoom(Player player)
            {
                if(photonView.IsMine) 
                {
                    photonView.RPC("AddMessageToLog", RpcTarget.All, "Player " + player + " entered");
                }
            }

            public override void OnDisconnected(DisconnectCause cause)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu");
            }

            public override void OnLeftRoom()
            {
                PhotonNetwork.Disconnect();
            }

            public override void OnMasterClientSwitched(Player newMasterClient)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
                {
                    
                }
            }

            public override void OnPlayerLeftRoom(Player otherPlayer)
            {
               //TODO
            }

            public void OnSettingsButtonClicked()
            {
                //TODO
            }

            public void OnExitButtonClicked()
            {
                photonView.RPC("AddMessageToLog", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " left game");
                PhotonNetwork.Disconnect();   
            }
    }
}
