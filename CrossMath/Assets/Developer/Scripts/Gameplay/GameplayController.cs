using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {

    public GameplayConfiguration gameplayConfiguration;

    [Header("Gameplay Initialization")]
    public GameObject gameplayCellPrefab;

    [Header("Gameplay Configuration")]
    public GameObject solutionBank;
    public GameObject gridPanel;
    public LevelEditor.ColorConfiguration colorConfiguration;

    [Header("Pause Game")]
    public GameObject pausePanel;

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
    public const string km_levelToLoad = "Level 3.json";
    private string m_levelToLoad;

    // [TO DO]
    // time remaining is defaulted to 180s
    // in the future it should be in the level editor!
    private float m_timeRemaining;

    private void Start() {
        // Seeing which level needs to be loaded
        // [TO DO]
        // This is currently being done with PlayerPrefs, maybe there is a better way?!
        // i.e. Having an object that persists data between scenes
        m_levelToLoad = PlayerPrefs.GetString(SerializeUtility.LEVEL_TO_LOAD) ?? km_levelToLoad;

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
        pausePanel.SetActive(false);

        // Starting Timer
        m_timeRemaining = gameplayConfiguration.baseTime;
        StartCoroutine(TimerRoutine());
        m_feedbackUIReference.UpdateTimer(Mathf.RoundToInt(m_timeRemaining));
    }

    private IEnumerator TimerRoutine() {
        while (m_timeRemaining > 0) {
            yield return new WaitForSeconds(1.0f);
            m_timeRemaining -= 1.0f;
            m_feedbackUIReference.UpdateTimer(Mathf.RoundToInt(m_timeRemaining));
        }

        ShowGameOverPanel("You Lose!");
    }

    private void InitializeLevel() {
        LevelEditor.SerializableBoard boardToLoad = SerializeUtility.LoardBoardFromFile(m_levelToLoad);
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
        int childCount = solutionBank.transform.childCount;

        if (childCount == 0) {
            return;
        }

        for(int i = 0; i < 100; i++) {
            int randomChild = Random.Range(0, childCount);
            solutionBank.transform.GetChild(randomChild).SetAsFirstSibling();
        }
    }

    public void RefreshAnswersGridLayout() {
        m_answerBankGridLayout.enabled = false;
        m_answerBankGridLayout.enabled = true;
    }

    private void GiveFeedbackOnCell(GameplayCell _cell, bool _isPositive) {
        Color feedbackColor = _isPositive ? colorConfiguration.positiveFeedbackColor : colorConfiguration.negativeFeedbackColor;
        
        if(_isPositive) {
            m_timeRemaining += gameplayConfiguration.additionalTimeCorrectAnswer;
            m_feedbackUIReference.ShowVisualFeedbackText($"+{gameplayConfiguration.additionalTimeCorrectAnswer}", colorConfiguration.positiveFeedbackColor);
        } else {
            m_timeRemaining -= gameplayConfiguration.timePenaltyWrongAnswer;
            m_feedbackUIReference.ShowVisualFeedbackText($"-{gameplayConfiguration.timePenaltyWrongAnswer}", colorConfiguration.negativeFeedbackColor);
        }

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

    private void RemoveCellFromAnswers(GameplayCell _answerCell, GameplayCell _questionCell) {
        m_answerCells.Remove(_answerCell);
        m_questionCells.Remove(_questionCell);

        m_feedbackUIReference.UpdateRemainingUIText(m_answerCells.Count);

        if(m_answerCells.Count == 0) {
            ShowGameOverPanel("You Won!");
        }
    }

    private void ShowGameOverPanel(string _gameOverText) {
        gameOverText.text = _gameOverText;
        gameOverPanel.SetActive(true);
    }

    // ====================================
    // User Interface Functions
    // ====================================
    public void PauseGame() {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        LevelManager.instance.LoadLevel(0);
    }

    public void PlayAgain() {
        Time.timeScale = 1f;
        LevelManager.instance.ReloadLevel();
    }

    public void ShowHint() {
        m_timeRemaining -= gameplayConfiguration.hintTimePenalty;
        StartCoroutine(HintRoutine(m_questionCells.RandomOrDefault()));
    }

    private IEnumerator HintRoutine(GameplayCell _hintCell) {
        _hintCell.ShowCellWithColor(colorConfiguration.hintColor);
        yield return new WaitForSeconds(gameplayConfiguration.hintAvailableTime);
        _hintCell.MarkAsQuestion(colorConfiguration.hiddenBlockColor);
    }
}
