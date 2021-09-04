using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;

public class Launcher1 : MonoBehaviourPunCallbacks
{
    public static Launcher1 Instance;

    [SerializeField] InputField roomNameInputField;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] InputField usernameInput;

    private void Awake()
    {
        Instance = this;
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 144;
        PhotonNetwork.SerializationRate = 144;
    }

    private void Start()
    {
        Debug.Log("Connecting to Master");
        //  포톤 서버에 접속하게 해주는 함수.
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.Instance.OpenMenu("Loading");

        // 입력한 username으로 PlayerPrefs를 통해 로컬 저장함
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");

            // 입력한 username을 포톤 클라이언트의 유저 이름으로 저장
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            usernameInput.text = "Player " + Random.Range(0, 10000).ToString("0000");
            // 입력한 username을 포톤 클라이언트의 유저 이름으로 저장하는 함수 호출
            OnUsernameInputValueChanged();
        }
    }
     
    private void Update()
    {
        Player[] players = PhotonNetwork.PlayerList;
        // 현재 방에 설정한 인원 수 만큼 플레이어가 있으면, 시작가능 버튼이 활성화됨
        if (players.Count() >= 1)
        {
            startGameButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            startGameButton.GetComponent<Button>().interactable = false;
        }
    }

    // 메뉴화면으로 들어가게 하는 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Joined Lobby");
    }

    GameObject Custom;


    // 입력한 방제목으로 방을 만드는 함수
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");

    }

    // 방에 들어가있을 때, 방제목과 들어온 플레이어 리스트르 띄어줌
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;


        // 방에 들어온 플레이어 리스트
        Player[] players = PhotonNetwork.PlayerList;


        // 방에 누군가 들어오면, 모든 List를 제거
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // 들어온 플레이어의 정보를 가진 PlayerListItem 생성
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }

        // 방장 클라이언트에게만 시작 버튼이 활성화 됨.
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Room");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinFoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }
    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);                  //전부다 없앤 다음 새로 생성
        }
        /*
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
        */
        for(int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(cachedRoomList[entry.Key]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
        PlayerPrefs.SetString("username", usernameInput.text);
    }
}
