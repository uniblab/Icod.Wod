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

namespace Icod.Wod.Configuration {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		IncludeInSchema = false,
		Namespace = "http://Icod.Wod"
	)]
	public sealed class SalesForceCredentialSection : System.Configuration.ConfigurationSection {

		#region fields
		public const System.String DefaultSectionName = "icod.wod.sfCredentials";
		#endregion fields


		#region .ctor
		public SalesForceCredentialSection() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Configuration.ConfigurationProperty( "", IsDefaultCollection = true, IsRequired = false )]
		[System.Configuration.ConfigurationCollection( typeof( SalesForceCredentialCollection ),
			AddItemName = "add",
			ClearItemsName = "clear",
			RemoveItemName = "remove"
		)]
		public SalesForceCredentialCollection Credentials {
			get {
				return (SalesForceCredentialCollection)base[ "" ];
			}
		}
		#endregion properties


		#region static methods
		public static SalesForceCredentialSection GetSection() {
			return ( System.Configuration.ConfigurationManager.GetSection( SalesForceCredentialSection.DefaultSectionName ) as SalesForceCredentialSection );
		}
		#endregion static methods

	}

}
