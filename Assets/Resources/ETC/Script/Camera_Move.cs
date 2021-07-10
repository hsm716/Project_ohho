using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour
{

    public GameObject player;
    [SerializeField]
    private float offsetX = 0f;
    [SerializeField]
    private float offsetY = 25f;
    [SerializeField]
    private float offsetZ = -35;
   // [SerializeField] private float followSpeed = 1f;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;
    private float currentCameraRotationY = 0;


    Animator animator;

    public Vector3 cameraPosition;
    Vector3 cameraRotation;

    bool intromove = false;
    bool b = false;
    bool a = false;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        /*if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9895833f) && !b)
        {
            animator.enabled = false;
            cameraPosition.x = player.transform.position.x + offsetX;
            cameraPosition.y = player.transform.position.y + offsetY;
            cameraPosition.z = player.transform.position.z + offsetZ;

            transform.position = Vector3.Lerp(transform.position, cameraPosition, 1f * Time.deltaTime);
            //왜안댐
            if (!a)
                StartCoroutine(Wait());
        }

        if (intromove)
        {
            cameraPosition.x = player.transform.position.x + offsetX;
            cameraPosition.y = player.transform.position.y + offsetY;
            cameraPosition.z = player.transform.position.z + offsetZ;

            transform.position = cameraPosition;

        }*/
        cameraPosition.x = player.transform.position.x + offsetX;
        cameraPosition.y = player.transform.position.y + offsetY;
        cameraPosition.z = player.transform.position.z + offsetZ;

        transform.position = cameraPosition;
        // transform.position = Vector3.Lerp(transform.position,cameraPosition,followSpeed*Time.deltaTime);
    }

    IEnumerator Wait()
    {
        a = true;
        yield return new WaitForSeconds(5f);
        b = true;
        intromove = true;
    }
    /*
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        player.GetComponent<Rigidbody>().MoveRotation(player.GetComponent<Rigidbody>().rotation * Quaternion.Euler(_characterRotationY));

    }
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _yRotation = Input.GetAxisRaw("Mouse X");

        float _cameraRotationY = _yRotation * lookSensitivity;
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationY += _cameraRotationY;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX,-cameraRotationLimit,cameraRotationLimit);
        currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraRotationLimit, cameraRotationLimit);
        transform.localEulerAngles = new Vector3(currentCameraRotationX, currentCameraRotationY, 0f);
    }*/
    // Start is called before the first frame update



}