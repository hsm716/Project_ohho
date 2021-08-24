using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum Type { slime,demon,golem};
    public Type monsterType;
    public int golem_Index; // 0 : red , 1 : blue, 2 : green
    public Material[] skinMat;
    public SkinnedMeshRenderer SKMR;

    public PhotonView PV;

    public int myPoolNum;

    public CapsuleCollider myCol;
    public CapsuleCollider skillCol;
    public BoxCollider meleeArea;

    public AudioSource sound_melee_attack;
    public AudioSource sound_golem_PunchAttack;


    public GameObject FloatingText_prefab;



    public Player_Control Last_Hiter;

    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;

    Vector3 curPos;
    Quaternion curRot;
    
    public float atk;

    public Transform target;

    Vector3 Init_Pos;

    public bool isAttack;

    public bool isChase;
    public bool isStop;
    public bool isDead;
    public bool isSkill;

    public float curHP;
    public float maxHP;

    public float curSkillAmount;
    public float maxSkillAmount;


    public ParticleSystem shockWave;
    public ParticleSystem hitSword;

    public AudioSource sound_source;
    public AudioClip sound_slash_hit;
    public AudioClip sound_arrow_hit;
    public AudioClip sound_golem_PunchAttack_clip;

    public Vector3 InitPos;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        InitPos = this.transform.position;
        //StartCoroutine("StepShake");

        //golem_Index = Random.Range(0, 3);
        Init_Pos = transform.position;
        agent.isStopped = true;

       
    }
    private void Start()
    {
        if (monsterType == Type.demon)
        {
            curHP = 10000f;
            maxHP = 10000f;
            curSkillAmount = 0f;
            maxSkillAmount = 100f;
            atk = 500f;
        }

        if (monsterType == Type.slime)
        {
            curHP = 1000f;
            maxHP = 1000f;
            atk = 10f;
            curSkillAmount = 0f;
            maxSkillAmount = 0f;
        }
        if (monsterType == Type.golem)
        {
            curHP = 5000f;
            maxHP = 5000f;
            atk = 100f;


            SKMR.material = skinMat[golem_Index];
            curSkillAmount = 0f;
            maxSkillAmount = 100f;
        }


        isAttack = false;
        isChase = false;
        isDead = false;
        target = null;


    }
   


    [PunRPC]
    public void Hit(float atk_,int type,int critical)
    {
        bool isCritical = Random.Range(0, 100) < critical;
        if (isCritical)
            atk_ *= 1.5f;

        anim.SetTrigger("doHit");
        hitSword.Play();

        curHP -= atk_;
        if (PV.IsMine)
        {
            GameObject ft = PhotonNetwork.Instantiate("Damage_Text", transform.position, Quaternion.Euler(new Vector3(55f, 0f, 0f)));
            
           ft.transform.GetChild(0).transform.GetComponent<TextMesh>().text = "" + (int)atk_;
            if (isCritical)
            {
                ft.transform.GetChild(0).transform.GetComponent<TextMesh>().color = new Color(0.8962264f, 0.2352941f, 0f);
                ft.transform.GetChild(0).transform.GetComponent<TextMesh>().characterSize = 0.1f;
            }
        }
        if (type == 0)
        {
            sound_source.PlayOneShot(sound_slash_hit);
        }
        else if(type==1)
        {
            sound_source.PlayOneShot(sound_arrow_hit);
        }
            
        if (!isAttack&&!isSkill)
        {
            isChase = true;
            agent.isStopped = false;
        }
        if (curHP <= 0 && !isDead)
        {
            float expAmount=0f;
            if (monsterType == Type.slime)
            {
                
                expAmount = 20f;
                if (Last_Hiter.GetComponent<QuestData>().questIsActive[0])
                {
                    
                    Last_Hiter.GetComponent<QuestData>().slimeKillCount++;
                    
                    //Last_Hiter.GetComponent<QuestData>().Quest();
                }

            }
            else if(monsterType == Type.demon)
            {
                expAmount = 100f;
                if (Last_Hiter.GetComponent<QuestData>().questIsActive[2])
                {
                    
                    Last_Hiter.GetComponent<QuestData>().DemonKill=true;
                    
                }
            }
            else if (monsterType == Type.golem)
            {
                expAmount = 75f;
                switch (golem_Index)
                {
                    
                    case 0:
                        Player_Control LH = Last_Hiter.GetComponent<Player_Control>();
                        LH.redBuff_time = 100f;
                        LH.isRedBuff_benefit = true;
                        break;
                    case 1:
                        Player_Control LH1 = Last_Hiter.GetComponent<Player_Control>();
                        LH1.blueBuff_time = 100f;
                        LH1.isBlueBuff_benefit = true;
                        break;
                    case 2:
                        Player_Control LH2 = Last_Hiter.GetComponent<Player_Control>();
                        LH2.greenBuff_time = 100f;
                        LH2.isGreenBuff_benefit = true;
                        break;
                }
                

            }
            Last_Hiter.GetComponent<Player_Control>().curEXP += expAmount;
            myCol.enabled = false;
            isDead = true;
            anim.SetTrigger("doDead");

             
        }
    }
    
    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        FreezeVelocity();
        if (!isDead)
        {
            
            //Targeting();
            if (agent.isStopped == true)
            {
                anim.SetBool("isRunning", false);
            }
            else
            {
                anim.SetBool("isRunning", true);
            }
        }
    }
    void FreezeVelocity()
    {
        rgbd.velocity = Vector3.zero;
        rgbd.angularVelocity = Vector3.zero;
    }
    [PunRPC]
    void Targeting()
    {
        float targetRadius = 3f;
        float targetRange = 5f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, Vector3.up, targetRange, LayerMask.GetMask("Enemy"));

        if (rayHits.Length > 0 && !isAttack)
        {
            int count = 0;
            float min_dist = Mathf.Infinity;
            for (int i = 0; i < rayHits.Length; i++)
            {
                if (rayHits[i].collider.GetComponent<PhotonView>().Owner != PV.Owner && rayHits[i].collider.CompareTag("Player"))
                {
                    count++;
                    float playerToEnemy = Vector3.Magnitude(transform.position - rayHits[i].transform.position);
                    if (min_dist > playerToEnemy)
                    {
                        min_dist = playerToEnemy;
                        target = rayHits[i].collider.transform;
                    }

                }

            }

        }
    }
    [PunRPC]
    void Attack()
    {
        
        if (monsterType == Type.slime)
        {
            if (Vector3.Distance(transform.position, target.position) <= 1.5f && !isAttack)
            {
                if (target.CompareTag("Player"))
                {
                    isAttack = true;
                    agent.isStopped = true;
                    anim.transform.forward = target.position - transform.position;
                    anim.SetTrigger("doAttack");
                    Invoke("AttackEnd", 1.2f);
                }
            }
        }
        if (monsterType == Type.demon)
        {
            if (Vector3.Distance(transform.position, target.position) <= 2.5f && !isAttack)
            {
                if (target.CompareTag("Player"))
                {
                    isAttack = true;
                    agent.isStopped = true;
                    anim.transform.forward = target.position - transform.position;
                    anim.SetTrigger("doAttack");
                    Invoke("AttackEnd", 2.5f);
                }
            }
        }
        if (monsterType == Type.golem)
        {
            if (Vector3.Distance(transform.position, target.position) <= 2.5f && !isAttack)
            {
                if (target.CompareTag("Player"))
                {
                    isAttack = true;
                    agent.isStopped = true;
                    anim.transform.forward = target.position - transform.position;
                    sound_source.PlayOneShot(sound_golem_PunchAttack_clip);
                    anim.SetTrigger("doAttack");
                    Invoke("AttackEnd", 1.5f);
                }
            }
        }

    }

    // melee 공격 관련 ////////////////
    public void Attack_areaOn()
    {
        meleeArea.enabled = true;
        Invoke("Attack_areaOff", 0.2f);

    }
    public void Attack_areaOff()
    {
        meleeArea.enabled = false;
    }
    public void AttackEnd()
    {
        isChase = true;
        isAttack = false;
        agent.isStopped = false;
    }
    public void Sound_melee_slash()
    {
        sound_melee_attack.Play();
    }
    ///////////////////////////////////
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player_Sword"))
        {
            if (other.transform.parent.GetComponent<Player_Control>().isSkill)
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk*1.5f, 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }
            else
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk , 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }
            Last_Hiter = other.transform.parent.GetComponent<Player_Control>();
            
        }


    }
    void Skill1()
    {
        isSkill = true;
        if(monsterType == Type.golem)
        {
            agent.speed = 30f;
        }
        else if(monsterType ==Type.demon)
            agent.speed = 50f;
        
    }
    public void FallLandNow()
    {
        if (monsterType == Type.demon)
        {
            Camera_Move.Instance.ShakeCamera(5f, 0.75f);
            shockWave.Play();
        }
        else if(monsterType == Type.golem)
        {
            Camera_Move.Instance.ShakeCamera(3f, 0.6f);
        }

        agent.velocity = Vector3.zero;
        skillCol.enabled = true;
        agent.isStopped = true;
        Invoke("skillEnable_false", 0.4f);
    }
    void skillEnable_false()
    {
        skillCol.enabled = false;
        
    }
    public void SkillOut()
    {
        agent.isStopped = false;
        if (monsterType == Type.golem)
        {
            agent.speed = 3f;
        }
        else if (monsterType == Type.demon)
        {
            agent.speed = 2f;
        }
        isSkill = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (Last_Hiter)
            {
                if (Last_Hiter.isDeath)
                {
                    Last_Hiter = null;
                    ChaseObject(InitPos);
                }
            }
            if (monsterType == Type.demon)
            {
                curSkillAmount += Time.deltaTime*5f;
                if (curSkillAmount>=maxSkillAmount && !isSkill && !isAttack && target)
                {
                    curSkillAmount = 0f;
                    
                    Skill1();
                    Invoke("SkillOut", 3f);
                    anim.SetTrigger("doSkill");
                    

                }
            }
            else if (monsterType == Type.golem)
            {
                curSkillAmount += Time.deltaTime * 8f;
                if (curSkillAmount >= maxSkillAmount && !isSkill && !isAttack && target)
                {
                    curSkillAmount = 0f;

                    Skill1();
                    Invoke("SkillOut", 2.5f);
                    anim.SetTrigger("doSkill");


                }
            }

            if (PV.IsMine)
            {
                if (Last_Hiter)
                    target = Last_Hiter.transform;
                else
                    target = null;
                /*if (Vector3.Distance(transform.position, target.position) <= 0.5f && !isChase)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;

                }*/
                if (!target)
                {
                    if(Vector3.Distance(transform.position,InitPos) <= 0.5f)
                    {
                        Debug.Log(Vector3.Distance(transform.position, InitPos));
                        isChase = false;
                        agent.isStopped = true;
                    }
                    ChaseObject(InitPos);
                }

                if (isChase)
                {

                    
                    //PV.RPC("ChaseObject", RpcTarget.Others,target.position);
                    if(target)
                        ChaseObject(target.position);
                    if(!isSkill && !isAttack)
                        Attack();
                }
                else
                {
                    agent.isStopped = true;
                }

            }
            else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else
            {
                transform.position = Vector3.Lerp(transform.position, curPos, Time.fixedDeltaTime * 20f);
                transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.fixedDeltaTime * 20f);
            }
        }
        else
        {
            agent.isStopped = true;
        }


    }
