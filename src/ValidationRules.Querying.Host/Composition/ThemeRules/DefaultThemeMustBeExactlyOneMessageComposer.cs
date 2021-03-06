﻿using System.Collections.Generic;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.ThemeRules
{
    public sealed class DefaultThemeMustBeExactlyOneMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var projectReference = references.Get<EntityTypeProject>();
            var themeCount = extra.ReadProjectThemeCount();

            var themeCountMessage =
                themeCount == 0 ? Resources.DefaultThemeMustBeExactlyOne_None : Resources.DefaultThemeMustBeExactlyOne_Many;

            return new MessageComposerResult(
                projectReference,
                themeCountMessage,
                projectReference);
        }
    }
}