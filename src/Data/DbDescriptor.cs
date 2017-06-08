using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public abstract class DbDescriptor : ITableSource, ITableDestination {

		#region fields
		private System.String myConnectionStringName;
		private System.String myCommandText;
		private System.Int32 myCommandTimeout;
		private System.Data.CommandType myCommandType;
		private System.String myNamespace;
		private System.Int32 myUpdateBatchSize;
		private System.Data.MissingSchemaAction myMissingSchemaAction;
		private System.Data.MissingMappingAction myMissingMappingAction;
		private ColumnMap[] myColumnMapping;
		private System.String myTableName;
		private System.String mySchemaQuery;
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		protected DbDescriptor() : base() {
			myCommandType = System.Data.CommandType.Text;
			myCommandTimeout = -2;
			myUpdateBatchSize = 1;
			myMissingSchemaAction = System.Data.MissingSchemaAction.Add;
			myMissingMappingAction = System.Data.MissingMappingAction.Passthrough;
		}
		public DbDescriptor( Icod.Wod.WorkOrder workOrder ) : this() {
			myWorkOrder = workOrder;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"connectionStringName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String ConnectionStringName {
			get {
				return myConnectionStringName;
			}
			set {
				myConnectionStringName = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"commandText",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String CommandText {
			get {
				return myCommandText;
			}
			set {
				myCommandText = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"commandTimeout",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( -2 )]
		public System.Int32 CommandTimeout {
			get {
				return myCommandTimeout;
			}
			set {
				myCommandTimeout = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"commandType",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.CommandType.Text )]
		public System.Data.CommandType CommandType {
			get {
				return myCommandType;
			}
			set {
				myCommandType = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"namespace",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Namespace {
			get {
				return myNamespace;
			}
			set {
				myNamespace = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"updateBatchSize",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 1 )]
		public System.Int32 UpdateBatchSize {
			get {
				return myUpdateBatchSize;
			}
			set {
				myUpdateBatchSize = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Add )]
		public System.Data.MissingSchemaAction MissingSchemaAction {
			get {
				return myMissingSchemaAction;
			}
			set {
				myMissingSchemaAction = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"missingMappingAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Passthrough )]
		public System.Data.MissingMappingAction MissingMappingAction {
			get {
				return myMissingMappingAction;
			}
			set {
				myMissingMappingAction = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			"columnMapping",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"map",
			typeof( ColumnMap ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public ColumnMap[] ColumnMapping {
			get {
				return myColumnMapping;
			}
			set {
				myColumnMapping = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"tableName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String TableName {
			get {
				return myTableName;
			}
			set {
				myTableName = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"schemaQuery",
			Namespace = "http://Icod.Wod"
		)]
		public System.String SchemaQuery {
			get {
				return mySchemaQuery;
			}
			set {
				mySchemaQuery = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.String NamespaceTableName {
			get {
				var ns = this.Namespace;
				return !System.String.IsNullOrEmpty( ns )
					? ns + "." + this.TableName
					: this.TableName
				;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public Icod.Wod.WorkOrder WorkOrder {
			get {
				return myWorkOrder;
			}
			set {
				myWorkOrder = value;
			}
		}
		#endregion properties


		#region methods
		public virtual System.Data.Common.DbConnection CreateConnection( Icod.Wod.WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}
			var cn = this.ConnectionStringName;
			var here = ( order.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			var cnxn = ( null == here )
				? there.CreateConnection()
				: here.CreateConnection()
			;
			cnxn.Open();
			return cnxn;
		}
		public virtual System.Data.Common.DbCommand CreateCommand( System.Data.Common.DbConnection connection ) {
			if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}
			var timeout = this.CommandTimeout;
			return connection.CreateCommand( null, this.CommandText, this.CommandType, ( -2 == timeout ) ? connection.ConnectionTimeout : timeout );
		}

		public virtual System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			using ( var connection = this.CreateConnection( order ) ) {
				using ( var command = this.CreateCommand( connection ) ) {
					using ( var adapter = this.CreateDataAdapter( connection, order, command ) ) {
						using ( var set = new System.Data.DataSet() ) {
							adapter.Fill( set );
							foreach ( var table in set.Tables.OfType<System.Data.DataTable>() ) {
								using ( var t = table ) {
									yield return table;
								}
							}
						}
					}
				}
			}
		}
		public virtual void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			using ( var cnxn = this.CreateConnection( order ) ) {
				using ( var adapter = this.CreateDataAdapter( cnxn, order ) ) {
					var amap = adapter.TableMappings;
					using ( var cb = this.CreateCommandBuilder( order, adapter ) ) {
						System.Data.Common.DataTableMapping tmap;
						System.Data.DataTable dest;
						using ( var set = new System.Data.DataSet() ) {
							this.FillSchema( adapter, set );
							System.String tableName = this.TableName;
							dest = set.Tables[ tableName ];
							foreach ( var t in source.ReadTables( order ) ) {
								amap.Clear();
								tmap = amap.Add( tableName, t.TableName );
								foreach ( var cmap in this.CreateDataColumnMapping( t, dest ) ) {
									tmap.ColumnMappings.Add( cmap );
								}
								adapter.Update( t );
							}
						}
					}
				}
			}
		}

		protected System.String GenerateSchemaQuery() {
			var map = ( this.ColumnMapping ?? new ColumnMap[ 0 ] ).Where(
				x => !x.Skip
			);
			var sq = this.SchemaQuery;
			return !System.String.IsNullOrEmpty( sq )
				? sq
				: !map.Any()
					? "select top 0 * from " + this.NamespaceTableName
					: "select top 0 " + System.String.Join( ", ", map.Select( x => x.ToName ) ) + " from " + this.NamespaceTableName
			;
		}
		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			} else if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}

			return this.CreateDataAdapter( connection, order, this.GenerateSchemaQuery() );
		}
		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder order, System.String schemaQuery ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			} else if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}

			schemaQuery = schemaQuery.TrimToNull();
			if ( System.String.IsNullOrEmpty( schemaQuery ) ) {
				schemaQuery = this.GenerateSchemaQuery();
			}
			var cn = this.ConnectionStringName;
			var here = ( order.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			var output = ( null == here )
				? there.CreateDataAdapter( schemaQuery, connection )
				: here.CreateDataAdapter( schemaQuery, connection )
			;
			output.UpdateBatchSize = this.UpdateBatchSize;
			output.MissingMappingAction = this.MissingMappingAction;
			output.MissingSchemaAction = this.MissingSchemaAction;
			return output;
		}
		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder order, System.Data.Common.DbCommand command ) {
			if ( null == command ) {
				throw new System.ArgumentNullException( "command" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			} else if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}

			var cn = this.ConnectionStringName;
			var here = ( order.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			var output = ( null == here )
				? there.CreateDataAdapter( command, connection )
				: here.CreateDataAdapter( command, connection )
			;
			output.UpdateBatchSize = this.UpdateBatchSize;
			output.MissingMappingAction = this.MissingMappingAction;
			output.MissingSchemaAction = this.MissingSchemaAction;
			return output;
		}
		protected virtual System.Data.Common.DbCommandBuilder CreateCommandBuilder( Icod.Wod.WorkOrder order, System.Data.Common.DbDataAdapter adapter ) {
			if ( null == adapter ) {
				throw new System.ArgumentNullException( "adapter" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			var cn = this.ConnectionStringName;
			var here = ( order.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			return ( null == here )
				? there.CreateCommandBuilder( adapter )
				: here.CreateCommandBuilder( adapter )
			;
		}

		protected virtual void FillSchema( System.Data.Common.DbDataAdapter adapter, System.Data.DataSet set ) {
			if ( null == set ) {
				throw new System.ArgumentNullException( "set" );
			} else if ( null == adapter ) {
				throw new System.ArgumentNullException( "adapter" );
			}

			adapter.FillSchema( set, System.Data.SchemaType.Source, this.TableName );
		}
		protected virtual void FillSchema( System.Data.Common.DbDataAdapter adapter, System.Data.DataTable table ) {
			if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( null == adapter ) {
				throw new System.ArgumentNullException( "adapter" );
			}

			adapter.FillSchema( table, System.Data.SchemaType.Source );
		}

		protected virtual System.Collections.Generic.IEnumerable<System.Data.Common.DataColumnMapping> CreateDataColumnMapping( System.Data.DataTable source, System.Data.DataTable dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}

			var destCol = dest.Columns.OfType<System.Data.DataColumn>();
			var sourceCol = source.Columns.OfType<System.Data.DataColumn>();
			var columnMapping = ( this.ColumnMapping ?? new ColumnMap[ 0 ] ).Where(
				x => !x.Skip
			).Where(
				x => sourceCol.Select(
					y => y.ColumnName
				).Contains( x.FromName, System.StringComparer.OrdinalIgnoreCase )
			);
			var output = columnMapping.Select(
				x => new System.Data.Common.DataColumnMapping( x.ToName, x.FromName )
			).Union( sourceCol.Where(
				x => !columnMapping.Select(
					y => y.FromName
				).Contains( x.ColumnName, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => new System.Data.Common.DataColumnMapping( x.ColumnName, x.ColumnName )
			) ).Where(
				x => destCol.Select(
					y => y.ColumnName
				).Contains( x.SourceColumn, System.StringComparer.OrdinalIgnoreCase )
			);

			var addedCol = destCol.Where(
				x => !output.Select(
					y => y.SourceColumn
				).Contains( x.ColumnName, System.StringComparer.OrdinalIgnoreCase )
			);
			System.Data.DataColumn a;
			foreach ( var ac in addedCol ) {
				a = new System.Data.DataColumn( ac.ColumnName, typeof( System.String ) );
				a.AllowDBNull = true;
				a.DefaultValue = ac.DefaultValue;
				a.ReadOnly = true;
				source.Columns.Add( a );
			}
			output = output.Union( addedCol.Select(
				x => new System.Data.Common.DataColumnMapping( x.ColumnName, x.ColumnName )
			) );
			return output;
		}
		#endregion methods

	}

}