using System.Collections.ObjectModel;
using System.Windows;

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
