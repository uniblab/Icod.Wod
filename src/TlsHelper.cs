namespace Icod.Wod {
	public static class TlsHelper {
		public static System.Net.SecurityProtocolType GetSecurityProtocol() {
#if NET48_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
			var ssl = System.Net.SecurityProtocolType.Tls13;
#else
			var ssl = System.Net.SecurityProtocolType.Tls12;
#endif
#if DEBUG
#pragma warning disable 0618
			ssl = ssl | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
#pragma warning restore 0618
#if NET48_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
			ssl = ssl | System.Net.SecurityProtocolType.Tls12;
#endif
#endif
			return ssl;
		}

	}

}
