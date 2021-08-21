using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Custonmization : MonoBehaviourPunCallbacks
{

    public GameObject MalePrefab;   //���� ��
    public GameObject FemalePrefab; //���� ��

    public Transform Male_Hairs;  //���� ��Ÿ��
    public Transform Female_Hairs;  //���� ��Ÿ��
    public Transform Beards; //�μ���
    public Material[] Skins;    //�Ǻλ� ��Ƽ����

    public GameObject BeardPointer; //�����϶� ���� ��Ȱ��ȭ�ϱ�����

    public TextMeshProUGUI HairText;
    public TextMeshProUGUI BeardText;
    public TextMeshProUGUI SkinText;

    //��Ƽ����(�Ǻλ�, �Ӹ�ī����) �迭   //�Լ��� num �μ��� �ް� ��ĭ��(��� �Ӹ�ī��, ����, �Ǻ� �迭 foreach) ��Ƽ���� �迭���� �Ҵ�
    //Ȱ��ȭ�� ���� ��Ƽ���� �ο�

    //public int[] TempCustomDetail;  //{�Ӹ�, �μ���, �Ǻ�} // �����ϱ� ��
    public int[] CustomDetail = { 1, 0, 0, 0 };  //{����, �Ӹ�, �μ���, �Ǻ�} // ������ �Ŀ� ����

    bool Gender = true; //���� : �� - true / �� - false

    private void OnEnable()
    {


        if (CustomDetail[0] == 1)    //������ ��
        {
            Gender = true;
            BeardPointer.SetActive(true);
            FemalePrefab.SetActive(false);
            MalePrefab.SetActive(true);

            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(false);  //���� �Ӹ�
            male_hairNum = CustomDetail[1];
            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(true);

            Beards.GetChild(beardNum).gameObject.SetActive(false);  //���� ����
            beardNum = CustomDetail[2];
            Beards.GetChild(beardNum).gameObject.SetActive(true);

            male_skinNum = CustomDetail[3];
            foreach (Transform hair in Male_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];   //���� �Ӹ�
            }
            foreach (Transform beard in Beards)
            {
                beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];  //���� ����
            }
            MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum];   //���� �Ǻ�
        }
        else                        //������ ��
        {
            Gender = false;
            MalePrefab.SetActive(false);
            FemalePrefab.SetActive(true);

            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);  //���� �Ӹ�
            female_hairNum = CustomDetail[1];
            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(true);

            female_skinNum = CustomDetail[3];
            foreach (Transform hair in Female_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum];   //���� �Ӹ�
            }
            FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum];   //���� �Ǻ�
        }

        HairText.text = "Hair" + (CustomDetail[1] + 1);
        BeardText.text = "Beard" + (CustomDetail[2] + 1);
        SkinText.text = "Skin" + (CustomDetail[3] + 1);




    }

    int male_hairNum = 0;
    int female_hairNum = 0;

    public void NextHair()
    {
        if (Gender) //������ ��
        {
            if(male_hairNum >= Male_Hairs.childCount - 1)
            {
                return;
            }
            else
            {
                Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(false);
                Male_Hairs.GetChild(male_hairNum +1 ).gameObject.SetActive(true);
                male_hairNum++;
                HairText.text = "Hair" + (male_hairNum + 1);
            }
            
        }
        else //������ ��
        {
            if (female_hairNum >= Female_Hairs.childCount - 1)
            {
                return;
            }
            else
            {
                Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);
                Female_Hairs.GetChild(female_hairNum + 1).gameObject.SetActive(true);
                female_hairNum++;
                HairText.text = "Hair" + (female_hairNum + 1);
            }
        }
    }

    public void PreviousHair()
    {
        if (Gender) //������ ��
        {
            if (male_hairNum < 1)
            {
                return;
            }
            else
            {
                Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(false);
                Male_Hairs.GetChild(male_hairNum - 1).gameObject.SetActive(true);
                male_hairNum--;
                HairText.text = "Hair" + (male_hairNum + 1);
            }
            
        }
        else //������ ��
        {
            if (female_hairNum < 1)
            {
                return;
            }
            else
            {
                Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);
                Female_Hairs.GetChild(female_hairNum - 1).gameObject.SetActive(true);
                female_hairNum--;
                HairText.text = "Hair" + (female_hairNum + 1);
            }
        }
    }

    int beardNum = 0;

    public void NextBeard()
    {

        if (beardNum >= Beards.childCount - 1)
        {
            return;
        }
        else
        {
            Beards.GetChild(beardNum).gameObject.SetActive(false);
            Beards.GetChild(beardNum + 1).gameObject.SetActive(true);
            beardNum++;
            BeardText.text = "Beard" + (beardNum + 1);
        }

    }

    public void PreviousBeard()
    {
        if (Gender) //������ ��
        {
            if (beardNum < 1)
            {
                return;
            }
            else
            {
                Beards.GetChild(beardNum).gameObject.SetActive(false);
                Beards.GetChild(beardNum - 1).gameObject.SetActive(true);
                beardNum--;
                BeardText.text = "Beard" + (beardNum + 1);
            }

        }
    }

    int male_skinNum = 0;
    int female_skinNum = 0;

    public void NextSkin()
    {
        if (Gender) //������ ��
        {
            if (male_skinNum >= Skins.Length - 1)
            {
                return;
            }
            else
            {
                //Male_Hairs.GetChild(male_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];   //�Ӹ�
                //Beards.GetChild(beardNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];  //����

                foreach (Transform hair in Male_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];   //�Ӹ�
                }
                foreach (Transform beard in Beards)
                {
                    beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];  //����
                }
                MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum + 1];   //�Ǻ�

                male_skinNum++;
                SkinText.text = "Skin" + (male_skinNum + 1);
            }
        }
        else        //������ ��
        {
            if (female_skinNum >= Skins.Length - 1)
            {
                return;
            }
            else
            {
                //Female_Hairs.GetChild(female_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum + 1];   //�Ӹ�

                foreach (Transform hair in Female_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum + 1];   //�Ӹ�
                }
                FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum + 1];   //�Ǻ�

                female_skinNum++;
                SkinText.text = "Skin" + (female_skinNum + 1);
            }
        }
    }

    public void PreviousSkin()
    {
        if (Gender) //������ ��
        {
            if (male_skinNum < 1)
            {
                return;
            }
            else
            {
                //Male_Hairs.GetChild(male_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //�Ӹ�
                //Beards.GetChild(beardNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //����

                foreach (Transform hair in Male_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //�Ӹ�
                }
                foreach (Transform beard in Beards)
                {
                    beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //����
                }
                MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum - 1];   //�Ǻ�

                male_skinNum--;
                SkinText.text = "Skin" + (male_skinNum + 1);
            }
        }
        else        //������ ��
        {
            if (female_skinNum < 1)
            {
                return;
            }
            else
            {
                //Female_Hairs.GetChild(female_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum - 1];  //�Ӹ�

                foreach (Transform hair in Female_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum - 1];  //�Ӹ�
                }
                FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum - 1];   //�Ǻ�

                female_skinNum--;
                SkinText.text = "Skin" + (female_skinNum + 1);
            }
        }
    }

    public void ChooseMale()
    {
        HairText.text = "Hair" + (male_hairNum + 1);
        SkinText.text = "Skin" + (male_skinNum + 1);
        Gender = true;
        FemalePrefab.SetActive(false);
        MalePrefab.SetActive(true);
        BeardPointer.SetActive(true);
    }

    public void ChooseFemale()
    {
        HairText.text = "Hair" + (female_hairNum + 1);
        SkinText.text = "Skin" + (female_skinNum + 1);
        Gender = false;
        MalePrefab.SetActive(false);
        FemalePrefab.SetActive(true);
        BeardPointer.SetActive(false);
    }

    public void Randomize()
    {
        if (Gender) //������ ��
        {
            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(false);
            male_hairNum = Random.Range(0, Male_Hairs.childCount);

            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(true);
            HairText.text = "Hair" + (male_hairNum + 1);
            
            //////////////////

            Beards.GetChild(beardNum).gameObject.SetActive(false);
            beardNum = Random.Range(0, Beards.childCount);

            Beards.GetChild(beardNum).gameObject.SetActive(true);
            BeardText.text = "Beard" + (beardNum + 1);

            ////////////////

            male_skinNum = Random.Range(0, Skins.Length);

            foreach (Transform hair in Male_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];   //�Ӹ� ��
            }
            foreach (Transform beard in Beards)
            {
                beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];  //���� ��
            }
            MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum];   //�Ǻ� ��
            SkinText.text = "Skin" + (male_skinNum + 1);
        }
        else        //������ ��
        {
            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);
            female_hairNum = Random.Range(0, Female_Hairs.childCount);

            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(true);
            HairText.text = "Hair" + (female_hairNum + 1);

            //////////////////

            female_skinNum = Random.Range(0, Skins.Length);

            foreach (Transform hair in Female_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum];   //�Ӹ� ��
            }

            FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum];   //�Ǻ� ��
            SkinText.text = "Skin" + (female_skinNum + 1);
        }
    }
    bool saveCheck = false;
    public void Save()
    {
        if (Gender) //����
        {
            CustomDetail[0] = 1;
            CustomDetail[1] = male_hairNum;
            CustomDetail[2] = beardNum;
            CustomDetail[3] = male_skinNum;
        }
        else        //����
        {
            CustomDetail[0] = 0;
            CustomDetail[1] = female_hairNum;
            CustomDetail[2] = 0;
            CustomDetail[3] = female_skinNum;
        }
        saveCheck = true;
        string temp = "";
        for (int i = 0; i < CustomDetail.Length; i++)
        {
            temp += CustomDetail[i].ToString();
        }


        RoomManager.Instance.custom_string = temp;

    }
}
