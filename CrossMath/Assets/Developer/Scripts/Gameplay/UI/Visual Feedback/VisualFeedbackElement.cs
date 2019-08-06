using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualFeedbackElement : MonoBehaviour {
    private const float km_timeToWait = 1.0f;
    private Vector3 m_offset = new Vector3(.1f, .1f, 0f);
    private Text m_feedbackText;
    private RectTransform m_rectTransformReference;

    private void Awake() {
        m_feedbackText = GetComponent<Text>();
        m_rectTransformReference = GetComponent<RectTransform>();
    }

    public void MoveToPoint(Vector3 _pointPosition) {
        this.gameObject.SetActive(true);

        Vector3 newTextPosition = _pointPosition;
        newTextPosition.z = 0;
        m_rectTransformReference.position = newTextPosition + m_offset;
        StartCoroutine(BusyDelayRoutine());
    }

    private IEnumerator BusyDelayRoutine() {
        for(float i = 0; i < km_timeToWait; i += Time.deltaTime) {
            m_rectTransformReference.position += new Vector3(0f, 0.01f, 0f);
            yield return null;
        }

        this.gameObject.SetActive(false);
    }

    public void UpdateText(string _textContent, Color _color) {
        m_feedbackText.text = _textContent;
        m_feedbackText.color = _color;
    }
}
