// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DbIODescriptorBase ) )]
	public abstract class DbDescriptorBase {

		#region fields
		private System.Int32 myCommandTimeout;
		private System.Data.CommandType myCommandType;
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		protected DbDescriptorBase() : base() {
			myCommandType = System.Data.CommandType.Text;
			myCommandTimeout = -2;
		}
		public DbDescriptorBase( Icod.Wod.WorkOrder workOrder ) : this() {
			myWorkOrder = workOrder;
		}
		#endregion .ctor


		#region properties
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

		[System.Xml.Serialization.XmlAttribute(
			"connectionStringName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String ConnectionStringName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"commandText",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String CommandText {
			get;
			set;
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
			get;
			set;
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
		public virtual System.Data.Common.DbConnection CreateConnection( Icod.Wod.WorkOrder workOrder ) {
			if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
			var cn = this.ConnectionStringName;
			var here = ( workOrder.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
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

		protected virtual System.Data.Common.DbCommandBuilder CreateCommandBuilder( Icod.Wod.WorkOrder workOrder, System.Data.Common.DbDataAdapter adapter ) {
			if ( null == adapter ) {
				throw new System.ArgumentNullException( "adapter" );
			} else if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}

			var cn = this.ConnectionStringName;
			var here = ( workOrder.ConnectionStrings ?? new Icod.Wod.ConnectionStringEntry[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( cn, System.StringComparison.OrdinalIgnoreCase )
			);
			var there = System.Configuration.ConfigurationManager.ConnectionStrings[ cn ];
			return ( null == here )
				? there.CreateCommandBuilder( adapter )
				: here.CreateCommandBuilder( adapter )
			;
		}
		#endregion methods

	}

}
