using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerCCControl : MonoBehaviourPunCallbacks,IPunObservable
{
    public Animator animator;
    public Rigidbody rgbd;
    public PhotonView PV;
    public Text NickNameText;
    public Image HealthImage;
    public float hp_;
    public GameObject playerEquipPoint;
    public GameObject Head;
    public GameObject GunHead;

    public Color Head_color;

    Vector3 curPos;
    Quaternion curRot;

    public AudioSource jump;
    public AudioSource Dbjump;


    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;
    private float walkSpeed;
    [SerializeField]
    private float jumpPower = 7.0f;
    
    [SerializeField]
    private int jumpCount=2;
    [SerializeField]
    private int AttackCount = 3;

    private Vector3 movement;

    private bool isWall = false;
    private bool isRespawn = false;
    private bool isGrounded = true;
    private bool isJumping;
    private bool isRunning;
    private bool isAttacking;
    private bool isDoubleJump;
    private bool isHi;
    private bool isPicking;
    private bool isItemEnter;
    private bool clickC;

    private bool isSwordAttacking1;
    private bool isSwordAttacking2;
    private bool isSwordAttacking3;

    private bool isDodge;

    private bool dDown;


    float horizontalMove;
    float verticalMove;


    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);


    private void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

       
        if (PV.IsMine)
        {
            var CM = GameObject.Find("Main Camera").GetComponent<Camera_Move>();
            CM.player = this.gameObject;
        }

        rgbd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        walkSpeed = moveSpeed;
    }
    private void FixedUpdate()
    {
        Turn();
        Jump();
        Run();
        Walk();
        Attack();
        DoubleJump();
        Respawn();
    }
    [PunRPC]
    void Dodge()
    {
        if (!isDodge && dDown)
        {
            moveSpeed= 10;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }

    }
    [PunRPC]
    void DodgeOut()
    {
        moveSpeed = 30f;
        isDodge = false;

    }



    [PunRPC]
    void Run()
    {
        
        if (!isRunning)
            return;
        /*movement.Set(horizontalMove, 0, verticalMove);
        movement = movement.normalized * 1.5f*moveSpeed * Time.deltaTime;

        rgbd.MovePosition(transform.position + movement);*/
        moveSpeed = 20f;

    }
   [PunRPC]
    void Attack()
    {
        if (!isAttacking)
            return;
        isAttacking = false;
    }

    [PunRPC]
    void DoubleJump()
    {
        if (!isDoubleJump)
            return;
        Dbjump.Play();
        rgbd.AddForce(Vector3.up * jumpPower*1.2f, ForceMode.Impulse);
        jumpCount-=1;
        isDoubleJump = false;
        isGrounded = false;
    }
    [PunRPC]
    void Walk()
    {
        
        
        movement.Set(horizontalMove,0,verticalMove);
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        rgbd.transform.position += movement;
        //rgbd.MovePosition(transform.position + movement);
    }
   [PunRPC]
    void Jump()
    {
        if (!isJumping)
            return;
        jump.Play();
        rgbd.velocity = Vector3.zero;
        rgbd.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        jumpCount-=1;
        isJumping = false;
        isGrounded = false;
    }
   [PunRPC] void Turn()
    {
        if (horizontalMove == 0 && verticalMove == 0)
            return;
        Quaternion newRotation = Quaternion.LookRotation(movement);

        rgbd.rotation = Quaternion.Slerp(rgbd.rotation, newRotation, rotateSpeed * Time.deltaTime); 
    }
    void Respawn()
    {
        if (!isRespawn)
            return;
        transform.position = new Vector3(0f, 15f, 0f);
        isRespawn = false;
    }
    public void Hit()
    {
        HealthImage.fillAmount -= 0.1f;
        if (HealthImage.fillAmount <= 0)
        {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
    void Update()
    {
        if (PV.IsMine)
        {
            rgbd.velocity = new Vector3(0f, rgbd.velocity.y, 0f);

            horizontalMove = Input.GetAxisRaw("Horizontal");
            verticalMove = Input.GetAxisRaw("Vertical");


            if (transform.position.y < -100)
            {
                isRespawn = true;
            }
/*            if (isGrounded || isWall)
            {
                if (jumpCount == 2)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        isJumping = true;
                    }
                }

            }
            else
            {
                if (jumpCount == 1)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        isDoubleJump = true;
                    }
                }
            }*/


            

            AnimationUpdate();
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 10);
        }
    }

    IEnumerator DelaySec()
    {
        yield return new WaitForSeconds(2f);
        AttackCount = 3;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            isDoubleJump = false;
            jumpCount = 2;
        }
        if (col.gameObject.CompareTag("wall"))
        {
            isWall = true;
            //isGrounded = true;
            jumpCount = 2;
        }

    }
    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Weapon") && clickC)
        {
            col.transform.SetParent(playerEquipPoint.transform);
            col.transform.localPosition = Vector3.zero;
            col.transform.rotation = new Quaternion(0, 0, 0, 0);
            col.transform.localScale = new Vector3(0.07f, 0.02f, 0.03f);

            Pickup(col.transform.gameObject);
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("wall"))
        {
            isWall = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine&&other.gameObject.CompareTag("Attack_spot") &&other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            PV.RPC("head_jump", RpcTarget.All);
            //other.gameObject.transform.parent.GetComponent<Player_Control>().Hit(other.gameObject.transform.parent.GetComponent<Player_Control>().atk);
        }

    }
    [PunRPC]
    void head_jump()
    {
        rgbd.velocity = Vector3.zero;
        rgbd.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }



    public void Pickup(GameObject item)
    {
        SetEquip(item, true);

        isPicking = true;
    }
    void Drop()
    {
        GameObject item = playerEquipPoint.GetComponentInChildren<Rigidbody>().gameObject;
        SetEquip(item, false);

        playerEquipPoint.transform.DetachChildren();
        isPicking = false;
    }
    void SetEquip(GameObject item, bool isEquip)
    {
        Collider[] itemColliders = item.GetComponents<Collider>();
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();

        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = !isEquip;
        }
        itemRigidbody.isKinematic = isEquip;

    }





    void AnimationUpdate()
    {
        if (horizontalMove == 0 && verticalMove == 0)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        if (isGrounded==true)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }

        if (isAttacking == true)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
        
        if (isDoubleJump == true)
        {
            animator.SetBool("DoubleJump", true);
        }
        else
        {
            animator.SetBool("DoubleJump", false);
        }

        if (isRunning == true)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(HealthImage.fillAmount);
            
        }
        else
        { 
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
