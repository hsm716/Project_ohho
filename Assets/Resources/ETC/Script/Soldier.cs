using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviourPunCallbacks,IPunObservable
{
    public Transform mySet;
    public int myNumber;
    public Transform target;
    public PhotonView PV;
    public Soldier_HpBar SHP_bar;

    public CapsuleCollider myCol;
    public BoxCollider meleeArea;

    private Player_Control myPlayer;
    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;
    public GameObject CM;
    private Camera characterCamera;

    Vector3 curPos;
    Quaternion curRot;
    public float atk;

    Vector3 mouseDir;


    public bool isAttack;
    public bool isFollow;
    public bool isChase;
    public bool isDead;

    public float curHP;
    public float maxHP;
    void Start()
    {
        anim = GetComponent<Animator>();
       rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        CM = GameObject.Find("Main Camera");
        characterCamera = CM.GetComponent<Camera>();
        curHP = 5000f;
        maxHP = 5000f;
        atk = 100f;
        isFollow=true;
        isAttack = false;
        isChase = false;

        FindMyPlayer();
    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                target = p.transform;
                mySet = target.GetChild(16).GetChild(myNumber);
                myPlayer = p.GetComponent<Player_Control>();
                break;
            }
        }
    }
    public void Hit(float atk_)
    {
        curHP -= atk_;

        if (curHP <= 0 && !isDead)
        {
            myCol.enabled = false;
            isDead = true;
            anim.SetTrigger("doDead");
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
        if (agent.isStopped==true)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
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
        float targetRadius = 10f;
        float targetRange = 3f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, Vector3.up, targetRange, LayerMask.GetMask("Enemy"));

        if (rayHits.Length >0 && !isAttack)
        {
            int count = 0;
            float min_dist = Mathf.Infinity;
            for (int i = 0; i < rayHits.Length; i++) 
            {
                if(rayHits[i].collider.transform.GetComponent<Soldier>().PV.Owner != PV.Owner && rayHits[i].collider.CompareTag("Soldier"))
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
            if (count == 0)
            {
                target = mySet.transform;
            }
        }

    }
    [PunRPC]
    void Attack()
    {
        
        isFollow = false;
        isAttack = true;
        anim.transform.forward = target.position - transform.position;
        agent.isStopped = true;
        anim.SetTrigger("doAttack");
        Invoke("AttackEnd",1.2f);
    }
    
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
        isFollow = true;
        isAttack = false;
        agent.isStopped = false;
    }
    void Turn()
    {
        Ray ray = characterCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult))
        {
            mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            anim.transform.forward = mouseDir;

        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Soldier_Attack") && other.transform.parent.GetComponent<Soldier>().PV.Owner != PV.Owner)
        {
            Hit(other.transform.parent.GetComponent<Soldier>().atk);
        }
        if(other.CompareTag("Player_Sword") && other.transform.parent.GetComponent<Player_Control>().PV.Owner != PV.Owner)
        {
            Hit(other.transform.parent.GetComponent<Player_Control>().atk);
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Targeting();
            if (PV.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    isChase = (isChase == false ? true : false);
                }
                if (!isAttack)
                    Turn();


                if (isChase)
                {

                    ChaseObject(target.position);

                    if (Vector3.Distance(transform.position, target.position) <= 1.5f && !isAttack)
                    {
                        if (!target.CompareTag("SetNumber") && target.GetComponent<Soldier>().PV.Owner != PV.Owner)
                            Attack();
                    }
                }

            }
            else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else
            {
                transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 20f);
                transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 20f);
            }




            if (!target)
            {
                FindMyPlayer();
                //FindMyLocation();
            }
            else if (target)
            {
                if (!isAttack)
                {

                    if (isFollow && !isChase)
                    {
                        agent.isStopped = false;
                        ChaseObject(mySet.position);
                    }
                    else if (isFollow && isChase)
                    {
                        agent.isStopped = false;
                        ChaseObject(target.position);
                    }

                    if (Vector3.Distance(transform.position, mySet.position) <= 0.5f && !isChase)
                    {
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;

                    }
                    if (Vector3.Distance(transform.position, target.position) <= 1.5f && isChase)
                    {
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                    }
                    //PV.RPC("ChaseObject", RpcTarget.All, mySet.position);
                }


            }
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
            stream.SendNext(myNumber);
            stream.SendNext(curHP);
            stream.SendNext(isChase);
            //stream.SendNext(mySet);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            myNumber = (int)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            isChase = (bool)stream.ReceiveNext();
            //mySet = (Transform)stream.ReceiveNext();
        }
    }
}
