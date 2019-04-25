﻿using System;
using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.ConsistencyRules
{
    public sealed class LinkedFirmAddressShouldBeValidMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var firmAddressReference = references.Get<EntityTypeFirmAddress>();

            var firmAddressState = extra.ReadFirmAddressState();

            return new MessageComposerResult(
                orderReference,
                GetFormat(firmAddressState),
                orderPositionReference,
                firmAddressReference);
        }

        private static string GetFormat(InvalidFirmAddressState firmAddressState)
        {
            switch (firmAddressState)
            {
                case InvalidFirmAddressState.Deleted:
                    return Resources.LinkedFirmAddressShouldBeValid_Deleted;
                case InvalidFirmAddressState.NotActive:
                    return Resources.LinkedFirmAddressShouldBeValid_NotActive;
                case InvalidFirmAddressState.ClosedForAscertainment:
                    return Resources.LinkedFirmAddressShouldBeValid_ClosedForAscertainment;
                case InvalidFirmAddressState.NotBelongToFirm:
                    return Resources.LinkedFirmAddressShouldBeValid_NotBelongToFirm;
                case InvalidFirmAddressState.InvalidBuildingPurpose:
                    return Resources.LinkedFirmAddressShouldBeValid_InvalidBuildingPurpose;
                case InvalidFirmAddressState.MissingEntrance:
                    return Resources.LinkedFirmAddressShouldBeValid_MissingEntrance;
                default:
                    throw new Exception(nameof(firmAddressState));
            }
        }
    }
}