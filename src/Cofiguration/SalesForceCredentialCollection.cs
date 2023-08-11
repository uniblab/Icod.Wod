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
