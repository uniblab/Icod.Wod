namespace Icod.Wod {

	public interface IStep {

		void DoWork( WorkOrder workOrder );
		void DoWork( WorkOrder workOrder, IStack<ContextRecord> context );

		[System.Xml.Serialization.XmlIgnore]
		IStack<ContextRecord> Context {
			get;
			set;
		}

	}

}