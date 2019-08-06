using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour {
    public Text remainingBlocksText;
    private VisualFeedbackElement[] m_visualFeedbackElements;

    [Header("Timer")]
    public Text timerText;
    public Image timerImage;

    // Visual Feedback Elements Handling
    private int m_visualFeedbackElementIndex = 0;

    private void Start() {
        m_visualFeedbackElements = FindObjectsOfType<VisualFeedbackElement>();

        foreach(VisualFeedbackElement element in m_visualFeedbackElements) {
            element.gameObject.SetActive(false);
        }
    }

    public void UpdateRemainingUIText(int _amount) {
        remainingBlocksText.text = $"Remaining Blocks: {_amount}";
    }

    public void UpdateTimer(int _timeRemaining) {
        timerText.text = $"{_timeRemaining}";
    }

    public void ShowVisualFeedbackText(string _textContent, Color _color) {
        m_visualFeedbackElements[m_visualFeedbackElementIndex].UpdateText(_textContent, _color);
        m_visualFeedbackElements[m_visualFeedbackElementIndex].MoveToPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        m_visualFeedbackElementIndex = ((m_visualFeedbackElementIndex + 1) % m_visualFeedbackElements.Length);
    }
}
