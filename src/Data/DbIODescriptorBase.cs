// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileExport ) )]
	public abstract class DbIODescriptorBase : DbOperationBase, ITableSource, ITableDestination {

		#region fields
		private System.Int32 myUpdateBatchSize;
		private System.Data.MissingSchemaAction myMissingSchemaAction;
		private System.Data.MissingMappingAction myMissingMappingAction;
		#endregion fields


		#region .ctor
		protected DbIODescriptorBase() : base() {
			myUpdateBatchSize = 1;
			myMissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			myMissingMappingAction = System.Data.MissingMappingAction.Ignore;
		}
		protected DbIODescriptorBase( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base() {
			myUpdateBatchSize = 1;
			myMissingSchemaAction = missingSchemaAction;
			myMissingMappingAction = missingMappingAction;
		}
		public DbIODescriptorBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myUpdateBatchSize = 1;
			myMissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			myMissingMappingAction = System.Data.MissingMappingAction.Ignore;
		}
		protected DbIODescriptorBase( Icod.Wod.WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder ) {
			myUpdateBatchSize = 1;
			myMissingSchemaAction = missingSchemaAction;
			myMissingMappingAction = missingMappingAction;
		}
		#endregion .ctor


		#region properties
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

		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Ignore )]
		public virtual System.Data.MissingSchemaAction MissingSchemaAction {
			get {
				return myMissingSchemaAction;
			}
			set {
				myMissingSchemaAction = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Ignore )]
		public virtual System.Data.MissingMappingAction MissingMappingAction {
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
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"tableName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String TableName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"schemaQuery",
			Namespace = "http://Icod.Wod"
		)]
		public System.String SchemaQuery {
			get;
			set;
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
		#endregion properties


		#region methods
		protected System.String GenerateSchemaQuery() {
			var sq = this.SchemaQuery.TrimToNull();
			return !System.String.IsNullOrEmpty( sq )
				? sq
				: "select top 0 * from " + this.NamespaceTableName
			;
		}

		protected virtual System.Collections.Generic.IEnumerable<System.Data.Common.DataColumnMapping> CreateDataColumnMapping( System.Data.DataTable source, System.Data.DataTable dest ) {
			if ( dest is null ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			}

			var destNames = dest.Columns.OfType<System.Data.DataColumn>().Select(
				x => x.ColumnName
			).Where(
				x => !( this.ColumnMapping ?? new ColumnMap[ 0 ] ).Where(
					y => y.Skip
				).Select(
					y => y.ToName
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			);
			var sourceNames = source.Columns.OfType<System.Data.DataColumn>().Select(
				x => x.ColumnName
			).Where(
				x => !( this.ColumnMapping ?? new ColumnMap[ 0 ] ).Where(
					y => y.Skip
				).Select(
					y => y.FromName
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			);

			var originalMap = ( this.ColumnMapping ?? new ColumnMap[ 0 ] ).Where(
				x => !x.Skip
			).Where(
				x => destNames.Contains( x.ToName, System.StringComparer.OrdinalIgnoreCase )
			);
			var map = originalMap.Union( sourceNames.Where(
				x => destNames.Contains( x, System.StringComparer.OrdinalIgnoreCase )
			).Where(
				x => !originalMap.Select(
					y => y.FromName
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			).Join(
				destNames,
				q => q,
				w => w,
				( q, w ) => new ColumnMap {
					Skip = false,
					FromName = q,
					ToName = w
				},
				System.StringComparer.OrdinalIgnoreCase
			) );
			var output = map.Where(
				x => sourceNames.Contains( x.FromName, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => new System.Data.Common.DataColumnMapping( x.ToName, x.FromName )
			);
			return output;
		}

		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder workOrder ) {
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			} else if ( connection is null ) {
				throw new System.ArgumentNullException( "connection" );
			}

			return this.CreateDataAdapter( connection, workOrder, this.GenerateSchemaQuery() );
		}
		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder workOrder, System.String schemaQuery ) {
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			} else if ( connection is null ) {
				throw new System.ArgumentNullException( "connection" );
			}

			schemaQuery = this.GenerateSchemaQuery();
			var cn = this.ConnectionStringName;
			var here = ( workOrder.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			var output = ( here is null )
				? there.CreateDataAdapter( schemaQuery, connection )
				: here.CreateDataAdapter( schemaQuery, connection )
			;
			output.UpdateBatchSize = this.UpdateBatchSize;
			output.MissingMappingAction = this.MissingMappingAction;
			output.MissingSchemaAction = this.MissingSchemaAction;
			return output;
		}
		protected virtual System.Data.Common.DbDataAdapter CreateDataAdapter( System.Data.Common.DbConnection connection, Icod.Wod.WorkOrder workOrder, System.Data.Common.DbCommand command ) {
			if ( command is null ) {
				throw new System.ArgumentNullException( "command" );
			} else if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			} else if ( connection is null ) {
				throw new System.ArgumentNullException( "connection" );
			}

			var cn = this.ConnectionStringName;
			var here = ( workOrder.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			var output = ( here is null )
				? there.CreateDataAdapter( command )
				: here.CreateDataAdapter( command )
			;
			output.UpdateBatchSize = this.UpdateBatchSize;
			output.MissingMappingAction = this.MissingMappingAction;
			output.MissingSchemaAction = this.MissingSchemaAction;
			return output;
		}

		public virtual System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder workOrder ) {
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			}

			using ( var connection = this.CreateConnection( workOrder ) ) {
				using ( var command = this.CreateCommand( connection ) ) {
					using ( var adapter = this.CreateDataAdapter( connection, workOrder, command ) ) {
						using ( var set = new System.Data.DataSet() ) {
							_ = adapter.Fill( set );
							return set.Tables.OfType<System.Data.DataTable>();
						}
					}
				}
			}
		}
		public virtual void WriteRecords( Icod.Wod.WorkOrder workOrder, ITableSource source ) {
			if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( workOrder is null ) {
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
								if ( !System.String.IsNullOrEmpty( this.SchemaQuery ) ) {
									using ( var ic = cb.GetInsertCommand() ) {
										var ct = this.CommandTimeout;
										ic.CommandTimeout = ( -2 == ct ) ? cnxn.ConnectionTimeout : ct;
										adapter.InsertCommand = ic;
										_ = adapter.Update( t );
									}
								} else {
									_ = adapter.Update( t );
								}
								t.Dispose();
							}
						}
					}
				}
			}
		}

		protected virtual void FillSchema( System.Data.Common.DbDataAdapter adapter, System.Data.DataSet set ) {
			if ( set is null ) {
				throw new System.ArgumentNullException( "set" );
			} else if ( adapter is null ) {
				throw new System.ArgumentNullException( "adapter" );
			}

			_ = adapter.FillSchema( set, System.Data.SchemaType.Source, this.NamespaceTableName );
		}
		protected virtual void FillSchema( System.Data.Common.DbDataAdapter adapter, System.Data.DataTable table ) {
			if ( table is null ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( adapter is null ) {
				throw new System.ArgumentNullException( "adapter" );
			}

			_ = adapter.FillSchema( table, System.Data.SchemaType.Source );
		}

		public void ExecuteCommand( Icod.Wod.WorkOrder workOrder, ITableSource source ) {
			using ( var connection = this.CreateConnection( workOrder ) ) {
				connection.Open();
				foreach ( var table in source.ReadTables( workOrder ) ) {
					using ( var command = this.CreateCommand( connection ) ) {
						foreach ( var parameter in ( this.Parameters ?? new DbParameter[ 0 ] ) ) {
							_ = command.Parameters.Add( parameter.ToDbParameter( workOrder, command ) );
						}
						this.ExecuteCommand( command, table );
					}
					table.Dispose();
				}
			}
		}
		private void ExecuteCommand( System.Data.Common.DbCommand command, System.Data.DataTable source ) {
			if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( command is null ) {
				throw new System.ArgumentNullException( "command" );
			}
			this.ExecuteCommand( command, source, this.BuildSourceToParameter( source ) );
		}

		protected System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Data.DataColumn, DbParameter>> BuildSourceToParameter( System.Data.DataTable source ) {
			if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			}
			var parameters = this.Parameters ?? new DbParameter[ 0 ];
			if ( !parameters.Any() ) {
				return null;
			}
			var sourceColumns = source.Columns.OfType<System.Data.DataColumn>();
			if ( !sourceColumns.Any() ) {
				throw new System.InvalidOperationException();
			}
			var columnMaps = this.ColumnMapping ?? new ColumnMap[ 0 ];

			var sourceToColMap = new System.Collections.Generic.Dictionary<System.Data.DataColumn, ColumnMap>( System.Math.Max( sourceColumns.Count(), columnMaps.Count() ) );
			foreach ( var pair in sourceColumns.Join(
				columnMaps.Where(
					x => !x.Skip
				),
				x => x.ColumnName,
				y => y.FromName,
				( x, y ) => new {
					DataColumn = x,
					ColumnMap = y
				},
				System.StringComparer.OrdinalIgnoreCase
			) ) {
				sourceToColMap.Add( pair.DataColumn, pair.ColumnMap );
			}
			System.String sourceName;
			foreach ( var sourceColumn in sourceColumns.Where(
				x => !sourceToColMap.ContainsKey( x )
			) ) {
				sourceName = sourceColumn.ColumnName;
				sourceToColMap.Add(
					sourceColumn,
					new ColumnMap {
						ToName = sourceName,
						FromName = sourceName
					}
				);
			}

			var colMapToParameter = new System.Collections.Generic.Dictionary<ColumnMap, DbParameter>();
			foreach ( var pair in sourceToColMap.Values.Join(
				parameters,
				x => x.ToName,
				y => y.Name,
				( x, y ) => new {
					ColumnMap = x,
					Parameter = y
				},
				System.StringComparer.OrdinalIgnoreCase
			) ) {
				colMapToParameter.Add( pair.ColumnMap, pair.Parameter );
			}

			return sourceToColMap.Where(
				x => colMapToParameter.ContainsKey( x.Value )
			).Select(
				x => new System.Collections.Generic.KeyValuePair<System.Data.DataColumn, DbParameter>(
					x.Key, colMapToParameter[ x.Value ]
				)
			);
		}
		protected void ExecuteCommand( System.Data.Common.DbCommand command, System.Data.DataTable source, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Data.DataColumn, DbParameter>> columnParameterMap ) {
			if ( columnParameterMap is null ) {
				throw new System.ArgumentNullException( "columnParameterMap" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( command is null ) {
				throw new System.ArgumentNullException( "command" );
			}

			foreach ( var row in source.Rows.OfType<System.Data.DataRow>() ) {
				foreach ( var kvp in columnParameterMap ) {
					command.Parameters[ kvp.Value.ParameterName ].Value = row[ kvp.Key.ColumnName ] ?? System.DBNull.Value;
				}
				_ = command.ExecuteNonQuery();
			}
		}
		#endregion methods

	}

}
