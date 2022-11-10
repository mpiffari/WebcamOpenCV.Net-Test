using DirectShowLib;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static AnchorTest.Enums;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace AnchorTest
{
    public delegate void OnWebcamStatusHandler(WebcamStatus status, string cameraName);
    public delegate void OnFrameEventHandler(Mat frame);
    public delegate void OnCameraErrorEventHandler(string errorMsg);

    public class ObpWebcam
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public ObpWebcam(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }

    public class Ch1Settings : IWebcamSettings<Ch1Settings>
    {
        /// <inheritdoc />
        public bool FlipVertical
        {
            get => (bool)Properties.Ch1Settings.Default[nameof(FlipVertical)];
            set => Properties.Ch1Settings.Default[nameof(FlipVertical)] = value;
        }

        /// <inheritdoc />
        public bool FlipHorizontal
        {
            get => (bool)Properties.Ch1Settings.Default[nameof(FlipHorizontal)];
            set => Properties.Ch1Settings.Default[nameof(FlipHorizontal)] = value;
        }

        /// <inheritdoc />
        public int RotateClockStep
        {
            get => (int)Properties.Ch1Settings.Default[nameof(RotateClockStep)];
            set => Properties.Ch1Settings.Default[nameof(RotateClockStep)] = value;
        }

        /// <inheritdoc />
        public int Brightness
        {
            get => (int)Properties.Ch1Settings.Default[nameof(Brightness)];
            set => Properties.Ch1Settings.Default[nameof(Brightness)] = value;
        }

        /// <inheritdoc />
        public int Contrast
        {
            get => (int)Properties.Ch1Settings.Default[nameof(Contrast)];
            set => Properties.Ch1Settings.Default[nameof(Contrast)] = value;
        }

        /// <inheritdoc />
        public int Red
        {
            get => (int)Properties.Ch1Settings.Default[nameof(Red)];
            set => Properties.Ch1Settings.Default[nameof(Red)] = value;
        }

        /// <inheritdoc />
        public int Green
        {
            get => (int)Properties.Ch1Settings.Default[nameof(Green)];
            set => Properties.Ch1Settings.Default[nameof(Green)] = value;
        }

        /// <inheritdoc />
        public int Blue
        {
            get => (int)Properties.Ch1Settings.Default[nameof(Blue)];
            set => Properties.Ch1Settings.Default[nameof(Blue)] = value;
        }

        /// <inheritdoc />
        public int CameraIndex
        {
            get => (int)Properties.Ch1Settings.Default[nameof(CameraIndex)];
            set => Properties.Ch1Settings.Default[nameof(CameraIndex)] = value;
        }

        /// <inheritdoc />
        public string CameraName
        {
            get => (string)Properties.Ch1Settings.Default[nameof(CameraName)];
            set => Properties.Ch1Settings.Default[nameof(CameraName)] = value;
        }

        /// <inheritdoc />
        public string CameraId
        {
            get => (string)Properties.Ch1Settings.Default[nameof(CameraId)];
            set => Properties.Ch1Settings.Default[nameof(CameraId)] = value;
        }

        /// <inheritdoc />
        public string SnapshotPath
        {
            get => (string)Properties.Ch1Settings.Default[nameof(SnapshotPath)];
            set => Properties.Ch1Settings.Default[nameof(SnapshotPath)] = value;
        }

        /// <inheritdoc />
        public int ZoomFactor
        {
            get => (int)Properties.Ch1Settings.Default[nameof(ZoomFactor)];
            set => Properties.Ch1Settings.Default[nameof(ZoomFactor)] = value;
        }

        /// <inheritdoc />
        public int? ZoomCenterOnSettingsAreaX
        {
            get => (int?)Properties.Ch1Settings.Default[nameof(ZoomCenterOnSettingsAreaX)];
            set => Properties.Ch1Settings.Default[nameof(ZoomCenterOnSettingsAreaX)] = value;
        }

        /// <inheritdoc />
        public int? ZoomCenterOnSettingsAreaY
        {
            get => (int?)Properties.Ch1Settings.Default[nameof(ZoomCenterOnSettingsAreaY)];
            set => Properties.Ch1Settings.Default[nameof(ZoomCenterOnSettingsAreaY)] = value;
        }

        /// <inheritdoc />
        public string GetDescription()
        {
            var result = string.Empty;
            return result;
        }

        /// <inheritdoc />
        public void SaveSettings()
        {
            Properties.Ch1Settings.Default.Save();
        }

        /// <inheritdoc />
        public void UpgradeSettings()
        {
            Properties.Ch1Settings.Default.Upgrade();
        }
    }

    public interface IWebcamSettings<T> where T : class, IWebcamSettings<T>
    {
        /// <summary>
        /// 
        /// </summary>
        bool FlipVertical { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool FlipHorizontal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int RotateClockStep { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Brightness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Contrast { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Red { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Green { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int Blue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int CameraIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string CameraName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string CameraId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string SnapshotPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int ZoomFactor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? ZoomCenterOnSettingsAreaX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int? ZoomCenterOnSettingsAreaY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetDescription();

        /// <summary>
        /// 
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// 
        /// </summary>
        void UpgradeSettings();
    }

    public interface IWebcamService<T> where T : class, IWebcamSettings<T>
    {
        /// <summary>
        /// Image param
        /// </summary>
        int RotateClockStep { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        bool FlipVertical { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        bool FlipHorizontal { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        int Brightness { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        int Contrast { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        int Red { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        int Green { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        int Blue { get; set; }

        /// <summary>
        /// Image param
        /// </summary>
        string SnapshotPath { get; set; }

        /// <summary>
        /// Flag used to indicate if a webcam thread is already running
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Size? AcquiredFrameSize { get; set; }

        /// <summary>
        /// Points to plot over frame
        /// </summary>
        List<ObpFramePoint>? Points { get; set; }

        /// <summary>
        /// Webcam status
        /// </summary>
        WebcamStatus WebcamStatus { get; }

        /// <summary>
        /// Check if a webcam has been already set for specific bench channel
        /// looking in available cams if is available a cam with name and id same
        /// of saved one
        /// </summary>
        /// <returns>Flag that indicate if work can be started or not (false if no webcam has been selected)</returns>
        bool ChannelHasWebcamSet();

        /// <summary>
        /// Task
        /// </summary>
        /// <param name="picBoxSize"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="isSettings">Flag that indicate if method is called from Settings form</param>
        /// <returns></returns>
        void WebcamDoWork(Size picBoxSize, int? selectedIndex = null, CancellationToken cancellationToken = default, bool isWorkingArea = false);

        /// <summary>
        /// Clean video capture
        /// </summary>
        /// <param name="isFail"></param>
        /// <param name="cameraName"></param>
        /// <param name="desc"></param>
        void WebcamWorkCompleted(bool isFail = false, string? cameraName = null, string? desc = null);

        /// <summary>
        /// 
        /// </summary>
        event OnFrameEventHandler OnFrame;

        /// <summary>
        /// 
        /// </summary>
        event OnWebcamStatusHandler OnWebcamStatus;
    }

    public class ObpFramePoint
    {
        public PointType PointType { get; set; }

        public Eye? RelatedEye { get; set; }

        public Point Center { get; set; }

        public PointAppearanceType Appearance { get; set; }

        public int PointDimension { get; set; }

        public Scalar Color { get; set; }

        public int Thickness { get; set; }

        public bool IsEnabled { get; set; }

        public ObpFramePoint(Point center, PointAppearanceType appearance, int pointDimension, Scalar color, int thickness, Eye? eye = null, PointType type = PointType.UserPutted, Boolean enabled = true)
        {
            this.PointType = type;
            this.Center = center;
            this.Appearance = appearance;
            this.PointDimension = pointDimension;
            this.Color = color;
            this.Thickness = thickness;
            this.RelatedEye = eye;
            this.IsEnabled = enabled;
        }
    }

    public class WebcamService<T> : INotifyPropertyChanged, IWebcamService<T> where T : class, IWebcamSettings<T>
    {
        private readonly ILogger logger;
        private readonly IWebcamSettings<T> webcamSettings;

        #region Event delegates

        /// <inheritdoc />
        public event OnFrameEventHandler? OnFrame;

        /// <inheritdoc />
        public event OnWebcamStatusHandler? OnWebcamStatus;

        #endregion

        #region Interface properties

        /// <inheritdoc />
        public WebcamStatus webcamStatus = WebcamStatus.None;
        public WebcamStatus WebcamStatus
        {
            get => webcamStatus;
            set => webcamStatus = value;
        }

        /// <inheritdoc />
        private string webcamFrameInfo = string.Empty;
        public string WebcamFrameInfo
        {
            get => webcamFrameInfo;
            set
            {
                webcamFrameInfo = value;
                OnPropertyChanged(nameof(WebcamFrameInfo));
            }
        }

        #region Frame settings properties

        /// <inheritdoc />
        private int rotateClockStep = 0;
        public int RotateClockStep
        {
            get => rotateClockStep;
            set
            {
                int castedValue;
                if (value < 0)
                {
                    castedValue = 3;
                }
                else if (value > 3)
                {
                    castedValue = 0;
                }
                else
                {
                    castedValue = value;
                }
                rotateClockStep = castedValue;
            }
        }

        /// <inheritdoc />
        private bool flipVertical = false;
        public bool FlipVertical
        {
            get => flipVertical;
            set => flipVertical = value;
        }

        /// <inheritdoc />
        private bool flipHorizontal = false;
        public bool FlipHorizontal
        {
            get => flipHorizontal;
            set => flipHorizontal = value;
        }

        /// <inheritdoc />
        private int brightness = 0;
        public int Brightness
        {
            get => brightness;
            set => brightness = value;
        }

        /// <inheritdoc />
        private int contrast = 0;
        public int Contrast
        {
            get => contrast;
            set => contrast = value;
        }

        /// <inheritdoc />
        private int red = 0;
        public int Red
        {
            get => red;
            set => red = value;
        }

        /// <inheritdoc />
        private int green = 0;
        public int Green
        {
            get => green;
            set => green = value;
        }

        /// <inheritdoc />
        private int blue = 0;
        public int Blue
        {
            get => blue;
            set => blue = value;
        }

        #endregion

        /// <inheritdoc />
        private string snapshotPath = string.Empty;
        public string SnapshotPath
        {
            get => snapshotPath;
            set => snapshotPath = value;
        }

        /// <inheritdoc />
        private Size? acquiredFrameSize = null;
        public Size? AcquiredFrameSize
        {
            get => acquiredFrameSize;
            set => acquiredFrameSize = value;
        }

        /// <inheritdoc />
        private int settingsZoomFactor = 1;
        public int SettingsZoomFactor
        {
            get => settingsZoomFactor;
            set
            {
                int castedValue;
                if (value < 1)
                {
                    castedValue = 1;
                }
                else if (value > 200)
                {
                    castedValue = 200;
                }
                else
                {
                    castedValue = value;
                }
                settingsZoomFactor = castedValue;
            }
        }

        /// <inheritdoc />
        private int workZoomFactor = 1;
        public int WorkZoomFactor
        {
            get => workZoomFactor;
            set
            {
                int castedValue;
                if (value < 1)
                {
                    castedValue = 1;
                }
                else if (value > 200)
                {
                    castedValue = 200;
                }
                else
                {
                    castedValue = value;
                }
                workZoomFactor = castedValue;
            }
        }

        /// <inheritdoc />
        private Point? settingsZoomCenter = null;
        public Point? SettingsZoomCenter
        {
            get => settingsZoomCenter;
            set
            {
                lock (_locker)
                {
                    // value is null only when no settings are set for channel
                    // Add settings zoom point
                    if (value != null && (Points == null || Points.Where(x => x.PointType == PointType.CustomFrameCenter).Count() == 0))
                    {
                        if (Points == null)
                        {
                            Points = new List<ObpFramePoint>();
                        }

                        // For Ch1 and Ch4 not show frame center as default option
                        var isEnabled = true;
                        Points.Add(new ObpFramePoint(value.Value, PointAppearanceType.Cross, 30, Scalar.Red, 3, type: PointType.CustomFrameCenter, enabled: isEnabled));
                    }
                    else if (value != null)
                    {
                        var point = Points.First(x => x.PointType == PointType.CustomFrameCenter);
                        point.Center = value.Value;
                    }
                }

                settingsZoomCenter = value;
            }
        }

        /// <inheritdoc />
        private Point? workZoomCenter = null;
        public Point? WorkZoomCenter
        {
            get => workZoomCenter;
            set => workZoomCenter = value;
        }

        /// <inheritdoc />
        private List<ObpFramePoint>? points;
        public List<ObpFramePoint>? Points
        {
            get { lock (_locker) { return points; } }
            set { lock (_locker) { points = value; } }
        }

        /// <inheritdoc />
        private bool isRunning;
        public bool IsRunning
        {
            get { lock (_locker) { return isRunning; } }
            set { lock (_locker) { isRunning = value; } }
        }

        #endregion

        #region Private properties

        private readonly object _locker = new object();

        private Size? settingsCropSize;

        private Point? settingsCropLeftCorner;

        private Size? workingCropSize;

        private Point? workingCropLeftCorner;

        private VideoCapture capture;

        private List<ObpWebcam> camerasInfo = new List<ObpWebcam>();

        private DateTime lastGcCollect = DateTime.Now;

        #endregion

        public WebcamService(ILogger logger, IWebcamSettings<T> webcamSettings)
        {
            this.logger = logger;
            this.webcamSettings = webcamSettings;
        }

        #region Binding properties

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                if (Application.OpenForms.Count == 0)
                {
                    return;
                }

                var mainForm = Application.OpenForms[0];
                if (mainForm == null)
                {
                    return; // No main form - no calls
                }

                if (mainForm.InvokeRequired)
                {
                    // We are not in UI Thread now
                    mainForm.Invoke(handler, new object[] { this, new PropertyChangedEventArgs(property) });
                }
                else
                {
                    handler(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion

        #region Cameras scanning

        /// <inheritdoc />
        public bool ChannelHasWebcamSet()
        {
            var devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
            camerasInfo = new List<ObpWebcam>();
            foreach (var device in devices)
            {
                var deviceId = "-";
                var pathSplitted = device.DevicePath.Split('#');
                if (pathSplitted.Length >= 3)
                {
                    deviceId = pathSplitted[2];
                    var name = device.Name;
                    if (true)
                    {
                        logger.LogDebug(LogScope.WEBCAM.EventId(), "ID : {1} - Name : {2}", deviceId, name);
                    }
                }
                camerasInfo.Add(new ObpWebcam(device.Name, deviceId));
            }

            return true;
        }

        #endregion

        #region Webcam work

        // <inheritdoc />
        public void WebcamDoWork(Size mainPicBoxSize, int? selectedIndex = null, CancellationToken cancellationToken = default, bool isWorkingArea = false)
        {
            Task.Run(() =>
            {
                if (isRunning)
                {
                    logger.LogWarning(LogScope.WEBCAM.EventId(), "Thread exit: another webcam work is running...");
                    return;
                }
                capture = new VideoCapture();
                isRunning = true;
                Points = new List<ObpFramePoint>();
                AcquiredFrameSize = null;
                var idx = selectedIndex == null ? webcamSettings.CameraIndex : selectedIndex;
                var canOpen = false;
                var cameraName = "---";

                // LoadImageSettings();

                try
                {
                    if (idx != null)
                    {
                        logger.LogDebug(LogScope.WEBCAM.EventId(), "Start opening camera capture at index {1}...", idx);
                        // TODO: check if is better to search by name and/or path and after use specific index retrieved from devices (instead of use selected index from combo box)
                        cameraName = camerasInfo[(int)idx].Name;
                        WebcamStatus = WebcamStatus.Connecting;
                        OnWebcamStatus?.Invoke(WebcamStatus, cameraName);

                        canOpen = capture.Open((int)idx, VideoCaptureAPIs.ANY);
                        logger.LogDebug(LogScope.WEBCAM.EventId(), "Camera capture opened at index {1} with name {2}", idx, cameraName);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(LogScope.WEBCAM.EventId(), ex, "Exception on open video capture for camera {1}", cameraName);
                    WebcamWorkCompleted(true, cameraName);
                    return;
                }

                if (!canOpen)
                {
                    logger.LogError(LogScope.WEBCAM.EventId(), "Cannot open video capture for camera {1}", cameraName);
                    WebcamWorkCompleted(true, cameraName);
                    return;
                }

                if (capture == null || capture.IsDisposed || !capture.IsOpened())
                {
                    logger.LogError(LogScope.WEBCAM.EventId(), "Capture for camera {1} is null {2} /disposed {3} /not opened {4} ", cameraName, capture == null, capture.IsDisposed, !capture.IsOpened());
                    WebcamWorkCompleted(true, cameraName);
                    return;
                }

                Stopwatch firstFrameWatch = new Stopwatch();
                firstFrameWatch.Start();
                int retry = 0;

                // Try setting highest resolution available for camera
                capture.FrameWidth = 40000;
                capture.FrameHeight = 40000;

                bool firstFrameReading = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (false && DateTime.Now > lastGcCollect.AddMilliseconds(50))
                    {
                        GC.Collect();
                        lastGcCollect = DateTime.Now;
                        return;
                    }

                    try
                    {
                        using (var acquiredFrame = capture.RetrieveMat())
                        {
                            AcquiredFrameSize = new Size(acquiredFrame.Width, acquiredFrame.Height);
                            logger.LogTrace(LogScope.WEBCAM.EventId(), "Frame received from camera {2}", cameraName);

                            if (acquiredFrame.Empty())
                            {
                                logger.LogWarning(LogScope.WEBCAM.EventId(), "Retry number {1} capture", retry);
                                retry++;

                                if (retry == 3)
                                {
                                    logger.LogError(LogScope.WEBCAM.EventId(), "Reach maximum retry for reading from camera {1}", cameraName);
                                    WebcamWorkCompleted(true, cameraName);
                                    break;
                                }
                                continue;
                            }

                            if (firstFrameReading)
                            {
                                lock (_locker)
                                {
                                    // Add center of acquired frame
                                    var acquiredFrameCenter = new Point((acquiredFrame.Width / 2), (acquiredFrame.Height / 2));
                                    Points.Add(new ObpFramePoint(acquiredFrameCenter, PointAppearanceType.Cross, 30, Scalar.Green, 3, type: PointType.OriginalFrameCenter, enabled: !isWorkingArea));
                                }

                                WebcamStatus = WebcamStatus.Connected;
                                OnWebcamStatus?.Invoke(WebcamStatus, cameraName);

                                firstFrameWatch.Stop();
                                TimeSpan ts = firstFrameWatch.Elapsed;
                                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                                logger.LogTrace(LogScope.WEBCAM.EventId(), "TIME FOR RETRIEVING FIRST FRAME " + elapsedTime);
                                firstFrameReading = false;
                            }

                            OnFrame?.Invoke(acquiredFrame);

                            Task.Delay((1000 / 10));
                        }
                    }
                    catch (AccessViolationException e)
                    {
                        logger.LogError(LogScope.WEBCAM.EventId(), e, "Exception during capture frame - continue loop");
                        continue;
                    }
                }

                isRunning = false;
            }, cancellationToken).ConfigureAwait(false);
        }

        // <inheritdoc />
        public void WebcamWorkCompleted(bool isFail = false, string? cameraName = null, string? desc = null)
        {
            try
            {
                var name = cameraName;
                if (cameraName == null)
                {
                    name = webcamSettings.CameraName;
                }

                logger.LogDebug(LogScope.WEBCAM.EventId(), "******************* Releasing video capture - {1} *****************", name);
                capture?.Release();
                capture = new VideoCapture();
                logger.LogDebug(LogScope.WEBCAM.EventId(), "******************* Video capture released *****************");

                if (isFail)
                {
                    WebcamStatus = WebcamStatus.Fail;
                    OnWebcamStatus?.Invoke(WebcamStatus, desc == null ? WebcamStatus.Fail.Description(name) : desc);
                }
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
