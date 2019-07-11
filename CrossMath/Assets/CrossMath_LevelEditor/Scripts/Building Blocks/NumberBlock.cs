using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberBlock : BuildingBlock {
    public int numberValue;

    private void OnValidate() {
        buildingBlockType = EBuildingBlockType.Number;
    }
}
