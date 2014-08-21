using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace CDUINo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<RcoSong> SongTitleCollection = new ObservableCollection<RcoSong>();
        private static readonly CdInfoDbDataContext _CdCatalog = new CdInfoDbDataContext();
        public MainWindow()
        {
            InitializeComponent();
            PopulatedObservableCollection(SongTitleCollection);
            CdDataGrid.DataContext = SongTitleCollection;
        }

        private void PopulatedObservableCollection(ObservableCollection<RcoSong> songTitleCollection)
        {
            SongTitleCollection.Clear();
            foreach (var song in _CdCatalog.Songs)
            {
                var tmpSong = new RcoSong();
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.SongTitle;
                tmpSong.ArtistId = song.ArtistID; 
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Artist = song.Artist.ArtistName;
                songTitleCollection.Add(tmpSong);
            }
        }

        private void FindCDClick(object sender, RoutedEventArgs e)
        {
            SongTitleCollection.Clear();
            string tmpSearch = SearchText.Text;
            var songTable = _CdCatalog.GetTable<Song>();
            var q =
                songTable.Where(s => s.SongTitle.Contains(tmpSearch));
            UpDateObservableCollection(SongTitleCollection, q);
        }

        private void UpDateObservableCollection(ObservableCollection<RcoSong> songTitleCollection, IQueryable<Song> songs)
        {
            foreach (var song in songs)
            {
                var tmpSong = new RcoSong();
                tmpSong.Id = song.SongID;
                tmpSong.Name = song.SongTitle;
                tmpSong.ArtistId = song.ArtistID;
                tmpSong.AlbumId = song.AlbumID;
                tmpSong.Artist = song.Artist.ArtistName;
                songTitleCollection.Add(tmpSong);
            }
        }

        private void ClickListCds(object sender, RoutedEventArgs e)
        {
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void ClickAddCd(object sender, RoutedEventArgs e)
        {

        }

        private void ClickDeleteCD(object sender, RoutedEventArgs e)
        {

        }
    }

    public class RcoSong
    {
        private string _name;
        private int _id;
        private int _artistId;
        private string _artist;
        private int _albumId;
        private int _album;


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
    }
}
