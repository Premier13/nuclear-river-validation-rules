namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AssociatedPositionsGroupCountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public LocalizedMessage Serialize(Message message)
        {
            var priceReference = message.ReadPriceReference();
            var pricePositionReference = message.ReadPricePositionReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"�����-���� {_linkFactory.CreateLink(priceReference)}",
                                        $"� ������� �����-����� {_linkFactory.CreateLink(pricePositionReference)} ���������� ����� ����� ������ ������������� �������, ��� �� �������������� ��������");
        }
    }
}