﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingBlock : MonoBehaviour, IPointerClickHandler {
    public enum EBuildingBlockType {
        Number,
        Operation,
        Hide,
        Unused,
    }

    public EBuildingBlockType buildingBlockType;
    private Color m_buildingBlockColor;
    public Color BuildingBlockColor { get { return m_buildingBlockColor;  } }
    private Image m_imageReference;

    void Start() {
        m_imageReference = GetComponent<Image>();
        m_buildingBlockColor = m_imageReference.color;
    }

    public void OnPointerClick(PointerEventData eventData) {
        LevelEditor.instance.SelectBuildingBlock(this);
    }
}
