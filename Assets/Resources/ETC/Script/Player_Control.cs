using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Xml;
using Photon.Pun.Demo.Asteroids;
using Cinemachine;

public class Player_Control : MonoBehaviourPunCallbacks,IPunObservable
{
    public static GameObject Instance;

    /*    private Queue<Arrow> arrowQ = new Queue<Arrow>();*/

    //public bool[] duringQuestlist = { false };  //index = npcId(sectionID)

    public Animator animator;
    public Rigidbody rgbd;
    public PhotonView PV;
    public Text NickNameText;
    public Image HealthImage;
    public Player_HpBar Hp_Bar;

   

    public GameObject playerEquipPoint;
    public GameObject Head;
    public GameObject GunHead;

    int SoldierType; // 0~6;

    public ParticleSystem telportEffect;

    public Color Head_color;

    public Style.WeaponStyle curStyle;
    Vector3 curPos;
    Quaternion curRot;
    float curHpValue;

   /* public Arrow curArrow;*/

    [SerializeField] private float rotateSpeed;
    //수정테스트 ㅅ비라 진짜 제발;;
    public float walkSpeed;
    public float curSpeed;
    public float curHP;
    public float maxHP;

    public float atk;
    public int level;
    public float curEXP;
    public float maxEXP;

    public CapsuleCollider myCollider;

    [Header("R 무기 위치")]
    public GameObject WeaponPosition_R;
    [Header("L 무기 위치")]
    public GameObject WeaponPosition_L;

    private Vector3 movement;
    private Vector3 dodgeVec;
    public Vector3 mouseDir;
    public Vector3 mouseDir_y;
    public Vector3 dir_;

    #region
    // 상태관련 변수
    private bool isRespawn = false; // 리스폰
    private bool isAttack = false; // 공격중인지
    public bool isAttackReady = false; // 공격가능한 상태인지
    private bool isRunning; // 움직이고 있는지
    private bool isRunningBack; // 뒤로 움직이고 있는지
    private bool isDeffensing; // 방어중인지 (class.Sword만 가능)
    private bool isSkill;
    private bool isLevelUp;
    public bool isDodge; // 구르기 중인지
    #endregion


    #region
    // 입력 관련 변수

    float horizontalMove; // 키보드 입력
    float verticalMove;   // 키보드 입력

    private bool dDown; //LShift 입력
    private bool mLDown;//마우스 왼쪽 입력
    private bool mRDown;//마우스 오른쪽 입력
    private bool eDown; //e 입력 (스킬 사용)

    #endregion


    #region
    //------------ 클래스별 내용들--------------//
    // Class.Arrow 관련 내용들..
    public GameObject shootPoint; // Class.Arrow의 화살이 나가는 지점
    public float pullPower; // 활 당기는 힘


    // Class.Sword 관련 내용들..
    public float shieldAmount; // 쉴드량 정도
    private bool isShieldCharge; // 쉴드공격 챠지
    public BoxCollider attackArea;
    public TrailRenderer attackEffect;


    // 공통 사용 변수
    public float attackDelay = 0f;

    #endregion

    #region
    //사운드소스
    public AudioSource sound_Slash1;
    public AudioSource sound_Slash2;
    public AudioSource sound_Shoot1;

    public AudioSource sound_Teleport;
    #endregion
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public GameObject CM;
    public Camera characterCamera;

    public CinemachineVirtualCamera CVC;

    public GameObject Respawn_Center;

    public GameObject interface_player;

    #region
    // Soldier 세트 정보
    public GameObject SoldierAllSet;
    public Transform Set1;
    public Transform Set2;
    Vector3 Set1_Init_pos;
    Vector3 Set2_Init_pos;
    #endregion

