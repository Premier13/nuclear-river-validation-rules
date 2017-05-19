using System;
using System.Collections.Generic;

namespace NuClear.StateInitialization.Core.Factories
{
    /// <summary>
    /// ��������� ��������� ������������� ����� ������ �������� ������ � IStorageBasedDataObjectAccessor, ������������ ����������� ���� ��������
    /// </summary>
    public interface IAccessorTypesProvider
    {
        IReadOnlyCollection<Type> GetAccessorsFor(Type dataObjectType);
    }
}