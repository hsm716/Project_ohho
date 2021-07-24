using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
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

    Player_Control myPlayer;
    public BoxCollider meleeArea;
    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;
    public GameObject CM;
    private Camera characterCamera;

    Vector3 curPos;
    Quaternion curRot;
    public float atk;

    Vector3 mouseDir;

    bool isFollow;
    bool isAttack;
    bool isComeback;

    public float curHP;
    public float maxHP;
    void Start()
    {
        anim = GetComponent<Animator>();
       rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        CM = GameObject.Find("Main Camera");
        characterCamera = CM.GetComponent<Camera>();
        curHP = 500f;
        maxHP = 500f;
        atk = 100f;
        isComeback = true;

    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                target = p.transform;
                myPlayer = p.GetComponent<Player_Control>();
                mySet = target.GetChild(16).GetChild(myNumber);
                break;
            }
        }
    }
    public void Hit(float atk_)
    {
        curHP -= atk_;

        if (curHP <= 0)
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
    void FindMyLocation()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                target = p.transform.GetChild(5).transform.GetChild(myNumber);
                break;
            }
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
        float targetRadius = 3f;
        float targetRange = 3f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, Vector3.up, targetRange, LayerMask.GetMask("Enemy"));

        if (rayHits.Length >0 && !isAttack)
        {
            int count = 0;
            for (int i = 0; i < rayHits.Length; i++) 
            {
                if(rayHits[i].collider.transform.GetComponent<Soldier>().PV.Owner != PV.Owner)
                {
                    count++;
                    target = rayHits[0].collider.transform;
                    ChaseObject(target.position);
                    Attack();
                }
                break;
            }
            if (count == 0)
            {
                isComeback = true;
            }
            else
            {
                isComeback = false;
            }
            
           
        }
    }
    [PunRPC]
    void Attack()
    {
        
        isFollow = false;
        isAttack = true;
        anim.transform.forward = target.position;
        agent.isStopped = true;
        anim.SetTrigger("doAttack");
        Invoke("AttackEnd",0.8f);
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

        if (PV.IsMine)
        {
            if(!isAttack)
                Turn();
           
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 20f);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 20f);
        }

        Targeting();

        if (target == null)
        {
            FindMyPlayer();
            //FindMyLocation();
        }
        if (target != null)
        {
            if (Vector3.Distance(transform.position, mySet.position) <= 0.1f)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

            }
            else
            {
                if (!isAttack)
                {
                    agent.isStopped = false;
                    target = mySet;
                    if (isComeback == true)
                    {
                        ChaseObject(target.position);
                    }
                    //PV.RPC("ChaseObject", RpcTarget.All, mySet.position);
                }
            }
        }



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
            //stream.SendNext(mySet);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            myNumber = (int)stream.ReceiveNext();
            curHP = (float)stream.ReceiveNext();
            //mySet = (Transform)stream.ReceiveNext();
        }
    }
}
