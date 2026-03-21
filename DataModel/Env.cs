namespace Boostera
{
    public class Env
    {
        public string Key { get; set; }
        public string Value { get; set; }

        internal Env(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
