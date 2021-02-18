
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ChessGame;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ChessGame
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        private static MainMenu instance;
        public PhotonLobby photonLobby;
        public GameObject mainScreen;
        public GameObject splashScreen;
        public Text networkStatusText;
        public Image networkStatusIcon;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;
        public InputField RoomNameInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;
        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

        public static MainMenu GetInstance()
        {
            return instance;
        }


        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            PlayerNameInput.text = "Player " + Random.Range(1000, 10000);

            if(Application.internetReachability == NetworkReachability.NotReachable)
            {
                networkStatusText.text = "Offline";
            }else
            {
                networkStatusIcon.color = new Color32(0,255,0,100);
                networkStatusText.text = "Online";
            }

            instance = this; 
        }
        
        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            RoomOptions options = new RoomOptions {MaxPlayers = 2, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnPlayButtonClicked()
        {
            mainScreen.SetActive(false);
            splashScreen.SetActive(true);
            photonLobby.TryToJoinRandomRoom();
        } 

            public override void OnConnectedToMaster()
            {
                this.SetActivePanel(SelectionPanel.name);
            }

            public override void OnRoomListUpdate(List<RoomInfo> roomList)
            {
                ClearRoomListView();
                UpdateCachedRoomList(roomList);
                UpdateRoomListView();
            }

            public override void OnJoinedLobby()
            {
                cachedRoomList.Clear();
                ClearRoomListView();
            }

            public override void OnLeftLobby()
            {
                cachedRoomList.Clear();
                ClearRoomListView();
            }

            public override void OnCreateRoomFailed(short returnCode, string message)
            {
                SetActivePanel(SelectionPanel.name);
            }

            public override void OnJoinRoomFailed(short returnCode, string message)
            {
                SetActivePanel(SelectionPanel.name);
            }

            public override void OnJoinRandomFailed(short returnCode, string message)
            {
                string roomName = "Room " + Random.Range(1000, 10000);

                RoomOptions options = new RoomOptions {MaxPlayers = 2};

                PhotonNetwork.CreateRoom(roomName, options, null);
            }

            public override void OnJoinedRoom()
            {
               cachedRoomList.Clear();
               if(PhotonNetwork.LocalPlayer.IsMasterClient){
                   Hashtable hash = new Hashtable();
                   hash.Add("Side", Side.WHITE);
                   PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
               }
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

            }

            public override void OnLeftRoom()
            {
                SetActivePanel(SelectionPanel.name);

                foreach (GameObject entry in playerListEntries.Values)
                {
                    Destroy(entry.gameObject);
                }

                playerListEntries.Clear();
                playerListEntries = null;
            }

            public override void OnPlayerEnteredRoom(Player newPlayer)
            {
                //TODO
            }

            public override void OnPlayerLeftRoom(Player otherPlayer)
            {
                Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
                playerListEntries.Remove(otherPlayer.ActorNumber);
           
            }

            public override void OnMasterClientSwitched(Player newMasterClient)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
                {
                    //TODO
                }
            }

            private void ClearRoomListView()
            {
                foreach (GameObject entry in roomListEntries.Values)
                {
                    Destroy(entry.gameObject);
                }

                roomListEntries.Clear();
            }
            private void SetActivePanel(string activePanel)
            {
                LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
                SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
                CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
                JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
                RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));  
            }

            private void UpdateCachedRoomList(List<RoomInfo> roomList)
            {
                foreach (RoomInfo info in roomList)
                {
                    // Remove room from cached room list if it got closed, became invisible or was marked as removed
                    if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                    {
                        if (cachedRoomList.ContainsKey(info.Name))
                        {
                            cachedRoomList.Remove(info.Name);
                        }

                        continue;
                    }

                    // Update cached room info
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList[info.Name] = info;
                    }
                    // Add new room info to cache
                    else
                    {
                        cachedRoomList.Add(info.Name, info);
                    }
                }
            }

            private void UpdateRoomListView()
            {
                foreach (RoomInfo info in cachedRoomList.Values)
                {
                    GameObject entry = Instantiate(RoomListEntryPrefab);
                    entry.transform.SetParent(RoomListContent.transform);
                    entry.transform.localScale = Vector3.one;
                    entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                    roomListEntries.Add(info.Name, entry);
                }
            }
        }

}

