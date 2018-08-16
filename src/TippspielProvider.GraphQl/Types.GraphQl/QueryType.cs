using HotChocolate.Types;

namespace TippspielProvider.GraphQl.Types.GraphQl
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(t => t.GetMatchesAsync(default))
                .Name("matches")
                .Type<ListType<MatchDataModelType>>();

            descriptor.Field(t => t.GetGroupsAsync(default))
                .Name("groups")
                .Type<ListType<GroupInfoModelType>>();
        }
    }
}
