using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
    public static LevelEditor instance;
    private BuildingBlock m_currentlySelectedBuildingBlock;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SelectBuildingBlock(BuildingBlock _selected) {
        m_currentlySelectedBuildingBlock = _selected;
    }

    public BuildingBlock GetCurrentBuildingBlock() {
        return m_currentlySelectedBuildingBlock;
    }

    public bool HasBuildingBlockSelected() {
        return (m_currentlySelectedBuildingBlock != null);
    }
}
