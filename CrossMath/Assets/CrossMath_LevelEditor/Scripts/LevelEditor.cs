using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor {
    public class LevelEditor : MonoBehaviour {
        public static LevelEditor instance;

        [Header("Level Editor Configuration")]
        public ColorConfiguration colorConfiguration;
        public GameObject gridObject;
        public GameObject singleCellPrefab;

        [Header("Board Generation")]
        public GameObject generateBoardPanel;

        private BuildingBlock m_currentlySelectedBuildingBlock;
        private List<CellScript> m_cellList;

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }

            generateBoardPanel.SetActive(false);
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

        // -----------------------
        // Board Generation
        // -----------------------
        #region BOARD GENERATION

        public void ShowGenerateBoardPanel() {
            generateBoardPanel.SetActive(true);
        }

        public void HideGenerateBoardPanel() {
            generateBoardPanel.SetActive(false);
        }

        public void GenerateEmptyBoard(InputField _inputField) {
            int boardSize = int.Parse(_inputField.textComponent.text);
            GenerateEmptyBoard(boardSize);
            generateBoardPanel.SetActive(false);
        }

        public void GenerateEmptyBoard(int _size) {
            DestroyAllChildren(gridObject.transform);

            m_cellList = new List<CellScript>();
            GridLayoutGroup gridLayoutGroup = gridObject.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.constraintCount = _size;

            for(int i = 0; i < (_size * _size); i++) {
                CellScript cell = Instantiate(singleCellPrefab, gridObject.transform).GetComponent<CellScript>();
                cell.InitializeEmptyCell();
                m_cellList.Add(cell);
            }
        }

        public void GenerateBoard(SerializableBoard _board) {
            m_cellList = new List<CellScript>();
            GridLayoutGroup gridLayoutGroup = gridObject.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.constraintCount = _board.boardSize;

            for(int i = 0; i < (_board.boardSize * _board.boardSize); i++) {
                // Instantiating Cell
                CellScript instantiatedCell = Instantiate(singleCellPrefab, gridObject.transform).GetComponent<CellScript>();
                instantiatedCell.FetchDependencies();
                instantiatedCell.CopyFrom(_board.serializableGrid[i]);
                instantiatedCell.UpdateUI();
                m_cellList.Add(instantiatedCell);
            }
        }
        #endregion BOARD GENERATION

        // -----------------------
        // Save and Load Functions
        // -----------------------
        #region SAVE AND LOAD
        public void SavePuzzle() {
            SerializableBoard savedBoard = new SerializableBoard();
            List<SerializableCell> cellList = new List<SerializableCell>();

            savedBoard.boardSize = Mathf.RoundToInt(Mathf.Sqrt(m_cellList.Count));

            foreach (CellScript cell in m_cellList) {
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
            DeserializeBoard(board);
        }

        private void DeserializeBoard(SerializableBoard _board) {
            DestroyAllChildren(gridObject.transform);
            GenerateBoard(_board);
        }

        private void DestroyAllChildren(Transform _parent) {
            for(int i = 0; i < _parent.childCount; i++) {
                Destroy(_parent.GetChild(i).gameObject);
            }
        }
        #endregion SAVE AND LOAD
    }
}
