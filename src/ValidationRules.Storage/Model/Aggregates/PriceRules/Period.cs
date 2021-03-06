﻿using System;

namespace NuClear.ValidationRules.Storage.Model.Aggregates.PriceRules
{
    /// <summary>
    /// Неявная сущность из ERM, элементарные периоды размещения заказа/наличия прайс-листа.
    /// Должна расчитываться динамически, например, для единственного заведённого в системе заказа любой продолжительности будет один период.
    /// И в то же время, если заказы начнут размещаться посуточно/почасово - у одного заказа может быть множество периодов.
    /// 
    /// Должен соблюдаться инвариант: сумма всех периодов заказа/прайс-листа - неразрывна.
    /// </summary>
    public sealed class Period
    {
        public long ProjectId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
