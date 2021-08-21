using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Custonmization : MonoBehaviourPunCallbacks
{

    public GameObject MalePrefab;   //남자 모델
    public GameObject FemalePrefab; //여자 모델

    public Transform Male_Hairs;  //남자 헤어스타일
    public Transform Female_Hairs;  //여자 헤어스타일
    public Transform Beards; //턱수염
    public Material[] Skins;    //피부색 머티리얼

    public GameObject BeardPointer; //여자일때 수염 비활성화하기위해

    public TextMeshProUGUI HairText;
    public TextMeshProUGUI BeardText;
    public TextMeshProUGUI SkinText;

    //머티리얼(피부색, 머리카락색) 배열   //함수로 num 인수로 받고 한칸씩(모든 머리카락, 수염, 피부 배열 foreach) 머티리얼 배열에서 할당
    //활성화한 다음 머티리얼 부여

    //public int[] TempCustomDetail;  //{머리, 턱수염, 피부} // 결정하기 전
    public int[] CustomDetail = { 1, 0, 0, 0 };  //{남녀, 머리, 턱수염, 피부} // 결정한 후에 저장

    bool Gender = true; //성별 : 남 - true / 여 - false

    private void OnEnable()
    {


        if (CustomDetail[0] == 1)    //남자일 때
        {
            Gender = true;
            BeardPointer.SetActive(true);
            FemalePrefab.SetActive(false);
            MalePrefab.SetActive(true);

            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(false);  //남자 머리
            male_hairNum = CustomDetail[1];
            Male_Hairs.GetChild(male_hairNum).gameObject.SetActive(true);

            Beards.GetChild(beardNum).gameObject.SetActive(false);  //남자 수염
            beardNum = CustomDetail[2];
            Beards.GetChild(beardNum).gameObject.SetActive(true);

            male_skinNum = CustomDetail[3];
            foreach (Transform hair in Male_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];   //남자 머리
            }
            foreach (Transform beard in Beards)
            {
                beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];  //남자 수염
            }
            MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum];   //남자 피부
        }
        else                        //여자일 때
        {
            Gender = false;
            MalePrefab.SetActive(false);
            FemalePrefab.SetActive(true);

            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);  //여자 머리
            female_hairNum = CustomDetail[1];
            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(true);

            female_skinNum = CustomDetail[3];
            foreach (Transform hair in Female_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum];   //여자 머리
            }
            FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum];   //여자 피부
        }

        HairText.text = "Hair" + (CustomDetail[1] + 1);
        BeardText.text = "Beard" + (CustomDetail[2] + 1);
        SkinText.text = "Skin" + (CustomDetail[3] + 1);




    }

    int male_hairNum = 0;
    int female_hairNum = 0;

    public void NextHair()
    {
        if (Gender) //남자일 때
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
        else //여자일 때
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
        if (Gender) //남자일 때
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
        else //여자일 때
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
        if (Gender) //남자일 때
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
        if (Gender) //남자일 때
        {
            if (male_skinNum >= Skins.Length - 1)
            {
                return;
            }
            else
            {
                //Male_Hairs.GetChild(male_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];   //머리
                //Beards.GetChild(beardNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];  //수염

                foreach (Transform hair in Male_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];   //머리
                }
                foreach (Transform beard in Beards)
                {
                    beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum + 1];  //수염
                }
                MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum + 1];   //피부

                male_skinNum++;
                SkinText.text = "Skin" + (male_skinNum + 1);
            }
        }
        else        //여자일 때
        {
            if (female_skinNum >= Skins.Length - 1)
            {
                return;
            }
            else
            {
                //Female_Hairs.GetChild(female_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum + 1];   //머리

                foreach (Transform hair in Female_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum + 1];   //머리
                }
                FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum + 1];   //피부

                female_skinNum++;
                SkinText.text = "Skin" + (female_skinNum + 1);
            }
        }
    }

    public void PreviousSkin()
    {
        if (Gender) //남자일 때
        {
            if (male_skinNum < 1)
            {
                return;
            }
            else
            {
                //Male_Hairs.GetChild(male_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //머리
                //Beards.GetChild(beardNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //수염

                foreach (Transform hair in Male_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //머리
                }
                foreach (Transform beard in Beards)
                {
                    beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum - 1];  //수염
                }
                MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum - 1];   //피부

                male_skinNum--;
                SkinText.text = "Skin" + (male_skinNum + 1);
            }
        }
        else        //여자일 때
        {
            if (female_skinNum < 1)
            {
                return;
            }
            else
            {
                //Female_Hairs.GetChild(female_hairNum).GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum - 1];  //머리

                foreach (Transform hair in Female_Hairs)
                {
                    hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum - 1];  //머리
                }
                FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum - 1];   //피부

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
        if (Gender) //남자일 때
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
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];   //머리 색
            }
            foreach (Transform beard in Beards)
            {
                beard.GetChild(0).GetComponent<MeshRenderer>().material = Skins[male_skinNum];  //수염 색
            }
            MalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[male_skinNum];   //피부 색
            SkinText.text = "Skin" + (male_skinNum + 1);
        }
        else        //여자일 때
        {
            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(false);
            female_hairNum = Random.Range(0, Female_Hairs.childCount);

            Female_Hairs.GetChild(female_hairNum).gameObject.SetActive(true);
            HairText.text = "Hair" + (female_hairNum + 1);

            //////////////////

            female_skinNum = Random.Range(0, Skins.Length);

            foreach (Transform hair in Female_Hairs)
            {
                hair.GetChild(0).GetComponent<MeshRenderer>().material = Skins[female_skinNum];   //머리 색
            }

            FemalePrefab.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Skins[female_skinNum];   //피부 색
            SkinText.text = "Skin" + (female_skinNum + 1);
        }
    }
    bool saveCheck = false;
    public void Save()
    {
        if (Gender) //남자
        {
            CustomDetail[0] = 1;
            CustomDetail[1] = male_hairNum;
            CustomDetail[2] = beardNum;
            CustomDetail[3] = male_skinNum;
        }
        else        //여자
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
