namespace Icod.Wod.File {

	public interface IFile {

		System.String Path {
			get;
			set;
		}
		System.String Name {
			get;
			set;
		}
		System.String RegexPattern {
			get;
			set;
		}

		System.IO.SearchOption SearchOption {
			get;
			set;
		}

		System.Boolean Recurse {
			get;
			set;
		}

		System.String Username {
			get;
			set;
		}

		System.String Password {
			get;
			set;
		}

	}

}