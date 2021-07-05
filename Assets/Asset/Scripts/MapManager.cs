using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Section_Offsets;
    public GameObject[] Sections;

    void Start()
    {
        StartCoroutine(Call_Section());

    }


    IEnumerator Call_Section()
    {
        int j = 0;
        foreach (Transform offset in Section_Offsets)
        {
            yield return new WaitForSeconds(3f);
            GameObject section = Instantiate(Sections[j]);
            section.transform.parent = offset;
            section.transform.localPosition = new Vector3(0, 0, 0);
            section.transform.localRotation = Quaternion.identity;
            j++;
        }
    }
}
