// Icod.Wod is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		TypeName = "dbFileExport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileExport : DbIODescriptorBase, Icod.Wod.IStep {

		#region .ctor
		public FileExport() : base() {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
		}
		public FileExport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Add )]
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
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Passthrough )]
		public sealed override System.Data.MissingMappingAction MissingMappingAction {
			get {
				return base.MissingMappingAction;
			}
			set {
				base.MissingMappingAction = value;
			}
		}

		[System.Xml.Serialization.XmlElement(
			"destination", 
			Type = typeof( DataFileBase ), 
			IsNullable = false, 
			Namespace = "http://Icod.Wod" )
		]
		public DataFileBase Destination {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			this.Destination.WriteRecords( workOrder, this );
		}
		#endregion methods

	}

}
