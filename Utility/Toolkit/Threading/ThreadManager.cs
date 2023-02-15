using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using Utility.Core.Logging;

namespace Utility.Toolkit.Threading
{
    #region Delegates

    public delegate void WorkerEvent();

    #endregion

    public sealed class ThreadManager
    {
        #region Fields

        private static volatile ThreadManager _instance = new ThreadManager();
        private static object _syncInstanceObj = new object();

        private Hashtable _threadQueue = new Hashtable();
        private object _syncThreadQueueObj = new object();

        #endregion

        #region Properties

        public static ThreadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncInstanceObj)
                    {
                        if (_instance == null)
                            _instance = new ThreadManager();
                    }
                }
                return _instance;
            }
        }

        public ThreadEntity Thread(string threadName)
        {
            FileTrace.WriteMemberEntry();

            ThreadEntity t = null;
            try
            {
                if (_threadQueue.ContainsKey(threadName))
                {
                    t = (ThreadEntity) _threadQueue[threadName];
                }
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
            return t;
        }

        #endregion

        #region Constructor

        private ThreadManager()
        {
            FileTrace.WriteMemberEntry();
            try
            {}
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
        }

        #endregion

        /// <summary>
        /// Only register a thread when the originator wants to begin the worker process.
        /// </summary>
        /// <param name="threadName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool RegisterThread(string threadName, WorkerEvent worker, int stopEventToleranceTime)
        {
            FileTrace.WriteMemberEntry();

            bool registered = false;
            try
            {
                if (_threadQueue.ContainsKey(threadName))
                    throw new Exception("Failed to register thread. There is already a thread called '" + threadName + "'.");

                // Verify if the originator has wired up the calling thread method before thread initialization
                if (worker != null)
                {
                    // Pre-thread start checks complete - begin thread initialization
                    ThreadEntity thread = new ThreadEntity(worker);

                    // Set the wait time for a signalled termination of thread invocation
                    thread.StopEventToleranceTime = stopEventToleranceTime;

                    // Wire-up the new thread and add it to the thread pool
                    _threadQueue.Add(threadName, thread);

                    // Start-up the thread
                    if (thread.Start())
                        registered = true;
                }
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            Trace.WriteLine("registered is: " + registered);
            
            FileTrace.WriteMemberExit();
            return registered;
        }

        /// <summary>
        /// Only unregister a thread when the originator wants to stop the worker process
        /// </summary>
        /// <param name="threadName"></param>
        public void UnregisterThread(string threadName)
        {
            FileTrace.WriteMemberEntry();
            try
            {
                if (!_threadQueue.ContainsKey(threadName))
                    throw new Exception("Failed to unregister thread. There is no thread called '" + threadName + "'.");

                ThreadEntity thread = (ThreadEntity) _threadQueue[threadName];
                if (thread.Stop())
                    _threadQueue.Remove(threadName);
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
        }
    }
}
