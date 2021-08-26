using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public PhotonView PV;
    public Player_Control myPlayer;

    //public CinemachineVirtualCamera CVC;

    Vector3 curPos;
    public float screenSizeThickness;
    public float camSpeed;

    public Vector3 mousePos;

    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName ==PhotonNetwork.LocalPlayer.NickName)
            {

                myPlayer = p.GetComponent<Player_Control>();
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!myPlayer)
            FindMyPlayer();
        if (myPlayer)
        {
            mousePos = Input.mousePosition;

            curPos = this.transform.position;

            //up
            if (Input.mousePosition.y >= Screen.height / 2 + screenSizeThickness)
            {

                curPos.z += camSpeed * Time.deltaTime;
            }
            //down
            if (Input.mousePosition.y <= Screen.height / 2 - screenSizeThickness)
            {
                curPos.z -= camSpeed * Time.deltaTime;
            }
            //right
            if (Input.mousePosition.x >= Screen.width / 2 + screenSizeThickness)
            {
                curPos.x += camSpeed * Time.deltaTime;
            }
            //left
            if (Input.mousePosition.x <= Screen.width / 2 - screenSizeThickness)
            {
                curPos.x -= camSpeed * Time.deltaTime;
            }
            curPos.z = Mathf.Clamp(curPos.z, -4f + myPlayer.transform.position.z, myPlayer.transform.position.z + 4f);
            curPos.x = Mathf.Clamp(curPos.x, -4f + myPlayer.transform.position.x, myPlayer.transform.position.x + 4f);
            transform.position = curPos;
        }

    }
}
