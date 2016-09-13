namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var legalPersonProfileReference = message.ReadLegalPersonProfileReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"����� {_linkFactory.CreateLink(orderReference)}",
                                        $"� ��. ���� �������, � ������� {legalPersonProfileReference} ������� ������������ � ����� ��������� �������� ������ ���� ���������� ������");
        }
    }
}