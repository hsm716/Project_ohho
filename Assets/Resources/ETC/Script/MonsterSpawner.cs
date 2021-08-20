using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public PhotonView PV;
    public int poolNum;
    public Queue<GameObject> SlimePool = new Queue<GameObject>();
    public int curCount_Slime;
    int maxCount_Slime;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            maxCount_Slime = 10;
            StartCoroutine(Spawn());
            //StartCoroutine(Spawn_demon());
            //StartCoroutine(Spawn_golem());
        }
    }

    // Update is called once per frame
    void Update()
    {
        curCount_Slime = SlimePool.Count;
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
            if (SlimePool.Count <= maxCount_Slime)
            {
                GameObject Slime = PhotonNetwork.Instantiate("Monster_Slime", new Vector3(Random.Range(transform.position.x, transform.position.x + 6f), transform.position.y, Random.Range(transform.position.z, transform.position.z + 6f)), Quaternion.identity);
                SlimePool.Enqueue(Slime);
            }
            
            yield return new WaitForSeconds(5f);
        }
    }
    IEnumerator Spawn_demon()
    {
        while (true)
        {
            PhotonNetwork.Instantiate("Monster_Demon_", new Vector3(Random.Range(transform.position.x, transform.localPosition.x + 0.2f), transform.position.y, Random.Range(transform.position.z, transform.localPosition.z + 0.2f)), Quaternion.identity);
            

            yield return new WaitForSeconds(500f);
        }
    }
    IEnumerator Spawn_golem()
    {
        while (true)
        {
            GameObject golem =  PhotonNetwork.Instantiate("Monster_Golem", new Vector3(Random.Range(transform.position.x, transform.position.x + 0.2f), transform.position.y, Random.Range(transform.position.z, transform.position.z + 0.2f)), Quaternion.identity);
            golem.transform.GetChild(0).GetComponent<Monster>().golem_Index = Random.Range(0, 3);
            yield return new WaitForSeconds(20f);
        }
        
    }
}
