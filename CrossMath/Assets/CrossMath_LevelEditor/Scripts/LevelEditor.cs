using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
    public static LevelEditor instance;
    private BuildingBlock m_currentlySelectedBuildingBlock;

    [Header("Feedback Colors")]
    public Color mouseHoverColor = Color.yellow;
    public Color numberBlockColor = Color.white;
    public Color operationBlockColor = Color.white;
    public Color hiddenBlockColor = Color.white;
    public Color unusedBlockColor = Color.red;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SelectBuildingBlock(BuildingBlock _selected) {
        m_currentlySelectedBuildingBlock = _selected;
    }

    public BuildingBlock GetCurrentBuildingBlock() {
        return m_currentlySelectedBuildingBlock;
    }

    public bool HasBuildingBlockSelected() {
        return (m_currentlySelectedBuildingBlock != null);
    }

    // --------------------------------------------------------------------
    // Save and Load Functions
    //
    public void SavePuzzle() {
        SerializableBoard savedBoard = new SerializableBoard();
        CellScript[] allCells = FindObjectsOfType<CellScript>();
        List<SerializableCell> cellList = new List<SerializableCell>();

        savedBoard.boardSize = Mathf.RoundToInt(Mathf.Sqrt(allCells.Length));

        foreach (CellScript cell in allCells) {
            if(cell.cellType == CellScript.ECellType.None && cell.cellStatus == CellScript.ECellStatus.None) {
                cell.cellType = CellScript.ECellType.Unused;
                cell.cellStatus = CellScript.ECellStatus.Unused;
            }

            string content = "";
            if(cell.cellType == CellScript.ECellType.Number) {
                content = cell.intCellContent.ToString();
            } else if(cell.cellType == CellScript.ECellType.Operation) {
                content = cell.charCellContent.ToString();
            }

            cellList.Add(new SerializableCell(cell.cellStatus.GetHashCode(), cell.cellType.GetHashCode(), content));
        }

        savedBoard.serializableGrid = cellList.ToArray();
        SaveFile(JsonUtility.ToJson(savedBoard));
    }

    public void SaveFile(string _jsonFile) {
        Debug.Log($"Saving File: {_jsonFile}");
        File.WriteAllText(Application.dataPath + "/Levels/level.json", _jsonFile);
    }

    public void LoadFile() {
        string filePath = Application.dataPath + "/Levels/level.json";
        string jsonData = File.ReadAllText(filePath);
        SerializableBoard board = JsonUtility.FromJson<SerializableBoard>(jsonData);

        Debug.Log($"Loaded board of size {board.boardSize} with {board.serializableGrid.Length} cells");
        DeserializeBoard(board);
    }

    private void DeserializeBoard(SerializableBoard _board) {
        CellScript[] allCells = FindObjectsOfType<CellScript>();
        for(int i = 0; i < allCells.Length; i++) {
            CellScript.ECellType cellType = (CellScript.ECellType)_board.serializableGrid[i].cellType;
            CellScript.ECellStatus cellStatus = (CellScript.ECellStatus)_board.serializableGrid[i].cellStatus;
            allCells[i].cellType = cellType;
            allCells[i].cellStatus = cellStatus;

            if(cellType == CellScript.ECellType.Number) {
                allCells[i].intCellContent = int.Parse(_board.serializableGrid[i].cellContent);
                allCells[i].CurrentColor = numberBlockColor;
            } else if(cellType == CellScript.ECellType.Operation) {
                allCells[i].charCellContent = _board.serializableGrid[i].cellContent.ToCharArray()[0];
                allCells[i].CurrentColor = operationBlockColor;
            } else {
                allCells[i].CurrentColor = unusedBlockColor;
            }

            if(cellStatus == CellScript.ECellStatus.Hidden) {
                allCells[i].PreviousColor = allCells[i].CurrentColor;
                allCells[i].CurrentColor = hiddenBlockColor;
            }

            allCells[i].UpdateUI();
        }
    }
}
