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
namespace NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.ApplicationsForSpending {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class ApplicationsForSpending {
        
        private ThePriceOfEnteringThePartnersTerritory[] thePriceOfEnteringThePartnersTerritoryField;
        
        private long receiverLegalEntityBranchCodeField;
        
        private System.DateTime startDateField;
        
        private System.DateTime endDateField;
        
        private AccountingMethod accountingMethodField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ThePriceOfEnteringThePartnersTerritory")]
        public ThePriceOfEnteringThePartnersTerritory[] ThePriceOfEnteringThePartnersTerritory {
            get {
                return this.thePriceOfEnteringThePartnersTerritoryField;
            }
            set {
                this.thePriceOfEnteringThePartnersTerritoryField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long ReceiverLegalEntityBranchCode {
            get {
                return this.receiverLegalEntityBranchCodeField;
            }
            set {
                this.receiverLegalEntityBranchCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime StartDate {
            get {
                return this.startDateField;
            }
            set {
                this.startDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime EndDate {
            get {
                return this.endDateField;
            }
            set {
                this.endDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public AccountingMethod AccountingMethod {
            get {
                return this.accountingMethodField;
            }
            set {
                this.accountingMethodField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ThePriceOfEnteringThePartnersTerritory {
        
        private string regionalOrderNumberField;
        
        private long sourceOrganizationUnitCodeField;
        
        private long destinationOrganizationUnitCodeField;
        
        private long legalEntityCodeField;
        
        private float amountField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegionalOrderNumber {
            get {
                return this.regionalOrderNumberField;
            }
            set {
                this.regionalOrderNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long SourceOrganizationUnitCode {
            get {
                return this.sourceOrganizationUnitCodeField;
            }
            set {
                this.sourceOrganizationUnitCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long DestinationOrganizationUnitCode {
            get {
                return this.destinationOrganizationUnitCodeField;
            }
            set {
                this.destinationOrganizationUnitCodeField = value;
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float Amount {
            get {
                return this.amountField;
            }
            set {
                this.amountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    public enum AccountingMethod {
        
        /// <remarks/>
        Fact,
        
        /// <remarks/>
        Planned,
    }
}