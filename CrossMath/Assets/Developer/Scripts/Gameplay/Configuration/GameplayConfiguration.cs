using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayConfiguration", menuName = "CrossMath/Gameplay Configuration")]
public class GameplayConfiguration : ScriptableObject {
    public float baseTime = 90f;
    public float additionalTimeCorrectAnswer = 5f;
    public float timePenaltyWrongAnswer = 5f;
    public float hintTimePenalty = 5f;
    public float hintAvailableTime = 2.5f;
}
