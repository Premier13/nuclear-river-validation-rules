using System;

using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedFirmAddressShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var firmAddressReference = message.ReadFirmAddressReference();

            var firmAddressState = message.ReadFirmAddressState();
            string format;
            switch (firmAddressState)
            {
                case InvalidFirmAddressState.Deleted:
                    format = "� ������� {0} ����� ����� {1} ����� ��������";
                    break;
                case InvalidFirmAddressState.NotActive:
                    format = "� ������� {0} ����� ����� {1} ���������";
                    break;
                case InvalidFirmAddressState.ClosedForAscertainment:
                    format = "� ������� {0} ����� ����� {1} ����� �� ���������";
                    break;
                case InvalidFirmAddressState.NotBelongToFirm:
                    format = "� ������� {0} ����� ����� {1} �� ����������� ����� ������";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new LocalizedMessage(message.GetLevel(),
                                        $"����� {_linkFactory.CreateLink(orderReference)}",
                                        string.Format(format, _linkFactory.CreateLink(orderPositionReference), _linkFactory.CreateLink(firmAddressReference)));
        }
    }
}