using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCamera : MonoBehaviour
{

    bool move = false;
    float degree_i = 60f;
    bool rot = false;
    int count = 0;
    void Start()
    {

    }
    IEnumerator cameraMoving()
    {

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 5; i++)
        {
            move = false;
            yield return new WaitForSeconds(1.25f);
            count += 1;
            move = true;
            yield return new WaitForSeconds(0.75f);
        }
        yield return new WaitForSeconds(1f);
        move = false;
        rot = true;
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        transform.position = new Vector3(0f, 200f, 0f);
    }
    void DestroyCamera()
    {
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.doStartCamera)
        {
            GameManager.Instance.doStartCamera = false;
            Invoke("DestroyCamera", 20f);
            StartCoroutine(cameraMoving());
        }



        if (move == true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0f, 60f * count, 0f)), 0.1f);

        }
        if (rot == true)
        {
            transform.Rotate(0f, 0f, 0.2f);
            transform.position -= new Vector3(0f, 0.3f, 0f);
        }
    }
}
