using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Xml;
using Photon.Pun.Demo.Asteroids;
using Cinemachine;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon.StructWrapping;
using UnityEditor;

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

    public ParticleSystem Hp_Recovery1;
    public ParticleSystem Hp_Recovery2;

    public GameObject FloatingText;

    public GameObject playerEquipPoint;
    public GameObject Head;
    public GameObject GunHead;

    public int SoldierType; // 0~6;

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

    public float curStamina;//Magic = 마나
    public float maxStamina;

    public int curCritical;

    public int curDefense;

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

    // 상태관련 변수
    #region
    private bool isRespawn = false; // 리스폰
    private bool isAttack = false; // 공격중인지
    public bool isAttackReady = false; // 공격가능한 상태인지
    private bool isRunning; // 움직이고 있는지
    private bool isRunningBack; // 뒤로 움직이고 있는지
    private bool isDeffensing; // 방어중인지 (class.Sword만 가능)
    public bool isSkill;
    private bool isSkill_R;
    private bool isLevelUp;
    public bool isDodge; // 구르기 중인지
    public bool isDeath;
    #endregion

    // 입력 관련 변수
    #region


    public float horizontalMove; // 키보드 입력
    public float verticalMove;   // 키보드 입력

    private bool dDown; //LShift 입력
    private bool mLDown;//마우스 왼쪽 입력
    private bool mRDown;//마우스 오른쪽 입력
    public bool eDown; //e 입력 (스킬 사용)
    private bool tabDown;

    #endregion

    //------------ 직업 클래스별 내용들--------------//
    #region
    // Class.Magic 
    public GameObject spellPoint;
    // Class.Arrow 관련 내용들..
    public GameObject shootPoint; // Class.Arrow의 화살이 나가는 지점
    public float pullPower; // 활 당기는 힘


    // Class.Sword 관련 내용들..
    public float shieldAmount; // 쉴드량 정도
    private bool isShieldCharge; // 쉴드공격 챠지
    public BoxCollider attackArea;
    public ParticleSystem attackEffect;


    // 공통 사용 변수
    public float attackDelay = 0f;
    public float skill_E_Delay = 0f;
    public float skill_R_Delay = 0f;

    public float skill_E_cooltime=5f;
    public float skill_R_cooltime=5f;

    public bool isSkill_E_Ready;
    public bool isSkill_R_Ready;

    #endregion

    //사운드소스
    #region
    public AudioSource sound_Slash1;
    public AudioSource sound_Slash2;
    public AudioSource sound_Shoot1;

    public AudioSource sound_Teleport;

    public AudioSource sound_source;
    public AudioClip sound_slash_hit;
    public AudioClip sound_arrow_hit;

    public AudioClip sound_golem_hit;
    public AudioClip sound_demon_hit;
    public AudioClip sound_slime_hit;


    public AudioClip sound_dodge;
    public AudioClip sound_backJump;
    public AudioClip sound_skill_slash;

    public AudioClip sound_death_male;
    public AudioClip sound_death_female;

    #endregion
    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public GameObject CM;
    public Camera characterCamera;

    public GameObject MM;
    public Camera minimapCamera;

    public CinemachineVirtualCamera CVC;
    public CinemachineVirtualCamera CVC_mini;

    public GameObject Respawn_Center;

    public GameObject interface_player;
    public Player_Interface PI;

    public GameObject minimap;

    // Soldier 세트 정보
    #region

    public GameObject SoldierAllSet;
    public Transform Set1;
    public Transform Set2;
    Vector3 Set1_Init_pos;
    Vector3 Set2_Init_pos;



    public int SoldierPoint;
    public int SoldierPoint_max;
    #endregion


    public MotionTrail mt;
    public GameObject SKMR;

    public bool[] Inventory_item_is;


    public QuestData QD;
    public float curOccupied_value;



    // Step Up Stairs
    public GameObject stepRayUpper;
    public GameObject stepRayLower;

    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;


    public GameObject redBuff;
    public GameObject blueBuff;
    public GameObject greenBuff;

    public float redBuff_time; // 공격
    public float blueBuff_time; // 방어
    public float greenBuff_time; // 이속

    float addAtk;
    public bool isRedBuff_benefit;
    public bool isBlueBuff_benefit;
    public bool isGreenBuff_benefit;


    public string username;
    public int kill_point;
    public int death_point;

    public Player_Control Last_Hiter;


    public GameObject Male_Charactor;
    public GameObject Female_Charactor;

    public Transform Male_Hair_Offset;
    public Transform Female_Hair_Offset;
    public Transform Beard_Offset;

    public Material[] Skins;


    public string preset_data;
    public int[] preset_int;

    public int arenaRank;
    public bool arenaWin;
    public bool isArena;

    public GameObject KillLogs;

    public int yaktal = 0;
    public int star;

    public int monster_killpoint;
    public int score;

    public bool HP_CHANGE = false;
    private void Awake()
    {
        star = 0;
        score = 0;
        SoldierPoint = 20;
        SoldierPoint_max = 20;
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        username = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        HealthImage.color = PV.IsMine ? Color.green : Color.red;
        //NickNameText.color = PV.IsMine ? Color.green : Color.red;
        curCritical = 0;
        maxHP = 2000f;
        curHP = 2000f;
        curEXP = 0f;
        maxEXP = 100f;
        curDefense = 0;
        curStamina = 100f;
        maxStamina = 100f;
        pullPower = 20f;
        shieldAmount = 200f;
        Respawn_Center = GameObject.Find("Respawn_Spots");
        //

        if (PV.IsMine)
        {
            minimap.SetActive(true);
            interface_player.SetActive(true);
            PI = interface_player.GetComponent<Player_Interface>();
            level = 1;
            walkSpeed = 4f;
            curSpeed = walkSpeed;
            transform.position = Respawn_Center.transform.GetChild((int)(PV.ViewID / 1000)-1).transform.position;
            SoldierType = Random.Range(0,3);
            Set1_Init_pos = Set1.localPosition;
            Set2_Init_pos = Set2.localPosition;
            Instance = this.gameObject;


            stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.localPosition.x, stepHeight, stepRayUpper.transform.localPosition.z);





            /*            Instance = this;
                        ArrowIntialize(10);*/
            rgbd = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            KillLogs = GameObject.Find("KillLogs");

            CM = GameObject.Find("Main Camera");
            CVC =CM.GetComponent<CinemachineVirtualCamera>();
            CVC.Follow = this.transform;
            characterCamera = CM.GetComponent<Camera>();

            MM = GameObject.Find("Minimap Camera");
            MM.GetComponent<CameraMove>().Target = this.gameObject;
            //var CM_cm = CM.GetComponent<Camera_Move>();
            //CM_cm.player = this.gameObject;
        }

        
 

    }

    void Start()
    {
        arenaRank = GameManager.Instance.arenaRank;
        for (int i = 0; i < 4; i++)
        {
            preset_int[i] = preset_data[i] - '0';
        }


        if (preset_int[0] == 1)  //남자일 때
        {
            Female_Charactor.SetActive(false);
            Male_Charactor.SetActive(true);

            Female_Hair_Offset.gameObject.SetActive(false);
            Male_Hair_Offset.gameObject.SetActive(true);
            Beard_Offset.gameObject.SetActive(true);

            Male_Hair_Offset.GetChild(preset_int[1]).gameObject.SetActive(true);    //남자 머리
            Male_Hair_Offset.GetChild(preset_int[1]).GetChild(0).GetComponent<MeshRenderer>().material = Skins[preset_int[3]];
            Beard_Offset.GetChild(preset_int[2]).gameObject.SetActive(true);    //남자 수염
            Beard_Offset.GetChild(preset_int[2]).GetChild(0).GetComponent<MeshRenderer>().material = Skins[preset_int[3]];
            Male_Charactor.GetComponent<SkinnedMeshRenderer>().material = Skins[preset_int[3]];
        }
        else                //여자일 때
        {
            Male_Charactor.SetActive(false);
            Female_Charactor.SetActive(true);

            Male_Hair_Offset.gameObject.SetActive(false);
            Female_Hair_Offset.gameObject.SetActive(true);
            Beard_Offset.gameObject.SetActive(false);

            Female_Hair_Offset.GetChild(preset_int[1]).gameObject.SetActive(true);    //여자 머리
            Female_Hair_Offset.GetChild(preset_int[1]).GetChild(0).GetComponent<MeshRenderer>().material = Skins[preset_int[3]];
            Female_Charactor.GetComponent<SkinnedMeshRenderer>().material = Skins[preset_int[3]];
        }
    }

    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position,transform.TransformDirection(Vector3.forward),out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if(!Physics.Raycast(stepRayUpper.transform.position,transform.TransformDirection(Vector3.forward),out hitUpper, 0.2f))
            {
                rgbd.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f,0,1), out hitUpper45, 0.2f))
            {
                rgbd.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f,0,1), out hitLowerMinus45, 0.1f))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f,0,1), out hitUpperMinus45, 0.2f))
            {
                rgbd.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

    } 
    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        FreezeVelocity();
        if (isDeath == false)
        {
            //stepClimb();
            if (curStyle == Style.WeaponStyle.Arrow)
            {
                if (isAttackReady == true)
                {
                    Moving();
                    if (curStyle != Style.WeaponStyle.Magic)
                        Dodge();
                    Turn();
                }
            }
            else if(curStyle == Style.WeaponStyle.Magic)
            {
                if (isAttackReady == true)
                {
                    Moving();
                    if (curStyle != Style.WeaponStyle.Magic)
                        Dodge();
                   
                }
                Turn();
            }
            else
            {
                Moving();
                Dodge();
                Turn();
            }

        }
        score = kill_point * 10 + yaktal * 5 + monster_killpoint * 2;

    }
    [PunRPC]
    void BuffOnOff_RPC()
    {
        if (redBuff_time <= 0f)
        {
            if (redBuff.activeSelf == true)
            {
                redBuff.SetActive(false);
                atk = atk - 150f;
            }
            isRedBuff_benefit = false;
            
        }
        else
        {
            if (isRedBuff_benefit)
            {
                isRedBuff_benefit = false;
                atk = atk + 150f;
            }
            
            redBuff.SetActive(true);
            
            redBuff_time -= Time.deltaTime;
        }

        if (blueBuff_time <= 0f)
        {
            blueBuff.SetActive(false);
            isBlueBuff_benefit = false;
        }
        else
        {
            if (isBlueBuff_benefit)
            {
                isBlueBuff_benefit = false;
            }
            blueBuff.SetActive(true);
            blueBuff_time -= Time.deltaTime;
        }

        if (greenBuff_time <= 0f)
        {
            if (greenBuff.activeSelf == true)
            {
                greenBuff.SetActive(false);
                curSpeed = walkSpeed;
            }
           
            isGreenBuff_benefit = false;
        }
        else
        {
            if (isGreenBuff_benefit)
            {
                isGreenBuff_benefit = false;
                curSpeed = walkSpeed + 3f;
            }
            greenBuff.SetActive(true);
            greenBuff_time -= Time.deltaTime;
        }
    }
    void Skill()
    {
        skill_E_Delay +=Time.deltaTime;
        skill_R_Delay +=Time.deltaTime;

        isSkill_E_Ready = skill_E_cooltime < skill_E_Delay;
        isSkill_R_Ready = skill_R_cooltime < skill_R_Delay;

        if(curStyle == Style.WeaponStyle.Magic)
        {
            if (isSkill)
            {
                curStamina -= Time.deltaTime * 40f;
                attackDelay = 0.4f;
                skill_E_Delay = 0f;
            }
        }

        if (eDown && !isSkill_E_Ready)
            eDown = false;
        if (eDown && Input.GetMouseButtonUp(1))
        {
            eDown = false;
        }

        if (eDown && mLDown && isSkill_E_Ready && !isSkill && !isDodge)
        {
            isSkill = true;
            eDown = false;
            if (curStyle == Style.WeaponStyle.Sword && !isAttack)
            {
                PV.RPC("Skill_sword_E", RpcTarget.All);
                //Skill_sword_E();
            }
            else if (curStyle == Style.WeaponStyle.Arrow && mRDown)
            {
                Skill_arrow_E();
            }
            else if(curStyle == Style.WeaponStyle.Magic )
            {
                PV.RPC("Skill_magic_E", RpcTarget.All);
            }
            skill_E_Delay = 0f;
        }


    }
    public ParticleSystem skill_magic_E_effect;
    [PunRPC]
    void Skill_magic_E()
    {
        animator.SetBool("isSkill_E", true);
        animator.SetTrigger("doSpell_Skill_E");
        skill_magic_E_effect.Play();

    }
    [PunRPC]
    void Skill_magic_E_out()
    {
        skill_magic_E_effect.Stop();
        animator.SetBool("isSkill_E", false);
        isSkill = false;
    }


    Vector3 Skill_sword_E_Vector;
    public ParticleSystem sword_E_effect;
    public MeshCollider sword_E_area;
    [PunRPC]
    void Skill_sword_E()
    {
            animator.SetTrigger("doSkill_E");
            curSpeed = 2f;
            Skill_sword_E_Vector = mouseDir.normalized;
    }
    public void Skill_sword_E_effect()
    {
        sword_E_effect.Play();
        sound_source.PlayOneShot(sound_skill_slash);
        sword_E_area.enabled = true;
    }
    public void Skill_sword_E_Out()
    {
        if (greenBuff_time > 0)
            curSpeed = walkSpeed + 3f;
        else
            curSpeed = walkSpeed;

        sword_E_area.enabled = false;
        isSkill = false;

    }

    bool skill_setFalse=false;
    void isSkill_autoFalse() {
        isSkill = false;
        skill_setFalse = false;

    }


    void Update()
    {
        if (PV.IsMine)
        {
            if (isDeath == false)
            {
                if (PI.isActive_Input == true && GameManager.Instance.isActive)
                    InputKey();

                if (tabDown)
                    PI.GameBoard_Tab.SetActive(true);
                else
                    PI.GameBoard_Tab.SetActive(false);

                if (spellPoint)
                {
                    float degree_value = -((int)mouseDir_y.y * 10);
                    degree_value = Mathf.Clamp(degree_value, -90, 0);
                    spellPoint.transform.localRotation = Quaternion.Euler(new Vector3(degree_value, 0f, 0f));
                }
                if (shootPoint)
                {
                    float degree_value = -((int)mouseDir_y.y * 10);
                    degree_value = Mathf.Clamp(degree_value, -80, 0);
                    shootPoint.transform.localRotation = Quaternion.Euler(new Vector3(degree_value, 0f, 0f));
                }


                AnimationUpdate();
                Attack();
                Skill();
                if (curStyle != Style.WeaponStyle.Magic && isSkill == true && !skill_setFalse)
                {
                    skill_setFalse = true;
                    Invoke("isSkill_autoFalse", 1f);
                }
                if (curStyle == Style.WeaponStyle.Magic && isSkill&&curStamina <=2f)
                {
                    PV.RPC("Skill_magic_E_out",RpcTarget.All);
                }



                PV.RPC("BuffOnOff_RPC", RpcTarget.All);
                animator.SetFloat("RunningAmount", curSpeed / 4f);

                if (mRDown && !isDodge)
                {
                    pullPower += Time.deltaTime * 14f;
                    if (pullPower >= 40f)
                    {
                        pullPower = 40f;
                    }
                }
                if (shieldAmount <= 0f)
                {
                    shieldAmount = 0f;
                }
                if (shieldAmount >= 300f)
                {
                    shieldAmount = 300f;
                }
                shieldAmount += Time.deltaTime * 50f;


                curStamina += Time.deltaTime*10f;
                
                if(curStamina >= maxStamina)
                {
                    curStamina = maxStamina;
                }



                if (curHP >= maxHP)
                {
                    curHP = maxHP;
                }


                if (transform.position.y < -100)
                {
                    isRespawn = true;
                }

                if (curEXP >= maxEXP && !isLevelUp)
                {
                    float dif = maxEXP - curEXP;
                    
                    LevelUp();
                    curEXP += Mathf.Abs(dif);
                }

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


        
        if ( Input.GetKeyDown(KeyCode.E))
        {
            eDown = true;
        }

        dDown = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(curStyle == Style.WeaponStyle.Magic&&curStamina>=45f)
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
            if(curStyle == Style.WeaponStyle.Magic && isSkill)
            {
                PV.RPC("Skill_magic_E_out", RpcTarget.All);
            }
            //Deffense();
        }
        if (mRDown && Input.GetMouseButtonUp(1))
        {
            if (curStyle == Style.WeaponStyle.Arrow)
            {
                Invoke("ShootOut", 0.2f);
                Invoke("PullPower_valueChange", 0.1f);
            }
            else if(curStyle == Style.WeaponStyle.Sword)
                Invoke("DeffenseOut", 0.0f);
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Use_Item_num(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Use_Item_num(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Use_Item_num(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Use_Item_num(3);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (QD.QuestBoardPanel.activeSelf == false)
                QD.ShowQuestBoard();
            else
                QD.CloseQuestBoard();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (minimap.transform.localScale.x == 1.25f)
            {
                minimap.transform.localScale = new Vector3(2f, 2f, 1f);
                minimapCamera.orthographicSize = 25f;
               
            }
            else
            {
                minimap.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
                minimapCamera.orthographicSize = 15f;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PV.RPC("Skill_R", RpcTarget.All);
            //Skill_R();
        }
        tabDown = Input.GetKey(KeyCode.Tab);


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
        curStamina -= 45f;
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
        if (!isDodge && dDown &&!isAttack &&!isDeffensing&&!isSkill_R&& curStamina>=50f)
        {
            
            curStamina -= 50f;
            dodgeVec = movement;
           
            if (greenBuff_time > 0f)
                curSpeed = walkSpeed + 3f + 3f;
            else
                curSpeed = walkSpeed + 3f;



            if (isRunningBack)
            {
                sound_source.PlayOneShot(sound_backJump);
                animator.SetTrigger("doDodge_back");
                animator.transform.forward = -dodgeVec;

            }
            else
            {
                sound_source.PlayOneShot(sound_dodge);
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

  
    void FreezeVelocity()
    {
        rgbd.velocity = new Vector3(0f, rgbd.velocity.y, 0f);
        //rgbd.angularVelocity = Vector3.zero;
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
            if(isDodge&&!isSkill)
                if (curStyle != Style.WeaponStyle.Magic)
                    movement.Set(dodgeVec.x, dodgeVec.y, dodgeVec.z);
            if (!isDodge&&isSkill)
                if (curStyle == Style.WeaponStyle.Sword)
                    movement.Set(Skill_sword_E_Vector.x, Skill_sword_E_Vector.y, Skill_sword_E_Vector.z);
        }

        if (isDeffensing)
        {
            curSpeed = walkSpeed - 1f;
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
            int layerMask = (1 << LayerMask.NameToLayer("Environment")) |  (1 << LayerMask.NameToLayer("Wall"));
            RaycastHit hitResult;
            /////////////////////
            
            if (Physics.Raycast(ray, out hitResult,100f,layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitResult.distance, Color.yellow);
                //Debug.Log(hitResult.transform.gameObject.name);
                mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                animator.transform.forward = Vector3.Slerp(animator.transform.forward,mouseDir,0.5f);
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
        else if (curStyle ==Style.WeaponStyle.Magic && isSkill)
        {
            Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
            // 레이어마스크 /////
            int layerMask = (1 << LayerMask.NameToLayer("Environment")) | (1 << LayerMask.NameToLayer("Wall"));
            RaycastHit hitResult;
            /////////////////////

            if (Physics.Raycast(ray, out hitResult, 100f, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitResult.distance, Color.yellow);
                //Debug.Log(hitResult.transform.gameObject.name);
                mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                animator.transform.forward = Vector3.Slerp(animator.transform.forward, mouseDir, 0.05f);
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
    public void Respawn()
    {
        curHP = maxHP;
        transform.position = Respawn_Center.transform.GetChild((int)(PV.ViewID / 1000)-1).transform.position;
        GameObject.Find("MainCanvas").transform.Find("RespawnPanel").gameObject.SetActive(false);
        myCollider.enabled = true;
        rgbd.isKinematic = false;
        PV.RPC("InitAnim",RpcTarget.All);
        isDeath = false;
        
    }
    [PunRPC]
    void InitAnim()
    {
        animator.Rebind();
        if (curStyle == Style.WeaponStyle.Sword)
        {
            animator.Play("Idle_Sword");
        }
        else if (curStyle == Style.WeaponStyle.Arrow)
        {
            animator.Play("Idle_Arrow");
        }
        else if (curStyle == Style.WeaponStyle.Magic)
        {
            animator.Play("Idle_Magic");
        }
    }
    [PunRPC]
    public void Hit(float atk_,int type,int critical)
    {
        bool isCritical = Random.Range(0, 100) < critical;
        if (isCritical)
            atk_ *= 1.5f;

        atk_ -=atk_ *(float)(curDefense / 100f);

        if (type == 0)
        {
            sound_source.PlayOneShot(sound_slash_hit);
        }
        else if (type == 1)
        {
            sound_source.PlayOneShot(sound_arrow_hit);
        }
        else if(type == 2)
        {
            sound_source.PlayOneShot(sound_slime_hit);
        }
        else if (type == 3)
        {
            sound_source.PlayOneShot(sound_golem_hit);
        }
        else if (type == 4)
        {
            sound_source.PlayOneShot(sound_demon_hit);
        }
        else if (type == 5)
        {
            
        }


        if (PV.IsMine)
        {
            GameObject ft = PhotonNetwork.Instantiate("Damage_Text", transform.position, Quaternion.Euler(new Vector3(45f, 0f, 0f)));
            ft.transform.GetChild(0).transform.GetComponent<TextMesh>().text = "" + (int)atk_;
            if (isCritical)
            {
                ft.transform.GetChild(0).transform.GetComponent<TextMesh>().color = new Color(0.8962264f, 0.2352941f, 0f);
                ft.transform.GetChild(0).transform.GetComponent<TextMesh>().characterSize = 0.1f;
            }

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

            if (curHP <= 0 && isDeath == false)
            {

                if (preset_int[0] == 1)
                {
                    sound_source.PlayOneShot(sound_death_female);
                }
                else
                {

                    sound_source.PlayOneShot(sound_death_male);
                }

                if (isArena == true)
                {
                    arenaWin = false;
                    PV.RPC("setArenaRank", RpcTarget.All);
                    if(curStyle == Style.WeaponStyle.Magic && isSkill)
                    {
                        PV.RPC("Skill_magic_E_out", RpcTarget.All);
                    }
                    isDeath = true;
                    animator.SetTrigger("doDeath");
                    GameObject.Find("MainCanvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
                    myCollider.enabled = false;
                    rgbd.isKinematic = true;

                    //PI.ArenaOut();

                }
                else
                {
                    
                    PhotonNetwork.Instantiate("killLog", Vector3.zero, Quaternion.identity);
                    if (curStyle == Style.WeaponStyle.Magic && isSkill)
                    {
                        PV.RPC("Skill_magic_E_out", RpcTarget.All);
                    }
                    isDeath = true;
                    if(Last_Hiter)
                        PV.RPC("raiseKillPoint", RpcTarget.All);
                    animator.SetTrigger("doDeath");
                    myCollider.enabled = false;
                    rgbd.isKinematic = true;
                    death_point += 1;
                    GameObject.Find("MainCanvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
                    Invoke("Respawn", 4f * level);
                }
                //PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    void initRank()
    {
        GameManager.Instance.arenaRank = GameManager.Instance.ReadyCountMax;
    }
    [PunRPC]
    void setArenaRank()
    {
        arenaRank = GameManager.Instance.arenaRank--;
    }
    [PunRPC]
    void setArenaRank_stay()
    {
        arenaRank = GameManager.Instance.arenaRank;
    }
    [PunRPC]
    void raiseKillPoint()
    {
        if (!Last_Hiter.CompareTag("Player"))
            return;
        Last_Hiter.kill_point += 1;
        Last_Hiter.curEXP += 50f * level;
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

                if (isAttackReady && mLDown && !isDodge && !isDeffensing && !eDown && !isSkill)
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

                if (isAttackReady && mLDown && !isDodge && !isSkill && !eDown)
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

    [PunRPC]
    public void Slash1()
    {
        attackEffect.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        attackEffect.Play();
        
        attackArea.enabled = true;
        Invoke("SlashOut", 0.05f);
        // yield return new WaitForSeconds(0.1f);
    
   }
    [PunRPC]
    public void Slash2()
    {
        attackEffect.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
        attackEffect.Play();

        attackArea.enabled = true;
        Invoke("SlashOut", 0.05f);
        // yield return new WaitForSeconds(0.1f);
    }
    public void SlashOut()
    {
        attackArea.enabled = false;
    }


    public void EffectOn_Slash()
    {
        attackEffect.Play();
    }

    public void Sound_Slash1()
    {
        sound_Slash1.Play();
    }
    public void Sound_Slash2()
    {
        sound_Slash2.Play();
    }
    public void Sound_SlashSkill()
    {
        sound_source.PlayOneShot(sound_skill_slash);
    }
    public void Sound_Shoot1()
    {
        sound_Shoot1.Play();
    }

    void Shoot()
    {
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position, shootPoint.transform.rotation);
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
        animator.SetTrigger("doShoot");
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position+new Vector3(-0.4f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f,-10f,0f)));
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position+new Vector3(-0.2f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, -5f, 0f)));
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position , shootPoint.transform.rotation);
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position+new Vector3(0.2f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, 5f, 0f)));
        PhotonNetwork.Instantiate("Arrow_1", shootPoint.transform.position+new Vector3(0.4f,0,0), shootPoint.transform.rotation * Quaternion.Euler(new Vector3(0f, 10f, 0f)));
        attackDelay = 0.2f;
        isSkill = false;
    }
    [PunRPC]
    void Skill_R()
    {
        
        mt.TargetSkinMesh = SKMR;
        mt.gameObject.SetActive(false);
        mt.gameObject.SetActive(true);
        isSkill_R =true;
        

        curSpeed = walkSpeed +5f;
        Invoke("Skill_R_out", 3f);
    }
    void Skill_R_out()
    {
        if (greenBuff_time > 0f)
        {
            curSpeed = walkSpeed+3;
        }
        else
        {
            curSpeed = walkSpeed;
        }
        mt.TargetSkinMesh = null;
        isSkill_R = false;
        
    }


    void Spell()
    {
        PhotonNetwork.Instantiate("Spell_1", spellPoint.transform.position, spellPoint.transform.rotation);
    }





    private void OnCollisionEnter(Collision col)
    {
    }
/*    private void OnCollisionStay(Collision col)
    {
        foreach(ContactPoint p in col.contacts)
        {
            Vector3 bottom = myCollider.bounds.center - (Vector3.up * myCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * myCollider.radius);

            dir_ = curve - p.point;


        }


    }*/
    private void OnCollisionExit(Collision col)
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player_Sword"))
        {
            /*if ( PV.IsMine*//*&& PV.Owner != other.transform.parent.GetComponent<PhotonView>().Owner*//*)
            {
                
            }*/
            if (other.transform.parent.GetComponent<Player_Control>().isSkill)
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk * 1.5f, 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }
            else
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk, 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }

            Last_Hiter = other.transform.parent.GetComponent<Player_Control>();
        }
        if (other.CompareTag("Soldier_Attack") && PV.Owner != other.transform.parent.GetComponent<Soldier>().PV.Owner )
        {
            Hit(other.transform.parent.GetComponent<Soldier>().atk,0,0);
        }
        if (other.CompareTag("Monster_Attack"))
        {
            Monster hitObj = other.transform.parent.GetComponent<Monster>();
            int hit_sound = 0;
            if (hitObj.monsterType == Monster.Type.slime)
            {
                hit_sound = 2;
            }
            else if (hitObj.monsterType == Monster.Type.golem)
            {
                hit_sound = 3;
            }
            else if (hitObj.monsterType == Monster.Type.demon)
            {
                hit_sound = 4;
            }


            if (hitObj.isSkill)
            {
                if (hitObj.monsterType == Monster.Type.demon || hitObj.monsterType == Monster.Type.golem)
                {
                    Hit(hitObj.atk * 2f,0,hit_sound);
                }
            }
            else
            {
                
                Hit(hitObj.atk,0,hit_sound);
            }
        }

        if (other.CompareTag("Item"))
        {
            if (exist_CheckItem(other))
            {

            }
            else
            {
                not_exist_CheckItem(other);
            }
           /* for(int i = 0; i < 4; i++)
            {
                if (Inventory_item_is[i] == false)
                {
                    Player_Interface PI = interface_player.GetComponent<Player_Interface>();
                    Inventory_item_is[i] = true;
                    PI.Inventory_item_img[i].sprite = other.GetComponent<Item>().icon;
                    PI.Inventory_item_img[i].color = new Color(1f, 1f, 1f, 1f);
                    PI.Inventory_item_name[i] = ""+other.GetComponent<Item>().type;
                    PI.Inventory_item_num[i] += other.GetComponent<Item>().value;
                    PI.Inventory_item_txt[i].text = "x"+PI.Inventory_item_num[i];
                    
                    Destroy(other.gameObject);
                    break;
                }
                
            }*/
        }
        if (other.CompareTag("Item_Material"))
        {
            Item_Material item = other.GetComponent<Item_Material>();
            string name = "" + item.type;
            if (PI.item_Material.ContainsKey(name)){
                PI.item_Material[name] += item.value;
                Destroy(other.gameObject);
            }
        }


    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Occupied_Area"))
        {
            if(QD.questIsActive[3]==true)
                curOccupied_value += Time.deltaTime * 5f;
        }
    }
    [PunRPC]
    void RecoverHP()
    {
        Hp_Recovery1.Play();
        Hp_Recovery2.Play();
    }
    void Use_Item_num(int index)
    {
        bool acceptUse = true;
        if (Inventory_item_is[index] == true)
        {
            
            switch (PI.Inventory_item_name[index]) 
            {
                case "hp_potion":
                    if (curHP >= maxHP)
                    {
                        acceptUse = false;
                        break;
                    }
                    PV.RPC("RecoverHP", RpcTarget.All);
                    GameObject ft = PhotonNetwork.Instantiate("Damage_Text", transform.position, Quaternion.Euler(new Vector3(55f, 0f, 0f)));
                    ft.GetComponent<TextMesh>().text = "" + 200;
                    ft.transform.GetChild(0).transform.GetComponent<TextMesh>().color = Color.green;

                    curHP += 200f;
                    break;

                default:
                    acceptUse = false;
                    break;
            
            }
            if (acceptUse)
            {
                PI.Inventory_item_num[index] -= 1;
                PI.Inventory_item_txt[index].text = "x" + PI.Inventory_item_num[index];
                if (PI.Inventory_item_num[index] == 0)
                {
                    Inventory_item_is[index] = false;
                    PI.Inventory_item_txt[index].text = "";
                    PI.Inventory_item_img[index].sprite = null;
                    PI.Inventory_item_img[index].color = new Color(1f, 1f, 1f, 0f);
                }
            }
        }
    }

    bool exist_CheckItem(Collider other)
    {
        
        for (int i = 0; i < 4; i++)
        {
            if (Inventory_item_is[i] == true)
            {
               
                if ( (""+other.GetComponent<Item>().type)==PI.Inventory_item_name[i])
                {
                    PI.Inventory_item_num[i] += other.GetComponent<Item>().value;
                    PI.Inventory_item_txt[i].text = "x" + PI.Inventory_item_num[i];
                    Destroy(other.gameObject);
                    return true;
                }
            }
        }
        return false;

    }
    void not_exist_CheckItem(Collider other)
    {
       
        for (int i = 0; i < 4; i++)
        {
            if (Inventory_item_is[i] == false)
            {
                
                Inventory_item_is[i] = true;
                PI.Inventory_item_img[i].sprite = other.GetComponent<Item>().icon;
                PI.Inventory_item_img[i].color = new Color(1f, 1f, 1f, 1f);
                PI.Inventory_item_name[i] = "" + other.GetComponent<Item>().type;
                PI.Inventory_item_num[i] += other.GetComponent<Item>().value;
                PI.Inventory_item_txt[i].text = "x" + PI.Inventory_item_num[i];

                Destroy(other.gameObject);
                break;
            }

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
            stream.SendNext(maxHP);
            stream.SendNext(Hp_Bar.hpBar.value);
            stream.SendNext(atk);
            stream.SendNext(pullPower);
            stream.SendNext(curStyle);
            stream.SendNext(shieldAmount);
            stream.SendNext(level);
            stream.SendNext(redBuff_time);
            stream.SendNext(blueBuff_time);
            stream.SendNext(greenBuff_time);
            stream.SendNext(kill_point);
            stream.SendNext(monster_killpoint);
            stream.SendNext(star);
            stream.SendNext(yaktal);
            stream.SendNext(death_point);
            stream.SendNext(preset_data);
            stream.SendNext(Male_Hair_Offset.gameObject.activeSelf);
            stream.SendNext(Beard_Offset.gameObject.activeSelf);
            stream.SendNext(Female_Hair_Offset.gameObject.activeSelf);
            stream.SendNext(Male_Charactor.activeSelf);
            stream.SendNext(Female_Charactor.activeSelf);
            stream.SendNext(arenaRank);
            stream.SendNext(isDeath);
            stream.SendNext(arenaWin);
            stream.SendNext(isArena);
            stream.SendNext(isSkill);

            stream.SendNext(curStamina);
            stream.SendNext(maxStamina);

            stream.SendNext(curCritical);
            stream.SendNext(curDefense);

            stream.SendNext(HP_CHANGE);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            maxHP = (float)stream.ReceiveNext();
            curHpValue = (float)stream.ReceiveNext();
            atk = (float)stream.ReceiveNext();
            pullPower = (float)stream.ReceiveNext();
            curStyle = (Style.WeaponStyle)stream.ReceiveNext();
            shieldAmount = (float)stream.ReceiveNext();
            level = (int)stream.ReceiveNext();
            redBuff_time = (float)stream.ReceiveNext();
            blueBuff_time = (float)stream.ReceiveNext();
            greenBuff_time = (float)stream.ReceiveNext();
            kill_point = (int)stream.ReceiveNext();
            monster_killpoint = (int)stream.ReceiveNext();
            star = (int)stream.ReceiveNext();
            yaktal = (int)stream.ReceiveNext();
            death_point = (int)stream.ReceiveNext();
            preset_data = (string)stream.ReceiveNext();
            Male_Hair_Offset.gameObject.SetActive((bool)stream.ReceiveNext());
            Beard_Offset.gameObject.SetActive((bool)stream.ReceiveNext());
            Female_Hair_Offset.gameObject.SetActive((bool)stream.ReceiveNext());
            Male_Charactor.SetActive((bool)stream.ReceiveNext());
            Female_Charactor.SetActive((bool)stream.ReceiveNext());
            arenaRank = (int)stream.ReceiveNext();
            isDeath = (bool)stream.ReceiveNext();
            arenaWin = (bool)stream.ReceiveNext();
            isArena = (bool)stream.ReceiveNext();
            isSkill = (bool)stream.ReceiveNext();

            curStamina = (float)stream.ReceiveNext();
            maxStamina = (float)stream.ReceiveNext();

            curCritical = (int)stream.ReceiveNext();
            curDefense = (int)stream.ReceiveNext();

            HP_CHANGE = (bool)stream.ReceiveNext();
        }
    }
}
