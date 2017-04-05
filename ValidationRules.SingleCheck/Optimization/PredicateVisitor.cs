using System.Linq.Expressions;

namespace NuClear.ValidationRules.SingleCheck.Optimization
{
    internal sealed class PredicateVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private bool _isSimple = true;

        public PredicateVisitor(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        public bool IsSimple => _isSimple;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // �������� ������� �������, ���� � �� ������������ ��� �������� � ��������� - ������� ������� ��������
            _isSimple = _isSimple & (node == _parameter);
            return base.VisitParameter(node);
        }
    }
}