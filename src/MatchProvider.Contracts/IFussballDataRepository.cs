using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MatchProvider.Contracts.Models;

namespace MatchProvider.Contracts
{
    public interface IFussballDataRepository
    {
        bool IsSpieltagComplete { get; }

        Task<GroupInfoModel> GetCurrentGroupAsync();

        Task<IReadOnlyList<GroupInfoModel>> GetAllGroupsAsync();

        MatchDataModel GetNextMatch();
        MatchDataModel GetLastMatch();

        MatchDataModel GetMatchData(int matchId);

        List<MatchDataModel> GetMatchesByCurrentGroup();
        List<MatchDataModel> GetMatchesByGroup(int groupId);
        List<MatchDataModel> GetAllMatches();
    }
}
