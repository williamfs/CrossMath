using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayCell : MonoBehaviour {
    public enum ECellType {
        Hint,
        Question,
        Answer,
    }

    public ECellType cellType;

    private BoxCollider2D m_boxColliderReference;
    private Image m_imageReference;
    public Color CellColor {
        get {
            return m_imageReference.color;
        }
    }
    private Text m_textReference;
    public string TextContent {
        get {
            return m_textReference.text;
        }
    }

    public void FetchDependencies() {
        m_boxColliderReference = GetComponent<BoxCollider2D>();
        m_imageReference = GetComponent<Image>();
        m_textReference = GetComponentInChildren<Text>();
    }

    public void Assign(LevelEditor.SerializableCell _cell, LevelEditor.ColorConfiguration _colorConfiguration) {
        if (_cell.cellType == (int)LevelEditor.CellScript.ECellType.Number) {
            m_imageReference.color = _colorConfiguration.numberBlockColor;
        } else if(_cell.cellType == (int)LevelEditor.CellScript.ECellType.Operation) {
            m_imageReference.color = _colorConfiguration.operationBlockColor;
        } else {
            m_imageReference.color = _colorConfiguration.unusedBlockColor;
        }

        m_textReference.text = _cell.cellContent;
    }

    public void DeactivateCell() {
        m_imageReference.enabled = false;
        m_textReference.text = "";
        m_boxColliderReference.enabled = false;
    }

    public void MarkAsQuestion(Color _questionColor) {
        cellType = ECellType.Question;
        m_imageReference.color = _questionColor;
        m_textReference.enabled = false;
    }

    public void MarkAsAnswer() {
        cellType = ECellType.Answer;
    }

    public void MarkAsAnswered(GameplayCell _other) {
        cellType = ECellType.Hint;
        m_imageReference.color = _other.CellColor;
        m_textReference.enabled = true;
    }

    public bool IsEqual(GameplayCell _other) {
        return (m_textReference.text == _other.TextContent);
    }
}
