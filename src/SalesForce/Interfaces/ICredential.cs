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

namespace Icod.Wod.SalesForce {

	public interface ICredential {

		System.String Name {
			get;
		}
		System.String ClientId {
			get;
			set;
		}
		SalesForce.LoginMode LoginMode {
			get;
			set;
		}
		System.String ClientSecret {
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
		System.String SecurityToken {
			get;
			set;
		}
		System.String Scheme {
			get;
			set;
		}
		System.String Host {
			get;
			set;
		}
		System.Int32 Port {
			get;
			set;
		}
		System.String Path {
			get;
			set;
		}
		System.Uri SiteUrl {
			get;
		}

		System.String CallbackUrl {
			get;
			set;
		}
		System.String RefreshToken {
			get;
			set;
		}

	}

}
