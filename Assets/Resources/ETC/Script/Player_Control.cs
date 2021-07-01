using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Xml;
using Photon.Pun.Demo.Asteroids;

public class Player_Control : MonoBehaviourPunCallbacks,IPunObservable
{
    public static Player_Control Instance;

    [SerializeField]
    private GameObject arrowPrefab;

    private Queue<Arrow> arrowQ = new Queue<Arrow>();

    public Animator animator;
    public Rigidbody rgbd;
    public PhotonView PV;
    public Text NickNameText;
    public Image HealthImage;
    public Player_HpBar Hp_Bar;

    public GameObject playerEquipPoint;
    public GameObject Head;
    public GameObject GunHead;

    public Color Head_color;

    public Style.WeaponStyle curStyle;
    Vector3 curPos;
    Quaternion curRot;
    float curHpValue;

    public Arrow curArrow;

    public AudioSource jump;
    public AudioSource Dbjump;


    [SerializeField] private float rotateSpeed;
    [SerializeField] private float moveSpeed;
    public float curHP;
    public float maxHP;

    public float atk;

    [SerializeField]
    private int jumpCount = 2;


    public CapsuleCollider myCollider;

    [Header("R 무기 위치")]
    public GameObject WeaponPosition_R;
    [Header("L 무기 위치")]
    public GameObject WeaponPosition_L;

    private Vector3 movement;
    private Vector3 dodgeVec;
    private Vector3 mouseDir;
    public Vector3 dir_;

    private bool isWall = false;
    private bool isRespawn = false;
    private bool isGrounded = true;
    private bool isJumping;
    private bool isRunning;
    private bool isRunningBack;
    private bool isDoubleJump;
    private bool isHi;
    private bool isPicking;
    private bool isItemEnter;
    private bool clickC;



    public bool isDodge;

    private bool dDown;


    private bool mLDown;//마우스 왼쪽
    private bool mRDown;//마우스 오른쪽

    float horizontalMove;
    float verticalMove;


    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public GameObject CM;
    private Camera characterCamera;

    private void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        maxHP = 1000f;
        curHP = 1000f;

        if (PV.IsMine)
        {
            Instance = this;
            ArrowIntialize(10);

            CM = GameObject.Find("Main Camera");
            characterCamera = CM.GetComponent<Camera>();
            var CM_cm = CM.GetComponent<Camera_Move>();
            CM_cm.player = this.gameObject;
        }
        rgbd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private Arrow CreateNewArrow()
    {
        var newObj = PhotonNetwork.Instantiate("Arrow", attackArea.transform.localPosition, attackArea.transform.localRotation).GetComponent<Arrow>();
        newObj.transform.SetParent(Instance.transform);
        newObj.transform.localPosition = new Vector3(0.122f, 1.377f, 0.587f);
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void ArrowIntialize(int count)
    {
        for(int i=0; i < count; i++)
        {
            arrowQ.Enqueue(CreateNewArrow());
        }
    }
    [PunRPC]
    public static Arrow GetArrow()
    {
        if (Instance.arrowQ.Count > 0) 
        {
            var obj = Instance.arrowQ.Dequeue();
            //obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewArrow();
            //newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
    [PunRPC]
    public static void ReturnArrow(Arrow arrow)
    {
        arrow.transform.SetParent(Instance.transform);
        arrow.transform.localPosition = new Vector3(0.122f, 1.377f, 0.587f);
        arrow.transform.localRotation = Quaternion.identity;
        arrow.rgbd.isKinematic = true;
        arrow.gameObject.SetActive(false);

        Instance.arrowQ.Enqueue(arrow);
    }

    private void FixedUpdate()
    {
        Zoom();
        Turn();
        Moving();
        Respawn();
        Dodge();

    }
    void Update()
    {
        if (PV.IsMine)
        {
            InputKey();
            AnimationUpdate();
            Attack();
            rgbd.velocity = new Vector3(0f, rgbd.velocity.y, 0f);


            if (transform.position.y < -100)
            {
                isRespawn = true;
            }


        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 100);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 100);
            Hp_Bar.hpBar.value = curHpValue;
        }
    }

    // 구르기 동작코드, 210624_황승민
    [PunRPC]
    void Dodge()
    {
        if (!isDodge && dDown &&!isAttack)
        {
            dodgeVec = movement;
            moveSpeed = 7;
            if (isRunningBack)
            {
                animator.SetTrigger("doDodge_back");

            }
            else
            {
                animator.SetTrigger("doDodge");

            }
            
            isDodge = true;

            Invoke("DodgeOut", 0.6f);
        }

    }
    // 구르기 벗어나는 동작코드, 210624_황승민
    [PunRPC]
    void DodgeOut()
    {
        moveSpeed = 5f;
        isDodge = false;

    }


    // 움직임 동작코드, 210624_황승민
    [PunRPC]
    void Moving()
    {


        if (isDodge)
        {
            movement.Set(dodgeVec.x, dodgeVec.y, dodgeVec.z);
        }
        else
        {
            movement.Set(horizontalMove,0, verticalMove);
        }
        
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        rgbd.transform.position += movement;
       
        //rgbd.MovePosition(transform.position + movement);
    }

    // 방향전환 동작코드, 210624_황승민
    [PunRPC]
    void Turn()
    {
        if (!isDodge)
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
            if (Physics.Raycast(ray, out hitResult))
            {
                mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                animator.transform.forward = mouseDir;
                if (mouseDir.x * horizontalMove <= 0f && mouseDir.z * verticalMove <= 0f)
                {
                    if (mouseDir.x * horizontalMove == 0f && mouseDir.z * verticalMove == 0f)
                        isRunningBack = false;
                    else
                        isRunningBack = true;
                }
                else
                {
                    isRunningBack = false;
                }
            }
        }
    }

