using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor {
    public class BuildingBlock : MonoBehaviour, IPointerClickHandler {
        public enum EBuildingBlockType {
            Number,
            Operation,
            Hide,
            Unused,
        }

        public EBuildingBlockType buildingBlockType;
        private Image m_imageReference;

        void Start() {
            m_imageReference = GetComponent<Image>();

            switch(buildingBlockType) {
                case EBuildingBlockType.Number:
                    m_imageReference.color = LevelEditor.instance.colorConfiguration.numberBlockColor;
                    break;
                case EBuildingBlockType.Operation:
                    m_imageReference.color = LevelEditor.instance.colorConfiguration.operationBlockColor;
                    break;
                case EBuildingBlockType.Hide:
                    m_imageReference.color = LevelEditor.instance.colorConfiguration.hiddenBlockColor;
                    break;
                case EBuildingBlockType.Unused:
                    m_imageReference.color = LevelEditor.instance.colorConfiguration.unusedBlockColor;
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            LevelEditor.instance.SelectBuildingBlock(this);
        }
    }
}
