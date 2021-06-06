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

    void Start()
    {
        body = GetComponent<Rigidbody>();

    }
    private void Update()
    {

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            body.AddForce(Vector3.up * 22f, ForceMode.Impulse);
        }

    }
    void FixedUpdate()
    {
        body.velocity = new Vector3(0f, body.velocity.y,0f);
        body.angularVelocity = Vector3.zero;
        


        
        Vector3 direction = new Vector3(h, 0, v);

/*        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (90.0f > angle && angle > -90.0f)
            {
                angle = angle * rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, angle);
            }
        }*/

        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

}
