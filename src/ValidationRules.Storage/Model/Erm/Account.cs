﻿namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Account
    {
        public long Id { get; set; }
        public decimal Balance { get; set; }
        public bool IsArchived { get; set; }
    }
}
