using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchProvider.Contracts;
using MatchProvider.Contracts.Models;

namespace TippspielProvider.GraphQl
{
    public class Query
    {
        private readonly IFussballDataRepository _matchProvider;

        public Query(IFussballDataRepository matchProvider)
        {
            _matchProvider = matchProvider;
        }

        public IEnumerable<MatchDataModel> GetMatches(int? groupId)
        {
            if (groupId.HasValue)
            {
                return _matchProvider.GetMatchesByGroup(groupId.Value);
            }
            else
            {
                return _matchProvider.GetAllMatches();
            }
        }

        public async Task<IEnumerable<GroupInfoModel>> GetGroupsAsync(int? groupId)
        {
            if (groupId.HasValue)
            {
                return await _matchProvider.GetAllGroupsAsync();
            }
            else
            {
                return await _matchProvider.GetAllGroupsAsync();
            }
        }
    }
}
