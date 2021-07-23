using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviourPunCallbacks
{
    public Transform mySet;
    public int myNumber;
    public Transform target;
    public PhotonView PV;
    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;
    public GameObject CM;
    private Camera characterCamera;

    Vector3 mouseDir;

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
                break;
            }
        }
    }
    public void Hit(float atk_)
    {
        curHP -= atk_;

        if (curHP <= 0)
        {
            GameObject.Find("MainCanvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
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
        if(agent.isStopped==true)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
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
    // Update is called once per frame
    void Update()
    {
        rgbd.velocity = Vector3.zero;
        rgbd.angularVelocity = Vector3.zero;
        Turn();
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
                agent.isStopped = false;
                agent.SetDestination(mySet.position);
            }
        }
    }
}
