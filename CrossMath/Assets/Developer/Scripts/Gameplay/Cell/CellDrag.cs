using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CellDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

    public delegate void FeedbackOnCell(GameplayCell _cell, bool _isPositive);
    public delegate void ActOnCell(GameplayCell _answerCell, GameplayCell _questionCell);
    public event FeedbackOnCell OnInteractedWithCell;
    public event ActOnCell OnCellWasMatched;

    private Vector3 m_originalPosition;

    void Start() {
        m_originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 newCellPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        newCellPosition.z = 0;
        transform.position = newCellPosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Raycasting to see which cells should be dealt with
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        GameplayCell thisCell = GetComponent<GameplayCell>();
        GameplayCell otherCell = null;
        GraphicRaycaster graphicsRaycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        graphicsRaycaster.Raycast(eventData, raycastResults);

        // Getting the gameplay cells from the results
        foreach (RaycastResult result in raycastResults) {
            GameplayCell cell = result.gameObject.GetComponent<GameplayCell>();
            if (cell != null && cell != thisCell) {
                otherCell = cell;
            }
        }

        if (otherCell != null) {
            if(thisCell.IsEqual(otherCell)) {
                otherCell.MarkAsAnswered(thisCell);
                Destroy(thisCell.gameObject);

                OnInteractedWithCell?.Invoke(otherCell, true);
                OnCellWasMatched?.Invoke(thisCell, otherCell);
            } else {
                OnInteractedWithCell?.Invoke(otherCell, false);
            }
        }

        // if it is not, return the cell to the answer grid and refresh grid layout
        transform.position = m_originalPosition;
        // => Cache a reference?
        FindObjectOfType<GameplayController>().RefreshAnswersGridLayout();
    }
}
