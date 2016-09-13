using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class OrderRequiredFieldsShouldBeSpecifiedMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public OrderRequiredFieldsShouldBeSpecifiedMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.OrderRequiredFieldsShouldBeSpecified;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var dto = message.ReadOrderRequiredFieldsMessage();

            var parameters = new List<string>();

            if (dto.LegalPerson)
            {
                parameters.Add("��. ���� ���������");
            }
            if (dto.LegalPersonProfile)
            {
                parameters.Add("������� ��. ���� ���������");
            }
            if (dto.BranchOfficeOrganizationUnit)
            {
                parameters.Add("��. ���� �����������");
            }
            if (dto.Inspector)
            {
                parameters.Add("�����������");
            }
            if (dto.ReleaseCountPlan)
            {
                parameters.Add("����");
            }
            if (dto.Currency)
            {
                parameters.Add("������");
            }

            return new LocalizedMessage(message.GetLevel(),
                                    $"����� {_linkFactory.CreateLink(orderReference)}",
                                    "���������� ��������� ��� ������������ ��� ���������� ����: " + string.Join(", ", parameters));
        }
    }
}