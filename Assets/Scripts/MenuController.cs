using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MenuController : MonoBehaviourPunCallbacks
{
    //[SerializeField] private string versionName = "0.1";
    [SerializeField] private GameObject UserNameMenu;
    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private TMP_InputField userNameInputField;
    [SerializeField] private TMP_InputField crateGameInputField;
    [SerializeField] private TMP_InputField joinGameInputField;


    [SerializeField] private GameObject startButton;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void ChangeUserName()
    {
        if(userNameInputField.text.Length >= 3)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void SetUserName() {
        UserNameMenu.SetActive(false);
        PhotonNetwork.NickName = userNameInputField.text;
    }

    public void CreateGame() {
        PhotonNetwork.CreateRoom(crateGameInputField.text, new RoomOptions() { MaxPlayers = 5 }, null);
    }

    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom(joinGameInputField.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

}
