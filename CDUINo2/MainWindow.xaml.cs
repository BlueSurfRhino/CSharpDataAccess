using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            Song newSong = new Song();
            newSong.SongTitle = SongTitleInput.Text;
            newSong.ArtistID = 3;
            newSong.AlbumID = 5;
            newSong.GenreID = 1;
            _CdCatalog.Songs.InsertOnSubmit(newSong);
            _CdCatalog.SubmitChanges();
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void ClickDeleteCD(object sender, RoutedEventArgs e)
        {
            var songTable = _CdCatalog.GetTable<Song>();

            foreach (RcoSong song in CdDataGrid.SelectedItems)
            {

                foreach (Song s in songTable.Where(s => s.SongID == song.Id))
                {
                    _CdCatalog.Songs.DeleteOnSubmit(s);
                }
            }
            _CdCatalog.SubmitChanges();
            PopulatedObservableCollection(SongTitleCollection);
        }

        private void SearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox tmpBox = (TextBox) sender;
            string tmpSearch = tmpBox.Text;

            SongTitleCollection.Clear();
            var songTable = _CdCatalog.GetTable<Song>();
            var q =
                songTable.Where(s => s.SongTitle.Contains(tmpSearch));
            UpDateObservableCollection(SongTitleCollection, q);
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
