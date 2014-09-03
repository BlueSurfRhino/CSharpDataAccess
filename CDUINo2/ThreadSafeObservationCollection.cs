using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace CDUINo2
{
    //
    // Sasha Barber ThreadSafeObservationCollection Extension
    // Not RichOwens Code
    //
    public class ThreadSafeObservationCollection<T> : ObservableCollection<T>
    {
        private Dispatcher _dispatcher;
        private ReaderWriterLockSlim _lock;

        public ThreadSafeObservationCollection()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _lock = new ReaderWriterLockSlim();

        }

        protected override void ClearItems()
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.ClearItems();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        protected override void InsertItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                if(index >  this.Count)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.InsertItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            _dispatcher.InvokeIfRequired(() =>
            {

                _lock.EnterReadLock();
                Int32 itemCount = this.Count;
                _lock.ExitReadLock();
                if(oldIndex >= itemCount | newIndex >= itemCount | oldIndex == newIndex)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        protected override void RemoveItem(int index)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                if (index >= this.Count)
                    return;
                _lock.EnterWriteLock();
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        protected override void SetItem(int index, T item)
        {
            _dispatcher.InvokeIfRequired(() =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.SetItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        public T[] ToSyncArray()
        {
            _lock.EnterReadLock();
            try
            {
                T[] _sync = new T[this.Count];
                this.CopyTo(_sync, 0);
                return _sync;
            }
            finally 
            {
                
                _lock.ExitReadLock();
            }
        }

    }
}