using System;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    [KnownType(typeof(Picture))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(Music))]
    public abstract class Media
    {
        #region Fields
        #endregion

        #region Properties
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Uri Source { get; set; }

        [DataMember]
        public Path Path { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Genre { get; set; }

        [DataMember]
        public string Year { get; set; }
        #endregion

        #region Constructors
        public Media()
        {
        }

        public Media(int id, Uri source, Path path = null)
        {
            this.Id = id;
            this.Source = source;
            this.Path = path;
            this.Title = System.IO.Path.GetFileNameWithoutExtension(source.ToString());
        }
        #endregion
    }
}
