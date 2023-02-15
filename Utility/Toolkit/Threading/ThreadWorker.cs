using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Toolkit.Threading
{
    public class ThreadWorker
    {
        #region Delegates

        public delegate void WorkerEvent();
        private WorkerEvent _worker;

        #endregion

        public WorkerEvent Worker
        {
            get { return _worker; }
            set { _worker = value; }
        }

        public ThreadWorker()
        {
            //DataManager.ForwardOnlyResults = new DataManager.ForwardOnlyResultsEvent(ReadFieldDataTypes);
            //DataManager.ExecuteStoredProcedureForwardOnlyReturn(_connection, _spGetAllFieldOptions);
        }
    }
}
