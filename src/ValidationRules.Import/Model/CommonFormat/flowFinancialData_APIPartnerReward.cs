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
namespace NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.APIPartnerReward {
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class APIPartnerReward {

        private string iNNField;

        private string kPPField;

        private string aPIPartnerDirectorNameInNominativeField;

        private string aPIPartnerDirectorNameInGenitiveField;

        private string aPIPartnerOnBasicOfInGenitiveField;

        private string legalUnitDirectorNameInNominativeField;

        private string legalUnitDirectorNameInGenitiveField;

        private string legalUnitOnBasicOfInGenitiveField;

        private string agreementNumberField;

        private System.DateTime agreementDateField;

        private System.DateTime dateField;

        private bool isVATIncludedField;

        private int clicksCountField;

        private float totalClicksAmountField;

        private float rewardAmountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string INN {
            get {
                return this.iNNField;
            }
            set {
                this.iNNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string KPP {
            get {
                return this.kPPField;
            }
            set {
                this.kPPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string APIPartnerDirectorNameInNominative {
            get {
                return this.aPIPartnerDirectorNameInNominativeField;
            }
            set {
                this.aPIPartnerDirectorNameInNominativeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string APIPartnerDirectorNameInGenitive {
            get {
                return this.aPIPartnerDirectorNameInGenitiveField;
            }
            set {
                this.aPIPartnerDirectorNameInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string APIPartnerOnBasicOfInGenitive {
            get {
                return this.aPIPartnerOnBasicOfInGenitiveField;
            }
            set {
                this.aPIPartnerOnBasicOfInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LegalUnitDirectorNameInNominative {
            get {
                return this.legalUnitDirectorNameInNominativeField;
            }
            set {
                this.legalUnitDirectorNameInNominativeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LegalUnitDirectorNameInGenitive {
            get {
                return this.legalUnitDirectorNameInGenitiveField;
            }
            set {
                this.legalUnitDirectorNameInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LegalUnitOnBasicOfInGenitive {
            get {
                return this.legalUnitOnBasicOfInGenitiveField;
            }
            set {
                this.legalUnitOnBasicOfInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AgreementNumber {
            get {
                return this.agreementNumberField;
            }
            set {
                this.agreementNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime AgreementDate {
            get {
                return this.agreementDateField;
            }
            set {
                this.agreementDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime Date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsVATIncluded {
            get {
                return this.isVATIncludedField;
            }
            set {
                this.isVATIncludedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ClicksCount {
            get {
                return this.clicksCountField;
            }
            set {
                this.clicksCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float TotalClicksAmount {
            get {
                return this.totalClicksAmountField;
            }
            set {
                this.totalClicksAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public float RewardAmount {
            get {
                return this.rewardAmountField;
            }
            set {
                this.rewardAmountField = value;
            }
        }
    }
}
