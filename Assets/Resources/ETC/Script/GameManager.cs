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

    public int ArenaRank;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(arena_time);
            stream.SendNext(ReadyCountCur);
            stream.SendNext(ArenaRank);
        }
        else
        {
            arena_time = (float)stream.ReceiveNext();
            ReadyCountCur = (int)stream.ReceiveNext();
            ArenaRank = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        game_time = 0f;
        arena_time = 300f;
        Player[] players = PhotonNetwork.PlayerList;
        ReadyCountMax = players.Count();
        ArenaRank = players.Count();
    }

    // Update is called once per frame
    void Update()
    {
        game_time += Time.deltaTime;
        arena_time -= Time.deltaTime;
    }
}
