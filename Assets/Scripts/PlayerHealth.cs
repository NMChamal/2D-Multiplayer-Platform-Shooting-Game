using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public Image fillImage;
    public float healthAMount;

    public PlayerController plMove;
    public Rigidbody2D rigidbody2D;
    public BoxCollider2D boxCollider2D;
    public SpriteRenderer spriteRenderer;
    public GameObject playerCanvas;
    public ScoreManager scoreManager;


    private void Awake()
    {
        if(photonView.IsMine)
        {
            GameManager.Instance.localPlayer = this.gameObject;
        }
    }
    // RPC to Reduse Health
    [PunRPC] public void ReduceHealth(float amount, string arrowOwner) {
        ModifyHealth(amount, arrowOwner);
    }

    // Health amount update
    private void ModifyHealth(float amount, string arrowOwner)
    {
        if (photonView.IsMine)
        {
            healthAMount -= amount;
            //fillImage.fillAmount -= (amount/30);
        }
        else
        {
            healthAMount -= amount;
            //fillImage.fillAmount -= (amount / 30);
        }
        CheckHealth(arrowOwner);
    }

    // Check player is Dead or not and update health bar
    private void CheckHealth(string arrowOwner)
    {
        fillImage.fillAmount = healthAMount/100f;
        if(photonView.IsMine && healthAMount <= 0) {
            GameManager.Instance.EnableRespawn();
            plMove.disableInput = true;
            this.GetComponent<PhotonView>().RPC("DeadPlayer", RpcTarget.AllBuffered);
            scoreManager.GetDetailsToUpdateScore(arrowOwner, 1);
        }

    }

    public void EnableInput() {
        plMove.disableInput = false;
        plMove.arrowType2 = false;
    }

    [PunRPC]
    private void DeadPlayer()
    {
        rigidbody2D.gravityScale = 0;
        boxCollider2D.enabled = false;
        spriteRenderer.enabled = false;
        playerCanvas.SetActive(false);
    }


    [PunRPC]
    private void Respawn()
    {
        rigidbody2D.gravityScale = 1.0f;
        boxCollider2D.enabled = true;
        spriteRenderer.enabled = true;
        playerCanvas.SetActive(true);

        fillImage.fillAmount = 1f;
        healthAMount = 100;
    }
}
