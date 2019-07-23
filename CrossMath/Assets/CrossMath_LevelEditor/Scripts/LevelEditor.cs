using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LevelEditor {
    public class LevelEditor : MonoBehaviour {
        public class BoardAction {
            public int cellListPosition;
            public SerializableCell currentCellStatus;
        }

        public static LevelEditor instance;

        [Header("Level Editor Configuration")]
        public ColorConfiguration colorConfiguration;
        public GameObject gridObject;
        public GameObject singleCellPrefab;

        [Header("Level Editor Feedback Section")]
        public CellScript currentlySelectedBuildingBlock;

        [Header("Board Generation")]
        public GameObject generateBoardPanel;

        [Header("Save")]
        public GameObject saveBoardPanel;
        public GameObject boardSavedPanel;
        public GameObject overrideFilePanel;

        [Header("Load")]
        public GameObject loadBoardPanel;
        public GameObject availableFilesParent;
        public GameObject textPrefab;

        // Saving
        private const string km_preprendPath = "/Levels";
        // Caching Save
        private string m_cachedFilename;


        // Loading

        // Internal Variables
        private BuildingBlock m_currentlySelectedBuildingBlock;
        private List<CellScript> m_cellList;
        private Stack<BoardAction> m_actionStack;

        // Board Manipulation
        private int m_cellSize = 75;
        private Vector3 previousFrameMousePosition;

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
            loadBoardPanel?.SetActive(false);

            // Initializing Feedback Section
            currentlySelectedBuildingBlock.InitializeEmptyCell();
        }

        private void Update() {
            if(Input.mouseScrollDelta != Vector2.zero) {
                ChangeGridCellSize(Input.mouseScrollDelta);
            }

            if(Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
                ProcessGridPosition(Input.mousePosition - previousFrameMousePosition);
            }

            previousFrameMousePosition = Input.mousePosition;
        }

        private void ChangeGridCellSize(Vector2 _variation) {
            GridLayoutGroup layoutGroup = gridObject.GetComponent<GridLayoutGroup>();
            layoutGroup.cellSize = layoutGroup.cellSize + new Vector2(_variation.y, _variation.y);
        }

        private void ProcessGridPosition(Vector2 _deltaMovement) {
            gridObject.transform.Translate(_deltaMovement);
        }

        public void SelectBuildingBlock(BuildingBlock _selected) {
            m_currentlySelectedBuildingBlock = _selected;
            currentlySelectedBuildingBlock.AssignBuildingBlock(_selected, true);
            currentlySelectedBuildingBlock.UpdateUI();
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
            InitializeDependencies();
            GridLayoutGroup gridLayoutGroup = gridObject.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.constraintCount = _size;

            for(int i = 0; i < (_size * _size); i++) {
                CellScript cell = Instantiate(singleCellPrefab, gridObject.transform).GetComponent<CellScript>();
                cell.InitializeEmptyCell();
                m_cellList.Add(cell);
            }
        }

        public void InitializeDependencies() {
            m_cellList = new List<CellScript>();
            m_actionStack = new Stack<BoardAction>();
        }

        public void GenerateBoard(SerializableBoard _board) {
            InitializeDependencies();
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
                cellList.Add(GetSerializableCellFromCellScript(cell));
            }

            savedBoard.serializableGrid = cellList.ToArray();
            SaveFile(_filename, JsonUtility.ToJson(savedBoard));
        }

        public SerializableCell GetSerializableCellFromCellScript(CellScript _cellScript) {
            if (_cellScript.cellType == CellScript.ECellType.None && _cellScript.cellStatus == CellScript.ECellStatus.None) {
                _cellScript.cellType = CellScript.ECellType.Unused;
                _cellScript.cellStatus = CellScript.ECellStatus.Unused;
            }

            string content = "";
            if (_cellScript.cellType == CellScript.ECellType.Number) {
                content = _cellScript.intCellContent.ToString();
            } else if (_cellScript.cellType == CellScript.ECellType.Operation) {
                content = _cellScript.charCellContent.ToString();
            }

            return new SerializableCell(_cellScript.cellStatus.GetHashCode(), _cellScript.cellType.GetHashCode(), content);
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
        public void ShowLoadFilePanel() {
            loadBoardPanel?.SetActive(true);

            // Showing Available Files to Load...
            DestroyAllChildren(availableFilesParent.transform);
            // getting available files
            string[] filesAvailable = Directory.GetFiles($"{Application.dataPath}{km_preprendPath}");
            string[] validFiles = filesAvailable.Where(filename => {
                return (filename.Contains(".json") && !filename.Contains(".meta"));
            }).ToArray();

            foreach (string validFile in validFiles) {
                Text filenameText = Instantiate(textPrefab, availableFilesParent.transform).GetComponent<Text>();
                filenameText.text = validFile.Split('/', '\\').Last();
            }
        }

        public void HideLoadFilePanel() {
            loadBoardPanel?.SetActive(false);
        }

        public void LoadFile(InputField _inputField) {
            string fileName = _inputField.textComponent.text;

            if(!fileName.Contains(".json")) {
                fileName += ".json";
            }

            string filePath = $"{Application.dataPath}{km_preprendPath}/{fileName}";
            string jsonData = File.ReadAllText(filePath);
            SerializableBoard board = JsonUtility.FromJson<SerializableBoard>(jsonData);
            DeserializeBoard(board);
            HideLoadFilePanel();
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

        #region UNDO
        public void AddToActionStack(CellScript _cell) {
            BoardAction boardAction = new BoardAction();

            // finding where the cell is on the list
            int index = m_cellList.FindIndex(cell => {
                return cell == _cell;
            });
            boardAction.cellListPosition = index;
            boardAction.currentCellStatus = GetSerializableCellFromCellScript(_cell);

            m_actionStack.Push(boardAction);
        }

        public void Undo() {
            if(m_actionStack.Count == 0) {
                return;
            }

            BoardAction lastAction = m_actionStack.Pop();
            m_cellList[lastAction.cellListPosition].CopyFrom(lastAction.currentCellStatus);
            m_cellList[lastAction.cellListPosition].UpdateUI();
        }
        #endregion UNDO
    }
}
