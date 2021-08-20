using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBakerTest : MonoBehaviour
{
    public GameObject navMesh_parent;
    public NavMeshSurface[] navMeshSurface;
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartMesh", 27f);
    }

    void StartMesh()
    {
        navMesh_parent.transform.GetChild(0).GetComponent<NavMeshSurface>().BuildNavMesh();
        gm.arena_time = 300f;
        gm.game_time = 0f;
    }


}