    string[] SoldierType_melee_str = { "Soldier_main_melee", "Soldier_main_melee_B", "Soldier_main_melee_C" };
    string[] SoldierType_arrow_str = {"Soldier_main_arrow","Soldier_main_arrow_B", "Soldier_main_arrow_C" };
    private void Awake()
    {
        

        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        maxHP = 2000f;
        curHP = 2000f;
        curEXP = 0f;
        maxEXP = 100f;
        pullPower = 20f;
        shieldAmount = 500f;
        Respawn_Center = GameObject.Find("Respawn_Spots");

        //

        if (PV.IsMine)
        {
            interface_player.SetActive(true);
            level = 1;
            walkSpeed = 4f;
            curSpeed = walkSpeed;
            transform.position = Respawn_Center.transform.GetChild((int)(PV.ViewID / 1000)).transform.position;
            SoldierType = Random.Range(0,3);
            Set1_Init_pos = Set1.localPosition;
            Set2_Init_pos = Set2.localPosition;
            Instance = this.gameObject;


           /* for (int i = 0; i < 10; i++)
            {

                GameObject go = PhotonNetwork.Instantiate(SoldierType_melee_str[SoldierType], transform.position, transform.rotation);
                Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                so.myNumber = i;
                so.mySetNumber = 2;

            }
            for (int i = 0; i < 10; i++)
            {
                GameObject go = PhotonNetwork.Instantiate(SoldierType_arrow_str[SoldierType], transform.position, transform.rotation);
                Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                so.myNumber = i;
                so.mySetNumber = 1;
            }*/


            /*            Instance = this;
                        ArrowIntialize(10);*/
            rgbd = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            CM = GameObject.Find("Main Camera");
            CVC =CM.GetComponent<CinemachineVirtualCamera>();
            CVC.Follow = this.transform;
            characterCamera = CM.GetComponent<Camera>();
            //var CM_cm = CM.GetComponent<Camera_Move>();
            //CM_cm.player = this.gameObject;
        }

    }
    private void FixedUpdate()
    {

        if (curStyle == Style.WeaponStyle.Arrow||curStyle==Style.WeaponStyle.Magic)
        {
            if (isAttackReady == true)
            {
                Moving();
                if (curStyle != Style.WeaponStyle.Magic)
                    Dodge();
                Turn();
            }
        }
        else
        {
            Moving();
            Dodge();
            Turn();
        }
        Respawn();


    }
    void Update()
    {
        if (PV.IsMine)
        {
            InputKey();
            AnimationUpdate();
            Attack();
            rgbd.velocity = new Vector3(0f, rgbd.velocity.y, 0f);

            if (mRDown&&!isDodge)
            {
                pullPower += Time.deltaTime *14f;
                if (pullPower >= 40f)
                {
                    pullPower = 40f;
                }
            }
            if (shieldAmount <= 0f)
            {
                shieldAmount = 0f;
            }
            if (shieldAmount >= 1000f)
            {
                shieldAmount = 1000f;
            }
            shieldAmount += Time.deltaTime * 50f;

            if (transform.position.y < -100)
            {
                isRespawn = true;
            }

            if(curEXP >= maxEXP && !isLevelUp)
            {
                LevelUp();
            }



        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.fixedDeltaTime * 20);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.fixedDeltaTime * 20);
            //Hp_Bar.transform.rotation = Quaternion.Euler(new Vector3())
            //Hp_Bar.transform.rotation = Quaternion.Euler(new Vector3)
            Hp_Bar.hpBar.value = curHpValue;
        }
        switch (curStyle)
        {
            case Style.WeaponStyle.None:
                break;
            case Style.WeaponStyle.Sword:
                WeaponPosition_R.transform.GetChild(1).gameObject.SetActive(true);
                WeaponPosition_L.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case Style.WeaponStyle.Arrow:
                WeaponPosition_L.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case Style.WeaponStyle.Magic:
                WeaponPosition_R.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
    }
    // 키보드 및 마우스 입력관련, 210624_황승민
    void InputKey()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        //퀘스트보드


        if (Input.GetKeyDown(KeyCode.E))
        {
            eDown = true;
        }
        if(eDown && Input.GetMouseButtonDown(0))
        {
            if(curStyle == Style.WeaponStyle.Arrow)
                Invoke("Skill_arrow_E",0.1f);
            else if(curStyle == Style.WeaponStyle.Sword)
            {
                Skill_E();

            }
        }

        dDown = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(curStyle == Style.WeaponStyle.Magic)
                PV.RPC("Teleport",RpcTarget.All);
        }

        mLDown = Input.GetMouseButtonDown(0);
        if (Input.GetMouseButtonDown(0) && isDeffensing && shieldAmount >=500f)
        {
            if (curStyle == Style.WeaponStyle.Sword)
                PV.RPC("ShieldAttack", RpcTarget.All);
        }

        if (Input.GetMouseButtonDown(1))
        {
            mRDown = true;
            if (curStyle == Style.WeaponStyle.Sword && !dDown)
            {
                isDeffensing = true;
            }
            //Deffense();
        }
        if (mRDown && Input.GetMouseButtonUp(1))
        {
            Invoke("ShootOut", 0.2f);
            Invoke("DeffenseOut", 0.0f);
            Invoke("PullPower_valueChange",0.1f);
        }

