using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SerializeUtility {
    public static LevelEditor.SerializableBoard LoardBoardFromFile(string _filePath) {
        string jsonDataOnFile = File.ReadAllText($"{Application.dataPath}/{_filePath}");
        LevelEditor.SerializableBoard serializableBoard = JsonUtility.FromJson<LevelEditor.SerializableBoard>(jsonDataOnFile);
        return serializableBoard;
    }
}
