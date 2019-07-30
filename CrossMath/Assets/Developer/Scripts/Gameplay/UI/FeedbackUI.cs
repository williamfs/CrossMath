using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour {
    public Text remainingBlocksText;

    [Header("Timer")]
    public Text timerText;
    public Image timerImage;

    public void UpdateRemainingUIText(int _amount) {
        remainingBlocksText.text = $"Remaining Blocks: {_amount}";
    }

    public void UpdateTimer(int _timeRemaining) {
        timerText.text = $"{_timeRemaining}";
    }
}
