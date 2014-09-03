namespace CDUINo2
{
    public class SongDisplayInfo
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