namespace Boostera
{
    public class HistoryEncrypted
    {
        public string Key { get; set; }
        public string Iv { get; set; }
        public string Data { get; set; }

        public HistoryEncrypted(string key, string iv, string data)
        {
            Key = key;
            Iv = iv;
            Data = data;
        }
    }
}
