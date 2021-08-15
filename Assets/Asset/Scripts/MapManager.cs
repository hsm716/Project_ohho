using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Transform Section_Offsets;
    public GameObject[] Sections;
    public Transform Bridges;
    //public GameObject[] Sections2;
    public int[] Order;

    public Material mat;
    public float split = -1;

    void Start()
    {

        mat.SetFloat("_SplitValue", split);
        if (PhotonNetwork.IsMasterClient)
            Shuffle();

        StartCoroutine(Call_Section());
        StartCoroutine(CallBridges());
        //PV.RPC("Shuffle", RpcTarget.All);
    }

    void Shuffle()
    {
        bool[] selected_state = new bool[] { false, false, false, false, false, false };
        int count = 0;
        while (count < 6)
        {

            int rand_idx = Random.Range(0, 6);
            if (selected_state[rand_idx] == false)
            {
                selected_state[rand_idx] = true;
                Order[count] = rand_idx;
                count++;
            }
            else
            {
                continue;
            }
        }

        if(Order[0] == 4 || Order[1] == 4 || Order[5] == 4)
        {
            PV.RPC("Shuffle_Result", RpcTarget.All, Order);
        }
        else
        {
            Shuffle();
        }

        //Shuffle_Result();


    }

    [PunRPC]
    void Shuffle_Result(int[] order)
    {
        Order = order;
    }


    IEnumerator Call_Section()
    {
        int j = 0;
        foreach (Transform offset in Section_Offsets)
        {
            yield return new WaitForSeconds(3f);
            GameObject sections = Instantiate(Sections[Order[j]]);
            sections.transform.GetChild(0).localRotation = Quaternion.Euler(-90, 0, -60 * j);
            sections.transform.parent = offset;
            sections.transform.localPosition = new Vector3(0, 0, 0);
            sections.transform.localRotation = Quaternion.identity;
            j++;
        }
        //StartCoroutine(Phase());

    }

    IEnumerator CallBridges()
    {
        yield return new WaitForSeconds(3f);
        foreach (Transform bridge in Bridges)
        {
            yield return new WaitForSeconds(1.5f);
            bridge.gameObject.SetActive(true);
        }
    }

    /*
    IEnumerator Phase()
    {

        while (split < 1)
        {
            split += Time.deltaTime * 0.25f;
            mat.SetFloat("_SplitValue", split);
            yield return 0;
        }

    }
    */


}
