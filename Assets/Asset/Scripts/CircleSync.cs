using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material wallMat;
    public Camera camera;
    public LayerMask Mask;
    float fade = 0;
    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        var dir = camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            if (fade < 1)
            {
                fade += Time.deltaTime;
                wallMat.SetFloat(SizeID, fade);
            }
        }
        else
        {
            if (fade > 0)
            {
                fade -= Time.deltaTime;
                wallMat.SetFloat(SizeID, fade);
            }
        }

        var view = camera.WorldToViewportPoint(transform.position);
        wallMat.SetVector(PosID, view);
    }
}
