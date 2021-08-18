using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Material : MonoBehaviour
{

    public enum Type { twig, forest_spirit, block };
    public Type type;
    public int value;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up);
    }

}
