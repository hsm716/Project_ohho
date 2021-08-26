using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    public GameObject continueButton;

    public void FadeIn()
    {
        StartCoroutine(FadeIn_());
    }

    public void FadeOut()
    {
        continueButton.SetActive(false);
        StartCoroutine(FadeOut_());
    }

    IEnumerator FadeIn_()
    {
        float t = 1f;

        while(t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        yield return new WaitForSeconds(1.5f);

        RewardManager.Instance.RewardPlace();
    }

    IEnumerator FadeOut_()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        //RewardManager.Instance.DialogueDisplay();
        yield return new WaitForSeconds(0.1f);
        RewardManager.Instance.ToThrone();
    }
}
