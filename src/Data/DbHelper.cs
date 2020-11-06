using System.Linq;

namespace Icod.Wod.Data {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class DbHelper {

		public static System.Data.Common.DbCommandBuilder CreateCommandBuilder( this System.Configuration.ConnectionStringSettings connectionString, System.Data.Common.DbDataAdapter adapter ) {
			return CreateCommandBuilder( connectionString.ProviderName, adapter );
		}
		public static System.Data.Common.DbCommandBuilder CreateCommandBuilder( this Icod.Wod.ConnectionStringEntry connectionString, System.Data.Common.DbDataAdapter adapter ) {
			return CreateCommandBuilder( connectionString.ProviderName, adapter );
		}
		public static System.Data.Common.DbCommandBuilder CreateCommandBuilder( System.String providerName, System.Data.Common.DbDataAdapter adapter ) {
			var output = System.Data.Common.DbProviderFactories.GetFactory(
				providerName
			).CreateCommandBuilder();
			output.DataAdapter = adapter;
			return output;
		}

		public static System.Data.Common.DbDataAdapter CreateDataAdapter( System.String providerName, System.Data.Common.DbCommand command, System.Data.Common.DbConnection connection ) {
			var output = System.Data.Common.DbProviderFactories.GetFactory(
				providerName
			).CreateDataAdapter();
			output.SelectCommand = command;
			return output;
		}
		public static System.Data.Common.DbDataAdapter CreateDataAdapter( this Icod.Wod.ConnectionStringEntry connectionString, System.Data.Common.DbCommand command, System.Data.Common.DbConnection connection ) {
			return CreateDataAdapter( connectionString.ProviderName, command, connection );
		}
		public static System.Data.Common.DbDataAdapter CreateDataAdapter( this System.Configuration.ConnectionStringSettings connectionString, System.Data.Common.DbCommand command, System.Data.Common.DbConnection connection ) {
			return CreateDataAdapter( connectionString.ProviderName, command, connection );
		}

		public static System.Data.Common.DbDataAdapter CreateDataAdapter( System.String providerName, System.String selectCommand, System.Data.Common.DbConnection connection ) {
			var output = System.Data.Common.DbProviderFactories.GetFactory(
				providerName
			).CreateDataAdapter();
			output.SelectCommand = CreateCommand( connection, null, selectCommand, System.Data.CommandType.Text, connection.ConnectionTimeout );
			return output;
		}
		public static System.Data.Common.DbDataAdapter CreateDataAdapter( this Icod.Wod.ConnectionStringEntry connectionString, System.String selectCommand, System.Data.Common.DbConnection connection ) {
			return CreateDataAdapter( connectionString.ProviderName, selectCommand, connection );
		}
		public static System.Data.Common.DbDataAdapter CreateDataAdapter( this System.Configuration.ConnectionStringSettings connectionString, System.String selectCommand, System.Data.Common.DbConnection connection ) {
			return CreateDataAdapter( connectionString.ProviderName, selectCommand, connection );
		}

		public static System.Data.Common.DbConnection CreateConnection( System.String connectionString ) {
			return CreateConnection( new ConnectionStringEntry {
				ConnectionString = connectionString,
				ProviderName = "System.Data.SqlClient"
			} );
		}
		public static System.Data.Common.DbConnection CreateConnection( this Icod.Wod.ConnectionStringEntry connectionString ) {
			if ( null == connectionString ) {
				throw new System.ArgumentNullException( "connectionString" );
			}
			try {
				var output = System.Data.Common.DbProviderFactories.GetFactory(
					connectionString.ProviderName
				).CreateConnection();
				output.ConnectionString = connectionString.ConnectionString;
				return output;
			} catch ( System.Exception ex ) {
				ex.Data.Add( "connectionString", connectionString.GetType().AssemblyQualifiedName );
				ex.Data.Add( "connectionString.Name", connectionString.Name );
				ex.Data.Add( "connectionString.ProviderName", connectionString.ProviderName );
				ex.Data.Add( "connectionString.ProviConnectionStringderName", connectionString.ConnectionString );
				throw;
			}
		}
		public static System.Data.Common.DbConnection CreateConnection( this System.Configuration.ConnectionStringSettings connectionString ) {
			if ( null == connectionString ) {
				throw new System.ArgumentNullException( "connectionString" );
			}
			try {
				var output = System.Data.Common.DbProviderFactories.GetFactory(
					connectionString.ProviderName
				).CreateConnection();
				output.ConnectionString = connectionString.ConnectionString;
				return output;
			} catch ( System.Exception ex ) {
				ex.Data.Add( "connectionString", connectionString.GetType().AssemblyQualifiedName );
				ex.Data.Add( "connectionString.Name", connectionString.Name );
				ex.Data.Add( "connectionString.ProviderName", connectionString.ProviderName );
				ex.Data.Add( "connectionString.ProviConnectionStringderName", connectionString.ConnectionString );
				throw;
			}
		}

		public static System.Data.Common.DbCommand CreateCommand( this System.Data.Common.DbConnection connection ) {
			if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}
			var command = connection.CreateCommand();
			command.CommandTimeout = connection.ConnectionTimeout;
			return command;
		}
		public static System.Data.Common.DbCommand CreateCommand(
			this System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction
		) {
			if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}
			var command = connection.CreateCommand();
			command.Transaction = transaction;
			command.CommandTimeout = connection.ConnectionTimeout;
			return command;
		}
		public static System.Data.Common.DbCommand CreateCommand(
			this System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction,
			System.String commandText, System.Data.CommandType commandType
		) {
			if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}
			var command = connection.CreateCommand();
			command.Transaction = transaction;
			command.CommandText = commandText;
			command.CommandType = commandType;
			command.CommandTimeout = connection.ConnectionTimeout;
			return command;
		}
		public static System.Data.Common.DbCommand CreateCommand(
			this System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction,
			System.String commandText, System.Data.CommandType commandType, System.Nullable<System.Int32> commandTimeout
		) {
			if ( null == connection ) {
				throw new System.ArgumentNullException( "connection" );
			}
			var command = connection.CreateCommand();
			command.Transaction = transaction;
			command.CommandText = commandText;
			command.CommandType = commandType;
			command.CommandTimeout = commandTimeout ?? connection.ConnectionTimeout;
			return command;
		}

	}

}
