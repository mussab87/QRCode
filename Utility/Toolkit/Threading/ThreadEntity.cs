using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Utility.Core.Logging;

namespace Utility.Toolkit.Threading
{
    public class ThreadEntity
    {
        #region Delegates

        //public delegate void WorkerEvent();
        //public WorkerEvent Worker;

        #endregion

        #region Fields

        private Thread _thread;
        private object _syncWorkerActivity = new object();
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private ManualResetEvent _cycleCompleteEvent = new ManualResetEvent(false);
        private WaitHandle[] _waitObjects;

        private int _stopEventToleranceTime = 10000;
        private DateTime _lastUpdateTimestamp;

        #endregion

        #region Properties

        internal Thread ThreadHandle
        {
            get { return _thread; }
        }

        public DateTime LastUpdateTimestamp
        {
            get { return _lastUpdateTimestamp; }
            set { _lastUpdateTimestamp = value; }
        }

        public WaitHandle[] WaitObjects
        {
            get { return _waitObjects; }
        }

        public ManualResetEvent StopEvent
        {
            get { return _stopEvent; }
        }

        public ManualResetEvent CycleCompleteEvent
        {
            get { return _cycleCompleteEvent;  }
        }

        public object SyncWorkerActivity
        {
            get { return _syncWorkerActivity; }
        }

        public int StopEventToleranceTime
        {
            get { return _stopEventToleranceTime; }
            set { _stopEventToleranceTime = value; }
        }

        #endregion

        #region Constructor

        internal ThreadEntity(WorkerEvent we)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                _waitObjects = new WaitHandle[] { _stopEvent };
                _thread = new Thread(new ThreadStart(we));
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
        }

        #endregion

        public bool Start()
        {
            FileTrace.WriteMemberEntry();

            bool started = false;
            try
            {
                if (_thread != null && _thread.IsAlive)
                {
                    Trace.WriteLine("Thread has already started");
                }
                else
                {
                    _thread.Start();
                    started = true;
                }
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            Trace.WriteLine("started is: " + started);

            FileTrace.WriteMemberExit();
            return started;
        }

        public bool Stop()
        {
            FileTrace.WriteMemberEntry();

            bool stopped = false;
            try
            {
                _stopEvent.Set();
                if (Monitor.TryEnter(_syncWorkerActivity, _stopEventToleranceTime))
                {
                    Trace.WriteLine("The thread has safely stopped.");
                    Monitor.Exit(_syncWorkerActivity);
                }
                else
                {
                    Trace.WriteLine("Force shutdown of thread");
                    try
                    {
                        _thread.Abort();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Successfully force shutdown of thread, message: " + e.Message);
                    }
                }
                stopped = true;
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            Trace.WriteLine("stopped is: " + stopped);

            FileTrace.WriteMemberExit();
            return stopped;
        }
    }
}