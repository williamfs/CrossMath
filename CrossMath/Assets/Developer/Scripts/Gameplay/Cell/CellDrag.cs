using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CellDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

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
        List<RaycastResult> results = new List<RaycastResult>();
        List<GameplayCell> gameplayCells = new List<GameplayCell>();
        GraphicRaycaster caster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        caster.Raycast(eventData, results);

        // Getting the gameplay cells from the results
        foreach (RaycastResult result in results) {
            GameplayCell cell = result.gameObject.GetComponent<GameplayCell>();
            if (cell != null) {
                gameplayCells.Add(cell);
            }
        }

        if (gameplayCells.Count == 2) {
            GameplayCell thisCell = GetComponent<GameplayCell>();
            gameplayCells.Remove(thisCell);
            GameplayCell otherCell = gameplayCells[0];

            if(thisCell.IsEqual(otherCell)) {
                otherCell.MarkAsAnswered(thisCell);
                Destroy(thisCell.gameObject);
            }
        }

        // if it is not, return the cell to the answer grid and refresh grid layout
        transform.position = m_originalPosition;
        // => Cache a reference?
        FindObjectOfType<GameplayController>().RefreshAnswersGridLayout();
    }
}
