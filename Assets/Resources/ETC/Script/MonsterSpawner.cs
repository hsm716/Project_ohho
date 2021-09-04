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


    // ���� ����
    IEnumerator SpawnLater()
    {
        // ���ӽ��� �ִϸ��̼��� ���� ��, ���͸� ȣ��
        yield return new WaitForSeconds(20f);

        // ���� ���� ��, ���� �ڷ�ƾ �Լ��� ȣ��
        // ���� Ŭ���̾�Ʈ������ ��Ʈ��ũ ������Ʈ�� ���͵��� ��ȯ.
        if (PhotonNetwork.IsMasterClient)
        {
            // ������ ���� ������.
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
