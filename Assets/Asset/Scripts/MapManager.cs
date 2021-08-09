using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Transform Section_Offsets;
    public GameObject[] Sections;
    public GameObject[] Sections2;

    public Material mat;
    public float split = -1;

    void Start()
    {
        mat.SetFloat("_SplitValue", split);
        PV.RPC("Shuffle", RpcTarget.All);
    }

    [PunRPC]
    void Shuffle()
    {
        bool[] selected_state = { false, false, false, false, false, false };
        int count = 0;
        while (count < 6)
        {
            int rand_idx = Random.Range(0, 6);
            if (selected_state[rand_idx] == false)
            {
                selected_state[rand_idx] = true;
                Sections2[rand_idx] = Sections[count];
                count++;
            }
            else
            {
                continue;
            }
        }
        PV.RPC("Shuffle_Result", RpcTarget.All);
    }

    [PunRPC]
    void Shuffle_Result()
    {
        Sections = Sections2;
        StartCoroutine(Call_Section());
    }


    IEnumerator Call_Section()
    {
        int j = 0;
        foreach (Transform offset in Section_Offsets)
        {
            yield return new WaitForSeconds(3f);
            GameObject sections = Instantiate(Sections[j]);
            sections.transform.GetChild(0).localRotation = Quaternion.Euler(-90, 0, -60 * j);
            sections.transform.parent = offset;
            sections.transform.localPosition = new Vector3(0, 0, 0);
            sections.transform.localRotation = Quaternion.identity;
            j++;
        }
        StartCoroutine(Phase());
    }

    IEnumerator Phase()
    {

        while (split < 1)
        {
            split += Time.deltaTime * 0.25f;
            mat.SetFloat("_SplitValue", split);
            yield return 0;
        }
        
    }
}
