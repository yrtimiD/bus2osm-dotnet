// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

// 
//This source code was auto-generated by MonoXSD
//
namespace Schemas {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class osm {
        
        private decimal versionField;
        
        private string generatorField;
        
        private osmNode[] nodeField;
        
        private osmRelation relationField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string generator {
            get {
                return this.generatorField;
            }
            set {
                this.generatorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("node")]
        public osmNode[] node {
            get {
                return this.nodeField;
            }
            set {
                this.nodeField = value;
            }
        }
        
        /// <remarks/>
        public osmRelation relation {
            get {
                return this.relationField;
            }
            set {
                this.relationField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class osmNode {
        
        private sbyte idField;
        
        private bool visibleField;
        
        private decimal latField;
        
        private decimal lonField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool visible {
            get {
                return this.visibleField;
            }
            set {
                this.visibleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lat {
            get {
                return this.latField;
            }
            set {
                this.latField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lon {
            get {
                return this.lonField;
            }
            set {
                this.lonField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class osmRelation {
        
        private sbyte idField1;
        
        private bool visibleField1;
        
        private osmRelationMember[] memberField;
        
        private osmRelationTag[] tagField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte id {
            get {
                return this.idField1;
            }
            set {
                this.idField1 = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool visible {
            get {
                return this.visibleField1;
            }
            set {
                this.visibleField1 = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("member")]
        public osmRelationMember[] member {
            get {
                return this.memberField;
            }
            set {
                this.memberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public osmRelationTag[] tag {
            get {
                return this.tagField;
            }
            set {
                this.tagField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class osmRelationMember {
        
        private string typeField;
        
        private sbyte refField;
        
        private string roleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte @ref {
            get {
                return this.refField;
            }
            set {
                this.refField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role {
            get {
                return this.roleField;
            }
            set {
                this.roleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.1433")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class osmRelationTag {
        
        private string kField;
        
        private string vField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string k {
            get {
                return this.kField;
            }
            set {
                this.kField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string v {
            get {
                return this.vField;
            }
            set {
                this.vField = value;
            }
        }
    }
}
