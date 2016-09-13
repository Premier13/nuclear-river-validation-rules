namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class BargainScanShouldPresentMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public BargainScanShouldPresentMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.BargainScanShouldPresent;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"����� {_linkFactory.CreateLink(orderReference)}",
                                        $"����������� ������������� ����� ��������");
        }
    }
}