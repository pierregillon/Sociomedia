namespace Sociomedia.Articles.Infrastructure
{
    public class FrenchKeywordDictionaryConfiguration
    {
        public FrenchKeywordDictionaryConfiguration(string dictionaryFilePath, string blackListFilePath)
        {
            BlackListFilePath = blackListFilePath;
            DictionaryFilePath = dictionaryFilePath;
        }
        public string BlackListFilePath { get;}
        public string DictionaryFilePath { get; }
    }
}