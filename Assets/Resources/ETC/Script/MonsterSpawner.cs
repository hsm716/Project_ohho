using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public PhotonView PV;
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Spawn());
            //StartCoroutine(Spawn_demon());
            //StartCoroutine(Spawn_golem());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Spawn()
    {
        while (true)
        {
            PhotonNetwork.Instantiate("Monster_Slime",new Vector3( Random.Range(transform.position.x, transform.position.x + 0.2f),transform.position.y, Random.Range(transform.position.z, transform.position.z + 0.2f)), Quaternion.identity);
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
            yield return new WaitForSeconds(10f);
        }
        
    }
}
