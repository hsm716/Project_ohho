using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public PhotonView PV;
    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material[] wallMat;
    public Camera camera;
    public LayerMask Mask;
    float fade = 0;
    private void Start()
    {
        if (!PV.IsMine)
            return;

        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            foreach (var mat in wallMat)
            {
                mat.SetVector(PosID, new Vector2(0.5f, 0.5f));
                mat.SetFloat(SizeID, 0);
            }

    }

    RaycastHit hit;

    void Update()
    {
        if (!PV.IsMine)
            return;

        if (camera)
        {
            var dir = camera.transform.position - transform.position;
            var ray = new Ray(transform.position, dir.normalized);

            if (Physics.Raycast(ray, out hit, 3000, Mask))
            {
                //wallMat = hit.transform.GetComponent<MeshRenderer>().material;

                if (fade < 1)
                {
                    fade += Time.deltaTime;
                    foreach (var mat in wallMat)
                    {
                        mat.SetFloat(SizeID, fade);
                    }

                }
            }
            else
            {
                if (fade > 0)
                {
                    fade -= Time.deltaTime;
                    foreach (var mat in wallMat)
                    {
                        mat.SetFloat(SizeID, fade);
                    }
                }
            }

            var view = camera.WorldToViewportPoint(transform.position);
            foreach (var mat in wallMat)
            {
                mat.SetVector(PosID, view);
            }
        }
        
        
    }
}
