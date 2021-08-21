using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public int playerCount = 0;

    public InputField IF;
    public string setValue;
    public int[] customPreset;

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
    public void Check()
    {
        setValue = IF.text;
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




    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {

        if(scene.buildIndex == 1)   //we're in the game scene
        {
            //PhotonNetwork.Instantiate("Player_Check", Vector3.zero, Quaternion.identity);

            
            PhotonView pv =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity,0).GetComponent<PhotonView>();
            pv.GetComponent<PlayerManager>().getValue = setValue;

        }
    }

}
