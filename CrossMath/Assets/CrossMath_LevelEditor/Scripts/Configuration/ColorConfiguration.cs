using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor {
    [CreateAssetMenu(fileName = "ColorConfiguration", menuName = "CrossMath/Color Configuration")]
    public class ColorConfiguration : ScriptableObject {
        [Header("Feedback Colors")]
        public Color mouseHoverColor = Color.yellow;

        [Header("Blocks Colors")]
        public Color numberBlockColor = Color.white;
        public Color operationBlockColor = Color.white;
        public Color hiddenBlockColor = Color.white;
        public Color unusedBlockColor = Color.red;
    }
}
