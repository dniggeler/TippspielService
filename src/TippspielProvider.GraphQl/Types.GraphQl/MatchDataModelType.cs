using HotChocolate.Types;
using MatchProvider.Contracts.Models;

namespace TippspielProvider.GraphQl.Types.GraphQl
{
    public class MatchDataModelType : ObjectType<MatchDataModel>
    {
        protected override void Configure(IObjectTypeDescriptor<MatchDataModel> descriptor)
        {
            descriptor.Field(t => t.KickoffTime)
                .Type<DateTimeType>();
            descriptor.Field(t => t.KickoffTimeUtc)
                .Type<DateTimeType>();

        }
    }
}
