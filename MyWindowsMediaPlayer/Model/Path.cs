using System;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    public class Path
    {
        #region Fields
        #endregion

        #region Properties
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Uri PathSource { get; set; }
        #endregion

        #region Contructors
        public Path(int id, Uri pathSource)
        {
            this.Id = id;
            this.PathSource = pathSource;
        }
        #endregion
    }
}
