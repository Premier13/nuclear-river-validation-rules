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
namespace NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.BillsForContractors {
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class BillsForContractors {

        private ServicesEntryIntoAdNetworkOnOffer[] servicesEntryIntoAdNetworkOnOfferField;

        private Site2GISServicesOnOffer[] site2GISServicesOnOfferField;

        private SetOffOfClaimsSite2GISServicesOnOffer[] setOffOfClaimsSite2GISServicesOnOfferField;

        private long receiverLegalEntityBranchCodeField;

        private System.DateTime startDateField;

        private System.DateTime endDateField;

        private AccountingMethod accountingMethodField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServicesEntryIntoAdNetworkOnOffer")]
        public ServicesEntryIntoAdNetworkOnOffer[] ServicesEntryIntoAdNetworkOnOffer {
            get {
                return this.servicesEntryIntoAdNetworkOnOfferField;
            }
            set {
                this.servicesEntryIntoAdNetworkOnOfferField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Site2GISServicesOnOffer")]
        public Site2GISServicesOnOffer[] Site2GISServicesOnOffer {
            get {
                return this.site2GISServicesOnOfferField;
            }
            set {
                this.site2GISServicesOnOfferField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SetOffOfClaimsSite2GISServicesOnOffer")]
        public SetOffOfClaimsSite2GISServicesOnOffer[] SetOffOfClaimsSite2GISServicesOnOffer {
            get {
                return this.setOffOfClaimsSite2GISServicesOnOfferField;
            }
            set {
                this.setOffOfClaimsSite2GISServicesOnOfferField = value;
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
    public partial class ServicesEntryIntoAdNetworkOnOffer {

        private Order[] orderField;

        private DirectionOfBill directionField;

        private string regionalOrderNumberField;

        private long legalEntityCodeField;

        private long senderLegalEntityBranchCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Order")]
        public Order[] Order {
            get {
                return this.orderField;
            }
            set {
                this.orderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DirectionOfBill Direction {
            get {
                return this.directionField;
            }
            set {
                this.directionField = value;
            }
        }

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
        public long SenderLegalEntityBranchCode {
            get {
                return this.senderLegalEntityBranchCodeField;
            }
            set {
                this.senderLegalEntityBranchCodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Order {

        private string orderNumberField;

        private int organizationUnitCodeField;

        private string organizationUnitNameField;

        private long legalEntityBranchCodeField;

        private string mediaInfoField;

        private bool isSalesConditionsViolatedField;

        private float amountField;

        public Order() {
            this.isSalesConditionsViolatedField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrderNumber {
            get {
                return this.orderNumberField;
            }
            set {
                this.orderNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int OrganizationUnitCode {
            get {
                return this.organizationUnitCodeField;
            }
            set {
                this.organizationUnitCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OrganizationUnitName {
            get {
                return this.organizationUnitNameField;
            }
            set {
                this.organizationUnitNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long LegalEntityBranchCode {
            get {
                return this.legalEntityBranchCodeField;
            }
            set {
                this.legalEntityBranchCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MediaInfo {
            get {
                return this.mediaInfoField;
            }
            set {
                this.mediaInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IsSalesConditionsViolated {
            get {
                return this.isSalesConditionsViolatedField;
            }
            set {
                this.isSalesConditionsViolatedField = value;
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
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetOffOfClaimsSite2GISServicesOnOffer {

        private string regionalOrderNumberField;

        private long legalEntityCodeField;

        private long senderLegalEntityBranchCodeField;

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
        public long SenderLegalEntityBranchCode {
            get {
                return this.senderLegalEntityBranchCodeField;
            }
            set {
                this.senderLegalEntityBranchCodeField = value;
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
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Site2GISServicesOnOffer {

        private DirectionOfBill directionField;

        private string regionalOrderNumberField;

        private long legalEntityCodeField;

        private long senderLegalEntityBranchCodeField;

        private float amountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DirectionOfBill Direction {
            get {
                return this.directionField;
            }
            set {
                this.directionField = value;
            }
        }

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
        public long SenderLegalEntityBranchCode {
            get {
                return this.senderLegalEntityBranchCodeField;
            }
            set {
                this.senderLegalEntityBranchCodeField = value;
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
    public enum DirectionOfBill {

        /// <remarks/>
        Incoming,

        /// <remarks/>
        Outgoing,
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
