using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    public string levelName;
    private Text m_buttonText;

    public void SetLevel(string _level) {
        levelName = _level;
        GetComponentInChildren<Text>().text = _level;
    }

    public void SelectLevel() {
        PlayerPrefs.SetString(SerializeUtility.LEVEL_TO_LOAD, levelName);
        LevelManager.instance.LoadLevel(1);
    }
}
