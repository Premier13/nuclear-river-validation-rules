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
namespace NuClear.ValidationRules.Import.Model.CommonFormat.flowFinancialData.LegalUnit {
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class LegalUnit {

        private LegalEntityBranch[] legalEntityBranchesField;

        private long codeField;

        private string nameField;

        private string legalAddressField;

        private string tINField;

        private string additionalRequisitesField;

        private long taxSchemaCodeField;

        private int contributionTypeCodeField;

        private bool isHiddenField;

        public LegalUnit() {
            this.isHiddenField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public LegalEntityBranch[] LegalEntityBranches {
            get {
                return this.legalEntityBranchesField;
            }
            set {
                this.legalEntityBranchesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long Code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LegalAddress {
            get {
                return this.legalAddressField;
            }
            set {
                this.legalAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TIN {
            get {
                return this.tINField;
            }
            set {
                this.tINField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AdditionalRequisites {
            get {
                return this.additionalRequisitesField;
            }
            set {
                this.additionalRequisitesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long TaxSchemaCode {
            get {
                return this.taxSchemaCodeField;
            }
            set {
                this.taxSchemaCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ContributionTypeCode {
            get {
                return this.contributionTypeCodeField;
            }
            set {
                this.contributionTypeCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IsHidden {
            get {
                return this.isHiddenField;
            }
            set {
                this.isHiddenField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LegalEntityBranch {

        private ExecutorSigner[] executorSignersField;

        private long codeField;

        private string legalEntityBranchCode1CField;

        private string registrationCertificateField;

        private long organizationUnitCodeField;

        private string shortNameField;

        private string additionalRequisitesField;

        private string paymentDetailsField;

        private string actualAddressField;

        private string emailField;

        private bool isDefaultField;

        private bool isDefaultForRegionalSalesField;

        private bool isHiddenField;

        public LegalEntityBranch() {
            this.isHiddenField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ExecutorSigner[] ExecutorSigners {
            get {
                return this.executorSignersField;
            }
            set {
                this.executorSignersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long Code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LegalEntityBranchCode1C {
            get {
                return this.legalEntityBranchCode1CField;
            }
            set {
                this.legalEntityBranchCode1CField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RegistrationCertificate {
            get {
                return this.registrationCertificateField;
            }
            set {
                this.registrationCertificateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long OrganizationUnitCode {
            get {
                return this.organizationUnitCodeField;
            }
            set {
                this.organizationUnitCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ShortName {
            get {
                return this.shortNameField;
            }
            set {
                this.shortNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AdditionalRequisites {
            get {
                return this.additionalRequisitesField;
            }
            set {
                this.additionalRequisitesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PaymentDetails {
            get {
                return this.paymentDetailsField;
            }
            set {
                this.paymentDetailsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ActualAddress {
            get {
                return this.actualAddressField;
            }
            set {
                this.actualAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Email {
            get {
                return this.emailField;
            }
            set {
                this.emailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsDefault {
            get {
                return this.isDefaultField;
            }
            set {
                this.isDefaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsDefaultForRegionalSales {
            get {
                return this.isDefaultForRegionalSalesField;
            }
            set {
                this.isDefaultForRegionalSalesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IsHidden {
            get {
                return this.isHiddenField;
            }
            set {
                this.isHiddenField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ExecutorSigner {

        private long codeField;

        private string positionInNominativeField;

        private string positionInGenitiveField;

        private string nameInNominativeField;

        private string nameInGenitiveField;

        private string onBasicOfInGenitiveField;

        private bool isMainSignerField;

        private bool isMainSignerFieldSpecified;

        private bool isHiddenField;

        private bool isHiddenFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long Code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PositionInNominative {
            get {
                return this.positionInNominativeField;
            }
            set {
                this.positionInNominativeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PositionInGenitive {
            get {
                return this.positionInGenitiveField;
            }
            set {
                this.positionInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NameInNominative {
            get {
                return this.nameInNominativeField;
            }
            set {
                this.nameInNominativeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NameInGenitive {
            get {
                return this.nameInGenitiveField;
            }
            set {
                this.nameInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OnBasicOfInGenitive {
            get {
                return this.onBasicOfInGenitiveField;
            }
            set {
                this.onBasicOfInGenitiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsMainSigner {
            get {
                return this.isMainSignerField;
            }
            set {
                this.isMainSignerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsMainSignerSpecified {
            get {
                return this.isMainSignerFieldSpecified;
            }
            set {
                this.isMainSignerFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IsHidden {
            get {
                return this.isHiddenField;
            }
            set {
                this.isHiddenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsHiddenSpecified {
            get {
                return this.isHiddenFieldSpecified;
            }
            set {
                this.isHiddenFieldSpecified = value;
            }
        }
    }
}
