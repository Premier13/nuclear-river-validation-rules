namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryShouldBelongToFirmMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryShouldBelongToFirmMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBelongToFirm;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var categoryReference = message.ReadCategoryReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"����� {_linkFactory.CreateLink(orderReference)}",
                                        $"� ������� {_linkFactory.CreateLink(orderPositionReference)} ������� ������� {_linkFactory.CreateLink(categoryReference)}, �� ������������� ����� ������");
        }
    }
}