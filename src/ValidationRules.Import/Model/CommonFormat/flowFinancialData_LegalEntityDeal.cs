﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalEntityDeal {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class LegalEntityDeal {
        
        private long[] dealsField;
        
        private long legalEntityCodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("DealCode", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public long[] Deals {
            get {
                return this.dealsField;
            }
            set {
                this.dealsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long LegalEntityCode {
            get {
                return this.legalEntityCodeField;
            }
            set {
                this.legalEntityCodeField = value;
            }
        }
    }
}