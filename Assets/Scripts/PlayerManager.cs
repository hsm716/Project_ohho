using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    public GameObject MC;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        RoomManager.Instance.playerCount++;
        MC = GameObject.Find("MainCanvas");

    }
    bool spawnCheck = true;
    void Update()
    {
        if(RoomManager.Instance.playerCount == 2 && spawnCheck)
        {
            spawnCheck = false;
            if (PV.IsMine)
            {

                CreateController();

            }
        }
    }

    void CreateController()
    {
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(-2,23.5f,-4.1f), Quaternion.identity);
        StartCoroutine(LateSpawn());
    }

    IEnumerator LateSpawn()
    {
        yield return new WaitForSeconds(0);
        PhotonNetwork.Instantiate("Player", new Vector3(-2, 23.5f, -4.1f), Quaternion.identity);
        MC.transform.GetChild(2).gameObject.SetActive(true);
    }
}
