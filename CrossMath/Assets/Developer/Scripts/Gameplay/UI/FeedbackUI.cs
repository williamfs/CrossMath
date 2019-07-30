using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour {
    public Text remainingBlocksText;

    public void UpdateRemainingUIText(int _amount) {
        remainingBlocksText.text = $"Remaining Blocks: {_amount}";
    }
}
