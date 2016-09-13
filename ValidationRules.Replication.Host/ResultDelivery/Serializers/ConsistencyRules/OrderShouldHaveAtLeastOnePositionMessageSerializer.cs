namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderShouldHaveAtLeastOnePositionMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderShouldHaveAtLeastOnePositionMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderShouldHaveAtLeastOnePosition;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                    $"����� {_linkFactory.CreateLink(orderReference)}",
                                    "����� �� �������� �� ����� �������");
        }
    }
}