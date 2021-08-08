using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Section_Offsets;
    public GameObject[] Sections;
    public GameObject[] Sections2;

    void Start()
    {
        Shuffle();
        
    }

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
        StartCoroutine(Call_Section());

    }


    IEnumerator Call_Section()
    {
        int j = 0;
        foreach (Transform offset in Section_Offsets)
        {
            yield return new WaitForSeconds(3f);
            GameObject sections2 = Instantiate(Sections2[j]);
            sections2.transform.GetChild(0).localRotation = Quaternion.Euler(-90, 0, -60 * j);
            sections2.transform.parent = offset;
            sections2.transform.localPosition = new Vector3(0, 0, 0);
            sections2.transform.localRotation = Quaternion.identity;
            j++;
        }
    }
}
