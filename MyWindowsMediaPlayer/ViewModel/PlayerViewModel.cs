using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MyWindowsMediaPlayer.Model;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        #region Fields
        private MediaElement mediaElement;

        private Media media;
        private double mediaDuration;
        private double mediaPosition;
        
        private ICommand mediaOpenedCommand;
        private ICommand mediaEndedCommand;
        private ICommand mediaDoubleClickCommand;

        private int playerColumnSpan;
        #endregion

        #region Properties
        public MediaElement MediaElement
        {
            get { return this.mediaElement; }
        }

        public Media Media
        {
            get { return this.media; }
            set
            {
                this.media = value;
                if (this.media == null)
                {
                    if (this.mediaElement != null)
                    {
                        this.mediaElement.Stop();
                        this.mediaElement = null;
                    }
                    this.MediaPosition = 0;
                    this.MediaDuration = double.MaxValue;
                }

                this.RaisePropertyChanged("Media");
                this.RaisePropertyChanged("MediaSource");
            }
        }

        public Uri MediaSource
        {
            get
            {
                if (this.media != null)
                    return this.media.Source;

                return null;
            }
        }
        
        public double MediaDuration
        {
            get { return this.mediaDuration; }
            set
            {
                this.mediaDuration = value;
                this.RaisePropertyChanged("MediaDuration");
            }
        }

        public double MediaPosition
        {
            get { return this.mediaPosition; }
            set
            {
                this.mediaPosition = value;
                this.RaisePropertyChanged("MediaPosition");
            }
        }

        public int PlayerColumnSpan
        {
            get { return this.playerColumnSpan; }
            set
            {
                this.playerColumnSpan = value;
                this.RaisePropertyChanged("PlayerColumnSpan");
            }
        }
        #endregion

        #region Constructors
        public PlayerViewModel()
        {
            this.MediaPosition = 0;
            this.MediaDuration = double.MaxValue;
        }
        #endregion

        #region Commands
        #region Command Media Opened
        public ICommand MediaOpenedCommand
        {
            get
            {
                if (this.mediaOpenedCommand == null)
                    this.mediaOpenedCommand = new DelegateCommand(MediaOpened, CanMediaOpened);

                return this.mediaOpenedCommand;
            }
        }

        private void MediaOpened(object param)
        {
            this.mediaElement = param as MediaElement;

            this.MediaPosition = 0;
            this.MediaDuration = double.MaxValue;

            if (this.EventMediaOpened != null)
                this.EventMediaOpened(this, new EventArgs());
        }

        private bool CanMediaOpened(object param)
        {
            return true;
        }
        #endregion

        #region Command Media Ended
        public ICommand MediaEndedCommand
        {
            get
            {
                if (this.mediaEndedCommand == null)
                    this.mediaEndedCommand = new DelegateCommand(MediaEnded, CanMediaEnded);

                return this.mediaEndedCommand;
            }
        }

        private void MediaEnded(object param)
        {
            this.MediaPosition = 0;
            this.MediaDuration = double.MaxValue;

            if (this.EventMediaEnded != null)
                this.EventMediaEnded(this, new EventArgs());
        }

        private bool CanMediaEnded(object param)
        {
            return true;
        }
        #endregion

        #region Command Media DoubleClick
        public ICommand MediaDoubleClickCommand
        {
            get
            {
                if (this.mediaDoubleClickCommand == null)
                    this.mediaDoubleClickCommand = new DelegateCommand(MediaDoubleClick, CanMediaDoubleClick);

                return this.mediaDoubleClickCommand;
            }
        }

        private void MediaDoubleClick(object param)
        {
            if (this.EventMediaDoubleClicked != null)
                this.EventMediaDoubleClicked(this, new EventArgs());
        }

        private bool CanMediaDoubleClick(object param)
        {
            return true;
        }
        #endregion
        #endregion

        #region Events
        public event EventHandler EventMediaOpened;
        public event EventHandler EventMediaEnded;
        public event EventHandler EventMediaDoubleClicked;
        #endregion

        #region Methods
        #endregion
    }
}
