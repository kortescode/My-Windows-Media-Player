using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using MyWindowsMediaPlayer.Model;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Input;
using MyWindowsMediaPlayer.Dialog;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class PlaylistViewModel : ViewModelBase
    {
        #region Fields
        private bool isPlaylistVisible;
        private bool isShuffleMode;
        private int isRepeatMode;

        private bool isPlaylistsVisible;
        private bool isMediasVisible;

        private Media selectedMedia;
        private Media currentMedia;

        private Playlist selectedPlaylist;
        private Playlist currentPlaylist;
        private List<Media> currentPlaylistList;
        private List<Media> currentPlaylistListTmp;

        private ICommand viewSelectedPlaylistCommand;
        private ICommand playSelectedMediaCommand;
        private ICommand newPlaylistCommand;
        private ICommand delPlaylistCommand;
        private ICommand returnCommand;
        private ICommand delTrackCommand;
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        #endregion

        #region Properties
        public bool IsPlaylistVisible
        {
            get { return this.isPlaylistVisible; }
            set
            {
                this.isPlaylistVisible = value;
                this.RaisePropertyChanged("IsPlaylistVisible");
            }
        }

        public bool IsShuffleMode
        {
            get { return this.isShuffleMode; }
            set
            {
                this.isShuffleMode = value;
                this.RaisePropertyChanged("IsShuffleMode");
            }
        }

        public int IsRepeatMode
        {
            get { return this.isRepeatMode; }
            set
            {
                this.isRepeatMode = value;
                this.RaisePropertyChanged("IsRepeatMode");
            }
        }

        public ObservableCollection<Playlist> Playlists { get; set; }

        public Media CurrentMedia
        {
            get { return this.currentMedia; }
            set
            {
                this.currentMedia = value;
                this.RaisePropertyChanged("CurrentMedia");
            }
        }

        public Media SelectedMedia
        {
            get { return this.selectedMedia; }
            set
            {
                this.selectedMedia = value;
                this.RaisePropertyChanged("SelectedMedia");
            }
        }

        public Playlist SelectedPlaylist
        {
            get { return this.selectedPlaylist; }
            set
            {
                this.selectedPlaylist = value;
                this.RaisePropertyChanged("SelectedPlaylist");
            }
        }

        public bool IsPlaylistsVisible
        {
            get { return this.isPlaylistsVisible; }
            set
            {
                this.isPlaylistsVisible = value;
                this.RaisePropertyChanged("IsPlaylistsVisible");
            }
        }

        public bool IsMediasVisible
        {
            get { return this.isMediasVisible; }
            set
            {
                this.isMediasVisible = value;
                this.RaisePropertyChanged("IsMediasVisible");
            }
        }
        #endregion

        #region Constructors
        public PlaylistViewModel()
        {
            this.IsPlaylistVisible = true;
            this.IsPlaylistsVisible = true;
            this.IsMediasVisible = false;
            this.IsShuffleMode = false;
            this.IsRepeatMode = 0;
            this.LoadPlaylists();
        }
        #endregion

        #region Destructors
        ~PlaylistViewModel()
        {
            this.IsShuffleMode = false;
            this.RandomizeCurrentPlaylist();
            this.SavePlaylists();
        }
        #endregion

        #region Commands
        #region Command ViewSelectedPlaylist
        public ICommand ViewSelectedPlaylistCommand
        {
            get
            {
                if (this.viewSelectedPlaylistCommand == null)
                    this.viewSelectedPlaylistCommand = new DelegateCommand(ViewSelectedPlaylist, CanViewSelectedPlaylist);

                return this.viewSelectedPlaylistCommand;
            }
        }

        private void ViewSelectedPlaylist(object param)
        {
            if (this.SelectedPlaylist != null)
            {
                this.IsPlaylistsVisible = false;
                this.IsMediasVisible = true;
            }
        }

        private bool CanViewSelectedPlaylist(object param)
        {
            return true;
        }
        #endregion

        #region Command PlaySelectedMedia
        public ICommand PlaySelectedMediaCommand
        {
            get
            {
                if (this.playSelectedMediaCommand == null)
                    this.playSelectedMediaCommand = new DelegateCommand(PlaySelectedMedia, CanPlaySelectedMedia);

                return this.playSelectedMediaCommand;
            }
        }

        public void PlaySelectedMedia(object param)
        {
            if (this.SelectedMedia != null)
            {
                if (this.SelectedPlaylist == this.currentPlaylist)
                {
                    this.CurrentMedia = this.SelectedMedia;

                    if (this.IsShuffleMode)
                    {
                        int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                        Media tmp = this.currentPlaylistList[0];

                        this.currentPlaylistList[0] = this.currentPlaylistList[idx];
                        this.currentPlaylistList[idx] = tmp;
                    }
                }
                else
                {
                    this.currentPlaylist = this.SelectedPlaylist;
                    this.currentPlaylistList = this.currentPlaylist.Medias.ToList();

                    this.CurrentMedia = this.SelectedMedia;

                    if (this.IsShuffleMode)
                    {
                        this.RandomizeCurrentPlaylist();

                        int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                        Media tmp = this.currentPlaylistList[0];

                        this.currentPlaylistList[0] = this.currentPlaylistList[idx];
                        this.currentPlaylistList[idx] = tmp;
                    }
                }

                if (this.EventChangeCurrentMedia != null)
                    this.EventChangeCurrentMedia(this, new EventArgs());
            }
        }

        private bool CanPlaySelectedMedia(object param)
        {
            return true;
        }
        #endregion

        #region Command NewPlaylist
        public ICommand NewPlaylistCommand
        {
            get
            {
                if (this.newPlaylistCommand == null)
                    this.newPlaylistCommand = new DelegateCommand(NewPlaylist, CanNewPlaylist);

                return this.newPlaylistCommand;
            }
        }

        private void NewPlaylist(object param)
        {
            int idMax = (this.Playlists.Count > 0) ? this.Playlists.OrderByDescending(item => item.Id).First().Id + 1 : 1;
            string stream = InputDialog.ShowDialog("New Playlist", "Playlist Name:", "playlist" + idMax.ToString());

            if (stream != "")
            {
                this.SelectedPlaylist = new Playlist(idMax, stream);
                this.Playlists.Add(this.SelectedPlaylist);
            }
        }

        private bool CanNewPlaylist(object param)
        {
            return true;
        }
        #endregion

        #region Command DelPlaylist
        public ICommand DelPlaylistCommand
        {
            get
            {
                if (this.delPlaylistCommand == null)
                    this.delPlaylistCommand = new DelegateCommand(DelPlaylist, CanDelPlaylist);

                return this.delPlaylistCommand;
            }
        }

        private void DelPlaylist(object param)
        {
            if (this.SelectedPlaylist != null)
            {
                if (this.SelectedPlaylist == this.currentPlaylist)
                {
                    this.Playlists.Remove(this.currentPlaylist);

                    this.SelectedMedia = null;
                    this.SelectedPlaylist = null;

                    this.CurrentMedia = null;
                    this.currentPlaylist = null;
                    this.currentPlaylistList = null;
                    this.currentPlaylistListTmp = null;

                    if (this.EventChangeCurrentMedia != null)
                        this.EventChangeCurrentMedia(this, new EventArgs());
                }
                else
                {
                    this.Playlists.Remove(this.SelectedPlaylist);

                    this.SelectedMedia = null;
                    this.SelectedPlaylist = null;
                }
            }
        }

        private bool CanDelPlaylist(object param)
        {
            return true;
        }
        #endregion

        #region Command Return
        public ICommand ReturnCommand
        {
            get
            {
                if (this.returnCommand == null)
                    this.returnCommand = new DelegateCommand(Return, CanReturn);

                return this.returnCommand;
            }
        }

        private void Return(object param)
        {
            this.SelectedMedia = null;

            this.IsPlaylistsVisible = true;
            this.IsMediasVisible = false;
        }

        private bool CanReturn(object param)
        {
            return true;
        }
        #endregion

        #region Command DelTrack
        public ICommand DelTrackCommand
        {
            get
            {
                if (this.delTrackCommand == null)
                    this.delTrackCommand = new DelegateCommand(DelTrack, CanDelTrack);

                return this.delTrackCommand;
            }
        }

        public void DelTrack(object param)
        {
            if (this.SelectedMedia != null)
            {
                if (this.SelectedPlaylist == this.currentPlaylist)
                {
                    if (this.SelectedMedia == this.CurrentMedia)
                    {
                        Media tmp = this.SelectedMedia;

                        this.PlayNextMedia();

                        this.currentPlaylistList.Remove(tmp);
                        if (this.currentPlaylistListTmp != null)
                            this.currentPlaylistListTmp.Remove(tmp);
                        this.currentPlaylist.RemoveFromPlaylist(tmp);
                    }
                    else
                    {
                        this.currentPlaylistList.Remove(this.SelectedMedia);
                        if (this.currentPlaylistListTmp != null)
                            this.currentPlaylistListTmp.Remove(this.SelectedMedia);
                        this.currentPlaylist.RemoveFromPlaylist(this.SelectedMedia);

                        this.SelectedMedia = null;
                    }
                }
                else
                {
                    this.SelectedPlaylist.RemoveFromPlaylist(this.SelectedMedia);

                    this.SelectedMedia = null;
                }
            }
        }

        private bool CanDelTrack(object param)
        {
            return true;
        }
        #endregion

        #region Command MoveUp
        public ICommand MoveUpCommand
        {
            get
            {
                if (this.moveUpCommand == null)
                    this.moveUpCommand = new DelegateCommand(MoveUp, CanMoveUp);

                return this.moveUpCommand;
            }
        }

        private void MoveUp(object param)
        {
            int idx;
            Media tmp;

            if (this.SelectedMedia != null)
            {
                if (this.currentPlaylist == this.SelectedPlaylist)
                {
                    idx = this.currentPlaylistList.IndexOf(this.SelectedMedia);
                    if (idx > 0)
                    {
                        tmp = this.currentPlaylistList[idx - 1];
                        this.currentPlaylistList[idx - 1] = this.currentPlaylistList[idx];
                        this.currentPlaylistList[idx] = tmp;
                    }

                    if (this.currentPlaylistListTmp != null)
                    {
                        idx = this.currentPlaylistListTmp.IndexOf(this.SelectedMedia);
                        if (idx > 0)
                        {
                            tmp = this.currentPlaylistListTmp[idx - 1];
                            this.currentPlaylistListTmp[idx - 1] = this.currentPlaylistListTmp[idx];
                            this.currentPlaylistListTmp[idx] = tmp;
                        }
                    }
                }

                idx = this.SelectedPlaylist.Medias.IndexOf(this.SelectedMedia);
                if (idx > 0)
                {
                    tmp = this.SelectedPlaylist.Medias[idx - 1];
                    this.SelectedPlaylist.Medias[idx - 1] = this.SelectedPlaylist.Medias[idx];
                    this.SelectedPlaylist.Medias[idx] = tmp;
                }
            }
        }

        private bool CanMoveUp(object param)
        {
            return true;
        }
        #endregion

        #region Command MoveDown
        public ICommand MoveDownCommand
        {
            get
            {
                if (this.moveDownCommand == null)
                    this.moveDownCommand = new DelegateCommand(MoveDown, CanMoveDown);

                return this.moveDownCommand;
            }
        }

        private void MoveDown(object param)
        {
            int idx;
            Media tmp;

            if (this.SelectedMedia != null)
            {
                if (this.currentPlaylist == this.SelectedPlaylist)
                {
                    idx = this.currentPlaylistList.IndexOf(this.SelectedMedia);
                    if (idx + 1 < this.currentPlaylistList.Count)
                    {
                        tmp = this.currentPlaylistList[idx + 1];
                        this.currentPlaylistList[idx + 1] = this.currentPlaylistList[idx];
                        this.currentPlaylistList[idx] = tmp;
                    }

                    if (this.currentPlaylistListTmp != null)
                    {
                        idx = this.currentPlaylistListTmp.IndexOf(this.SelectedMedia);
                        if (idx + 1 < this.currentPlaylistListTmp.Count)
                        {
                            tmp = this.currentPlaylistListTmp[idx + 1];
                            this.currentPlaylistListTmp[idx + 1] = this.currentPlaylistListTmp[idx];
                            this.currentPlaylistListTmp[idx] = tmp;
                        }
                    }
                }

                idx = this.SelectedPlaylist.Medias.IndexOf(this.SelectedMedia);
                if (idx + 1 < this.SelectedPlaylist.Medias.Count)
                {
                    tmp = this.SelectedPlaylist.Medias[idx + 1];
                    this.SelectedPlaylist.Medias[idx + 1] = this.SelectedPlaylist.Medias[idx];
                    this.SelectedPlaylist.Medias[idx] = tmp;
                }
            }
        }

        private bool CanMoveDown(object param)
        {
            return true;
        }
        #endregion
        #endregion

        #region Methods
        public void PlayNextMedia()
        {
            if (this.IsRepeatMode == 0)
            {
                if (this.currentPlaylist != null && this.currentPlaylistList != null && this.CurrentMedia != null)
                {
                    int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                    this.CurrentMedia = (idx + 1 >= this.currentPlaylistList.Count) ? null : this.currentPlaylistList[idx + 1];
                }

            }
            else if (this.IsRepeatMode == 1)
            {
                if (this.currentPlaylist != null && this.currentPlaylistList != null && this.CurrentMedia != null)
                {
                    int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                    if (this.currentPlaylistList.Count == 0)
                        this.CurrentMedia = null;
                    else
                        this.CurrentMedia = (idx + 1 >= this.currentPlaylistList.Count) ? this.currentPlaylistList[0] : this.currentPlaylistList[idx + 1];
                }
            }

            if (this.currentPlaylist != null && this.currentPlaylist.Id > 0)
                this.SelectedMedia = this.CurrentMedia;
            else if (this.currentPlaylist != null)
            {
                if (this.EventChangeSelectedMediaOfLibrary != null)
                    this.EventChangeSelectedMediaOfLibrary(this, new EventArgs());
            }

            if (this.EventChangeCurrentMedia != null)
                this.EventChangeCurrentMedia(this, new EventArgs());
        }

        public void PlayPreviousMedia()
        {
            if (this.IsRepeatMode == 0)
            {
                if (this.currentPlaylist != null && this.currentPlaylistList != null && this.CurrentMedia != null)
                {
                    int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                    this.CurrentMedia = (idx - 1 < 0) ? null : this.currentPlaylistList[idx - 1];
                }

            }
            else if (this.IsRepeatMode == 1)
            {
                if (this.currentPlaylist != null && this.currentPlaylistList != null && this.CurrentMedia != null)
                {
                    int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                    this.CurrentMedia = (idx - 1 < 0) ? this.currentPlaylistList[this.currentPlaylistList.Count - 1] : this.currentPlaylistList[idx - 1];
                }
            }

            if (this.currentPlaylist != null && this.currentPlaylist.Id > 0)
                this.SelectedMedia = this.CurrentMedia;
            else if (this.currentPlaylist != null)
            {
                if (this.EventChangeSelectedMediaOfLibrary != null)
                    this.EventChangeSelectedMediaOfLibrary(this, new EventArgs());
            }

            if (this.EventChangeCurrentMedia != null)
                this.EventChangeCurrentMedia(this, new EventArgs());
        }

        public void RandomizeCurrentPlaylist()
        {
            if (this.IsShuffleMode && this.currentPlaylist != null && this.currentPlaylistList != null)
            {
                int n = this.currentPlaylistList.Count;
                Random rnd = new Random();

                this.currentPlaylistListTmp = new List<Media>(this.currentPlaylistList);

                while (n > 1)
                {
                    int k = (rnd.Next(0, n) % n--);
                    Media value = this.currentPlaylistList[k];

                    this.currentPlaylistList[k] = this.currentPlaylistList[n];
                    this.currentPlaylistList[n] = value;
                }

                int idx = this.currentPlaylistList.FindIndex(media => media == this.CurrentMedia);
                Media tmp = this.currentPlaylistList[0];

                this.currentPlaylistList[0] = this.currentPlaylistList[idx];
                this.currentPlaylistList[idx] = tmp;
            }
            else if (this.currentPlaylist != null && this.currentPlaylistList != null && this.currentPlaylistListTmp != null)
            {
                this.currentPlaylistList = this.currentPlaylistListTmp;
                this.currentPlaylistListTmp = null;
            }
        }

        public void AddMediaOnCurrentPlaylistIfCurrentPlaylistIsLibrary(Media media)
        {
            if (this.currentPlaylist != null && this.currentPlaylist.Id < 0)
            {
                this.currentPlaylist.AddToPlaylist(media);
                this.currentPlaylistList.Add(media);
                if (this.currentPlaylistListTmp != null)
                    this.currentPlaylistListTmp.Add(media);
            }
        }

        public void DelMediaFromAllPlaylists(Media media)
        {
            this.PlayNextMedia();

            if (this.currentPlaylist != null && this.currentPlaylist.Id < 0)
                this.currentPlaylist.RemoveFromPlaylist(media);
            if (this.currentPlaylistList != null)
                this.currentPlaylistList.Remove(media);
            if (this.currentPlaylistListTmp != null)
                this.currentPlaylistListTmp.Remove(media);

            for (int i = 0; i < this.Playlists.Count; i++)
                this.Playlists[i].RemoveFromPlaylist(media);
        }

        public void AddToTrackToPlaylist(Media media)
        {
            if (this.SelectedPlaylist != null)
            {
                this.SelectedPlaylist.AddToPlaylist(media);

                if (this.SelectedPlaylist == this.currentPlaylist)
                {
                    this.currentPlaylistList.Add(media);
                    if (this.currentPlaylistListTmp != null)
                        this.currentPlaylistListTmp.Add(media);
                }
            }
        }

        private void LoadPlaylists()
        {
            var tmp = new ObservableCollection<Playlist>();
            FileStream fs = new FileStream("../../Config/playlists.xml", FileMode.OpenOrCreate);
            DataContractSerializer dcs = new DataContractSerializer(typeof(ObservableCollection<Playlist>));

            try { tmp = dcs.ReadObject(fs) as ObservableCollection<Playlist>; }
            catch { }

            this.Playlists = tmp;
            fs.Close();
        }

        private void SavePlaylists()
        {
            FileStream fs = new FileStream("../../Config/playlists.xml", FileMode.Create);
            DataContractSerializer dcs = new DataContractSerializer(typeof(ObservableCollection<Playlist>));
            
            dcs.WriteObject(fs, this.Playlists);
            fs.Close();
        }

        #endregion

        #region Events
        public event EventHandler EventChangeSelectedMediaOfLibrary;
        public event EventHandler EventChangeCurrentMedia;
        #endregion
    }
}
