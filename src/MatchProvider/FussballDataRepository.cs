using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchProvider.Contracts;
using MatchProvider.Contracts.Models;
using Microsoft.Extensions.Options;
using Tippspiel.SportsdataSvc;

namespace MatchProvider
{
    public class FussballDataRepository : IFussballDataRepository, IAccessStats, IDisposable
    {
        private readonly IOptions<MatchProviderSettings> _settings;
        private readonly ICacheProvider _cache;
        private readonly SportsdataSoapClient _client;
        private static int _remoteHits = 0;
        private static int _cacheHits = 0;
        private const int CacheDuration = 60;
        private string _cacheMatchPrefix = "cacheGame";
        private bool _disposed;

        public FussballDataRepository(IOptions<MatchProviderSettings> settings, ICacheProvider cacheProvider)
        {
            _settings = settings;
            _cache = cacheProvider;

            _client = new SportsdataSoapClient(SportsdataSoapClient.EndpointConfiguration
                .SportsdataSoap);
        }

        public bool IsSpieltagComplete { get; }

        public async Task<GroupInfoModel> GetCurrentGroupAsync()
        {
            const string cacheTag = "cacheCurrGrp";

            Group g = null;
            if (_cache.IsSet(cacheTag))
            {
                g = (Group)_cache.Get(cacheTag);
                _cacheHits++;
            }
            else
            {
                var response = await _client.GetCurrentGroupAsync(_settings.Value.LeagueShortcut);

                g = response.Body.GetCurrentGroupResult;

                _cache.Set(cacheTag, g, CacheDuration);

                _remoteHits++;
            }

            return new GroupInfoModel
            {
                Id = g.groupOrderID,
                Text = g.groupName
            };
        }

        public async Task<IReadOnlyList<GroupInfoModel>> GetAllGroupsAsync()
        {
            const string cacheTag = "cacheAllGrps";

            Group[] groups = null;
            if (_cache.IsSet(cacheTag))
            {
                groups = (Group[])_cache.Get(cacheTag);
                _cacheHits++;
            }
            else
            {
                var groupResponse = await _client.GetAvailGroupsAsync( _settings.Value.LeagueShortcut, _settings.Value.Season);
                groups = groupResponse.Body.GetAvailGroupsResult;

                _cache.Set(cacheTag, groups, CacheDuration);
                _remoteHits++;
            }

            var groupList = new List<GroupInfoModel>();
            foreach (var g in groups)
            {
                groupList.Add(new GroupInfoModel
                {
                    Id = g.groupOrderID,
                    Text = g.groupName
                });
            }

            return groupList;
        }

        public MatchDataModel GetNextMatch()
        {
            throw new NotImplementedException();
        }

        public MatchDataModel GetLastMatch()
        {
            throw new NotImplementedException();
        }

        public MatchDataModel GetMatchData(int matchId)
        {
            return new MatchDataModel
            {
                AwayTeamId = 111,
                HomeTeamId = 222,
                AwayTeam = "FC Bayern",
                HomeTeam = "RB Leipzig",
                AwayTeamScore = 1,
                HomeTeamScore = 0,
                GroupId = 1,
                GroupOrderId = 1,
                HasVerlaengerung = false,
                KickoffTime = DateTime.Now,
                KickoffTimeUtc = DateTime.UtcNow,
                MatchId = 11999,
                MatchNr = 34,
                LeagueShortcut = "bl1"
            };
        }

        public List<MatchDataModel> GetMatchesByCurrentGroup()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MatchDataModel>> GetMatchesByGroup(int groupId)
        {
            string cacheMatchGroupTag = "cacheGameByGrp" + groupId.ToString() + _settings.Value.LeagueShortcut + _settings.Value.Season;
            string cacheMatchTag = _cacheMatchPrefix + _settings.Value.LeagueShortcut;

            Matchdata[] matches = null;
            if (_cache.IsSet(cacheMatchGroupTag))
            {
                matches = (Matchdata[])_cache.Get(cacheMatchGroupTag);

                _cacheHits++;
            }
            else
            {
                var response = await _client.GetMatchdataByGroupLeagueSaisonAsync(groupId, _settings.Value.LeagueShortcut, _settings.Value.Season);

                matches = response.Body.GetMatchdataByGroupLeagueSaisonResult;

                // check if data has been found at all
                if (matches.Count() == 1)
                {
                    if (matches.First().matchID == -1)
                    {
                        return new List<MatchDataModel>();
                    }
                }

                _cache.Set(cacheMatchGroupTag, matches, CacheDuration);

                // cache single matches
                foreach (var m in matches)
                {
                    _cache.Set(cacheMatchTag + m.matchID, m, CacheDuration);
                }

                _remoteHits++;
            }

            var mList = new List<MatchDataModel>();

            foreach (var m in matches)
            {
                mList.Add(Create(m));
            }

            return mList;

        }

        List<MatchDataModel> IFussballDataRepository.GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<MatchDataModel> GetAllMatches()
        {
            return null;
        }

        public int GetRemoteHits()
        {
            return _remoteHits;
        }

        public int GetCacheHits()
        {
            return _cacheHits;
        }

        private static MatchDataModel Create(Matchdata match)
        {
            var matchModelObj = new MatchDataModel
            {
                MatchId = match.matchID,
                GroupId = match.groupOrderID,
                KickoffTime = match.matchDateTime,
                KickoffTimeUtc = match.matchDateTimeUTC,
                HomeTeamId = match.idTeam1,
                AwayTeamId = match.idTeam2,
                HomeTeamScore = match.pointsTeam1,
                AwayTeamScore = match.pointsTeam2,
                HomeTeamIcon = match.iconUrlTeam1,
                AwayTeamIcon = match.iconUrlTeam2,
                HomeTeam = match.nameTeam1,
                AwayTeam = match.nameTeam2,
                IsFinished = match.matchIsFinished,
                LeagueShortcut = match.leagueShortcut
            };

            if (match.matchResults != null && match.matchResults.Count > 0)
            {
                var result = (from r in match.matchResults orderby r.resultTypeId descending select r).FirstOrDefault();

                if (result == null) return matchModelObj;

                matchModelObj.HomeTeamScore = result.pointsTeam1;
                matchModelObj.AwayTeamScore = result.pointsTeam2;
            }

            return matchModelObj;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _client.CloseAsync().GetAwaiter().GetResult();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }
    }
}
