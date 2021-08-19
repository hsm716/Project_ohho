using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowVFX : MonoBehaviour
{
    public GameObject SnowBall;
    public Transform SnowBallSpawnPoint;

    public GameObject[] WindMagicCircle;    //바람 마법진
    public ParticleSystem[] windmagic;    //바람 마법
    public ParticleSystem[] CircleCrash;    //마법진 파괴

    Material[] magicCircle = { null, null, null, null, null }; //마법진의 메터리얼
    Material[] windball = { null, null, null, null, null };   //토네이도볼 메터리얼

    public int[] Order;

    void Start()
    {
        int i = 0;
        foreach (var circle in WindMagicCircle)
        {
            magicCircle[i] = circle.GetComponent<MeshRenderer>().material;
            magicCircle[i].SetFloat("_RemovedSegment", 0);
            i++;
        }
        i = 0;
        foreach (var magicParticle in windmagic)
        {
            windball[i] = magicParticle.transform.GetChild(0).GetComponent<MeshRenderer>().material;
            windball[i].SetFloat("_Dissolve", 0);
            i++;
        }
        Shuffle();
        StartCoroutine(RandomMagic());
        StartCoroutine(SnowBallSpawn());
    }

    void Shuffle()
    {
        bool[] selected_state = new bool[] { false, false, false, false, false };
        int count = 0;
        while (count < 5)
        {

            int rand_idx = Random.Range(0, 5);
            if (selected_state[rand_idx] == false)
            {
                selected_state[rand_idx] = true;
                Order[count] = rand_idx;
                count++;
            }
            else
            {
                continue;
            }
        }
    }

    IEnumerator SnowBallSpawn()
    {
        float randterm = Random.Range(3, 6);
        yield return new WaitForSeconds(randterm);
        float randXposition = Random.Range(-3, 3);
        GameObject ball = Instantiate(SnowBall, SnowBallSpawnPoint.position + new Vector3(randXposition, 0, 0), Quaternion.identity);
        float randScale = Random.Range(8, 15);
        ball.transform.localScale = new Vector3(randScale, randScale, randScale);
        StartCoroutine(SnowBallSpawn());
    }

    IEnumerator RandomMagic()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(CircleReady(Order[i]));
        }
    }

    IEnumerator CircleReady(int num)
    {
        magicCircle[num].SetFloat("_RemovedSegment", 0);

        yield return new WaitForSeconds(5f);

        float term = 0;
        while (term <= 1)
        {
            magicCircle[num].SetFloat("_RemovedSegment", term);
            term += Time.deltaTime * 0.1f;
            yield return 0;
        }

        windmagic[num].Play();

        term = 0;
        while (term <= 1)
        {
            windball[num].SetFloat("_Dissolve", 0.5f + term * 0.2f);  //0.5 ~ 0.7
            term += Time.deltaTime * 0.3f;
            yield return 0;
        }
        yield return new WaitForSeconds(2f);

        while (term > 0.4f)
        {
            windball[num].SetFloat("_Dissolve", term * 0.7f);
            term -= Time.deltaTime * 0.3f;
            yield return 0;
        }
        CircleCrash[num].Play();
        StartCoroutine(CircleReady(num));
    }
}
