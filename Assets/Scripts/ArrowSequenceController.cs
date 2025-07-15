using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSequenceController : MonoBehaviour
{
    public GameObject leftArrow;
    public GameObject middleArrow;
    public GameObject rightArrow;

    public int repeatsPerPhase = 5;

    private void Start()
    {
        StartCoroutine(RunAllPhases());
    }

    IEnumerator RunAllPhases()
    {
        yield return RunPhase(() => TogglePair(leftArrow, middleArrow), "Left <-> Middle");
        yield return RunPhase(() => TogglePair(rightArrow, middleArrow), "Right <-> Middle");
        yield return RunPhase(() => TogglePair(leftArrow, rightArrow), "Left <-> Right");
        Debug.Log("All phases finished.");
    }

    IEnumerator RunPhase(System.Action toggleAction, string phaseName)
    {
        Debug.Log("Starting Phase: " + phaseName);
        for (int repeat = 0; repeat < repeatsPerPhase; repeat++)
        {
            for (int i = 0; i < 60; i++)
            {
                toggleAction.Invoke();
                yield return new WaitForSeconds(1f);
            }
        }
        Debug.Log("Finished Phase: " + phaseName);
    }

    void TogglePair(GameObject a, GameObject b)
    {
        bool aActive = a.activeSelf;
        a.SetActive(!aActive);
        b.SetActive(aActive);
    }
}
