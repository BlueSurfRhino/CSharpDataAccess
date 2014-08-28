using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CDUINo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ThreadSafeObservationCollection<RcoSong> SongTitleCollection = new ThreadSafeObservationCollection<RcoSong>();
        //private static readonly CdInfoDbDataContext _CdCatalog = new CdInfoDbDataContext();
        private static readonly iTunesCatalogDataContext _ITunesCatalog = new iTunesCatalogDataContext();
        public MainWindow()
        {
            InitializeComponent();
            ThreadData ThreadData = new ThreadData();
            ThreadData.ITunesDC = _ITunesCatalog;
            ThreadData.ObservableCollection = SongTitleCollection;
            Worker workerObject = new Worker();
            Thread workerThread = new Thread(workerObject.LoadObverablesCollection);
            workerThread.Start(ThreadData);
            //PopulatedObservableCollection(SongTitleCollection);
            CdDataGrid.DataContext = SongTitleCollection;
        }

        private void PopulatedObservableCollection(ThreadSafeObservationCollection<RcoSong> songTitleCollection)
        {
            SongTitleCollection.Clear();
            foreach (var song in _ITunesCatalog.Songs)
            {
                var tmpSong = new RcoSong();
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.Name;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Album = _ITunesCatalog.Albums.First(id => id.AlbumID == song.AlbumID).Title;
                tmpSong.Artist = song.Artist.ArtistName;
                tmpSong.PlayCount = song.PlayCount;
                tmpSong.TrackLength = song.SongTrackLength;
                songTitleCollection.Add(tmpSong);
            }
        }

        private void UpDateObservableCollection(ObservableCollection<RcoSong> songTitleCollection, IQueryable<Song> songs)
        {
            foreach (var song in songs)
            {
                var tmpSong = new RcoSong();
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.Name;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Album = _ITunesCatalog.Albums.First(id => id.AlbumID == song.AlbumID).Title;
                tmpSong.Artist = song.Artist.ArtistName;
                tmpSong.PlayCount = song.PlayCount;
                tmpSong.TrackLength = song.SongTrackLength;
                songTitleCollection.Add(tmpSong);
            }
        }

        private void ClickListCds(object sender, RoutedEventArgs e)
        {
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void ClickAddCd(object sender, RoutedEventArgs e)
        {
            Song newSong = new Song();
            newSong.Name = SongTitleInput.Text;
            newSong.ArtistID = 3;
            newSong.AlbumID = 5;
            newSong.GenreID = 1;
            _ITunesCatalog.Songs.InsertOnSubmit(newSong);
            _ITunesCatalog.SubmitChanges();
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void ClickDeleteCD(object sender, RoutedEventArgs e)
        {
            var songTable = _ITunesCatalog.GetTable<Song>();

            foreach (RcoSong song in CdDataGrid.SelectedItems)
            {

                foreach (Song s in songTable.Where(s => s.SongID == song.Id))
                {
                    _ITunesCatalog.Songs.DeleteOnSubmit(s);
                }
            }
            _ITunesCatalog.SubmitChanges();
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void SearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox tmpBox = (TextBox) sender;
            string tmpSearch = tmpBox.Text;

            SongTitleCollection.Clear();
            var songTable = _ITunesCatalog.GetTable<Song>();
            var q =
                songTable.Where(s => s.Name.Contains(tmpSearch)).Take(100);
           UpDateObservableCollection(SongTitleCollection, q);
        }       
    }

    public class Worker
    {
        public void LoadObverablesCollection(object data)
        {
            ThreadData TData = (ThreadData) data;

            ObservableCollection<RcoSong> SongTitleCollection = TData.ObservableCollection;
            iTunesCatalogDataContext iTunesCatalog = TData.ITunesDC;

            SongTitleCollection.Clear();
            foreach (var song in iTunesCatalog.Songs)
            {
                var tmpSong = new RcoSong();
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.Name;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Album = iTunesCatalog.Albums.First(id => id.AlbumID == song.AlbumID).Title;
                tmpSong.Artist = song.Artist.ArtistName;
                tmpSong.PlayCount = song.PlayCount;
                tmpSong.TrackLength = song.SongTrackLength;
                SongTitleCollection.Add(tmpSong);
            }

        }

    }

    public class ThreadData
    {
        public iTunesCatalogDataContext ITunesDC;
        public ThreadSafeObservationCollection<RcoSong> ObservableCollection;
    }

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

    public static class WPFControlThreadingExtensions
    {
        public static void InvokeIfRequired(this Dispatcher disp,
            Action dotIt,
            DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
            {
                disp.Invoke(priority, dotIt);
            }
            else
            {
                dotIt();
            }
        }
    }
    public class RcoSong
    {
        private string _name;
        private int _id;
        private int _artistId;
        private string _artist;
        private int _albumId;
        private string _album;
        private int _playCount;
        private int _trackLength;


       public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int ArtistId
        {
            get { return _artistId; }
            set { _artistId = value; }
        }

        public int AlbumId
        {
            get { return _albumId; }
            set { _albumId = value; }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; }
        }

        public string Album
        {
            get { return _album; }
            set { _album = value; }
        }

        public int PlayCount
        {
            get { return _playCount; }
            set { _playCount = value; }
        }

        public int TrackLength
        {
            get { return _trackLength; }
            set { _trackLength = value; }
        }
    }
}
