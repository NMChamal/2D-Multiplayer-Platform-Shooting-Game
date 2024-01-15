using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView photonViewV;
    public Rigidbody2D rigidbody;
    public Animator animator;
    public GameObject playerCamera;
    public SpriteRenderer spriteRenderer;
    public TMP_Text playerNameText;

    public GameObject arrowObj;
    public GameObject arrowObjType2;
    public Transform firePos;
    public bool arrowType2 = false;

    public bool isGrounded = false;
    public float moveSpeed;
    public float jumpForce;
    public ScoreManager scoreManager;
    private bool isShooting;

    public bool disableInput = false;
    

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerCamera.SetActive(true);
            playerNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            playerCamera.SetActive(false);
            playerNameText.text = photonView.Owner.NickName;
            playerNameText.color = Color.red;
        }
    }

    private void Update()
    {
        if (photonView.IsMine && !disableInput)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (scoreManager.gameOn)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
            else
            {
                var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
                transform.position += move * moveSpeed * Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Shoot();
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    photonView.RPC("FlipTrue", RpcTarget.All);
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    photonView.RPC("FlipFalse", RpcTarget.All);
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                }
            }
        }
    }

    [PunRPC]
    private void FlipTrue()
    {
        spriteRenderer.flipX = true;
    }
    [PunRPC]
    private void FlipFalse()
    {
        spriteRenderer.flipX = false;
    }

    private void Shoot()
    {
        if (!isShooting)
        {
            isShooting = true;
            animator.SetTrigger("shootTrigger");

            Invoke("ShootArrow", 0.35f);
            Invoke("NotShooting", 1f);
        }
    }
    private void Jump()
    {
        animator.SetTrigger("IsJumping");
        /*var move = new Vector3(0, Input.GetAxisRaw("Vertical"));
        transform.position += move * jumpForce * Time.deltaTime;*/
        rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void NotShooting()
    {
        isShooting = false;
    }
    private void ShootArrow()
    {
        if (!arrowType2)
        {
            if (!spriteRenderer.flipX)
            {
                GameObject obj = PhotonNetwork.Instantiate(arrowObj.name, new Vector2(firePos.transform.position.x, firePos.transform.position.y), Quaternion.identity);
                obj.GetComponent<PhotonView>().RPC("ChangeInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, false);
            }
            if (spriteRenderer.flipX)
            {
                GameObject obj = PhotonNetwork.Instantiate(arrowObj.name, new Vector2(firePos.transform.position.x, firePos.transform.position.y), Quaternion.identity);
                obj.GetComponent<PhotonView>().RPC("ChangeInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, true);
            }
        }
        else
        {
            if (!spriteRenderer.flipX)
            {
                GameObject obj = PhotonNetwork.Instantiate(arrowObjType2.name, new Vector2(firePos.transform.position.x, firePos.transform.position.y), Quaternion.identity);
                obj.GetComponent<PhotonView>().RPC("ChangeInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, false);
            }
            if (spriteRenderer.flipX)
            {
                GameObject obj = PhotonNetwork.Instantiate(arrowObjType2.name, new Vector2(firePos.transform.position.x, firePos.transform.position.y), Quaternion.identity);
                obj.GetComponent<PhotonView>().RPC("ChangeInfo", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName, true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine && collision.gameObject.name.Contains("Arrow2Upgrade"))
        {
            arrowType2 = true;
        }
    }
}
