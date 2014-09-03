using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CDUINo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ThreadSafeObservationCollection<SongDisplayInfo> _songTitleCollection = new ThreadSafeObservationCollection<SongDisplayInfo>();
        private static readonly iTunesCatalogDataContext _iTunesCatalog = new iTunesCatalogDataContext();
        public MainWindow()
        {
            InitializeComponent();
            // with 20K songs app launch takes too long
            // so a thread is queries the data and displays it
            // TO-DO - Add some sort of control showing loading progress
            var threadData = new ThreadData {ITunesDC = _iTunesCatalog, ObservableCollection = _songTitleCollection};
            var workerObject = new Worker();
            var workerThread = new Thread(workerObject.LoadObservablesCollection);
            workerThread.Start(threadData);
            CdDataGrid.DataContext = _songTitleCollection;
        }

        private void PopulatedObservableCollection(ThreadSafeObservationCollection<SongDisplayInfo> songTitleCollection)
        {
            var threadData = new ThreadData { ITunesDC = _iTunesCatalog, ObservableCollection = songTitleCollection };
            var workerObject = new Worker();
            var workerThread = new Thread(workerObject.LoadObservablesCollection);
            workerThread.Start(threadData);

        }

        private void UpDateObservableCollection(ThreadSafeObservationCollection<SongDisplayInfo> songTitleCollection, IQueryable<Song> songs)
        {
            var tmpSong = new SongDisplayInfo();
            foreach (var song in songs)
            {
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.Name;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Album = _iTunesCatalog.Albums.First(id => id.AlbumID == song.AlbumID).Title;
                tmpSong.Artist = song.Artist.ArtistName;
                tmpSong.PlayCount = song.PlayCount;
                tmpSong.TrackLength = song.SongTrackLength;
                songTitleCollection.Add(tmpSong);
            }
        }

        private void ClickListCds(object sender, RoutedEventArgs e)
        {
            PopulatedObservableCollection(_songTitleCollection);
        }

        private void ClickAddCd(object sender, RoutedEventArgs e)
        {
            var newSong = new Song();
            newSong.Name = SongTitleInput.Text;
            newSong.ArtistID = 3;
            newSong.AlbumID = 5;
            newSong.GenreID = 1;
            _iTunesCatalog.Songs.InsertOnSubmit(newSong);
            _iTunesCatalog.SubmitChanges();
            PopulatedObservableCollection(_songTitleCollection);
        }

        private void ClickDeleteCD(object sender, RoutedEventArgs e)
        {
            var songTable = _iTunesCatalog.GetTable<Song>();

            foreach (SongDisplayInfo song in CdDataGrid.SelectedItems)
            {

                foreach (Song s in songTable.Where(s => s.SongID == song.Id))
                {
                    _iTunesCatalog.Songs.DeleteOnSubmit(s);
                }
            }
            _iTunesCatalog.SubmitChanges();
            PopulatedObservableCollection(_songTitleCollection);
        }

        private void SearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox tmpBox = (TextBox) sender;
            string tmpSearch = tmpBox.Text;

            _songTitleCollection.Clear();
            var songTable = _iTunesCatalog.GetTable<Song>();
            // only grab the first 100 songs because it takes too long
            // with the initial single character, like "l" in "love"
            // too many matches
            var q =
                songTable.Where(s => s.Name.Contains(tmpSearch)).Take(100);
           UpDateObservableCollection(_songTitleCollection, q);
        }       
    }

    public class Worker
    {
        public void LoadObservablesCollection(object data)
        {
            var tData = (ThreadData) data;
            if (tData == null) throw new ArgumentNullException("tData");
            ObservableCollection<SongDisplayInfo> songTitleCollection = tData.ObservableCollection;
            if (songTitleCollection == null) throw new ArgumentNullException("songTitleCollection");
            var iTunesCatalog = tData.ITunesDC;

            songTitleCollection.Clear();
            var songs = iTunesCatalog.Songs.Select(x => x).ToList();
            var tmpSong = new SongDisplayInfo();
            foreach (var song in songs)
            {
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.Name;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Album = iTunesCatalog.Albums.First(id => id.AlbumID == song.AlbumID).Title;
                tmpSong.Artist = song.Artist.ArtistName;
                tmpSong.PlayCount = song.PlayCount;
                tmpSong.TrackLength = song.SongTrackLength;
                songTitleCollection.Add(tmpSong);
            }
        }
    }
}
