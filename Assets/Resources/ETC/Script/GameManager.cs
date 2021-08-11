using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public PhotonView PV;

    public float game_time;
    public float arena_time;
    public int ReadyCountCur;
    public int ReadyCountMax;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(arena_time);
            stream.SendNext(ReadyCountCur);
        }
        else
        {
            arena_time = (float)stream.ReceiveNext();
            ReadyCountCur = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        game_time = 0f;
        arena_time = 10f;
        Player[] players = PhotonNetwork.PlayerList;
        ReadyCountMax = players.Count();
    }

    // Update is called once per frame
    void Update()
    {
        game_time += Time.deltaTime;
        arena_time -= Time.deltaTime;
    }
}