namespace LevelEditor {
    [System.Serializable]
    public class SerializableCell {
        public int cellStatus;
        public int cellType;
        public string cellContent;

        public SerializableCell(int _cellStatus, int _cellType, string _cellContent) {
            cellStatus = _cellStatus;
            cellType = _cellType;
            cellContent = _cellContent;
        }
    }
}
