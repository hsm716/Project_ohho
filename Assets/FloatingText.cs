using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviourPunCallbacks,IPunObservable
{
    public TextMesh tm;
    public float DestroyTime = 2f;
    public Vector3 Offset = new Vector3(0,4,0);
    public Vector3 RandomizeIntensity = new Vector3(0.5f,0,0);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tm.text);
        }
        else
        {
            tm.text = (string)stream.ReceiveNext();
        }
    }

    void Start()
    {
        Destroy(gameObject, DestroyTime);

        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x,RandomizeIntensity.x)
            , Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y)
            , Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += new Vector3(0,0.005f,0);
    }
}
