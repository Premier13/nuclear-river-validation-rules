﻿using System;

namespace NuClear.Querying.Http.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EntityElementIdAttribute : Attribute
    {
        public EntityElementIdAttribute(string uri)
        {
            Uri = new Uri(uri);
        }

        public Uri Uri { get; private set; }
    }
}