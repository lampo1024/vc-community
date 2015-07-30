﻿using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public class DynamicPropertyDictionaryItem : AuditableEntity
    {
		public string PropertyId { get; set; }
        public string Name { get; set; }
        public DynamicPropertyDictionaryItemName[] DisplayNames { get; set; }
    }
}
