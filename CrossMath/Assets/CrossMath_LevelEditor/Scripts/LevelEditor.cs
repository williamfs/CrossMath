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

        [Header("Save")]
        public GameObject saveBoardPanel;
        public GameObject boardSavedPanel;
        public GameObject overrideFilePanel;

        [Header("Load")]
        public GameObject loadBoardPanel;

        // Saving
        private const string km_preprendPath = "/Levels";
        // Caching Save
        private string m_cachedFilename;


        // Loading

        private BuildingBlock m_currentlySelectedBuildingBlock;
        private List<CellScript> m_cellList;

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }

            // Desactivating all Save related panels
            generateBoardPanel?.SetActive(false);
            saveBoardPanel?.SetActive(false);
            boardSavedPanel?.SetActive(false);
            overrideFilePanel?.SetActive(false);

            // Desactivating all Load related panels.
            // loadBoardPanel?.SetActive(false);
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
        // Save Functions
        // -----------------------
        #region SAVE AND LOAD

        public void PromptSavePanel() {
            saveBoardPanel?.SetActive(true);
        }

        public void CloseSavePanel() {
            saveBoardPanel?.SetActive(false);
        }

        public void SaveBoard(InputField _inputField) {
            string filename = _inputField.textComponent.text;

            // Making sure it has the json extension
            if(!filename.Contains(".json")) {
                filename += ".json";
            }

            // Check if such file doesn't exist
            Debug.Log($"Checking {Application.dataPath}{km_preprendPath}/{filename}");
            if(System.IO.File.Exists($"{Application.dataPath}{km_preprendPath}/{filename}")) {
                m_cachedFilename = filename;
                overrideFilePanel?.SetActive(true);
            } else {
                SavePuzzle(filename);
            }
        }

        public void SavePuzzle(string _filename) {
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
            SaveFile(_filename, JsonUtility.ToJson(savedBoard));
        }

        public void SaveFile(string _filename, string _jsonFile) {
            File.WriteAllText($"{Application.dataPath}{km_preprendPath}/{_filename}", _jsonFile);
            boardSavedPanel?.SetActive(true);
        }

        // Called by the UI by the panel that shows feedback when a file was saved...
        public void CloseBoardSavedPanel() {
            saveBoardPanel?.SetActive(false);
            boardSavedPanel?.SetActive(false);
        }

        // Called by the UI when the user clicks "Yes" to override a save file
        public void OverrideSaveFile() {
            overrideFilePanel?.SetActive(false);
            SavePuzzle(m_cachedFilename);
        }

        // Called by the UI when the user clicks "No" to override a save file
        public void DontOverrideSaveFile() {
            overrideFilePanel?.SetActive(false);
        }

        // -----------------------
        // LOAD Functions
        // -----------------------
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
