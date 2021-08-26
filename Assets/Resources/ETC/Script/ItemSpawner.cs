using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    public Transform[] SpawnSpots;

    public PhotonView PV;
    public GameObject Twig;


    void Start()
    {
        StartCoroutine(SpawnLater());
    }
    IEnumerator SpawnLater()
    {
        yield return new WaitForSeconds(20f);
        if (PhotonNetwork.IsMasterClient)
        {

                StartCoroutine(Spawn());


        }
    }

    IEnumerator Spawn()
    {
        while (true)
        {

            GameObject Twig = PhotonNetwork.Instantiate("Item_twig", new Vector3(), Quaternion.identity);
            Twig.transform.parent = this.transform;

            yield return new WaitForSeconds(10f);
        }
    }

}
