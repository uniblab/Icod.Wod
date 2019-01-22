using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileImport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileImport : DbFileBase {

		#region .ctor
		public FileImport() : base() {
		}
		public FileImport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
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

		[System.Xml.Serialization.XmlArray(
			ElementName = "parameters",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"parameter",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public DbParameter[] Parameters {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			if ( System.String.IsNullOrEmpty( this.CommandText ) ) {
				this.WriteRecords( workOrder, this.Source );
			} else {
				this.ExecuteCommand( workOrder, this.Source );
			}
		}

		public void ExecuteCommand( Icod.Wod.WorkOrder workOrder, DataFileBase source ) {
			using ( var connection = this.CreateConnection( workOrder ) ) {
				connection.Open();
				foreach ( var table in this.Source.ReadTables( workOrder ) ) {
					using ( var command = this.CreateCommand( connection ) ) {
						foreach ( var parameter in ( this.Parameters ?? new DbParameter[ 0 ] ) ) {
							command.Parameters.Add( parameter.ToDbParameter( command ) );
						}
						this.ExecuteCommand( command, table );
					}
					table.Dispose();
				}
			}
		}
		private void ExecuteCommand( System.Data.Common.DbCommand command, System.Data.DataTable source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == command ) {
				throw new System.ArgumentNullException( "command" );
			}
			this.ExecuteCommand( command, source, this.BuildSourceToParameter( source ) );
		}
		private System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Data.DataColumn, DbParameter>> BuildSourceToParameter( System.Data.DataTable source ) {
			if ( null == source ) {
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
		private void ExecuteCommand( System.Data.Common.DbCommand command, System.Data.DataTable source, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Data.DataColumn, DbParameter>> columnParameterMap ) {
			if ( null == columnParameterMap ) {
				throw new System.ArgumentNullException( "columnParameterMap" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == command ) {
				throw new System.ArgumentNullException( "command" );
			}

			foreach ( var row in source.Rows.OfType<System.Data.DataRow>() ) {
				foreach ( var kvp in columnParameterMap ) {
					command.Parameters[ kvp.Value.ParameterName ].Value = row[ kvp.Key.ColumnName ] ?? System.DBNull.Value;
				}
				command.ExecuteNonQuery();
			}
		}
		#endregion methods

	}

}