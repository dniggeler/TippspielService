namespace MatchProvider.Contracts
{
    public interface IAccessStats
    {
        int GetRemoteHits();
        int GetCacheHits();
    }
}