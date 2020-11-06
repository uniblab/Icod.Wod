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
