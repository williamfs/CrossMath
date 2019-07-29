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
    public GameObject upperPanel;
    public LevelEditor.ColorConfiguration colorConfiguration;

    // Internal Gameplay Configuration variables
    private GridLayoutGroup m_boardGridLayoutPanel;
    private GridLayoutGroup m_answerBankGridLayout;
    private List<GameplayCell> m_unusedCells;
    private List<GameplayCell> m_questionCells;
    private List<GameplayCell> m_answerCells;
    private List<GameplayCell> m_gameplayCells;

    // testing purposes - in the final product the level to load should be variable and passed by another scene or another script
    // there is a prepend path on Level Editor - keep this in mind
    public const string km_levelToLoad = "Levels/Level 3.json";

    private void Start() {
        m_unusedCells = new List<GameplayCell>();
        m_questionCells = new List<GameplayCell>();
        m_answerCells = new List<GameplayCell>();
        m_gameplayCells = new List<GameplayCell>();

        m_boardGridLayoutPanel = gridPanel.GetComponent<GridLayoutGroup>();
        m_answerBankGridLayout = solutionBank.GetComponent<GridLayoutGroup>();
        InitializeLevel();
        RemoveUnusedCells(m_unusedCells);
    }

    void InitializeLevel() {
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
                answerCell.gameObject.AddComponent<CellDrag>();
                answerCell.MarkAsAnswer();
                m_answerCells.Add(answerCell);
            } else if(cell.cellStatus == (int)LevelEditor.CellScript.ECellStatus.Visible) {
                gameplayCell.cellType = GameplayCell.ECellType.Hint;
                m_gameplayCells.Add(gameplayCell);
            }
        }
    }

    void RemoveUnusedCells(List<GameplayCell> _unusedCells) {
        foreach(GameplayCell unusedCell in _unusedCells) {
            unusedCell.DeactivateCell();
        }
    }

    public void RefreshAnswersGridLayout() {
        m_answerBankGridLayout.enabled = false;
        m_answerBankGridLayout.enabled = true;
    }
}
