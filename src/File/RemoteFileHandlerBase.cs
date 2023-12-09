// Copyright 2023, Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	public abstract class RemoteFileHandlerBase : FileHandlerBase {

		#region .ctor
		protected RemoteFileHandlerBase() : base() {
		}
		protected RemoteFileHandlerBase( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public sealed override System.Char PathSeparator {
			get {
				return '/';
			}
		}
		#endregion properties


		#region methods
		protected virtual System.Collections.Generic.IEnumerable<System.String> ReadLines( System.Net.WebRequest request ) {
			System.Collections.Generic.ICollection<System.String> output = new System.Collections.Generic.List<System.String>();
			using ( var response = request.GetResponse() ) {
				using ( var stream = response.GetResponseStream() ) {
					using ( var reader = new System.IO.StreamReader( stream ) ) {
						System.String line = reader.ReadLine();
						while ( line is object ) {
							output.Add( line );
							line = reader.ReadLine();
						}
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}
