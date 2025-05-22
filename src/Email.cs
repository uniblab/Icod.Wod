// Copyright (C) 2025  Timothy J. Bruce
using Icod.Wod.Data;
using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"email",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Email : IStep {

		#region fields
		private System.String myTo;
		private System.String myCc;
		private System.String myBcc;
		private System.String mySubject;
		private System.String myBodyCodePage;
		private System.String mySubjectCodePage;
		private System.Boolean myBodyIsHtml;
		private Icod.Wod.File.FileDescriptor[] myAttachments;
		private System.Boolean mySendIfEmpty;
		private System.String myBody;
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		public Email() : base() {
			myBodyCodePage = "us-ascii";
			mySubjectCodePage = "us-ascii";
			myBodyIsHtml = false;
			mySendIfEmpty = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"to",
			Namespace = "http://Icod.Wod"
		)]
		public System.String To {
			get {
				return myTo;
			}
			set {
				myTo = value;
			}
		}
		public System.String ExpandedTo {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.To );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"cc",
			Namespace = "http://Icod.Wod"
		)]
		public System.String CC {
			get {
				return myCc;
			}
			set {
				myCc = value;
			}
		}
		public System.String ExpandedCC {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.CC );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"bcc",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Bcc {
			get {
				return myBcc;
			}
			set {
				myBcc = value;
			}
		}
		public System.String ExpandedBcc {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.Bcc );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"subject",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Subject {
			get {
				return mySubject;
			}
			set {
				mySubject = value;
			}
		}
		public System.String ExpandedSubject {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.Subject );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"bodyCodePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "us-ascii" )]
		public System.String BodyCodePage {
			get {
				return myBodyCodePage;
			}
			set {
				myBodyCodePage = value;
			}
		}
		public System.String ExpandedBodyCodePage {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.BodyCodePage );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"subjectCodePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "us-ascii" )]
		public System.String SubjectCodePage {
			get {
				return mySubjectCodePage;
			}
			set {
				mySubjectCodePage = value;
			}
		}
		public System.String ExpandedSubjectCodePage {
			get {
				return this.WorkOrder.ExpandPseudoVariables( this.SubjectCodePage );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"bodyIsHtml",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean BodyIsHtml {
			get {
				return myBodyIsHtml;
			}
			set {
				myBodyIsHtml = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"sendIfEmpty",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean SendIfEmpty {
			get {
				return mySendIfEmpty;
			}
			set {
				mySendIfEmpty = value;
			}
		}
		[System.Xml.Serialization.XmlElement(
			"body",
			typeof( System.String ),
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String Body {
			get {
				return myBody;
			}
			set {
				myBody = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public Icod.Wod.WorkOrder WorkOrder {
			get {
				return myWorkOrder;
			}
			set {
				myWorkOrder = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "attachments"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			ElementName = "attach",
			Type = typeof( Icod.Wod.File.FileDescriptor ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public Icod.Wod.File.FileDescriptor[] Attachments {
			get {
				return myAttachments;
			}
			set {
				myAttachments = value;
			}
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			using ( var msg = new System.Net.Mail.MailMessage() ) {
				msg.IsBodyHtml = this.BodyIsHtml;
				msg.BodyEncoding = CodePageHelper.GetCodePage( this.BodyCodePage );
				msg.Body = workOrder.ExpandPseudoVariables( this.Body );
				File.FileHandlerBase handler;
				foreach ( var a in ( this.Attachments ?? System.Array.Empty<Icod.Wod.File.FileDescriptor>() ) ) {
					handler = a.GetFileHandler( workOrder );
					System.String filePathName;
					foreach ( var fe in handler.ListFiles() ) {
						filePathName = fe.File;
						msg.Attachments.Add( new System.Net.Mail.Attachment( handler.OpenReader( filePathName ), System.IO.Path.GetFileName( filePathName ) ) );
					}
				}
				if ( !this.SendIfEmpty && System.String.IsNullOrEmpty( msg.Body ) && ( 0 == msg.Attachments.Count ) ) {
					foreach ( var stream in msg.Attachments.OfType<System.Net.Mail.Attachment>().Select(
						x => x.ContentStream
					).Where(
						x => x is object
					) ) {
						stream.Dispose();
					}
					return;
				}

				msg.SubjectEncoding = CodePageHelper.GetCodePage( this.SubjectCodePage );
				msg.Subject = this.ExpandedSubject;
				msg.To.Add( this.ExpandedTo );
				if ( !System.String.IsNullOrEmpty( this.CC ) ) {
					msg.CC.Add( this.ExpandedCC );
				}
				if ( !System.String.IsNullOrEmpty( this.Bcc ) ) {
					msg.Bcc.Add( this.ExpandedBcc );
				}
				using ( var client = new System.Net.Mail.SmtpClient() ) {
					client.Send( msg );
				}
			}
		}
		#endregion methods

	}

}