/*        if (Input.GetMouseButtonDown(2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit)){
                Set1.transform.localPosition = new Vector3(hit.point.x - Set1.transform.localPosition.x, 0f, hit.point.y - Set1.transform.localPosition.z) + new Vector3(Set1_Init_pos.x, 0, Set1_Init_pos.z);
                Set2.transform.localPosition = new Vector3(hit.point.x - Set2.transform.localPosition.x, 0f, hit.point.y - Set2.transform.localPosition.z) + new Vector3(Set1_Init_pos.x, 0, Set1_Init_pos.z);
            }

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Set1.transform.localPosition = new Vector3(Set1_Init_pos.x,0,Set1_Init_pos.z);
            Set2.transform.localPosition = new Vector3(Set2_Init_pos.x, 0, Set2_Init_pos.z);
        }*/
    }
    [PunRPC]
    void ShieldAttack()
    {
        animator.SetTrigger("doShieldAttack_Sword");
        shieldAmount -= 500f;
        Invoke("ShieldAttack_body", 0.1f);
    }

    void ShieldAttack_body()
    {
        rgbd.AddForce(transform.forward * 60f, ForceMode.Impulse);
        rgbd.AddForce(Vector3.up * 7f, ForceMode.Impulse);
    }


    [PunRPC]
    void Teleport()
    {
        telportEffect.Play();
        sound_Teleport.Play();
        rgbd.AddForce(movement*1000f, ForceMode.Impulse);
    }
    void PullPower_valueChange()
    {
        pullPower = 20f;
    }
    void Deffense()
    {
        isDeffensing = true;
        if (curStyle == Style.WeaponStyle.Sword)
        {
            if(isDeffensing)
                animator.SetBool("isDeffensing", true);
            //animator.SetTrigger("doDeffense");
        }
    }
    void DeffenseOut()
    {
        isDeffensing = false;
        mRDown = false;
        curSpeed = walkSpeed;
    }
