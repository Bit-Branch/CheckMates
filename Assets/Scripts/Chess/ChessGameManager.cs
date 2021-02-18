using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.Utilities;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ChessGame{
    public class ChessGameManager : MonoBehaviourPunCallbacks
    {
        public GameObject MainCamera;
        public static ChessGameManager Instance = null;
        public Text CurrentGameLog;
        private Board board;


        private PhotonView photonView;

            public void Awake()
            {

                Debug.Log("Chess gm awake");

                Instance = this;
                
                photonView = GetComponent<PhotonView>();

                board = GameObject.FindWithTag("Board").GetComponent<Board>();

                PhotonNetwork.AutomaticallySyncScene = true;

                //PunTurnManager.TurnExtensions.SetTurn(PhotonNetwork.CurrentRoom,1,true);

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
                board.InitializeBoard();
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
                    //TODO
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
