using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor {
    [RequireComponent(typeof(BoxCollider2D))]
    public class CellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        public enum ECellType {
            None = 0,
            Number = 1,
            Operation = 2,
            Unused = 3,
        }

        public enum ECellStatus {
            None = 0,
            Visible = 1,
            Hidden = 2,
            Unused = 3
        }

        private Color m_currentColor = Color.white;
        public Color CurrentColor {
            get {
                return m_currentColor;
            }
            set {
                m_currentColor = value;
            }
        }

        private Color m_previousColor;
        public Color PreviousColor {
            get {
                return m_previousColor;
            }
            set {
                m_previousColor = value;
            }
        }

        public ECellType cellType;
        public ECellStatus cellStatus;
        public int intCellContent;
        public char charCellContent;

        // Private Members
        private Image m_imageReference;
        private Text m_textReference;

        public void InitializeEmptyCell() {
            cellType = ECellType.None;
            cellStatus = ECellStatus.None;
            FetchDependencies();
        }

        public void FetchDependencies() {
            m_imageReference = GetComponent<Image>();
            m_textReference = GetComponentInChildren<Text>();
            m_textReference.text = "";
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            m_imageReference.color = LevelEditor.instance.colorConfiguration.mouseHoverColor;
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            m_imageReference.color = m_currentColor;
        }

        public void UpdateUI() {
            if(cellType == ECellType.Number) {
                m_textReference.text = intCellContent.ToString();
            } else if(cellType == ECellType.Operation) {
                m_textReference.text = charCellContent.ToString();
            } else {
                m_textReference.text = "";
            }

            m_imageReference.color = m_currentColor;
        }

        public void CopyFrom(SerializableCell _cell) {
           
            cellStatus = (ECellStatus)_cell.cellStatus;
            cellType = (ECellType)_cell.cellType;

            if (cellType == CellScript.ECellType.Number) {
                intCellContent = int.Parse(_cell.cellContent);
                m_currentColor = LevelEditor.instance.colorConfiguration.numberBlockColor;
            } else if (cellType == CellScript.ECellType.Operation) {
                charCellContent = _cell.cellContent.ToCharArray()[0];
                CurrentColor = LevelEditor.instance.colorConfiguration.operationBlockColor;
            } else {
                CurrentColor = LevelEditor.instance.colorConfiguration.unusedBlockColor;
            }

            if (cellStatus == CellScript.ECellStatus.Hidden) {
                PreviousColor = CurrentColor;
                CurrentColor = LevelEditor.instance.colorConfiguration.hiddenBlockColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if(LevelEditor.instance.HasBuildingBlockSelected()) {
                LevelEditor.instance.AddToActionStack(this);
                BuildingBlock currentlySelected = LevelEditor.instance.GetCurrentBuildingBlock();
                AssignBuildingBlock(currentlySelected, false);
            }
        }

        public void AssignBuildingBlock(BuildingBlock _block, bool _isFeedback) {
            switch (_block.buildingBlockType) {
                case BuildingBlock.EBuildingBlockType.Number:
                    cellType = ECellType.Number;
                    cellStatus = ECellStatus.Visible;

                    NumberBlock block = (NumberBlock)_block;
                    m_textReference.text = block.numberValue.ToString();
                    intCellContent = block.numberValue;
                    m_currentColor = LevelEditor.instance.colorConfiguration.numberBlockColor;
                    break;
                case BuildingBlock.EBuildingBlockType.Operation:
                    cellType = ECellType.Operation;
                    cellStatus = ECellStatus.Visible;

                    OperationBlock opBlock = (OperationBlock)_block;
                    m_textReference.text = opBlock.operationCharacter.ToString();
                    charCellContent = opBlock.operationCharacter;
                    m_currentColor = LevelEditor.instance.colorConfiguration.operationBlockColor;
                    break;
                case BuildingBlock.EBuildingBlockType.Hide:

                    if (cellStatus == ECellStatus.Hidden) {
                        cellStatus = ECellStatus.Visible;

                        // current color is now previous color
                        // and previous color is now current color
                        Color tempColor = m_currentColor;
                        m_currentColor = m_previousColor;
                        m_previousColor = tempColor;
                    } else if (cellStatus == ECellStatus.Visible) {
                        cellStatus = ECellStatus.Hidden;

                        m_previousColor = m_currentColor;
                        m_currentColor = LevelEditor.instance.colorConfiguration.hiddenBlockColor;
                    }


                    // just overriding what happened and making the cell blue if it's the feedback cell
                    if(_isFeedback) {
                        m_currentColor = LevelEditor.instance.colorConfiguration.hiddenBlockColor;
                        m_textReference.text = "";
                        cellStatus = ECellStatus.Hidden;
                        cellType = ECellType.None;
                    }
                    break;
                case BuildingBlock.EBuildingBlockType.Unused:
                    cellType = ECellType.Unused;
                    cellStatus = ECellStatus.Unused;

                    m_textReference.text = "";
                    m_currentColor = LevelEditor.instance.colorConfiguration.unusedBlockColor;
                    break;
            }

            m_imageReference.color = m_currentColor;
        }
    }
}
