using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class SerializeUtility {
    public static string LEVEL_TO_LOAD = "LevelToLoadKey";

    public static LevelEditor.SerializableBoard LoardBoardFromFile(string _filePath) {
        string jsonDataOnFile = File.ReadAllText($"{Application.dataPath}{LevelEditor.LevelEditor.PrependPath}/{_filePath}");
        LevelEditor.SerializableBoard serializableBoard = JsonUtility.FromJson<LevelEditor.SerializableBoard>(jsonDataOnFile);
        return serializableBoard;
    }

    public static string[] GetAllLevels() {
        string[] filesAvailable = Directory.GetFiles($"{Application.dataPath}{LevelEditor.LevelEditor.PrependPath}");
        string[] validFiles = filesAvailable.Where(filename => {
            return (filename.Contains(".json") && !filename.Contains(".meta"));
        }).ToArray();

        List<string> fileNamesOnly = new List<string>();

        foreach (string validFile in validFiles) {
            fileNamesOnly.Add(validFile.Split('/', '\\').Last());
        }

        return fileNamesOnly.ToArray();
    }
}
