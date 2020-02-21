using System.Collections.Generic;
using System.Linq;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.FirmRules
{
    public sealed class PartnerAdvertisementShouldNotHaveDifferentSalesModelMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.PartnerAdvertisementShouldNotHaveDifferentSalesModel;
        
        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orders = references.GetMany<EntityTypeOrder>().ToList();
            var firmAddress = references.Get<EntityTypeFirmAddress>();
            
            var currentOrder = orders[0];
            var conflictingOrders = orders.Skip(1).ToList();
            if (conflictingOrders.Count == 0)
            {
                // заказ противоречит сам себе
                conflictingOrders.Add(currentOrder);
            }

            var conflictingOrderPlaceholders = Enumerable.Range(1, conflictingOrders.Count).Select(i => $"{{{i}}}");
            var template = Resources.PartnerAdvertisementShouldNotHaveDifferentSalesModel.Replace("{1}", string.Join(", ", conflictingOrderPlaceholders));
            var args = new object[] { firmAddress }.Concat(conflictingOrders).ToArray();
            
            return new MessageComposerResult(
                currentOrder,
                template,
                args);
        }
        
        public IEnumerable<Message> Distinct(IEnumerable<Message> messages) =>
            messages
                .GroupBy(x => new
                {
                    x.OrderId,
                    FirmAddressId = x.References.Get<EntityTypeFirmAddress>().Id,
                })
                .Select(x => new Message
                {
                    MessageType = MessageType,
                    OrderId = x.Key.OrderId,
                    References = new[] { new Reference<EntityTypeOrder>(x.Key.OrderId.Value) }.Concat(x.SelectMany(y => y.References)).ToHashSet(Reference.Comparer)
                });
    }
}