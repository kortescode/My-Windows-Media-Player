using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    public class Playlist
    {
        #region Fields
        #endregion

        #region Properties
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ObservableCollection<Media> Medias { get; set; }
        #endregion

        #region Contructors
        public Playlist(int id, string name = "playlist")
        {
            this.Id = id;
            this.Name = name;
            this.Medias = new ObservableCollection<Media>();
        }
        #endregion

        #region Methods
        public void AddToPlaylist(Media media)
        {
            this.Medias.Add(media);
        }

        public void RemoveFromPlaylist(Media media)
        {
            this.Medias.Remove(media);
        }
        #endregion
    }
}
