using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 7.5f;
    public float rotationSpeed = 5.0f;

    float h;
    float v;
    Rigidbody body;
    Animator anim;

    bool isRun;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

    }
    private void Update()
    {
        AnimationManager();
        InputKey();

        if (h == 0 && v == 0)
        {
            isRun = false;
        }
        else
        {
            isRun = true;
        }

    }
    void InputKey()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
/*        if (Input.GetKeyDown(KeyCode.Space))
        {
            body.AddForce(Vector3.up * 22f, ForceMode.Impulse);
        }*/
    }
    void AnimationManager()
    {
        if (isRun)
        {
            anim.SetBool("isRun", true);
        }
        else
        {
            anim.SetBool("isRun", false);
        }
    }

    void PlayerMove()
    {
        //body.velocity = new Vector3(0f, body.velocity.y, 0f);
        body.angularVelocity = Vector3.zero;

        Vector3 direction= new Vector3(h, 0, v);

        transform.Translate(direction * moveSpeed * Time.deltaTime);

    }

    void FixedUpdate()
    {
        PlayerMove();
    }

}
