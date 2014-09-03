using System.Windows.Controls;

namespace CDUINo2
{
    public class ThreadData
    {
        public iTunesCatalogDataContext ITunesDC;
        public ThreadSafeObservationCollection<SongDisplayInfo> ObservableCollection;
    }
}