    // 줌인 줌아웃 동작코드, 210624_황승민
    void Zoom()
    {
        var scroll = Input.mouseScrollDelta;
        characterCamera.fieldOfView = Mathf.Clamp(characterCamera.fieldOfView - scroll.y*2f, 30f, 70f);
    }


    // 리스폰 동작코드, 210624_황승민
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

    bool isAttack = false;
    public bool isAttackReady=false;
    public BoxCollider attackArea;
    public float attackDelay=0f;

    [PunRPC]
    void Attack()
    {
        isAttack = false;
        attackDelay += Time.deltaTime;


        switch (curStyle)
        {
            case Style.WeaponStyle.Sword:
                isAttackReady = 0.15f < attackDelay;
                
                if (isAttackReady && mLDown && !isDodge)
                {
                    animator.SetTrigger("doSlash");
                    isAttack = true;
                    //Invoke("Slash", 0f);
                    attackDelay = 0f;
                }
                break;
            case Style.WeaponStyle.Arrow:
                isAttackReady = 0.75f < attackDelay;

                if (mRDown&&isAttackReady && mLDown && !isDodge)
                {
                    animator.SetBool("isAim_Arrow", true);
                    animator.SetTrigger("doShoot");
                    Invoke("Shoot", 0.1f);
                    isAttack = true;
                    //Invoke("Slash", 0f);
                    attackDelay = 0f;
                }

                if(mRDown && curArrow == null)
                {
                    if(isAttackReady)
                        curArrow = Player_Control.GetArrow();
                }
                break;

        }

    }
    public void Slash()
    {
        attackArea.enabled = true;
        Invoke("SlashOut", 0.2f);
        // yield return new WaitForSeconds(0.1f);
    }
    public void SlashOut()
    {
        attackArea.enabled = false;
    }
    void Shoot()
    {
        RaycastHit hitResult;
        if(Physics.Raycast(characterCamera.ScreenPointToRay(Input.mousePosition),out hitResult))
        {
            var direction = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            if (curArrow != null)
            {
                curArrow.transform.SetParent(null);
                curArrow.transform.position = curArrow.transform.position + direction.normalized;
                curArrow.Shoot(direction.normalized);
                curArrow = null;
            }
            
        }
        
    }
    // Style::Arrow 공격모션 벗어나는 동작코드, 210624_황승민
    void ShootOut()
    {
        if (curArrow != null)
        {
            curArrow.DestroyArrow();
        }
        mRDown = false;
        curArrow = null;
    }
    // 키보드 및 마우스 입력관련, 210624_황승민
    void InputKey()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        dDown = Input.GetKey(KeyCode.LeftShift);
        mLDown = Input.GetMouseButtonDown(0);
        if (Input.GetMouseButtonDown(1)) {
            mRDown = true;
        }
        if (mRDown&&Input.GetMouseButtonUp(1))
        {
            Invoke("ShootOut", 0.2f);
        }
    }

    



    private void OnCollisionEnter(Collision col)
    {
    }
    private void OnCollisionStay(Collision col)
    {
        foreach(ContactPoint p in col.contacts)
        {
            Vector3 bottom = myCollider.bounds.center - (Vector3.up * myCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * myCollider.radius);
            
            dir_ = curve - p.point;

            
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
        if (PV.IsMine && other.CompareTag("Player_Attack"))
        {
            curHP -= other.transform.parent.GetComponent<Player_Control>().atk;
        }

    }
    [PunRPC]
    void head_jump()
    {
        //rgbd.velocity = Vector3.zero;
        //rgbd.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
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

 

    void AttackOut()
    {
        isAttack = false;
    }
    void AnimationUpdate()
    {
        switch (curStyle)
        {
            case Style.WeaponStyle.None:
                if (horizontalMove == 0 && verticalMove == 0)
                {

                    animator.SetBool("isRunning", false);
                }
                else
                {
                    animator.SetBool("isRunning", true);
                }
                if (isRunningBack)
                {
                    animator.SetBool("isRunningBack", true);
                }
                else
                {

                    animator.SetBool("isRunningBack", false);
                }
                break;
            case Style.WeaponStyle.Sword:
                //정지 시,
                if (horizontalMove == 0 && verticalMove == 0)
                {
                    animator.SetBool("isIdle_Sword", true);
                    animator.SetBool("isRunning_Sword",false);
                    animator.SetBool("isRunningBack_Sword", false);

      /*              if (mLDown && !isDodge)
                    {
                        Attack();
                    }*/

                }
                else//움직이고 있을 때,
                {
                    animator.SetBool("isRunning_Sword", true);
         /*           if (mLDown && !isDodge)
                    {

                        animator.SetTrigger("doSlash");
                    }*/

                    if (isRunningBack)// 뒤로움직일 때,
                    {
                        animator.SetBool("isRunningBack_Sword", true);
                    }
                    else
                    {
                        animator.SetBool("isRunningBack_Sword", false);
                    }
                }
                

                break;
            case Style.WeaponStyle.Arrow:
                
                //정지 시,
                if(horizontalMove == 0 && verticalMove == 0)
                {
                    if (mRDown)
                    {
                        animator.SetBool("isAim_Arrow", true);
                    }
                    else
                    {
                        animator.SetBool("isAim_Arrow", false);
                        animator.SetBool("isIdle_Arrow", true);


                    }
                    animator.SetBool("isRunning_Arrow", false);
                    animator.SetBool("isRunningBack_Arrow", false);
                }
                else// 움직이고 있을 때,
                {
                    animator.SetBool("isRunning_Arrow", true);

                    if (mRDown)
                    {
                        animator.SetBool("isAim_Arrow", true);
                    }
                    else
                    {
                        animator.SetBool("isAim_Arrow", false);
                    }

                    //뒤로 움직일 때,
                    if (isRunningBack)
                    {
                        animator.SetBool("isRunningBack_Arrow", true);

                        if (mRDown)
                        {
                            animator.SetBool("isAim_Arrow", true);
                        }
                        else
                        {
                            animator.SetBool("isAim_Arrow", false);
                        }
                    }
                    else
                    {
                        animator.SetBool("isRunningBack_Arrow", false);
                    }
                }


                break;  

               

            case Style.WeaponStyle.Magic:
                // 정지 시,
               if (horizontalMove == 0 && verticalMove == 0)
                {
                    animator.SetBool("isIdle_Magic", true);
                    animator.SetBool("isRunning_Magic", false);
                    animator.SetBool("isRunningBack_Magic", false);
                }
                else // 움직일 때,
                {
                    animator.SetBool("isRunning_Magic", true);

                    if (isRunningBack)
                    {
                        animator.SetBool("isRunningBack_Magic", true);
                    }
                    else
                    {
                        animator.SetBool("isRunningBack_Magic", false);
                    }
                }

                if (mLDown && !isDodge)
                {
                    animator.SetTrigger("doSpell");
                    Invoke("AttackOut", 0.6f);
                }

                break;

        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHP);
            stream.SendNext(Hp_Bar.hpBar.value);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            curHpValue = (float)stream.ReceiveNext();
        }
    }
}
