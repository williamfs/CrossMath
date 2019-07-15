using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class CellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("Cell Configuration")]
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

    private Image m_imageReference;
    private Text m_textReference;

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

    public ECellType cellType;
    public ECellStatus cellStatus;
    public int intCellContent;
    public char charCellContent;

    void Start() {
        m_imageReference = GetComponent<Image>();
        m_textReference = GetComponentInChildren<Text>();

        cellType = ECellType.None;
        cellStatus = ECellStatus.None;
        m_textReference.text = "";
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        m_imageReference.color = LevelEditor.instance.mouseHoverColor;
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        m_imageReference.color = m_currentColor;
    }

    public void UpdateUI() {
        if(cellType == ECellType.Number) {
            m_textReference.text = intCellContent.ToString();
        } else if(cellType == ECellType.Operation) {
            m_textReference.text = charCellContent.ToString();
        }

        m_imageReference.color = m_currentColor;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(LevelEditor.instance.HasBuildingBlockSelected()) {
            BuildingBlock currentlySelected = LevelEditor.instance.GetCurrentBuildingBlock();

            switch(currentlySelected.buildingBlockType) {
                case BuildingBlock.EBuildingBlockType.Number:
                    cellType = ECellType.Number;
                    cellStatus = ECellStatus.Visible;

                    NumberBlock block = (NumberBlock)currentlySelected;
                    m_textReference.text = block.numberValue.ToString();
                    intCellContent = block.numberValue;
                    m_currentColor = LevelEditor.instance.numberBlockColor;
                    break;
                case BuildingBlock.EBuildingBlockType.Operation:
                    cellType = ECellType.Operation;
                    cellStatus = ECellStatus.Visible;

                    OperationBlock opBlock = (OperationBlock)currentlySelected;
                    m_textReference.text = opBlock.operationCharacter.ToString();
                    charCellContent = opBlock.operationCharacter;
                    m_currentColor = LevelEditor.instance.operationBlockColor;
                    break;
                case BuildingBlock.EBuildingBlockType.Hide:

                    if(cellStatus == ECellStatus.Hidden) {
                        cellStatus = ECellStatus.Visible;

                        // current color is now previous color
                        // and previous color is now current color
                        Color tempColor = m_currentColor;
                        m_currentColor = m_previousColor;
                        m_previousColor = tempColor;
                    } else if(cellStatus == ECellStatus.Visible) {
                        cellStatus = ECellStatus.Hidden;

                        m_previousColor = m_currentColor;
                        m_currentColor = LevelEditor.instance.hiddenBlockColor;
                    }
                    break;
                case BuildingBlock.EBuildingBlockType.Unused:
                    cellType = ECellType.Unused;
                    cellStatus = ECellStatus.Unused;

                    m_textReference.text = "";
                    m_currentColor = LevelEditor.instance.unusedBlockColor;
                    break;
            }

            m_imageReference.color = m_currentColor;
        }
    }
}
