using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public PhotonView PV;

    public float time;
    public int ReadyCountCur;
    public int ReadyCountMax;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
            stream.SendNext(ReadyCountCur);
        }
        else
        {
            time = (float)stream.ReceiveNext();
            ReadyCountCur = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 5f;
        Player[] players = PhotonNetwork.PlayerList;
        ReadyCountMax = players.Count();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
    }
}
