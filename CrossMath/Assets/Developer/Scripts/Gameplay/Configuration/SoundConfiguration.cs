using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundConfiguration", menuName = "CrossMath/Sound Configuration")]
public class SoundConfiguration : ScriptableObject {
    [Header("Feedback Sounds")]
    public AudioClip positiveFeedbackClip;
    public AudioClip negativeFeedbackClip;
}
