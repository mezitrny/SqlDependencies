namespace SqlAnalyser.Internal.Scripters
{
    public interface IScripter
    {
        string GetScript(string name, string schema=null, string database=null);
    }
}