using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Player_Control : MonoBehaviourPunCallbacks,IPunObservable
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
    private int jumpCount = 2;
    [SerializeField]
    private int AttackCount = 3;

    private Vector3 movement;
    private Vector3 dodgeVec;

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

    public bool isDodge;

    private bool dDown;


    float horizontalMove;
    float verticalMove;


    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public GameObject CM;
    private Camera characterCamera;

    private void Awake()
    {
        if (PV.IsMine)
        {
            CM = GameObject.Find("Main Camera");
            characterCamera = CM.GetComponent<Camera>();
            var CM_cm = CM.GetComponent<Camera_Move>();
            CM_cm.player = this.gameObject;
        }

        rgbd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        walkSpeed = moveSpeed;
    }
    private void FixedUpdate()
    {
        Zoom();
        MouseTurn();
        //Turn();
        Jump();
        Run();
        Walk();
        Attack();
        DoubleJump();
        Respawn();
        Dodge();
    }
    [PunRPC]
    void Dodge()
    {
        if (!isDodge && dDown)
        {
            dodgeVec = movement;
            moveSpeed = 7;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.6f);
        }

    }
    [PunRPC]
    void DodgeOut()
    {
        moveSpeed = 5f;
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
        rgbd.AddForce(Vector3.up * jumpPower * 1.2f, ForceMode.Impulse);
        jumpCount -= 1;
        isDoubleJump = false;
        isGrounded = false;
    }
    [PunRPC]
    void Walk()
    {

        if (isDodge)
        {
            movement.Set(dodgeVec.x, dodgeVec.y, dodgeVec.z);
        }
        else
        {
            movement.Set(horizontalMove, 0, verticalMove);
        }
        
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
        jumpCount -= 1;
        isJumping = false;
        isGrounded = false;
    }
/*    [PunRPC]
    void Turn()
    {
        if (horizontalMove == 0 && verticalMove == 0)
            return;
        Quaternion newRotation = Quaternion.LookRotation(movement);

        rgbd.rotation = Quaternion.Slerp(rgbd.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }*/
    void Zoom()
    {
        var scroll = Input.mouseScrollDelta;
        characterCamera.fieldOfView = Mathf.Clamp(characterCamera.fieldOfView - scroll.y*2f, 30f, 70f);
    }
    void MouseTurn()
    {
        Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if(Physics.Raycast(ray,out hitResult))
        {
            Vector3 mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            animator.transform.forward = mouseDir;
        }

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
            dDown = Input.GetKey(KeyCode.LeftShift);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine && other.gameObject.CompareTag("Attack_spot") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine)
        {
            PV.RPC("head_jump", RpcTarget.All);
            other.gameObject.transform.parent.GetComponent<Player_Control>().Hit();
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
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

        if (isGrounded == true)
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


    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
