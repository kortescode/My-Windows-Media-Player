using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace MyWindowsMediaPlayer.Model
{
    [DataContract]
    public class Picture : Media
    {
        #region Properties
        [DataMember]
        public string Author { get; set; }
        #endregion
    
        #region Constructors
        public Picture(int id, Uri source, Path path = null)
        {
            this.Id = id;
            this.Source = source;
            this.Path = path;
            this.Title = System.IO.Path.GetFileNameWithoutExtension(source.ToString());
        }
        #endregion
    }
}