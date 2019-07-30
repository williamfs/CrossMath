using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {

    [Header("Gameplay Initialization")]
    public GameObject gameplayCellPrefab;

    [Header("Gameplay Configuration")]
    public GameObject solutionBank;
    public GameObject gridPanel;
    public LevelEditor.ColorConfiguration colorConfiguration;

    [Header("End of Game")]
    public GameObject gameOverPanel;
    public Text gameOverText;

    // Internal Gameplay Configuration variables
    private GridLayoutGroup m_boardGridLayoutPanel;
    private GridLayoutGroup m_answerBankGridLayout;
    private List<GameplayCell> m_unusedCells;
    private List<GameplayCell> m_questionCells;
    private List<GameplayCell> m_answerCells;
    private List<GameplayCell> m_gameplayCells;

    // References
    private FeedbackUI m_feedbackUIReference;

    // testing purposes - in the final product the level to load should be variable and passed by another scene or another script
    // there is a prepend path on Level Editor - keep this in mind
    public const string km_levelToLoad = "Levels/Level 3.json";

    // [TO DO]
    // time remaining is defaulted to 180s
    // in the future it should be in the level editor!
    private readonly float m_timeToComplete = 180.0f;

    private void Start() {
        m_feedbackUIReference = FindObjectOfType<FeedbackUI>();

        m_unusedCells = new List<GameplayCell>();
        m_questionCells = new List<GameplayCell>();
        m_answerCells = new List<GameplayCell>();
        m_gameplayCells = new List<GameplayCell>();

        m_boardGridLayoutPanel = gridPanel.GetComponent<GridLayoutGroup>();
        m_answerBankGridLayout = solutionBank.GetComponent<GridLayoutGroup>();
        InitializeLevel();
        RemoveUnusedCells(m_unusedCells);
        ShuffleAnswers();

        // Deactivating Panels
        gameOverPanel.SetActive(false);

        // Starting Timer
        StartCoroutine(TimerRoutine(m_timeToComplete));
        m_feedbackUIReference.UpdateTimer(Mathf.RoundToInt(m_timeToComplete));
    }

    private IEnumerator TimerRoutine(float _timeRemaining) {
        while (_timeRemaining > 0) {
            yield return new WaitForSeconds(1.0f);
            _timeRemaining -= 1.0f;
            m_feedbackUIReference.UpdateTimer(Mathf.RoundToInt(_timeRemaining));
        }

        ShowGameOverPanel("You Lose!");
    }

    private void InitializeLevel() {
        LevelEditor.SerializableBoard boardToLoad = SerializeUtility.LoardBoardFromFile(km_levelToLoad);
        m_boardGridLayoutPanel.constraintCount = boardToLoad.boardSize;

        // Instantiating all cells
        foreach(LevelEditor.SerializableCell cell in boardToLoad.serializableGrid) {
            GameplayCell gameplayCell = Instantiate(gameplayCellPrefab, gridPanel.transform).GetComponent<GameplayCell>();
            gameplayCell.FetchDependencies();
            gameplayCell.Assign(cell, colorConfiguration);

            // see if it would be a hint or answer, and then do stuff...
            if (cell.cellType == (int)LevelEditor.CellScript.ECellType.Unused) {
                m_unusedCells.Add(gameplayCell);
            } else if(cell.cellStatus == (int)LevelEditor.CellScript.ECellStatus.Hidden) {
                gameplayCell.MarkAsQuestion(colorConfiguration.hiddenBlockColor);
                m_questionCells.Add(gameplayCell);

                // Instantiating the cell that will be shown on the answer bank and can be dragged
                GameplayCell answerCell = Instantiate(gameplayCellPrefab, solutionBank.transform).GetComponent<GameplayCell>();
                answerCell.FetchDependencies();
                answerCell.Assign(cell, colorConfiguration);

                // Adding the Drag Component and stablishing all the actions necessary
                CellDrag answerCellDragComponent = answerCell.gameObject.AddComponent<CellDrag>();
                answerCellDragComponent.OnInteractedWithCell += GiveFeedbackOnCell;
                answerCellDragComponent.OnCellWasMatched += RemoveCellFromAnswers;

                answerCell.MarkAsAnswer();
                m_answerCells.Add(answerCell);
            } else if(cell.cellStatus == (int)LevelEditor.CellScript.ECellStatus.Visible) {
                gameplayCell.cellType = GameplayCell.ECellType.Hint;
                m_gameplayCells.Add(gameplayCell);
            }
        }

        m_feedbackUIReference.UpdateRemainingUIText(m_answerCells.Count);
    }

    private void RemoveUnusedCells(List<GameplayCell> _unusedCells) {
        foreach(GameplayCell unusedCell in _unusedCells) {
            unusedCell.DeactivateCell();
        }
    }

    // Select a random children on the Answer Bank and set it as the first, repeat 100 times
    private void ShuffleAnswers() {
        // [TO DO]
        // Shuffle steps maybe could be a variable?

        for(int i = 0; i < 100; i++) {
            int randomChild = Random.Range(0, solutionBank.transform.childCount);
            solutionBank.transform.GetChild(randomChild).SetAsFirstSibling();
        }
    }

    public void RefreshAnswersGridLayout() {
        m_answerBankGridLayout.enabled = false;
        m_answerBankGridLayout.enabled = true;
    }

    private void GiveFeedbackOnCell(GameplayCell _cell, bool _isPositive) {
        Color feedbackColor = _isPositive ? colorConfiguration.positiveFeedbackColor : colorConfiguration.negativeFeedbackColor;
        
        // [TO DO]
        // Play a positive or negative feedback sound here!
        // Play a fancy particle effect here also

        StartCoroutine(GiveFeedbackOnCellRoutine(_cell, feedbackColor));
    }

    private IEnumerator GiveFeedbackOnCellRoutine(GameplayCell _cell, Color _color) {
        // [TO DO]
        // some magic numbers here, these could be variables

        Color originalCellColor = _cell.CellColor;
        for(int i = 0; i < 3; i++) {
            _cell.CellColor = _color;
            yield return new WaitForSeconds(0.1f);
            _cell.CellColor = originalCellColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void RemoveCellFromAnswers(GameplayCell _cell) {
        m_answerCells.Remove(_cell);
        m_feedbackUIReference.UpdateRemainingUIText(m_answerCells.Count);

        if(m_answerCells.Count == 0) {
            ShowGameOverPanel("You Won!");
        }
    }

    private void ShowGameOverPanel(string _gameOverText) {
        gameOverText.text = _gameOverText;
        gameOverPanel.SetActive(true);
    }
}
