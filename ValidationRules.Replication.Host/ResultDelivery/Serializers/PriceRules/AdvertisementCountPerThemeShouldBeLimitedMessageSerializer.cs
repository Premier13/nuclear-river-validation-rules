namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class AdvertisementCountPerThemeShouldBeLimitedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementCountPerThemeShouldBeLimitedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var themeReference = message.ReadThemeReference();
            var dto = message.ReadOversalesMessage();

            return new LocalizedMessage(message.GetLevel(),
                                        $"����� {_linkFactory.CreateLink(orderReference)}",
                                        $"������� ����� ������ � �������� {_linkFactory.CreateLink(themeReference)}. ������� {dto.Count} ������� ������ {dto.Max} ���������");
        }
    }
}