/*    private Arrow CreateNewArrow()
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
    }*/



    // 구르기 동작코드, 210624_황승민
    [PunRPC]
    void Dodge()
    {
        if (!isDodge && dDown &&!isAttack &&!isDeffensing)
        {
            dodgeVec = movement;

            curSpeed = 7;
            if (isRunningBack)
            {
                animator.SetTrigger("doDodge_back");
                animator.transform.forward = -dodgeVec;

            }
            else
            {
                animator.SetTrigger("doDodge");
                animator.transform.forward = dodgeVec;

            }

            isDodge = true;

            Invoke("DodgeOut", 0.7f);
        }

    }
    // 구르기 벗어나는 동작코드, 210624_황승민
    [PunRPC]
    void DodgeOut()
    {
        curSpeed = walkSpeed;

        isDodge = false;

    }

    Vector3 Skill_Vector;
    void Skill_E()
    {
        isSkill = true;
        attackEffect.enabled = false;
        animator.SetTrigger("doSkill_E");
        Invoke("Slash", 0.2f);
        curSpeed = 2f;
        Skill_Vector = mouseDir.normalized;
        Invoke("Skill_E_Out", 0.7076923f);
    }
    void Skill_E_Out()
    {
        curSpeed = walkSpeed;
        attackEffect.enabled = false;
        isSkill = false;
        eDown = false;

    }
    // 움직임 동작코드, 210624_황승민
    [PunRPC]
    void Moving()
    {


        if (!isDodge && !isSkill)
        {
            movement.Set(horizontalMove, 0, verticalMove);
        }
        else
        {
            if(isDodge)
                if (curStyle != Style.WeaponStyle.Magic)
                    movement.Set(dodgeVec.x, dodgeVec.y, dodgeVec.z);
            if (isSkill)
                if (curStyle == Style.WeaponStyle.Sword)
                    movement.Set(Skill_Vector.x, Skill_Vector.y, Skill_Vector.z);
        }

        if (isDeffensing)
        {
            curSpeed = 3f;
        }
        movement = movement.normalized * curSpeed * Time.deltaTime;
        rgbd.transform.position += movement;

        //rgbd.MovePosition(transform.position + movement);
    }

    // 방향전환 동작코드, 210624_황승민
    [PunRPC]
    void Turn()
    {
        if (!isDodge && !isSkill)
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            // 레이어마스크 /////
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            RaycastHit hitResult;
            /////////////////////
            ///
            if (Physics.Raycast(ray, out hitResult,200f,layerMask))
            {
                mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                animator.transform.forward = mouseDir;
                mouseDir_y = new Vector3(hitResult.point.x, hitResult.point.y, hitResult.point.z) - transform.position;
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
/*    void Zoom()
    {
        var scroll = Input.mouseScrollDelta;
        characterCamera.fieldOfView = Mathf.Clamp(characterCamera.fieldOfView - scroll.y*2f, 30f, 70f);
    }*/


    // 리스폰 동작코드, 210624_황승민
    void Respawn()
    {
        if (!isRespawn)
            return;
        transform.position = new Vector3(0f, 15f, 0f);
        isRespawn = false;
    }
    public void Hit(float atk_)
    {

        if (isDeffensing)
        {
            if (shieldAmount > 0)
            {
                float dmg = shieldAmount - atk_;
                shieldAmount -= atk_;
                if (dmg < 0)
                {
                    curHP -= (-dmg);
                }
            }
            else
                curHP -= atk_;
        }
        else
            curHP -= atk_;

        if (curHP <= 0)
        {
            GameObject.Find("MainCanvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    void Attack()
    {
        isAttack = false;
        attackDelay += Time.deltaTime;


        switch (curStyle)
        {
            case Style.WeaponStyle.Sword:
                isAttackReady = 0.15f < attackDelay;

                if (isAttackReady && mLDown && !isDodge && !isDeffensing && !eDown)
                {

                    animator.SetTrigger("doSlash");
                    isAttack = true;
                    //Invoke("Slash", 0f);
                    attackDelay = 0f;
                }


                break;
            case Style.WeaponStyle.Arrow:
                isAttackReady = 0.6f < attackDelay;

                if (mRDown&&isAttackReady && mLDown && !isDodge)
                {

                    animator.SetBool("isAim_Arrow", true);
                    animator.SetTrigger("doShoot");
                    sound_Shoot1.volume = pullPower / 120f;
                    Invoke("Shoot", 0.1f);
                    isAttack = true;
                    Invoke("PullPower_valueChange", 0.1f);
                    attackDelay = 0f;
                }

/*                if(mRDown && curArrow == null)
                {
                    if(isAttackReady)
                        curArrow = GetArrow();
                }*/
                break;
            case Style.WeaponStyle.Magic:
                isAttackReady = 0.82f < attackDelay;

                if (isAttackReady && mLDown && !isDodge)
                {
                    animator.SetTrigger("doSpell");
                    Invoke("Spell", 0.35f);
                    isAttack = true;
                    attackDelay = 0f;
                }

                /*                if(mRDown && curArrow == null)
                                {
                                    if(isAttackReady)
                                        curArrow = GetArrow();
                                }*/
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


    public void EffectOn_Slash()
    {
        attackEffect.enabled = true;
    }
    public void EffectOff_Slash()
    {
        attackEffect.enabled = false;
    }
    public void Sound_Slash1()
    {
        sound_Slash1.Play();
    }
    public void Sound_Slash2()
    {
        sound_Slash2.Play();
    }
    public void Sound_Shoot1()
    {
        sound_Shoot1.Play();
    }

    void Shoot()
    {
        PhotonNetwork.Instantiate("Arrow", shootPoint.transform.position, shootPoint.transform.rotation);
        /*if(Physics.Raycast(characterCamera.ScreenPointToRay(Input.mousePosition),out hitResult,10))
        {
            var direction = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            *//*if (curArrow != null)
            {
                curArrow.transform.SetParent(null);
                curArrow.transform.position = curArrow.transform.position + direction.normalized;

                curArrow.PV.RPC("Shoot", RpcTarget.AllBuffered, direction.normalized);
                curArrow = null;
            }*//*

        }*/
    }

    // Style::Arrow 공격모션 벗어나는 동작코드, 210624_황승민
    void ShootOut()
    {/*
        if (curArrow != null)
        {
            curArrow.DestroyArrow();
        }*/
        mRDown = false;/*
        curArrow = null;*/
    }

    void Skill_arrow_E()
    {
        PhotonNetwork.Instantiate("Arrow", shootPoint.transform.position+new Vector3(-0.2f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f,-10f,0f)));
        PhotonNetwork.Instantiate("Arrow", shootPoint.transform.position+new Vector3(-0.1f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, -5f, 0f)));
        PhotonNetwork.Instantiate("Arrow", shootPoint.transform.position+new Vector3(0.1f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, 5f, 0f)));
        PhotonNetwork.Instantiate("Arrow", shootPoint.transform.position+new Vector3(0.2f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, 10f, 0f)));

        eDown = false;
    }


    void Spell()
    {
        RaycastHit hitResult;
        if (Physics.Raycast(characterCamera.ScreenPointToRay(Input.mousePosition), out hitResult))
        {
            var direction = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            /*if (curArrow != null)
            {
                curArrow.transform.SetParent(null);
                curArrow.transform.position = curArrow.transform.position + direction.normalized;

                curArrow.PV.RPC("Shoot", RpcTarget.AllBuffered, direction.normalized);
                curArrow = null;
            }*/
            PhotonNetwork.Instantiate("Spell", attackArea.transform.position, attackArea.transform.rotation);
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

        if (PV.IsMine && other.CompareTag("Player_Sword"))
        {
            Hit(other.transform.parent.GetComponent<Player_Control>().atk);
        }
        if (PV.IsMine && other.CompareTag("Soldier_Attack"))
        {
            Hit(other.transform.parent.GetComponent<Player_Control>().atk);
        }
        if (other.CompareTag("Monster_Attack"))
        {
            Hit(other.transform.parent.GetComponent<Monster>().atk);
        }


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

                if (isDeffensing)
                {
                    animator.SetBool("isDeffensing", true);
                }
                else
                {
                    animator.SetBool("isDeffensing", false);
                }

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
                if (mRDown)
                {
                    animator.SetBool("isDeffensing", true);
                }
                else
                {
                    animator.SetBool("isDeffensing", false);
                }


                break;
            case Style.WeaponStyle.Arrow:

                //정지 시,
                if(horizontalMove == 0 && verticalMove == 0)
                {
                    animator.SetBool("isIdle_Arrow", true);
                    if (mRDown)
                    {
                        animator.SetBool("isAim_Arrow", true);
                    }
                    else
                    {
                        animator.SetBool("isAim_Arrow", false);



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

                break;

        }
    }

    public void LevelUp()
    {
        isLevelUp = true;
        interface_player.GetComponent<Player_Interface>().StartCoroutine("Shuffle");
        curEXP = 0f;
        maxEXP += 50f;
        level += 1;
        Invoke("LevelUpDelay", 0.2f);
    }
    //수정테스트
    void LevelUpDelay()
    {
        isLevelUp = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHP);
            stream.SendNext(Hp_Bar.hpBar.value);
            stream.SendNext(atk);
            stream.SendNext(pullPower);
            stream.SendNext(curStyle);
            stream.SendNext(shieldAmount);
            stream.SendNext(level);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            curHpValue = (float)stream.ReceiveNext();
            atk = (float)stream.ReceiveNext();
            pullPower = (float)stream.ReceiveNext();
            curStyle = (Style.WeaponStyle)stream.ReceiveNext();
            shieldAmount = (float)stream.ReceiveNext();
            level = (int)stream.ReceiveNext();
        }
    }
}
