using HotChocolate.Types;
using MatchProvider.Contracts;

namespace TippspielProvider.GraphQl.Types.GraphQl
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(t => t.GetMatches(default))
                .Name("matches")
                .Type<ListType<MatchDataModelType>>();

            descriptor.Field(t => t.GetGroupsAsync(default))
                .Name("groups")
                .Type<ListType<GroupInfoModelType>>();
        }
    }
}
