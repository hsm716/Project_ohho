using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviourPunCallbacks
{
    public Transform target;
    public PhotonView PV;
    Animator anim;
    NavMeshAgent agent;
    Rigidbody rgbd;
    void Start()
    {
        anim = GetComponent<Animator>();
       rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                target = p.transform;
                break;
            }
        }
    }
    private void FixedUpdate()
    {
        if(agent.velocity == Vector3.zero)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindMyPlayer();
        }
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) <= 4f)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }
}