/*    IEnumerator StepShake()
    {
        while (true)
        {
            if (!isSkill && !isAttack && agent.isStopped==false)
                Camera_Move.Instance.ShakeCamera(3f, 0.05f);
            yield return new WaitForSeconds(0.7f);
        }
    }*/
    public void Dead()
    {
        int DropPercent= Random.Range(0,100);
        if (5 < DropPercent && DropPercent < 35)
        {
            PhotonNetwork.Instantiate("Item_potion", transform.position, transform.rotation);
        }
        else if (36 < DropPercent && DropPercent < 90)
        {
            PhotonNetwork.Instantiate("Item_Forest_Spirit", transform.position, transform.rotation);
        }

            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
    }
    void Dead_RPC()
    {
        
    }
    [PunRPC]
    void ChaseObject(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

   

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHP);
            stream.SendNext(maxHP);
            stream.SendNext(isChase);
            stream.SendNext(curSkillAmount);
            stream.SendNext(maxSkillAmount);
            stream.SendNext(golem_Index);
            //stream.SendNext(mySet);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            maxHP = (float)stream.ReceiveNext();

            isChase = (bool)stream.ReceiveNext();
            curSkillAmount = (float)stream.ReceiveNext();
            maxSkillAmount = (float)stream.ReceiveNext();
            golem_Index = (int)stream.ReceiveNext();
            //mySet = (Transform)stream.ReceiveNext();
        }
    }
}
