// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod {

	public static class CodePageHelper {

		public static System.Text.Encoding GetCodePage( System.String codePage ) {
			System.Text.Encoding output = null;

			if ( System.Int32.TryParse( codePage, out var cpNumber ) ) {
				output = System.Text.Encoding.GetEncoding( cpNumber );
			}
			if ( null == output ) {
				output = System.Text.Encoding.GetEncoding( codePage );
			}

			return output;
		}

	}

}
