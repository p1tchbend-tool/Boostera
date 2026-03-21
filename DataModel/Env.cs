namespace Boostera
{
    internal class Env
    {
        internal string Key { get; set; }
        internal string Value { get; set; }

        internal Env(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
