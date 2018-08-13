using HotChocolate.Types;
using MatchProvider.Contracts;
using MatchProvider.Contracts.Models;

namespace TippspielProvider.GraphQl.Types.GraphQl
{
    public class GroupInfoModelType : ObjectType<GroupInfoModel>
    {
        protected override void Configure(IObjectTypeDescriptor<GroupInfoModel> descriptor)
        {
        }
    }
}
