using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public PhotonView PV;
    public enum Type { slime, golem,demon};
    public Type spawnType;
    public int poolNum;
    //public Queue<GameObject> SlimePool = new Queue<GameObject>();
    public int curCount_Slime;
    int maxCount_Slime;
    void Start()
    {
        StartCoroutine(SpawnLater());
    }


    // 지연 스폰
    IEnumerator SpawnLater()
    {
        // 게임시작 애니매이션이 지난 후, 몬스터를 호출
        yield return new WaitForSeconds(20f);

        // 스폰 지연 후, 스폰 코루틴 함수를 호출
        // 방장 클라이언트에서만 네트워크 오브젝트인 몬스터들을 소환.
        if (PhotonNetwork.IsMasterClient)
        {
            // 슬라임 수를 제한함.
            maxCount_Slime = 7;
            if (spawnType == Type.demon)
            {
                StartCoroutine(Spawn_demon());
            }
            if (spawnType == Type.golem)
            {
                StartCoroutine(Spawn_golem());
            }
            if (spawnType == Type.slime)
            {
                StartCoroutine(Spawn());
            }

            //

        }
    }


    // Update is called once per frame
    void Update()
    {
        //curCount_Slime = SlimePool.Count;
    }
/*    public void InsertQue(GameObject slime)
    {
        SlimePool.Enqueue(slime);
    }
    public void DeQue(GameObject slimte)
    {
        SlimePool.Dequeue()
    }*/
    IEnumerator Spawn()
    {
        while (true)
        {
            if (curCount_Slime < maxCount_Slime)
            {
                GameObject Slime = PhotonNetwork.Instantiate("Monster_Slime", new Vector3(Random.Range(transform.position.x, transform.position.x + 6f), transform.position.y, Random.Range(transform.position.z, transform.position.z + 6f)), Quaternion.identity);
            Slime.transform.parent = this.transform;
                curCount_Slime += 1;
                //SlimePool.Enqueue(Slime);
                yield return new WaitForSeconds(7f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
            
            
        }
    }
    IEnumerator Spawn_demon()
    {
        while (true)
        {
            yield return new WaitForSeconds(27f);
            GameObject demon = PhotonNetwork.Instantiate("Monster_Demon_", transform.position, Quaternion.identity);
            demon.transform.parent = this.transform;
            

            yield return new WaitForSeconds(500f);
        }
    }
    IEnumerator Spawn_golem()
    {
        while (true)
        {
            yield return new WaitForSeconds(27f);
            GameObject golem =  PhotonNetwork.Instantiate("Monster_Golem", transform.position, Quaternion.identity);
            golem.transform.GetChild(0).GetComponent<Monster>().golem_Index = Random.Range(0, 3);
            golem.transform.parent = this.transform;
            yield return new WaitForSeconds(230f);
        }
        
    }
}
