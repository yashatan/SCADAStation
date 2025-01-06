namespace SCADAStationNetFrameWork
{
    public class ItemEvent
    {
        private ItemEventType eventType;

        public ItemEvent()
        {

        }

       public enum ItemEventType
        {
            emClick,
            emPress,
            emRelease
        }

        public enum ItemActiontype
        {
            emSetbit,
            emResetBit,
            emSetValue,
            emOpenScreen
        }
        private string name;
        public int Value { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ItemEventType EventType { get => eventType; set => eventType = value; }
        public ItemActiontype ActionType { get; set; }
        public virtual TagInfo Tag { get; set; }
        public int PageID { get; set; }
        public int PageType { get; set; }
    }
}
