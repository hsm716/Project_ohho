using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInitialize : MonoBehaviour
{
    public GameObject Hexagon;
    public Transform[] Sections;

    void Start()
    {
        StartCoroutine(HexaCreate());
    }

    IEnumerator HexaCreate()
    {
        foreach (var section in Sections)
        {
            yield return new WaitForSeconds(1f);
            Instantiate(Hexagon, section);
        }
    }
}
