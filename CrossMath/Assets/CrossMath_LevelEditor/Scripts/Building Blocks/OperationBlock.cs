using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationBlock : BuildingBlock {
    public char operationCharacter;

    private void OnValidate() {
        buildingBlockType = EBuildingBlockType.Operation;
    }
}
