using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Arrow : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool moveDir = false; // false: right, true: left
    public float moveSpeed;
    public float destroyTime;
    public float arrowDamage;
    public SpriteRenderer arrowRenderer;

    private string arrowOwner;

    private void Awake()
    {
        StartCoroutine("DestroyByTime");    
    }

    IEnumerator DestroyByTime() { 
        yield return new WaitForSeconds(destroyTime);
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
    }

    [PunRPC]
    public void ChangeInfo(string playerName, bool left) {
        moveDir = left;
        arrowRenderer.flipX = left;
        //print("THis arrow from  " + playerName);
        arrowOwner = playerName;
    }
    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (!moveDir)
            transform.Translate(Vector2.right* moveSpeed* Time.deltaTime);
        else
            transform.Translate(Vector2.left* moveSpeed* Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;
        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if(target != null && (!target.IsMine || target.IsSceneView)){
            if(target.tag == "Player")
            {
                target.RPC("ReduceHealth", RpcTarget.All, arrowDamage, arrowOwner);
            }
            this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}
