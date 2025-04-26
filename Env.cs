namespace Boostera
{
    public class Env
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Env(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
