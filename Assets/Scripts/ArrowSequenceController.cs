using TMPro;
using System.Collections;
using UnityEngine;

public class ArrowSequenceController : MonoBehaviour
{
    public GameObject leftArrow;
    public GameObject middleArrow;
    public GameObject rightArrow;
    public GameObject confirmButton;
    public GameObject finishButton;

    public GameObject settingsPanel;
    public TMP_InputField repeatInputField;
    public TMP_InputField switchCountInputField;

    private int repeatsPerPhase = 5;
    private int switchesPerRepeat = 60;
    private bool isConfirmed = false;

    public void OnStartPressed()
    {
        if (!int.TryParse(repeatInputField.text, out repeatsPerPhase))
        {
            repeatsPerPhase = 5;
        }

        // Parse switch count
        if (!int.TryParse(switchCountInputField.text, out switchesPerRepeat))
        {
            switchesPerRepeat = 60;
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        //here is where I should start running the python script
        //StartPythonScript();

        //start training
        StartCoroutine(RunAllPhases());
    }

    IEnumerator RunAllPhases()
    {
        yield return RunPhase(() => TogglePair(leftArrow, middleArrow), "Left <-> Middle");
        yield return RunPhase(() => TogglePair(rightArrow, middleArrow), "Right <-> Middle");
        yield return RunPhase(() => TogglePair(leftArrow, rightArrow), "Left <-> Right");
        Debug.Log("All phases finished.");

        if (confirmButton != null)
        {
            confirmButton.SetActive(false);
        }

        if (finishButton != null)
        {
            finishButton.SetActive(true);
        }
    }

    IEnumerator RunPhase(System.Action toggleAction, string phaseName)
    {
        if (confirmButton != null)
        {
            confirmButton.SetActive(false);
        }
        if (finishButton != null)
        {
            finishButton.SetActive(false);
        }

        Debug.Log("Starting Phase: " + phaseName);
        for (int repeat = 0; repeat < repeatsPerPhase; repeat++)
        {
            Debug.Log($"Running repeat {repeat + 1}/{repeatsPerPhase} for {phaseName}");

            for (int i = 0; i < switchesPerRepeat; i++)
            {
                toggleAction.Invoke();
                yield return new WaitForSeconds(1f);
            }

            // 每次执行完一轮后，等待玩家确认
            Debug.Log("Waiting for confirmation...");
            ResetArrowColors();

            if (confirmButton != null)
            {
                confirmButton.SetActive(true);
            }

            isConfirmed = false;
            yield return new WaitUntil(() => isConfirmed);
        }
        Debug.Log("Finished Phase: " + phaseName);
    }

    public void OnConfirmPressed()
    {
        isConfirmed = true;
        if (confirmButton != null)
        {
            confirmButton.SetActive(false);
        }
    }

    void TogglePair(GameObject a, GameObject b)
    {
        SpriteRenderer sra = a.GetComponent<SpriteRenderer>();
        SpriteRenderer srb = b.GetComponent<SpriteRenderer>();
        if (sra.color == Color.red)
        {
            sra.color = Color.white;
            srb.color = Color.red;
        }
        else
        {
            sra.color = Color.red;
            srb.color = Color.white;
        }
    }

    void ResetArrowColors()
    {
        leftArrow.GetComponent<SpriteRenderer>().color = Color.white;
        middleArrow.GetComponent<SpriteRenderer>().color = Color.white;
        rightArrow.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
