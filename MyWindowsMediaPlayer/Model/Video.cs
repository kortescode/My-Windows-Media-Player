using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    public class Video : Media
    {
        #region Properties
        [DataMember]
        public string Actor { get; set; }

        [DataMember]
        public string Director { get; set; }
        #endregion

        #region Constructors
        public Video(int id, Uri source, Path path = null)
        {
            this.Id = id;
            this.Source = source;
            this.Path = path;
            this.Title = System.IO.Path.GetFileNameWithoutExtension(source.ToString());
        }
        #endregion
    }
}
