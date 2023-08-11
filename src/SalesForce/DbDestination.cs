// Icod.Wod.dll is the Work on Demand framework.
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

namespace Icod.Wod.SalesForce {

	[System.Serializable]
	public sealed class DbDestination : Icod.Wod.Data.DbIODescriptorBase {

		#region .ctor
		public DbDestination() : base() {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
		}
		public DbDestination( WorkOrder workOrder ) : this() {
			this.WorkOrder = workOrder;
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
		#endregion properties


		#region methods
		public sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, Icod.Wod.Data.ITableSource source ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			if ( System.String.IsNullOrEmpty( this.CommandText ) ) {
				this.WriteRecordsOverride( workOrder, source );
			} else {
				this.ExecuteCommand( workOrder, source );
			}
		}
		private void WriteRecordsOverride( Icod.Wod.WorkOrder workOrder, Icod.Wod.Data.ITableSource source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}

			using ( var cnxn = this.CreateConnection( workOrder ) ) {
				using ( var adapter = this.CreateDataAdapter( cnxn, workOrder ) ) {
					var amap = adapter.TableMappings;
					using ( var cb = this.CreateCommandBuilder( workOrder, adapter ) ) {
						System.Data.Common.DataTableMapping tmap;
						using ( var set = new System.Data.DataSet() ) {
							this.FillSchema( adapter, set );
							var tableName = this.NamespaceTableName;
							var dest = set.Tables[ tableName ];
							foreach ( var t in source.ReadTables( workOrder ) ) {
								if ( System.String.IsNullOrEmpty( t.TableName ) && !System.String.IsNullOrEmpty( dest.TableName ) ) {
									t.TableName = dest.TableName;
								}
								amap.Clear();
								tmap = amap.Add( "Table", t.TableName );
								foreach ( var cmap in this.CreateDataColumnMapping( t, dest ) ) {
									_ = tmap.ColumnMappings.Add( cmap );
								}
								_ = adapter.Update( t );
								t.Dispose();
							}
						}
					}
				}
			}
		}
		#endregion methods

	}

}
