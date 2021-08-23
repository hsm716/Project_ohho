using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
{
    static public GameManager Instance;
    public PhotonView PV;

    public bool isActive;
    public float game_time;
    public float arena_time;
    public int ReadyCountCur;
    public int ReadyCountMax;

    public int arenaRank;
    public int areanaCount;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(arena_time);
            stream.SendNext(ReadyCountCur);
            stream.SendNext(arenaRank);
            stream.SendNext(areanaCount);
        }
        else
        {
            arena_time = (float)stream.ReceiveNext();
            ReadyCountCur = (int)stream.ReceiveNext();
            arenaRank = (int)stream.ReceiveNext();
            areanaCount = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        game_time = 0f;
        arena_time = 300f;
        Player[] players = PhotonNetwork.PlayerList;
        ReadyCountMax = players.Count();
        arenaRank = ReadyCountMax;
        areanaCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        game_time += Time.deltaTime;
        arena_time -= Time.deltaTime;
    }
}
