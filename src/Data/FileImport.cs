// Copyright 2023, Timothy J. Bruce

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		TypeName = "dbFileImport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileImport : DbIODescriptorBase, Icod.Wod.IStep {

		#region .ctor
		public FileImport() : base() {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Ignore )]
		public sealed override System.Data.MissingSchemaAction MissingSchemaAction {
			get {
				return base.MissingSchemaAction;
			}
			set {
				base.MissingSchemaAction = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"missingMappingAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Ignore )]
		public sealed override System.Data.MissingMappingAction MissingMappingAction {
			get {
				return base.MissingMappingAction;
			}
			set {
				base.MissingMappingAction = value;
			}
		}

		[System.Xml.Serialization.XmlElement(
			"source",
			Type = typeof( DataFileBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod" )
		]
		public DataFileBase Source {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			if ( System.String.IsNullOrEmpty( this.CommandText ) ) {
				this.WriteRecords( workOrder, this.Source );
			} else {
				this.ExecuteCommand( workOrder, this.Source );
			}
		}
		#endregion methods

	}

}
