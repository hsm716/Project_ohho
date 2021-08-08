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
    public enum Type { slime};
    public Type monsterType;


    public PhotonView PV;
    public Soldier_HpBar SHP_bar;

    public CapsuleCollider myCol;
    public BoxCollider meleeArea;

    public AudioSource sound_melee_attack;

    public Player_Control Last_Hiter;

    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;

    Vector3 curPos;
    Quaternion curRot;
    
    public float atk;

    Transform target;

    Vector3 Init_Pos;

    public bool isAttack;

    public bool isChase;
    public bool isStop;
    public bool isDead;

    public float curHP;
    public float maxHP;

    public AudioSource sound_source;
    public AudioClip sound_hit;
    void Start()
    {
        anim = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        Init_Pos = transform.position;

        curHP = 1000f;
        maxHP = 1000f;
        atk = 10f;
        isAttack = false;
        isChase = false;
        isDead = false;
        target = null;
    }
    public void Hit(float atk_)
    {
        curHP -= atk_;
        sound_source.PlayOneShot(sound_hit);
        isChase = true;
        if (curHP <= 0 && !isDead)
        {
            if (Last_Hiter.GetComponent<QuestData>().questIsActive[0])
            {
                Last_Hiter.GetComponent<QuestData>().slimeKillCount++;
                //Last_Hiter.GetComponent<QuestData>().Quest();
            }
            Last_Hiter.GetComponent<Player_Control>().curEXP += 20f;
            myCol.enabled = false;
            isDead = true;
            anim.SetTrigger("doDead");
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        if (!isDead)
        {
            FreezeVelocity();
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
        float targetRange = 3f;

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
                    anim.transform.forward = target.position - transform.position;
                    agent.isStopped = true;
                    anim.SetTrigger("doAttack");
                    Invoke("AttackEnd", 1.2f);
                }
            }
        }

    }

    // melee ���� ���� ////////////////
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
            Last_Hiter = other.transform.parent.GetComponent<Player_Control>();
            Hit(other.transform.parent.GetComponent<Player_Control>().atk);
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {

            if (PV.IsMine)
            {
                target = Last_Hiter.transform;
                /*if (Vector3.Distance(transform.position, target.position) <= 0.5f && !isChase)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;

                }*/
                if (isChase)
                {
                    
                    ChaseObject(target.position);
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
    public void Dead()
    {
        PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
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
            stream.SendNext(isChase);
            //stream.SendNext(mySet);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            isChase = (bool)stream.ReceiveNext();
            //mySet = (Transform)stream.ReceiveNext();
        }
    }
}
