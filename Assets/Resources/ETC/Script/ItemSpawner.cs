using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawner : MonoBehaviour
{
    public Transform[] SpawnSpots;
    GameObject[] Twigs = { null , null , null , null };
    bool[] twiglive = { false, false, false, false };
    bool[] twiga = { false, false, false, false };

    public PhotonView PV;


    void Start()
    {
        for (int i = 0; i < SpawnSpots.Length; i++)
        {
            Twigs[i] = null;
            twiglive[i] = false;
            twiga[i] = false;
        }
        StartCoroutine(SpawnLater());
    }

    private void Update()
    {
        if (FirstSpawn)
        {
            for (int i = 0; i < SpawnSpots.Length; i++)
            {
                if (!twiglive[i])
                    StartCoroutine(Spawn(i));
                if (!Twigs[i] && !twiga[i])
                {
                    twiga[i] = true;
                    twiglive[i] = false;
                }
            }
        }

    }

    IEnumerator SpawnLater()
    {
        yield return new WaitForSeconds(20f);
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Spawn());
        }
    }
    bool FirstSpawn = false;
    IEnumerator Spawn()
    {
        for (int i = 0; i < SpawnSpots.Length; i++)
        {
            Twigs[i] = PhotonNetwork.Instantiate("Item_twig", SpawnSpots[i].position, Quaternion.identity);
            Twigs[i].transform.parent = this.transform;
            twiglive[i] = true;
            yield return new WaitForSeconds(10f);
        }
        FirstSpawn = true;
    }

    IEnumerator Spawn(int index)
    {
        twiglive[index] = true;
        yield return new WaitForSeconds(10f);
        Twigs[index] = PhotonNetwork.Instantiate("Item_twig", SpawnSpots[index].position, Quaternion.identity);
        Twigs[index].transform.parent = this.transform;
        twiga[index] = false;
    }
}
