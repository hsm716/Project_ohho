using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    GameObject[] player_list;
    GameObject[] player_list1;
    int Player_num;

    bool isStop;
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            player_list = GameObject.FindGameObjectsWithTag("PlayerList");
            Player_num = player_list.Length;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1 && !isStop)
        {
            player_list1 = GameObject.FindGameObjectsWithTag("PlayerList");
        }
        
        if (player_list1.Length == Player_num && !isStop)
        {
            isStop = true;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {

        if(scene.buildIndex == 1)   //we're in the game scene
        {
            PhotonNetwork.Instantiate("Player_Check", Vector3.zero, Quaternion.identity);
            //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

}
