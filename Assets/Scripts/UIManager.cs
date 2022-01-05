using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] GameManager gameManager;
    [SerializeField] Button startButton;
    [SerializeField] Button pauseButton;
    [SerializeField] Button resetButton;
    [SerializeField] TMPro.TMP_Text timeStepsText;
    void Awake() => Instance = this;

    public void StartButtonCallBack()
    {
        gameManager.StartSimulation();
        resetButton.interactable = false;
        pauseButton.interactable = true;
    }

    public void PauseButtonCallBack()
    {
        gameManager.StopSimulation();
        resetButton.interactable = true;
        pauseButton.interactable = false;
    }

    public void ResetButtonCallBack()
    {
        gameManager.CleanGrid();
    }
    
    public void UpdateTimeSteps(int No)
    {
        timeStepsText.text = "Steps Elapsed: " + No;
    }
}
