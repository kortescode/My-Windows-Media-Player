using System;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    public class Music : Media
    {
        #region Properties
        [DataMember]
        public string Artist { get; set; }

        [DataMember]
        public string Album { get; set; }
        #endregion

        #region Constructors
        public Music(int id, Uri source, Path path = null)
        {
            this.Id = id;
            this.Source = source;
            this.Path = path;
            this.Title = System.IO.Path.GetFileNameWithoutExtension(source.ToString());
        }
        #endregion
    }
}
