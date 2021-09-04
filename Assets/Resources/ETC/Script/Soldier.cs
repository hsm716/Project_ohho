using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviourPunCallbacks,IPunObservable
{
    public enum Type { melee,arrow};
    public Type soldierType;

    public GameObject arrow;

    public Transform mySet;

    public int mySetNumber;
    public int myNumber;
    public Transform target;
    public PhotonView PV;
    public Soldier_HpBar SHP_bar;

    public CapsuleCollider myCol;
    public BoxCollider meleeArea;
    public GameObject shotPoint;

    public AudioSource melee_slash;
    public AudioSource arrow_shoot;


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
    public AudioSource sound_source;
    public AudioClip sound_slash_hit;
    public AudioClip sound_arrow_hit;

    public bool isAttack;
    public bool isFollow;
    public bool isChase;
    public bool isStop;
    public bool isDead;

    public float curHP;
    public float maxHP;

    public ParticleSystem hitEffect_blood;
    void Start()
    {
        anim = GetComponent<Animator>();
       rgbd = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

       // CM = GameObject.Find("Main Camera");
        //characterCamera = CM.GetComponent<Camera>();
        curHP = 5000f;
        maxHP = 5000f;
        atk = 100f;
        isFollow=true;
        isAttack = false;
        isChase = false;

        FindMyPlayer();
    }

    // 나의 플레이어를 찾는 함수
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            // 플레이어와 병사에는 각각 PhotonView 라는 컴포넌트가 있는데, 해당 컴포넌트에는 오브젝트가 생성될 때 생성한 클라이언트의 USERNAME을 Owner로 설정 되어있음.
            // 플레이어의 owner 유저이름과 병사의 owner 유저이름을 비교하여 같은 경우, 자신의 주군이라고 판단
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                // 타겟을 플레이어로 설정
                target = p.transform;

                // 배치판에서 4행 10열로 병사를 배치했었는데,
                // mySetNumber 값은 행 값을 나타낸다. myNumber 열을 나타낸다.
                if(mySetNumber==0)
                    mySet = target.GetChild(6).GetChild(myNumber);
                else if(mySetNumber==1)
                    mySet = target.GetChild(7).GetChild(myNumber);
                else if (mySetNumber == 2)
                    mySet = target.GetChild(8).GetChild(myNumber);
                else if (mySetNumber == 3)
                    mySet = target.GetChild(9).GetChild(myNumber);



                myPlayer = p.GetComponent<Player_Control>();
                break;
            }
        }
    }
    public void Hit(float atk_,int type,int critical)
    {
        curHP -= atk_;
        bool isCritical = Random.Range(0, 100) < critical;
        if (isCritical)
            atk_ *= 1.5f;

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
            hitEffect_blood.Play();
            sound_source.PlayOneShot(sound_slash_hit);
        }
        else if (type == 1)
        {
            hitEffect_blood.Play();
            sound_source.PlayOneShot(sound_arrow_hit);
        }
        if (curHP <= 0 && !isDead)
        {
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
            Targeting();
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
                if( rayHits[i].collider.GetComponent<PhotonView>().Owner != PV.Owner && (rayHits[i].collider.CompareTag("Soldier") || rayHits[i].collider.CompareTag("Player")) )
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
    void Attack() // 공격 함수
    {
        // 병사 타입에 따라서, 근거리 원거리를 다르게 처리함.
        if (soldierType == Type.melee)
        {
            // 공격범위 안에 들며, 타겟이 되는 적들이 아군이 아니면 공격을 시전함.
            if (Vector3.Distance(transform.position, target.position) <= 1.5f && !isAttack)
            {
                if (!target.CompareTag("SetNumber") && target.GetComponent<PhotonView>().Owner != PV.Owner)
                {
                    isFollow = false;
                    isAttack = true;
                    anim.transform.forward = target.position - transform.position;
                    agent.isStopped = true;
                    anim.SetTrigger("doAttack");
                    Invoke("AttackEnd", 1.2f);
                }
            }
        }
        else if(soldierType == Type.arrow)
        {
            // 원거리 병사들은 타겟을 향해 비교적 긴 사거리를 가지며, 아군이 아니면 공격함.
            if (Vector3.Distance(transform.position, target.position) <= 14f && !isAttack)
            {
                if (!target.CompareTag("SetNumber") && target.GetComponent<PhotonView>().Owner != PV.Owner)
                {
                    isFollow = false;
                    isAttack = true;
                    anim.transform.forward = target.position - transform.position;
                    agent.isStopped = true;
                    anim.SetTrigger("doAttack");
                }
            }
        }
    }

    public void Shot()
    {
        PhotonNetwork.Instantiate("Soldier_Arrow", shotPoint.transform.position, shotPoint.transform.rotation);
        Sound_arrow_shoot();
        Invoke("AttackEnd", 1.0f);
    }
    public void Sound_arrow_shoot()
    {
        arrow_shoot.Play();
    }
    // melee 공격 관련 ////////////////
    public void Attack_areaOn()
    {
        meleeArea.enabled = true;
        Sound_melee_slash();
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
    public void Sound_melee_slash()
    {
        melee_slash.Play();
    }
    ///////////////////////////////////
    void Turn()
    {
        anim.transform.forward = target.position - transform.position;
       /* Ray ray = myPlayer.characterCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult))
        {
            mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
            anim.transform.forward = mouseDir;

        }*/
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Soldier_Attack") && other.transform.parent.GetComponent<Soldier>().PV.Owner.NickName != PV.Owner.NickName)
        {
            Hit(other.transform.parent.GetComponent<Soldier>().atk,0,0);
        }
        if(other.CompareTag("Player_Sword") && other.transform.parent.GetComponent<Player_Control>().PV.Owner != PV.Owner)
        {
            if (other.transform.parent.GetComponent<Player_Control>().isSkill)
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk * 1.5f, 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }
            else
            {
                Hit(other.transform.parent.GetComponent<Player_Control>().atk, 0, other.transform.parent.GetComponent<Player_Control>().curCritical);
            }
        }


    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {

            // 내 클라이언트의 명령만 들음
            if (PV.IsMine)
            {
                // T버튼 클릭시, 탐색 및 공격 명령
                if (Input.GetKeyDown(KeyCode.T))
                {
                    isChase = (isChase == false ? true : false);
                }
                // H버튼 클릭시, 정지 명령
                if (Input.GetKeyDown(KeyCode.H))
                {
                    isStop = (isStop == false ? true : false);
                }
                
                if (!isAttack)
                    Turn();

                
                // 탐색 및 공격 명령이 내려졌을 때, 타겟을 행해 쫓고 공격함.
                if (isChase)
                {

                    ChaseObject(target.position);
                    Attack();

                }

            }
            else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else
            {
                // 병사들의 위치 및 방향을 동기화함
                transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime*20f);
                transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime*20f);
            }

           



            // 타겟이 없으면, 내 플레이어를 찾아서 타겟에 넣어줌.
            if (!target)
            {
                FindMyPlayer();
                //FindMyLocation();
            }
            else if (target || mySet)
            {
                // 타겟이나 mySet이 존재하고, 공격중이 아닐때
                if (!isAttack)
                {
                    // 적 탐색 및 공격 명령이 없을때는, 배치된 위치를 쫓아 이동하게함 
                    if (isFollow && !isChase && !isStop)
                    {
                        agent.isStopped = false;
                        ChaseObject(mySet.position);
                    } // 적 탐색 및 공격 명령이 있을때는, target을 쫓아 이동함.
                    else if (isFollow && isChase && !isStop)
                    {
                        agent.isStopped = false;
                        ChaseObject(target.position);
                    }
                    // 정지 명령이 있을 때, 정지함
                    if (isStop)
                    {
                        agent.isStopped = true;
                    }
                    else
                    {
                        // 탐색 및 공격 명령이 없을 때, 배치된 위치에 가까워졌을때, 정지함.
                        if (Vector3.Distance(transform.position, mySet.position) <= 0.5f && !isChase)
                        {
                            agent.isStopped = true;
                            agent.velocity = Vector3.zero;

                        }// 탐색 및 공격 명령이 있을 때, 타겟에 공격할 사거리만큼 가까워졌을때, 정지함
                        else if (Vector3.Distance(transform.position, target.position) <= 1.5f && isChase)
                        {
                            agent.isStopped = true;
                            agent.velocity = Vector3.zero;
                        }
                    }
                    //PV.RPC("ChaseObject", RpcTarget.All, mySet.position);
                }


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
