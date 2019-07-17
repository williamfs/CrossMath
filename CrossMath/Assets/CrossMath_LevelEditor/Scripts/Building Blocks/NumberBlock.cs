using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor {
    public class NumberBlock : BuildingBlock {
        public int numberValue;

        private void OnValidate() {
            buildingBlockType = EBuildingBlockType.Number;
        }
    }
}
