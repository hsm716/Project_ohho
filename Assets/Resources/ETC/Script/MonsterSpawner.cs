using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Spawn()
    {
        while (true)
        {
            PhotonNetwork.Instantiate("Monster_Slime",new Vector3( Random.Range(transform.position.x, transform.position.x + 5f),transform.position.y, Random.Range(transform.position.x, transform.position.z + 5f)), transform.rotation);
            yield return new WaitForSeconds(10f);
        }
    }
}
