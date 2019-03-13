using System.Linq;

namespace Icod.Wod.Configuration {

	[System.Serializable]
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
				if ( null != this.BaseGet( index ) ) {
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
		protected sealed override System.Configuration.ConfigurationElement CreateNewElement() {
			return new SalesForceCredentialElement();
		}
		protected sealed override System.Object GetElementKey( System.Configuration.ConfigurationElement element ) {
			return ( (SalesForceCredentialElement)element ).ClientId;
		}

		protected sealed override System.Configuration.ConfigurationElement CreateNewElement( System.String elementName ) {
			return new SalesForceCredentialElement {
				ClientId = elementName
			};
		}
		#endregion methods

	}

}