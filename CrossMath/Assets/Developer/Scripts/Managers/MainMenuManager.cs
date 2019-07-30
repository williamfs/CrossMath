using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    [Header("Level Selection")]
    public GameObject levelSelectionPanel;
    public GameObject levelSelectionButtonsParent;
    public GameObject levelSelectionButtonPrefab;

    private void Start() {
        levelSelectionPanel.SetActive(false);
    }

    public void PlayGame() {
        string[] possibleLevels = SerializeUtility.GetAllLevels();
        
        foreach(string possibleLevel in possibleLevels) {
            LevelSelectionButton levelSelectionButton = Instantiate(levelSelectionButtonPrefab, levelSelectionButtonsParent.transform).GetComponent<LevelSelectionButton>();
            levelSelectionButton.SetLevel(possibleLevel);
        }

        levelSelectionPanel.SetActive(true);
    }

    public void ShowOptions() {
        Debug.Log("Show Options");
    }

    public void ShowCredits() {
        Debug.Log("Show Credits");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
