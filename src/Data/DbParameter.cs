﻿using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"dbParameter",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class DbParameter {

		#region .ctor
		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Name {
			get;
			set;
		}

		public DbParameter() : base() {
			this.DbType = System.Data.DbType.AnsiString;
			this.Direction = System.Data.ParameterDirection.Input;
			this.ParameterName = null;
			this.Precision = 0;
			this.Scale = 0;
			this.Size = 0;
			this.SourceColumn = null;
			this.SourceColumnNullMapping = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"dbType",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.DbType.AnsiString )]
		public System.Data.DbType DbType {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"direction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.ParameterDirection.Input )]
		public System.Data.ParameterDirection Direction {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"parameterName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String ParameterName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"size",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.Int32)0 )]
		public System.Int32 Size {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"scale",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.Byte)0 )]
		public System.Byte Scale {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"sourceColumn",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String SourceColumn {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"precision",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.Byte)0 )]
		public System.Byte Precision {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"sourceColumnNullMapping",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean SourceColumnNullMapping {
			get;
			set;
		}
		#endregion properties


		#region methods
		public System.Data.Common.DbParameter ToDbParameter( System.Data.Common.DbCommand command ) {
			if ( null == command ) {
				throw new System.ArgumentNullException( "command" );
			}

			var output = command.CreateParameter();
			if ( null != output ) {
				output.DbType = this.DbType;
				output.Direction = this.Direction;
				output.ParameterName = this.ParameterName;
				output.Size = this.Size;
				output.SourceColumn = this.SourceColumn;
				output.SourceColumnNullMapping = this.SourceColumnNullMapping;
				var iDbDataParameter = ( output as System.Data.IDbDataParameter );
				if ( null != iDbDataParameter ) {
					iDbDataParameter.Precision = this.Precision;
					iDbDataParameter.Scale = this.Scale;
				}
			}

			return output;
		}
		#endregion methods

	}

}