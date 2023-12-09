// Copyright (C) 2023  Timothy J. Bruce

namespace Icod.Wod.Configuration {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		IncludeInSchema = false,
		Namespace = "http://Icod.Wod"
	)]
	[System.Configuration.ConfigurationCollection( typeof( SalesForceCredentialElement ) )]
	public sealed class SalesForceCredentialCollection : System.Configuration.ConfigurationElementCollection {

		#region .ctor
		public SalesForceCredentialCollection() : base() {
		}
		public SalesForceCredentialCollection( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region properties
		public override System.Configuration.ConfigurationElementCollectionType CollectionType {
			get {
				return System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}
		public SalesForceCredentialElement this[ System.Int32 index ] {
			get {
				return (SalesForceCredentialElement)this.BaseGet( index );
			}
			set {
				if ( this.BaseGet( index ) is object ) {
					this.BaseRemoveAt( index );
				}
				this.BaseAdd( index, value );
			}
		}
		new public SalesForceCredentialElement this[ System.String clientId ] {
			get {
				return (SalesForceCredentialElement)this.BaseGet( clientId );
			}
		}
		#endregion properties


		#region methods
		public void Add( Icod.Wod.Configuration.SalesForceCredentialElement element ) {
			this.BaseAdd( element );
		}
		protected sealed override void BaseAdd( System.Int32 index, System.Configuration.ConfigurationElement element ) {
			if ( index == -1 ) {
				base.BaseAdd( element, false );
			} else {
				base.BaseAdd( index, element );
			}
		}
		public void Clear() {
			this.BaseClear();
		}
		protected sealed override System.Configuration.ConfigurationElement CreateNewElement() {
			return new SalesForceCredentialElement();
		}
		protected sealed override System.Configuration.ConfigurationElement CreateNewElement( System.String name ) {
			return new SalesForceCredentialElement {
				Name = name
			};
		}
		protected sealed override System.Object GetElementKey( System.Configuration.ConfigurationElement element ) {
			return ( (SalesForceCredentialElement)element ).Name;
		}

		public System.Int32 IndexOf( SalesForceCredentialElement element ) {
			return base.BaseIndexOf( element );
		}
		public void RemoveAt( System.Int32 index ) {
			base.BaseRemoveAt( index );
		}
		public void Remove( SalesForceCredentialElement element ) {
			var i = base.BaseIndexOf( element );
			if ( 0 <= i ) {
				base.BaseRemoveAt( i );
			}
		}
		public void Remove( System.String name ) {
			base.BaseRemove( name );
		}
		#endregion methods

	}

}
