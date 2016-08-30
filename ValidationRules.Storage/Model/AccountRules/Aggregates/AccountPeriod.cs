using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class AccountPeriod
    {
        public long AccountId { get; set; }

        /// <summary>
        /// ������� �������� ������� ��.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// ����� ���������� �� ����� �� ������.
        /// </summary>
        public decimal LockedAmount { get; set; }

        /// <summary>
        /// ����� ���������� �� ����� �� �� �����.
        /// </summary>
        public decimal OwerallLockedAmount { get; set; }

        /// <summary>
        /// �����, ��������� � �������� �� ����� �� ������. ����������� �� ���� �������, ����������� �� �����.
        /// </summary>
        public decimal ReleaseAmount { get; set; }

        /// <summary>
        /// �����, �� ������� ������������� ������ �� ��.
        /// </summary>
        public decimal LimitAmount { get; set; }

        /// <summary>
        /// ������ �������, �� ������� �������� ������ ������� ��.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// ��������� �������, �� �������� �������� ������ ������� ��.
        /// </summary>
        public DateTime End { get; set; }
    }
}