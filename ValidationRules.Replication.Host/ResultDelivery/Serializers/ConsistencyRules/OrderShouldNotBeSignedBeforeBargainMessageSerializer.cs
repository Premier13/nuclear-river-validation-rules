namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderShouldNotBeSignedBeforeBargainMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderShouldNotBeSignedBeforeBargainMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderShouldNotBeSignedBeforeBargain;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                    $"����� {_linkFactory.CreateLink(orderReference)}",
                                    "������� �� ����� ����� ���� ���������� ������� ���� ���������� ������");
        }
    }
}