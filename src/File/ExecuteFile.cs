using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"executeFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ExecuteFile : FileOperationBase {

		#region fields
		private System.Int32 mySuccessExitCode;
		private System.String myArgs;
		private FileRedirection myStdErr;
		private FileRedirection myStdOut;
		#endregion fields


		#region .ctor
		public ExecuteFile() : base() {
			mySuccessExitCode = 0;
			myArgs = null;
			myStdErr = null;
			myStdOut = null;
		}
		public ExecuteFile( WorkOrder workOrder ) : base( workOrder ) {
			mySuccessExitCode = 0;
			myArgs = null;
			myStdErr = null;
			myStdOut = null;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"args",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Args {
			get {
				return myArgs;
			}
			set {
				myArgs = value.TrimToNull();
			}
		}

		public System.String ExpandedArgs {
			get {
				return this.WorkOrder.ExpandVariables( myArgs.TrimToNull() );
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"workingDirectory",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String WorkingDirectory {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"successExitCode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 SuccessExitCode {
			get {
				return mySuccessExitCode;
			}
			set {
				mySuccessExitCode = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"requireSuccessExitCode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean RequireSuccessExitCode {
			get;
			set;
		}

		[ System.Xml.Serialization.XmlElement(
			"stdOut",
			Namespace = "http://Icod.Wod"
		)]
		public FileRedirection StdOut {
			get {
				return myStdOut;
			}
			set {
				myStdOut = value;
			}
		}

		[System.Xml.Serialization.XmlElement(
			"stdErr",
			Namespace = "http://Icod.Wod"
		)]
		public FileRedirection StdErr {
			get {
				return myStdErr;
			}
			set {
				myStdErr = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );

			var prog = new LocalFileHandler( workOrder, this ).PathCombine( this.ExpandedPath, this.ExpandedName ).TrimToNull();
			if ( System.String.IsNullOrEmpty( prog ) ) {
				throw new System.InvalidOperationException();
			}
			var wd = this.WorkingDirectory.TrimToNull() ?? System.Environment.CurrentDirectory;
			var args = this.ExpandedArgs;

			var si = new System.Diagnostics.ProcessStartInfo( prog, args );
			si.CreateNoWindow = true;
			si.UseShellExecute = false;

			var stdErr = this.StdErr;
			if ( null != stdErr ) {
				stdErr.WorkOrder = workOrder;
				si.RedirectStandardError = true;
				si.StandardErrorEncoding = this.StdErr.GetEncoding();
			}
			var stdOut = this.StdOut;
			if ( null != stdOut ) {
				stdOut.WorkOrder = workOrder;
				si.RedirectStandardOutput = true;
				si.StandardOutputEncoding = this.StdOut.GetEncoding();
			}

			var ec = this.RunProcess( si );
			if ( this.RequireSuccessExitCode && ( this.SuccessExitCode != ec ) ) {
				var ex = new System.ApplicationException( "The process did not exit correctly." );
				ex.Data.Add( "Program", prog );
				ex.Data.Add( "Args", args );
				ex.Data.Add( "Working Direcotry", wd );
				throw ex;
			}
		}
		private System.Int32 RunProcess( System.Diagnostics.ProcessStartInfo startInfo ) {
			using ( var proc = new System.Diagnostics.Process() ) {
				proc.StartInfo = startInfo;
				proc.Start();
				proc.WaitForExit();

				var pErr = proc.StandardError;
				var stdErr = this.StdErr;
				if ( ( null != stdErr ) && ( null != pErr ) && !pErr.EndOfStream ) {
					var errH = stdErr.GetFileHandler( this.WorkOrder );
					errH.Overwrite( pErr.BaseStream, errH.PathCombine( stdErr.ExpandedPath, stdErr.ExpandedName ) );
				}

				var pOut = proc.StandardOutput;
				var stdOut = this.StdOut;
				if ( ( null != stdOut ) && ( null != pOut ) && !pOut.EndOfStream ) {
					var outH = stdOut.GetFileHandler( this.WorkOrder );
					outH.Overwrite( pOut.BaseStream, outH.PathCombine( stdOut.ExpandedPath, stdOut.ExpandedName ) );
				}

				return proc.ExitCode;
			}
		}
		#endregion methods

	}

}