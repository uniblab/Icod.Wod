namespace Icod.Wod {

	public sealed class TaskScheduler : System.Threading.Tasks.TaskScheduler {

		#region fields
		private readonly System.Int32 myMaximumConcurrencyLevel;
		[System.ThreadStatic]
		private static System.Boolean theCurrentThreadIsWorking;
		private readonly System.Collections.Generic.LinkedList<System.Threading.Tasks.Task> myTasks;
		private System.Int32 myDelegateCount;
		#endregion fields


		#region .ctor
		private TaskScheduler() : base() {
			myTasks = new System.Collections.Generic.LinkedList<System.Threading.Tasks.Task>();
			myDelegateCount = 0;
		}
		public TaskScheduler( System.Int32 maximumConcurrencyLevel ) : this() {
			if ( maximumConcurrencyLevel < 1 ) {
				throw new System.ArgumentOutOfRangeException( "maximumConcurrencyLevel", "maximumConcurrencyLevel must be positive." );
			}
			myMaximumConcurrencyLevel = maximumConcurrencyLevel;
		}
		#endregion .ctor


		#region properties
		public sealed override System.Int32 MaximumConcurrencyLevel {
			get {
				return myMaximumConcurrencyLevel;
			}
		}
		#endregion properties


		#region methods
		protected sealed override System.Boolean TryDequeue( Task task ) {
			lock ( myTasks ) {
				return myTasks.Remove( task );
			}
		}
		protected sealed override void QueueTask( Task task ) {
			lock ( myTasks ) {
				myTasks.AddLast( task );
				if ( myDelegateCount < myMaximumConcurrencyLevel ) {
					myDelegateCount++;
					this.NotifyThreadPool();
				}
			}
		}
		private void NotifyThreadPool() {
			System.Threading.ThreadPool.UnsafeQueueUserWorkItem(
				callBack => {
					theCurrentThreadIsWorking = true;
					try {
						System.Threading.Tasks.Task task;
						do {
							lock ( myTasks ) {
								if ( 0 == myTasks.Count ) {
									--myDelegateCount;
									break;
								}
								task = myTasks.First.Value;
								myTasks.RemoveFirst();
							}
							base.TryExecuteTask( task );
						} while ( true );
					} catch {
						throw;
					} finally {
						theCurrentThreadIsWorking = false;
					}
				},
				null
			);
		}
		protected sealed override System.Boolean TryExecuteTaskInline( System.Threading.Tasks.Task task, System.Boolean taskWasPreviouslyQueued ) {
			if ( !theCurrentThreadIsWorking ) {
				return false;
			}
			if ( taskWasPreviouslyQueued ) {
				if ( this.TryDequeue( task ) {
					return base.TryExecuteTask( task );
				} else {
					return false;
				}
			} else {
				return base.TryExecuteTask( task );
			}
		}
		protected sealed override System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> GetScheduledTasks() {
			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}