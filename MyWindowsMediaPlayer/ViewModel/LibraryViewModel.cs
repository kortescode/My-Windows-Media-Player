using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyWindowsMediaPlayer.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MyWindowsMediaPlayer.Dialog;
using GalaSoft.MvvmLight.Command;

namespace MyWindowsMediaPlayer.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
        #region Fields
        private bool isLibraryVisible;
        private int libraryColumnSpan;

        private bool isMusicsGridVisible;
        private bool isVideosGridVisible;
        private bool isPicturesGridVisible;

        private string imageGridSource;
        private const string ImageMusic = "/Image/App/music_folder.png";
        private const string ImagePicture = "/Image/App/picture_folder.png";
        private const string ImageVideo = "/Image/App/video_folder.png";

        private string selectedType;
        private Media selectedMedia;

        private List<MyWindowsMediaPlayer.Model.Path> paths;

        private ICommand viewSelectedTypeCommand;
        private ICommand playSelectedMediaCommand;
        private ICommand addFileToLibraryCommand;
        private ICommand addStreamToLibraryCommand;
        private ICommand delFromLibraryCommand;
        private ICommand libraryPathsCommand;
        private ICommand addToPlaylistSelectedCommand;
        private ICommand modifyCommand;
        private ICommand reorganizeLibraryCommand;
        #endregion

        #region Properties
        public bool IsLibraryVisible
        {
            get { return this.isLibraryVisible; }
            set
            {
                this.isLibraryVisible = value;
                this.RaisePropertyChanged("IsLibraryVisible");
            }
        }

        public int LibraryColumnSpan
        {
            get { return this.libraryColumnSpan; }
            set
            {
                this.libraryColumnSpan = value;
                this.RaisePropertyChanged("LibraryColumnSpan");
            }
        }

        public bool IsMusicsGridVisible
        {
            get { return this.isMusicsGridVisible; }
            set
            {
                this.isMusicsGridVisible = value;
                this.RaisePropertyChanged("IsMusicsGridVisible");
            }
        }

        public bool IsVideosGridVisible
        {
            get { return this.isVideosGridVisible; }
            set
            {
                this.isVideosGridVisible = value;
                this.RaisePropertyChanged("IsVideosGridVisible");
            }
        }

        public bool IsPicturesGridVisible
        {
            get { return this.isPicturesGridVisible; }
            set
            {
                this.isPicturesGridVisible = value;
                this.RaisePropertyChanged("IsPicturesGridVisible");
            }
        }

        public string ImageGridSource
        {
            get { return this.imageGridSource; }
            set
            {
                this.imageGridSource = value;
                this.RaisePropertyChanged("ImageGridSource");
            }
        }

        public string SelectedType
        {
            get { return this.selectedType; }
            set
            {
                this.selectedType = value;
                this.RaisePropertyChanged("SelectedType");
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

        public ObservableCollection<string> Types { get; set; }

        public ObservableCollection<Music> Musics { get; set; }

        public ObservableCollection<Video> Videos { get; set; }

        public ObservableCollection<Picture> Pictures { get; set; }
        #endregion

        #region Constructors
        public LibraryViewModel()
        {
            this.LibraryColumnSpan = 2;
            this.IsLibraryVisible = true;
            
            this.SelectedType = "Music";
            this.Types = new ObservableCollection<string>() { "Music", "Video", "Picture" };

            this.IsMusicsGridVisible = true;
            this.IsVideosGridVisible = false;
            this.IsPicturesGridVisible = false;
            this.ImageGridSource = ImageMusic;

            this.LoadLibrary();
        }
        #endregion

        #region Destructors
        ~LibraryViewModel()
        {
            this.SaveLibrary();
        }
        #endregion

        #region Commands
        #region Command ViewSelectedType
        public ICommand ViewSelectedTypeCommand
        {
            get
            {
                if (this.viewSelectedTypeCommand == null)
                    this.viewSelectedTypeCommand = new DelegateCommand(ViewSelectedType, CanViewSelectedType);

                return this.viewSelectedTypeCommand;
            }
        }

        private void ViewSelectedType(object param)
        {
            this.IsMusicsGridVisible = false;
            this.IsVideosGridVisible = false;
            this.IsPicturesGridVisible = false;

            if (this.SelectedType == "Music")
            {
                this.IsMusicsGridVisible = true;
                this.ImageGridSource = ImageMusic;
            }
            else if (this.SelectedType == "Video")
            {
                this.IsVideosGridVisible = true;
                this.ImageGridSource = ImageVideo;
            }
            else
            {
                this.IsPicturesGridVisible = true;
                this.ImageGridSource = ImagePicture;
            }

            this.SelectedMedia = null;
        }

        private bool CanViewSelectedType(object param)
        {
            return (this.SelectedType != null) ? true : false;
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

        private void PlaySelectedMedia(object param)
        {
            if (this.SelectedMedia != null)
            {
                if (this.EventPlaySelectedMedia != null)
                    this.EventPlaySelectedMedia(this, new EventArgs());
            }
        }

        private bool CanPlaySelectedMedia(object param)
        {
            return true;
        }
        #endregion

        #region Command AddFileToLibrary
        public ICommand AddFileToLibraryCommand
        {
            get
            {
                if (this.addFileToLibraryCommand == null)
                    this.addFileToLibraryCommand = new DelegateCommand(AddFileToLibrary, CanAddFileToLibrary);

                return this.addFileToLibraryCommand;
            }
        }

        private void AddFileToLibrary(object param)
        {
            List<string> files;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Add File To Library";
            dialog.Filter = "Audio Files (*.wav;*.mp3)|*.wav;*.mp3|Video Files (*.avi;*.mpg;*.mpeg;*.wmv;*.mov;*.mp4)|*.avi;*.mpg;*.mpeg;*.wmv;*.mov;*.mp4|Image Files (*.gif;*.jpg;*.jpeg;*.png;*.bpm;*.tif)|*.gif;*.jpg;*.jpeg;*.png;*.bpm;*.tif";
            dialog.Multiselect = true;
            dialog.ShowDialog();

            files = dialog.FileNames.ToList();

            if (files != null && files.Count > 0)
            {
                string ext = System.IO.Path.GetExtension(files[0]);

                for (int i = 0; i < files.Count; i++)
                {
                    if (ext == ".wav" || ext == ".mp3")
                    {
                        int idMaxMusic = (this.Musics.Count > 0) ? this.Musics.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                        Music music = new Music(idMaxMusic, new Uri(files[i]), null);
                        List<Music> exist = (from item in this.Musics where item.Source.AbsolutePath == music.Source.AbsolutePath select item).ToList();
                        if (exist.Count != 0)
                            MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                        {
                            this.Musics.Add(music);
                            if (this.EventMediaAdded != null)
                                this.EventMediaAdded(music, new EventArgs());
                        }
                    }
                    else if (ext == ".avi" || ext == ".mpg" || ext == ".mpeg" || ext == ".wmv" || ext == ".mov" || ext == ".mp4")
                    {
                        int idMaxVideo = (this.Videos.Count > 0) ? this.Videos.OrderByDescending(item => item.Id).First().Id + 1: 1;
                        Video video = new Video(idMaxVideo, new Uri(files[i]), null);
                        List<Video> exist = (from item in this.Videos where item.Source.AbsolutePath == video.Source.AbsolutePath select item).ToList();
                        if (exist.Count != 0)
                            MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                        {
                            this.Videos.Add(video);
                            if (this.EventMediaAdded != null)
                                this.EventMediaAdded(video, new EventArgs());
                        }
                    }
                    else if (ext == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bpm" || ext == ".tif")
                    {
                        int idMaxPicture = (this.Pictures.Count > 0) ? this.Pictures.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                        Picture picture = new Picture(idMaxPicture, new Uri(files[i]), null);
                        List<Picture> exist = (from item in this.Pictures where item.Source.AbsolutePath == picture.Source.AbsolutePath select item).ToList();
                        if (exist.Count != 0)
                            MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                        {
                            this.Pictures.Add(picture);
                            if (this.EventMediaAdded != null)
                                this.EventMediaAdded(picture, new EventArgs());
                        }
                    }
                    else
                        MessageBox.Show("Entered stream has a bad extension.", "Bad Extension Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool CanAddFileToLibrary(object param)
        {
            return true;
        }
        #endregion

        #region Command AddStreamToLibrary
        public ICommand AddStreamToLibraryCommand
        {
            get
            {
                if (this.addStreamToLibraryCommand == null)
                    this.addStreamToLibraryCommand = new DelegateCommand(AddStreamToLibrary, CanAddStreamToLibrary);

                return this.addStreamToLibraryCommand;
            }
        }

        private void AddStreamToLibrary(object param)
        {
            string stream = InputDialog.ShowDialog("Add Stream To Library", "Media Url: ", "");

            if (stream != "")
            {
                string ext = System.IO.Path.GetExtension(stream);

                if (ext == ".wav" || ext == ".mp3")
                {
                    int idMaxMusic = (this.Musics.Count > 0) ? this.Musics.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Music music = new Music(idMaxMusic, new Uri(stream), null);
                    List<Music> exist = (from item in this.Musics where item.Source.AbsolutePath == music.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Musics.Add(music);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(music, new EventArgs());
                    }
                }
                else if (ext == ".avi" || ext == ".mpg" || ext == ".mpeg" || ext == ".wmv" || ext == ".mov" || ext == ".mp4")
                {
                    int idMaxVideo = (this.Videos.Count > 0) ? this.Videos.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Video video = new Video(idMaxVideo, new Uri(stream), null);
                    List<Video> exist = (from item in this.Videos where item.Source.AbsolutePath == video.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Videos.Add(video);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(video, new EventArgs());
                    }
                }
                else if (ext == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bpm" || ext == ".tif")
                {
                    int idMaxPicture = (this.Pictures.Count > 0) ? this.Pictures.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Picture picture = new Picture(idMaxPicture, new Uri(stream), null);
                    List<Picture> exist = (from item in this.Pictures where item.Source.AbsolutePath == picture.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Pictures.Add(picture);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(picture, new EventArgs());
                    }
                }
                else
                    MessageBox.Show("Selected files have bad extensions.", "Bad Extension Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanAddStreamToLibrary(object param)
        {
            return true;
        }
        #endregion

        #region Command DelFromLibrary
        public ICommand DelFromLibraryCommand
        {
            get
            {
                if (this.delFromLibraryCommand == null)
                    this.delFromLibraryCommand = new DelegateCommand(DelFromLibrary, CanDelFromLibrary);

                return this.delFromLibraryCommand;
            }
        }

        private void DelFromLibrary(object param)
        {
            if (this.SelectedMedia != null)
            {
                if (this.SelectedType == "Music")
                    this.Musics.Remove(this.SelectedMedia as Music);
                else if (this.SelectedType == "Video")
                    this.Videos.Remove(this.SelectedMedia as Video);
                else
                    this.Pictures.Remove(this.SelectedMedia as Picture);

                if (this.EventDelSelectedMedia != null)
                    this.EventDelSelectedMedia(this, new EventArgs());

                this.SelectedMedia = null;
            }
        }

        private bool CanDelFromLibrary(object param)
        {
            return true;
        }
        #endregion

        #region Command LibraryPaths
        public ICommand LibraryPathsCommand
        {
            get
            {
                if (this.libraryPathsCommand == null)
                    this.libraryPathsCommand = new DelegateCommand(LibraryPaths, CanLibraryPaths);

                return this.libraryPathsCommand;
            }
        }

        private void LibraryPaths(object param)
        {
            LibraryPathsDialog.ShowDialog("Library Paths", "Contained Media's Paths:", this.paths, this);
        }

        private bool CanLibraryPaths(object param)
        {
            return true;
        }
        #endregion

        #region Command AddToPlaylistSelected
        public ICommand AddToPlaylistSelectedCommand
        {
            get
            {
                if (this.addToPlaylistSelectedCommand == null)
                    this.addToPlaylistSelectedCommand = new DelegateCommand(AddToPlaylistSelected, CanAddToPlaylistSelected);

                return this.addToPlaylistSelectedCommand;
            }
        }

        private void AddToPlaylistSelected(object param)
        {
            if (this.SelectedMedia != null)
            {
                if (this.EventAddSelectedMediaToPlaylist != null)
                    this.EventAddSelectedMediaToPlaylist(this, new EventArgs());
            }
        }

        private bool CanAddToPlaylistSelected(object param)
        {
            return true;
        }
        #endregion

        #region Command Modify
        public ICommand ModifyCommand
        {
            get
            {
                if (this.modifyCommand == null)
                    this.modifyCommand = new DelegateCommand(Modify, CanModify);

                return this.modifyCommand;
            }
        }

        private void Modify(object param)
        {
            if (this.SelectedMedia != null)
            {
                ModifyMediaDialog.ShowDialog("Modify", "Modify " + this.SelectedType + ":", this.SelectedType, this.SelectedMedia);
                // REFRESH LA GRID !!!
            }
        }

        private bool CanModify(object param)
        {
            return true;
        }
        #endregion

        #region Command ReorganizeLibrary
        public ICommand ReorganizeLibraryCommand
        {
            get
            {
                if (this.reorganizeLibraryCommand == null)
                    this.reorganizeLibraryCommand = new DelegateCommand(ReorganizeLibrary, CanReorganizeLibrary);

                return this.reorganizeLibraryCommand;
            }
        }

        private void ReorganizeLibrary(object param)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            Media selectedMediaTmp = this.SelectedMedia;
            string selectedTypeTmp = this.SelectedType;

            for (int i = 0; i < this.Musics.Count; i++)
            {
                if (!System.IO.File.Exists(this.Musics[i].Source.LocalPath))
                {
                    this.SelectedMedia = this.Musics[i];
                    this.SelectedType = "Music";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }
            for (int i = 0; i < this.Videos.Count; i++)
            {
                if (!System.IO.File.Exists(this.Videos[i].Source.LocalPath))
                {
                    this.SelectedMedia = this.Videos[i];
                    this.SelectedType = "Video";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }
            for (int i = 0; i < this.Pictures.Count; i++)
            {
                if (!System.IO.File.Exists(this.Pictures[i].Source.LocalPath))
                {
                    this.SelectedMedia = this.Pictures[i];
                    this.SelectedType = "Picture";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }

            for (int i = 0; i < this.paths.Count; i++)
                this.AddMediasFromPath(this.paths[i]);

            this.SelectedMedia = selectedMediaTmp;
            this.SelectedType = selectedTypeTmp;

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        private bool CanReorganizeLibrary(object param)
        {
            return true;
        }
        #endregion
        #endregion

        #region Methods
        public void AddMedia(string stream)
        {
            if (stream != "")
            {
                string ext = System.IO.Path.GetExtension(stream);

                if (ext == ".wav" || ext == ".mp3")
                {
                    int idMaxMusic = (this.Musics.Count > 0) ? this.Musics.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Music music = new Music(idMaxMusic, new Uri(stream), null);
                    List<Music> exist = (from item in this.Musics where item.Source.AbsolutePath == music.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Musics.Add(music);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(music, new EventArgs());
                    }
                }
                else if (ext == ".avi" || ext == ".mpg" || ext == ".mpeg" || ext == ".wmv" || ext == ".mov" || ext == ".mp4")
                {
                    int idMaxVideo = (this.Videos.Count > 0) ? this.Videos.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Video video = new Video(idMaxVideo, new Uri(stream), null);
                    List<Video> exist = (from item in this.Videos where item.Source.AbsolutePath == video.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Videos.Add(video);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(video, new EventArgs());
                    }
                }
                else if (ext == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bpm" || ext == ".tif")
                {
                    int idMaxPicture = (this.Pictures.Count > 0) ? this.Pictures.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    Picture picture = new Picture(idMaxPicture, new Uri(stream), null);
                    List<Picture> exist = (from item in this.Pictures where item.Source.AbsolutePath == picture.Source.AbsolutePath select item).ToList();
                    if (exist.Count != 0)
                        MessageBox.Show("Selected file already exists.", "File Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        this.Pictures.Add(picture);
                        if (this.EventMediaAdded != null)
                            this.EventMediaAdded(picture, new EventArgs());
                    }
                }
                else
                    MessageBox.Show("Selected files have bad extensions.", "Bad Extension Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddMediasFromPath(MyWindowsMediaPlayer.Model.Path path)
        {
            Stack<string> dirs = new Stack<string>();

            if (System.IO.Directory.Exists(path.PathSource.LocalPath))
            {
                dirs.Push(path.PathSource.LocalPath);

                while (dirs.Count > 0)
                {
                    string currentDir = dirs.Pop();
                    string[] subDirs;
                    string[] files = null;

                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    try
                    {
                        files = Directory.GetFiles(currentDir);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    foreach (string file in files)
                    {
                        try
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(file);
                            string ext = System.IO.Path.GetExtension(fi.FullName);

                            if (ext == ".wav" || ext == ".mp3")
                            {
                                int idMaxMusic = (this.Musics.Count > 0) ? this.Musics.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                                Music music = new Music(idMaxMusic, new Uri(fi.FullName), path);
                                List<Music> exist = (from item in this.Musics where item.Source.AbsolutePath == music.Source.AbsolutePath select item).ToList();
                                if (exist.Count == 0)
                                {
                                    this.Musics.Add(music);
                                    if (this.EventMediaAdded != null)
                                        this.EventMediaAdded(music, new EventArgs());
                                }
                            }
                            else if (ext == ".avi" || ext == ".mpg" || ext == ".mpeg" || ext == ".wmv" || ext == ".mov" || ext == ".mp4")
                            {
                                int idMaxVideo = (this.Videos.Count > 0) ? this.Videos.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                                Video video = new Video(idMaxVideo, new Uri(fi.FullName), path);
                                List<Video> exist = (from item in this.Videos where item.Source.AbsolutePath == video.Source.AbsolutePath select item).ToList();
                                if (exist.Count == 0)
                                {
                                    this.Videos.Add(video);
                                    if (this.EventMediaAdded != null)
                                        this.EventMediaAdded(video, new EventArgs());
                                }
                            }
                            else if (ext == ".gif" || ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bpm" || ext == ".tif")
                            {
                                int idMaxPicture = (this.Pictures.Count > 0) ? this.Pictures.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                                Picture picture = new Picture(idMaxPicture, new Uri(fi.FullName), path);
                                List<Picture> exist = (from item in this.Pictures where item.Source.AbsolutePath == picture.Source.AbsolutePath select item).ToList();
                                if (exist.Count == 0)
                                {
                                    this.Pictures.Add(picture);
                                    if (this.EventMediaAdded != null)
                                        this.EventMediaAdded(picture, new EventArgs());
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }

                    foreach (string str in subDirs)
                        dirs.Push(str);
                }
            }
        }

        public void DelMediasFromPath(MyWindowsMediaPlayer.Model.Path path)
        {
            Media selectedMediaTmp = this.SelectedMedia;
            string selectedTypeTmp = this.SelectedType;

            for (int i = 0; i < this.Musics.Count; i++)
            {
                if (this.Musics[i].Path != null && path.Id == this.Musics[i].Path.Id)
                {
                    this.SelectedMedia = this.Musics[i];
                    this.SelectedType = "Music";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }
            for (int i = 0; i < this.Videos.Count; i++)
            {
                if (this.Videos[i].Path != null && path.Id == this.Videos[i].Path.Id)
                {
                    this.SelectedMedia = this.Videos[i];
                    this.SelectedType = "Video";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }
            for (int i = 0; i < this.Pictures.Count; i++)
            {
                if (this.Pictures[i].Path != null && path.Id == this.Pictures[i].Path.Id)
                {
                    this.SelectedMedia = this.Pictures[i];
                    this.SelectedType = "Picture";
                    this.DelFromLibrary(null);
                    i = -1;
                }
            }

            this.SelectedMedia = selectedMediaTmp;
            this.SelectedType = selectedTypeTmp;
        }

        private void LoadLibrary()
        {
            var musicsTmp = new ObservableCollection<Music>();
            var videosTmp = new ObservableCollection<Video>();
            var picturesTmp = new ObservableCollection<Picture>();
            var pathsTmp = new List<MyWindowsMediaPlayer.Model.Path>();

            FileStream musicsFs = new FileStream("../../Config/musics.xml", FileMode.OpenOrCreate);
            FileStream videosFs = new FileStream("../../Config/videos.xml", FileMode.OpenOrCreate);
            FileStream picturesFs = new FileStream("../../Config/pictures.xml", FileMode.OpenOrCreate);
            FileStream pathsFs = new FileStream("../../Config/paths.xml", FileMode.OpenOrCreate);

            DataContractSerializer musicsDcs = new DataContractSerializer(typeof(ObservableCollection<Music>));
            DataContractSerializer videosDcs = new DataContractSerializer(typeof(ObservableCollection<Video>));
            DataContractSerializer picturesDcs = new DataContractSerializer(typeof(ObservableCollection<Picture>));
            DataContractSerializer pathsDcs = new DataContractSerializer(typeof(List<MyWindowsMediaPlayer.Model.Path>));

            try { musicsTmp = musicsDcs.ReadObject(musicsFs) as ObservableCollection<Music>; }
            catch { }

            try { videosTmp = videosDcs.ReadObject(videosFs) as ObservableCollection<Video>; }
            catch { }

            try { picturesTmp = picturesDcs.ReadObject(picturesFs) as ObservableCollection<Picture>; }
            catch { }

            try { pathsTmp = pathsDcs.ReadObject(pathsFs) as List<MyWindowsMediaPlayer.Model.Path>; }
            catch { }

            this.Musics = musicsTmp;
            this.Videos = videosTmp;
            this.Pictures = picturesTmp;
            this.paths = pathsTmp;

            musicsFs.Close();
            videosFs.Close();
            picturesFs.Close();
            pathsFs.Close();
        }

        private void SaveLibrary()
        {
            FileStream musicsFs = new FileStream("../../Config/musics.xml", FileMode.Create);
            FileStream videosFs = new FileStream("../../Config/videos.xml", FileMode.Create);
            FileStream picturesFs = new FileStream("../../Config/pictures.xml", FileMode.Create);
            FileStream pathsFs = new FileStream("../../Config/paths.xml", FileMode.Create);

            DataContractSerializer musicsDcs = new DataContractSerializer(typeof(ObservableCollection<Music>));
            DataContractSerializer videosDcs = new DataContractSerializer(typeof(ObservableCollection<Video>));
            DataContractSerializer picturesDcs = new DataContractSerializer(typeof(ObservableCollection<Picture>));
            DataContractSerializer pathsDcs = new DataContractSerializer(typeof(List<MyWindowsMediaPlayer.Model.Path>));

            musicsDcs.WriteObject(musicsFs, this.Musics);
            videosDcs.WriteObject(videosFs, this.Videos);
            picturesDcs.WriteObject(picturesFs, this.Pictures);
            pathsDcs.WriteObject(pathsFs, this.paths);

            musicsFs.Close();
            videosFs.Close();
            picturesFs.Close();
            pathsFs.Close();
        }
        #endregion

        #region Events
        public event EventHandler EventPlaySelectedMedia;
        public event EventHandler EventDelSelectedMedia;
        public event EventHandler EventMediaAdded;
        public event EventHandler EventAddSelectedMediaToPlaylist;
        #endregion
    }
}
