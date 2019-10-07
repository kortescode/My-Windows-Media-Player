using MyWindowsMediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class ControlerViewModel : ViewModelBase
    {
        #region Fields
        private PlayerViewModel playerViewModel;
        private PlaylistViewModel playlistViewModel;
        private LibraryViewModel libraryViewModel;

        private DispatcherTimer progressTimer;
        private ProgressBar seekProgressBar;

        private ICommand mediaSeekCommand;
        private ICommand mediaPlayCommand;
        private ICommand mediaPauseCommand;
        private ICommand mediaStopCommand;
        
        private ICommand mediaPreviousCommand;
        private ICommand mediaNextCommand;

        private bool isMediaMuted;
        private double mediaVolume;
        private ICommand mediaMuteCommand;
        private double mediaSavedSoundValue;

        private ICommand switchModeCommand;
        private ICommand switchPlaylistCommand;

        private ICommand shuffleModeCommand;
        private ICommand repeatModeCommand;

        private bool isMediaPlaying;
        private bool isMediaPaused;

        private bool isSeekProgressBarVisible;
        private bool isControlerVisible;
        #endregion

        #region Properties
        public PlayerViewModel PlayerViewModel
        {
            get { return this.playerViewModel; }
            set
            {
                this.playerViewModel = value;
                this.playerViewModel.EventMediaOpened += PlayerViewModel_MediaOpened;
                this.playerViewModel.EventMediaEnded += PlayerViewModel_MediaEnded;
                this.playerViewModel.EventMediaDoubleClicked += PlayerViewModel_MediaDoubleClick;
                this.RaisePropertyChanged("PlayerViewModel");
            }
        }
        
        public PlaylistViewModel PlaylistViewModel
        {
            get { return this.playlistViewModel; }
            set
            {
                this.playlistViewModel = value;
                this.playlistViewModel.EventChangeCurrentMedia += PlaylistViewModel_EventChangeCurrentMedia;
                this.playlistViewModel.EventChangeSelectedMediaOfLibrary += PlaylistViewModel_EventChangeSelectedMediaOfLibrary;
                this.RaisePropertyChanged("PlaylistViewModel");
            }
        }

        public LibraryViewModel LibraryViewModel
        {
            get { return this.libraryViewModel; }
            set
            {
                this.libraryViewModel = value;
                this.libraryViewModel.EventMediaAdded += LibraryViewModel_EventMediaAdded;
                this.libraryViewModel.EventDelSelectedMedia += LibraryViewModel_EventDelSelectedMedia;
                this.libraryViewModel.EventPlaySelectedMedia += LibraryViewModel_EventPlaySelectedMedia;
                this.libraryViewModel.EventAddSelectedMediaToPlaylist += LibraryViewModel_EventAddSelectedMediaToPlaylist;
                this.RaisePropertyChanged("LibraryViewModel");
            }
        }

        public Media Media
        {
            get
            {
                if (this.playerViewModel != null)
                    return this.playerViewModel.Media;

                return null;
            }
            set
            {
                if (this.playerViewModel != null)
                {
                    this.playerViewModel.Media = value;

                    if (this.playerViewModel.Media == null)
                    {
                        this.MediaPosition = 0;
                        this.IsMediaPlaying = false;
                        this.IsMediaPaused = true;
                        if (this.progressTimer != null)
                            this.progressTimer.Stop();
                    }
                }

                 this.RaisePropertyChanged("Media");
            }
        }

        public Media CurrentMediaOfPlaylist
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.CurrentMedia;

                return null;
            }
        }

        public Media SelectedMediaOfPlaylist
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.SelectedMedia;

                return null;
            }
            set
            {
                if (this.playlistViewModel != null)
                    this.playlistViewModel.SelectedMedia = value;
            }
        }

        public Playlist SelectedPlaylistOfPlaylist
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.SelectedPlaylist;

                return null;
            }
            set
            {
                if (this.playlistViewModel != null)
                    this.playlistViewModel.SelectedPlaylist = value;
            }
        }

        public Media SelectedMediaOfLibrary
        {
            get
            {
                if (this.libraryViewModel != null)
                    return this.libraryViewModel.SelectedMedia;

                return null;
            }
            set
            {
                if (this.libraryViewModel != null)
                    this.libraryViewModel.SelectedMedia = value;
            }
        }

        public string SelectedTypeOfLibrary
        {
            get
            {
                if (this.libraryViewModel != null)
                    return this.libraryViewModel.SelectedType;

                return null;
            }
        }

        public MediaElement MediaElement
        {
            get
            {
                if (this.playerViewModel != null)
                    return this.playerViewModel.MediaElement;

                return null;
            }
        }

        public double MediaPosition
        {
            get
            {
                if (this.playerViewModel != null)
                    return this.playerViewModel.MediaPosition;

                return 0.0;
            }
            set
            {
                if (this.playerViewModel != null)
                    this.playerViewModel.MediaPosition = value;

                this.RaisePropertyChanged("MediaPosition");
                this.RaisePropertyChanged("MediaPositionFormatted");
            }
        }

        public string MediaPositionFormatted
        {
            get
            {
                if (this.playerViewModel != null)
                    return TimeSpan.FromMilliseconds(this.playerViewModel.MediaPosition).ToString("hh\\:mm\\:ss");

                return string.Empty;
            }
        }

        public double MediaDuration
        {
            get
            {
                if (this.playerViewModel != null)
                    return this.playerViewModel.MediaDuration;

                return double.MaxValue;
            }
            set
            {
                if (this.PlayerViewModel != null)
                    this.PlayerViewModel.MediaDuration = value;

                RaisePropertyChanged("MediaDuration");
            }
        }

        public double MediaVolume
        {
            get
            {
                if (this.MediaElement != null)
                    return this.MediaElement.Volume;

                return this.mediaVolume;
            }
            set
            {
                this.mediaVolume = value;
                this.IsMediaMuted = (this.mediaVolume == 0.0)  ? true : false;
                this.MuteModeImage = (this.IsMediaMuted) ? "/Image/App/mute_blue.png" : "/Image/App/mute_black.png";
                if (this.MediaElement != null)
                    this.MediaElement.Volume = value;
                this.RaisePropertyChanged("MediaVolume");
            }
        }

        public bool IsMediaPlaying
        {
            get { return this.isMediaPlaying; }
            set
            {
                this.isMediaPlaying = value;
                this.RaisePropertyChanged("IsMediaPlaying");
            }
        }

        public bool IsMediaPaused
        {
            get { return this.isMediaPaused; }
            set
            {
                this.isMediaPaused = value;
                this.RaisePropertyChanged("IsMediaPaused");
            }
        }

        public bool IsMediaMuted
        {
            get { return this.isMediaMuted; }
            set
            {
                this.isMediaMuted = value;
                this.RaisePropertyChanged("IsMediaMuted");
            }
        }

        public bool IsShuffleMode
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.IsShuffleMode;

                return false;
            }
            set
            {
                if (this.playlistViewModel != null)
                    this.playlistViewModel.IsShuffleMode = value;

                this.RaisePropertyChanged("IsShuffleMode");
            }
        }

        public int IsRepeatMode
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.IsRepeatMode;

                return 0;
            }
            set
            {
                if (this.playlistViewModel != null)
                    this.playlistViewModel.IsRepeatMode = value;

                this.RaisePropertyChanged("IsRepeatMode");
            }
        }

        #region Visibility Properties
        public bool IsSeekProgressBarVisible
        {
            get { return this.isSeekProgressBarVisible; }
            set
            {
                this.isSeekProgressBarVisible = value;
                RaisePropertyChanged("IsSeekProgressBarVisible");
            }
        }

        public bool IsControlerVisible
        {
            get { return this.isControlerVisible; }
            set
            {
                this.isControlerVisible = value;
                this.RaisePropertyChanged("IsControlerVisible");
            }
        }

        public bool IsLibraryVisible
        {
            get
            {
                if (this.libraryViewModel != null)
                    return this.libraryViewModel.IsLibraryVisible;

                return false;
            }
            set
            {
                if (this.libraryViewModel != null)
                {
                    this.libraryViewModel.IsLibraryVisible = value;
                    this.RaisePropertyChanged("IsLibraryVisible");
                }
            }
        }

        public bool IsPlaylistVisible
        {
            get
            {
                if (this.playlistViewModel != null)
                    return this.playlistViewModel.IsPlaylistVisible;

                return false;
            }
            set
            {
                if (this.playlistViewModel != null)
                {
                    this.playlistViewModel.IsPlaylistVisible = value;
                    this.RaisePropertyChanged("IsPlaylistVisible");
                }
            }
        }
        #endregion
        #endregion

        #region Constructors
        public ControlerViewModel()
        {
            this.PlayerViewModel = new PlayerViewModel();
            this.PlaylistViewModel = new PlaylistViewModel();
            this.LibraryViewModel = new LibraryViewModel();

            this.IsControlerVisible = true;
            this.IsMediaPlaying = false;
            this.IsMediaPaused = true;
            this.MediaVolume = 0.5;
            this.IsMediaMuted = false;

            this.SwitchModeImage = "/Image/App/player_black.png";
            this.RepeatModeImage = "/Image/App/repeat_black.png";
            this.ShuffleModeImage = "/Image/App/shuffle_black.png";
            this.MuteModeImage = "/Image/App/mute_black.png";
        }
        #endregion

        #region Commands
        #region Command Media Seek
        public ICommand MediaSeekCommand
        {
            get
            {
                if (this.mediaSeekCommand == null)
                    this.mediaSeekCommand = new DelegateCommand(SeekMedia, CanSeekMedia);

                return this.mediaSeekCommand;
            }
        }

        private void SeekMedia(object param)
        {
            this.seekProgressBar = param as ProgressBar;
            this.seekProgressBar.MouseLeftButtonDown += new MouseButtonEventHandler(SeekSlider_MouseLeftButtonDown);
        }

        private bool CanSeekMedia(object param)
        {
            return (this.MediaElement != null && this.seekProgressBar == null) ? true : false;
        }
        #endregion

        #region Command Media Play
        public ICommand MediaPlayCommand
        {
            get
            {
                if (this.mediaPlayCommand == null)
                    this.mediaPlayCommand = new DelegateCommand(PlayMedia, CanPlayMedia);

                return this.mediaPlayCommand;
            }
        }

        private void PlayMedia(object param)
        {
            this.IsMediaPlaying = true;
            this.IsMediaPaused = false;
            this.MediaElement.Volume = this.MediaVolume;
            this.progressTimer.Start();
            this.MediaElement.Play();
        }

        private bool CanPlayMedia(object param)
        {
            return (this.MediaElement != null) ? true : false;
        }
        #endregion

        #region Command Media Pause
        public ICommand MediaPauseCommand
        {
            get
            {
                if (this.mediaPauseCommand == null)
                    this.mediaPauseCommand = new DelegateCommand(PauseMedia, CanPauseMedia);

                return this.mediaPauseCommand;
            }
        }

        private void PauseMedia(object param)
        {
            this.IsMediaPlaying = false;
            this.IsMediaPaused = true;
            this.progressTimer.Stop();
            this.MediaElement.Pause();
        }

        private bool CanPauseMedia(object param)
        {
            return (this.MediaElement != null) ? true : false;
        }
        #endregion

        #region Command Media Stop
        public ICommand MediaStopCommand
        {
            get
            {
                if (this.mediaStopCommand == null)
                    this.mediaStopCommand = new DelegateCommand(StopMedia, CanStopMedia);

                return this.mediaStopCommand;
            }
        }

        private void StopMedia(object param)
        {
            this.Media = null;
        }

        private bool CanStopMedia(object param)
        {
            return (this.MediaElement != null) ? true : false;
        }
        #endregion

        #region Command Media Previous
        public ICommand MediaPreviousCommand
        {
            get
            {
                if (this.mediaPreviousCommand == null)
                    this.mediaPreviousCommand = new DelegateCommand(PreviousMedia, CanPreviousMedia);

                return this.mediaPreviousCommand;
            }
        }

        private void PreviousMedia(object param)
        {
            this.PlaylistViewModel.PlayPreviousMedia();
        }

        private bool CanPreviousMedia(object param)
        {
            return (this.PlaylistViewModel != null) ? true : false;
        }
        #endregion

        #region Command Media Next
        public ICommand MediaNextCommand
        {
            get
            {
                if (this.mediaNextCommand == null)
                    this.mediaNextCommand = new DelegateCommand(NextMedia, CanNextMedia);

                return this.mediaNextCommand;
            }
        }

        private void NextMedia(object param)
        {
            this.PlaylistViewModel.PlayNextMedia();
        }

        private bool CanNextMedia(object param)
        {
            return (this.PlaylistViewModel != null) ? true : false;
        }
        #endregion

        #region Command Media Mute
        public ICommand MediaMuteCommand
        {
            get
            {
                if (this.mediaMuteCommand == null)
                    this.mediaMuteCommand = new DelegateCommand(MuteMedia, CanMuteMedia);

                return this.mediaMuteCommand;
            }
        }

        private void MuteMedia(object param)
        {
            if (this.IsMediaMuted)
            {
                this.MediaVolume = this.mediaSavedSoundValue;
                this.IsMediaMuted = false;
                this.MuteModeImage = "/Image/App/mute_black.png";
            }
            else
            {
                this.mediaSavedSoundValue = this.MediaVolume;
                this.MediaVolume = 0.0;
                this.IsMediaMuted = true;
                this.MuteModeImage = "/Image/App/mute_blue.png";
            }
        }

        private bool CanMuteMedia(object param)
        {
            return true;
        }
        #endregion

        #region Command Switch Mode
        public ICommand SwitchModeCommand
        {
            get
            {
                if (this.switchModeCommand == null)
                    this.switchModeCommand = new DelegateCommand(SwitchMode, CanSwitchMode);

                return this.switchModeCommand;
            }
        }

        private void SwitchMode(object param)
        {
            this.IsLibraryVisible = !this.IsLibraryVisible;
            this.SwitchModeImage = (this.IsLibraryVisible) ? "/Image/App/player_black.png" : "/Image/App/home_black.png";
        }

        private bool CanSwitchMode(object param)
        {
            return true;
        }
        #endregion

        #region Command Switch Playlist
        public ICommand SwitchPlaylistCommand
        {
            get
            {
                if (this.switchPlaylistCommand == null)
                    this.switchPlaylistCommand = new DelegateCommand(SwitchPlaylist, CanSwitchPlaylist);

                return this.switchPlaylistCommand;
            }
        }
        
        private void SwitchPlaylist(object param)
        {
            this.IsPlaylistVisible = !this.IsPlaylistVisible;

            if (this.EventSwitchPlaylist != null)
                this.EventSwitchPlaylist(this, new EventArgs());
        }

        private bool CanSwitchPlaylist(object param)
        {
            return true;
        }
        #endregion

        #region Command Shuffle Mode
        public ICommand ShuffleModeCommand
        {
            get
            {
                if (this.shuffleModeCommand == null)
                    this.shuffleModeCommand = new DelegateCommand(ShuffleMode, CanShuffleMode);

                return this.shuffleModeCommand;
            }
        }

        private void ShuffleMode(object param)
        {
            this.IsShuffleMode = !this.IsShuffleMode;
            this.ShuffleModeImage = (this.IsShuffleMode) ? "/Image/App/shuffle_blue.png" : "/Image/App/shuffle_black.png";

            this.PlaylistViewModel.RandomizeCurrentPlaylist();
        }

        private bool CanShuffleMode(object param)
        {
            return true;
        }
        #endregion

        #region Command Repeat Mode
        public ICommand RepeatModeCommand
        {
            get
            {
                if (this.repeatModeCommand == null)
                    this.repeatModeCommand = new DelegateCommand(RepeatMode, CanRepeatMode);

                return this.repeatModeCommand;
            }
        }

        private void RepeatMode(object param)
        {
            this.IsRepeatMode = (this.IsRepeatMode + 1 > 2) ? 0 : this.IsRepeatMode + 1;

            if (this.IsRepeatMode == 0)
                this.RepeatModeImage = "/Image/App/repeat_black.png";
            else if (this.IsRepeatMode == 1)
                this.RepeatModeImage = "/Image/App/repeat_blue.png";
            else
                this.RepeatModeImage = "/Image/App/repeat_one_blue.png";
        }

        private bool CanRepeatMode(object param)
        {
            return true;
        }
        #endregion
        #endregion

        #region Events
        public event EventHandler EventSwitchPlaylist;
        public event EventHandler EventMediaOpened;
        public event EventHandler EventMediaDoubleClicked;

        private void PlayerViewModel_MediaOpened(object sender, EventArgs e)
        {
            if (this.MediaElement != null)
            {
                this.InitMedia();

                this.MediaElement.LoadedBehavior = MediaState.Manual;

                this.progressTimer = new DispatcherTimer();
                this.progressTimer.Interval = TimeSpan.FromSeconds(0.1);
                this.progressTimer.Tick += ProgressTimer_Tick;

                if (this.CanPlayMedia(null))
                    this.PlayMedia(null);

                if (this.EventMediaOpened != null)
                    this.EventMediaOpened(this, new EventArgs());
            }
        }

        private void PlayerViewModel_MediaEnded(object sender, EventArgs e)
        {
            this.PlaylistViewModel.PlayNextMedia();
        }

        private void PlaylistViewModel_EventChangeCurrentMedia(object sender, EventArgs e)
        {
            Media tmp = this.CurrentMediaOfPlaylist;

            this.Media = null;
            this.Media = tmp;

            if (!(this.Media is Music) && this.IsLibraryVisible && this.CanSwitchMode(null))
                this.SwitchMode(null);
        }

        private void PlaylistViewModel_EventChangeSelectedMediaOfLibrary(object sender, EventArgs e)
        {
            this.SelectedMediaOfLibrary = this.CurrentMediaOfPlaylist;
        }

        private void LibraryViewModel_EventMediaAdded(object sender, EventArgs e)
        {
            this.PlaylistViewModel.AddMediaOnCurrentPlaylistIfCurrentPlaylistIsLibrary(sender as Media);
        }

        private void LibraryViewModel_EventPlaySelectedMedia(object sender, EventArgs e)
        {
            Playlist playlist = new Playlist(0);
            ObservableCollection<Media> medias = new ObservableCollection<Media>();
            Media mediaTmp = this.SelectedMediaOfPlaylist;
            Playlist playlistTmp = this.SelectedPlaylistOfPlaylist;

            if (this.SelectedTypeOfLibrary == "Music")
            {
                playlist.Id = -1;
                for (int i = 0; i < this.LibraryViewModel.Musics.Count; i++)
                    medias.Add(this.LibraryViewModel.Musics[i]);
            }
            else if (this.SelectedTypeOfLibrary == "Video")
            {
                playlist.Id = -2;
                for (int i = 0; i < this.LibraryViewModel.Videos.Count; i++)
                    medias.Add(this.LibraryViewModel.Videos[i]);
            }
            else
            {
                playlist.Id = -3;
                for (int i = 0; i < this.LibraryViewModel.Pictures.Count; i++)
                    medias.Add(this.LibraryViewModel.Pictures[i]);
            }
            playlist.Medias = medias;

            this.SelectedMediaOfPlaylist = this.SelectedMediaOfLibrary;
            this.SelectedPlaylistOfPlaylist = playlist;

            this.PlaylistViewModel.PlaySelectedMedia(null);

            this.SelectedMediaOfPlaylist = mediaTmp;
            this.SelectedPlaylistOfPlaylist = playlistTmp;
        }

        private void LibraryViewModel_EventDelSelectedMedia(object sender, EventArgs e)
        {
            this.PlaylistViewModel.DelMediaFromAllPlaylists(this.SelectedMediaOfLibrary);
        }

        private void LibraryViewModel_EventAddSelectedMediaToPlaylist(object sender, EventArgs e)
        {
            this.PlaylistViewModel.AddToTrackToPlaylist(this.SelectedMediaOfLibrary);
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (this.MediaElement != null)
                this.MediaPosition = this.MediaElement.Position.TotalMilliseconds;
        }

        private void SeekSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.seekProgressBar != null)
            {
                double position = e.GetPosition(this.seekProgressBar).X;
                double percent = position / this.seekProgressBar.ActualWidth;

                this.Seek(percent);
            }
        }

        private void PlayerViewModel_MediaDoubleClick(object sender, EventArgs e)
        {
            if (this.EventMediaDoubleClicked != null)
                this.EventMediaDoubleClicked(sender, e);
        }
        #endregion

        #region Methods
        private void InitMedia()
        {
            this.MediaPosition = 0;

            if (this.PlayerViewModel.Media is Picture)
            {
                this.IsSeekProgressBarVisible = false;
                this.MediaDuration = TimeSpan.FromSeconds(3).TotalMilliseconds;
            }
            else
            {
                this.IsSeekProgressBarVisible = true;
                this.MediaDuration = this.MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            }
        }

        private void Seek(double pourcent)
        {
            if (this.MediaElement != null)
            {
                double duration = this.MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                int newPosition = (int)(duration * pourcent);
                this.MediaElement.Position = new TimeSpan(0, 0, 0, 0, newPosition);
                this.MediaPosition = this.MediaElement.Position.TotalMilliseconds;
            }
        }
        #endregion

        #region Images
        private string switchModeImage;
        public string SwitchModeImage
        {
            get { return this.switchModeImage; }
            set
            {
                this.switchModeImage = value;
                this.RaisePropertyChanged("SwitchModeImage");
            }
        }

        private string repeatModeImage;
        public string RepeatModeImage
        {
            get { return this.repeatModeImage; }
            set
            {
                this.repeatModeImage = value;
                this.RaisePropertyChanged("RepeatModeImage");
            }
        }

        private string shuffleModeImage;
        public string ShuffleModeImage
        {
            get { return this.shuffleModeImage; }
            set
            {
                this.shuffleModeImage = value;
                this.RaisePropertyChanged("ShuffleModeImage");
            }
        }

        private string muteModeImage;
        public string MuteModeImage
        {
            get { return this.muteModeImage; }
            set
            {
                this.muteModeImage = value;
                this.RaisePropertyChanged("MuteModeImage");
            }
        }
        #endregion
    }
}
