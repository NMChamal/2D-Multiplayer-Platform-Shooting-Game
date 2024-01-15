using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public GameObject gameCanvas;
    public GameObject sceneCamera;
    public TMP_Text pingText;
    public GameObject disconnectUi;

    public GameObject playerFeed;
    public GameObject feedGrid;

    public GameObject gameEndPanal;
    public TMP_Text gameEndText, scoreboard;

    [HideInInspector] public GameObject localPlayer;
    public TMP_Text respawnTimerText;
    public GameObject respwanMenu;
    private float timerAmount = 5f;
    private int seconds;
    private bool runSpawanTimer = false;
    private bool oneSpawn = false;
    private bool off = false;

    private void Start()
    {
        Instance = this;
        gameCanvas.SetActive(true);
    }
    private void Update()
    {
        CheckInput();
        pingText.text = "Ping:" + PhotonNetwork.GetPing();

        if(runSpawanTimer )
        {
            StartRespawn();
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            SpawnPlayer();
        }
        else
        {
            print(PhotonNetwork.CurrentRoom.PlayerCount);
            print(PhotonNetwork.CurrentRoom.Name);

        }
    }

    private void CheckInput()
    {
        if(off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUi.SetActive(false);
            off = false;
        }else if (!off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUi.SetActive(true);
            off = true;
        }
    }
    public void SpawnPlayer()
    {
        
        if (!oneSpawn)
        {
            oneSpawn = true;
            float randomValue = Random.Range(-13f, 13f);

            PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(randomValue, 1f), Quaternion.identity, 0);
            gameCanvas.SetActive(false);
            sceneCamera.SetActive(false);
        }
    }

    private void StartRespawn()
    {
        timerAmount -= Time.deltaTime;
        seconds = (int)timerAmount;
        respawnTimerText.text = "Respwan in " + seconds.ToString("");

        if(timerAmount <= 0)
        {
            localPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered);
            localPlayer.GetComponent<PlayerHealth>().EnableInput();
            
            RespwanLocations();
            respwanMenu.SetActive(false);
            runSpawanTimer = false;
        }
    }
    public void RespwanLocations()
    {
        float value = Random.Range(-13f, 13f);
        localPlayer.transform.localPosition = new Vector2(value, 1f);
    }
    public void EnableRespawn() {
        timerAmount = 5f;
        runSpawanTimer = true;
        respwanMenu.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        GameObject obj = Instantiate(playerFeed, new Vector2(0,0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<TMP_Text>().text = newPlayer.NickName + " Joined the game.";
        obj.GetComponent<TMP_Text>().color = Color.green;
    }


    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        //print("Left player");
        GameObject obj = Instantiate(playerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<TMP_Text>().text = newPlayer.NickName + " left the game.";
        obj.GetComponent<TMP_Text>().color = Color.red;
    }
}
