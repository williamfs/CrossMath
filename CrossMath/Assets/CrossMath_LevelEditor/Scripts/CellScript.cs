using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class CellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("Cell Configuration")]
    public Color mouseHoverColor = Color.yellow;
    private Color m_currentColor = Color.white;
    private Color m_previousColor;

    private Image m_imageReference;
    private Text m_textReference;

    public enum ECellType {
        Number,
        Operation,
        Unused,
    }

    public enum ECellStatus {
        Visible,
        Hidden,
        Unused
    }

    public ECellType cellType;
    public ECellStatus cellStatus;
    public int intCellContent;
    public char charCellContent;

    void Start() {
        m_imageReference = GetComponent<Image>();
        m_textReference = GetComponentInChildren<Text>();

        m_textReference.text = "";
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        m_imageReference.color = mouseHoverColor;
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
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
                    m_currentColor = currentlySelected.BuildingBlockColor;
                    break;
                case BuildingBlock.EBuildingBlockType.Operation:
                    cellType = ECellType.Operation;
                    cellStatus = ECellStatus.Visible;

                    OperationBlock opBlock = (OperationBlock)currentlySelected;
                    m_textReference.text = opBlock.operationCharacter.ToString();
                    m_currentColor = currentlySelected.BuildingBlockColor;
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
                        m_currentColor = currentlySelected.BuildingBlockColor;
                    }
                    break;
                case BuildingBlock.EBuildingBlockType.Unused:
                    cellType = ECellType.Unused;
                    cellStatus = ECellStatus.Unused;

                    m_textReference.text = "";
                    m_currentColor = currentlySelected.BuildingBlockColor;
                    break;
            }

            m_imageReference.color = m_currentColor;
        }
    }
}
