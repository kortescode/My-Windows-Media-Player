using MyWindowsMediaPlayer.Model;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields
        private ControlerViewModel controlerViewModel;

        private int columnSpan;

        private double visibilityTime;
        private DispatcherTimer visibilityTimer;

        private ICommand mouseMoveCommand;

        private bool isYoutubeVisible;
        private ICommand youtubeCommand;
        private const string YoutubeHomePage = "http://www.youtube.com/";
        #endregion

        #region Properties
        public ControlerViewModel ControlerViewModel
        {
            get { return this.controlerViewModel; }
            set
            {
                this.controlerViewModel = value;
                this.controlerViewModel.EventSwitchPlaylist += ControlerViewModel_SwitchPlaylist;
                this.controlerViewModel.EventMediaOpened += ControlerViewModel_MediaOpened;
                this.controlerViewModel.EventMediaDoubleClicked += ControlerViewModel_MediaDoubleClicked;
                this.RaisePropertyChanged("ControlerViewModel");
            }
        }

        public PlayerViewModel PlayerViewModel
        {
            get
            {
                if (this.controlerViewModel != null && this.controlerViewModel.PlayerViewModel != null)
                    return this.controlerViewModel.PlayerViewModel;

                return null;
            }
        }

        public LibraryViewModel LibraryViewModel
        {
            get
            {
                if (this.controlerViewModel != null && this.controlerViewModel.LibraryViewModel != null)
                    return this.controlerViewModel.LibraryViewModel;

                return null;
            }
        }

        public PlaylistViewModel PlaylistViewModel
        {
            get
            {
                if (this.controlerViewModel != null && this.controlerViewModel.PlaylistViewModel != null)
                    return this.controlerViewModel.PlaylistViewModel;

                return null;
            }
        }

        public Media Media
        {
            get
            {
                if (this.controlerViewModel != null && this.controlerViewModel.PlayerViewModel != null)
                    return this.controlerViewModel.PlayerViewModel.Media;

                return null;
            }
        }

        public double VisibilityTime
        {
            get { return this.visibilityTime; }
            set
            {
                this.visibilityTime = value;
                this.RaisePropertyChanged("VisibilityTime");
            }
        }

        public bool IsYoutubeVisible
        {
            get { return this.isYoutubeVisible; }
            set
            {
                this.isYoutubeVisible = value;
                this.RaisePropertyChanged("IsYoutubeVisible");
            }
        }
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            this.ControlerViewModel = new ControlerViewModel();

            this.columnSpan = 1;
            this.SetVisibility();

            this.IsYoutubeVisible = false;

            this.InitVisibleTimer();
        }
        #endregion

        #region Commands
        #region Command MouseMove
        public ICommand MouseMoveCommand
        {
            get
            {
                if (this.mouseMoveCommand == null)
                    this.mouseMoveCommand = new DelegateCommand(MouseMove, CanMouseMove);

                return this.mouseMoveCommand;
            }
        }

        private void MouseMove(object param)
        {
            this.VisibilityTime = 0;
        }

        private bool CanMouseMove(object param)
        {
            return true;
        }
        #endregion

        #region Command Youtube
        public ICommand YoutubeCommand
        {
            get
            {
                if (this.youtubeCommand == null)
                    this.youtubeCommand = new DelegateCommand(ShowHideYoutube, CanShowHideYoutube);

                return this.youtubeCommand;
            }
        }

        private void ShowHideYoutube(object param)
        {
            if (!this.IsYoutubeVisible && this.ControlerViewModel != null)
                this.ControlerViewModel.Media = null;

            if (this.IsYoutubeVisible && param != null)
                ((System.Windows.Controls.WebBrowser)param).Source = new Uri(YoutubeHomePage);

            this.IsYoutubeVisible = !this.IsYoutubeVisible;
        }

        private bool CanShowHideYoutube(object param)
        {
            return true;
        }
        #endregion
        #endregion

        #region Events
        public event EventHandler EventMediaDoubleClicked;

        private void ControlerViewModel_SwitchPlaylist(object sender, EventArgs e)
        {
            this.columnSpan = (this.columnSpan > 1) ? 1 : 2;
            this.SetVisibility();
        }

        private void ControlerViewModel_MediaOpened(object sender, EventArgs e)
        {
            this.SetVisibility();
        }

        private void VisibilityTimer_Tick(object sender, EventArgs e)
        {
            this.VisibilityTime += 0.1;

            if (this.controlerViewModel != null)
            {
                if (this.VisibilityTime > 3)
                    this.controlerViewModel.IsControlerVisible = false;
                else
                    this.controlerViewModel.IsControlerVisible = true;
            }
        }

        private void ControlerViewModel_MediaDoubleClicked(object sender, EventArgs e)
        {
            if (this.EventMediaDoubleClicked != null)
                this.EventMediaDoubleClicked(sender, e);
        }
        #endregion

        #region Methods
        private void SetVisibility()
        {
            if (this.PlayerViewModel != null)
                this.PlayerViewModel.PlayerColumnSpan = this.columnSpan;
            if (this.LibraryViewModel != null)
                this.LibraryViewModel.LibraryColumnSpan = this.columnSpan;
        }

        private void InitVisibleTimer()
        {
            this.visibilityTimer = new DispatcherTimer();
            this.visibilityTimer.Interval = TimeSpan.FromSeconds(0.1);
            this.visibilityTimer.Tick += VisibilityTimer_Tick;
            this.visibilityTimer.Start();
            this.VisibilityTime = 0;
        }
        #endregion
    }
}
