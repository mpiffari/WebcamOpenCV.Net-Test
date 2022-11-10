using Microsoft.Extensions.Logging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Threading;
using System.Windows.Forms;
using static AnchorTest.Enums;

namespace AnchorTest
{
    public partial class Form1 : Form
    {
        private readonly ILogger logger;
        private readonly IWebcamService<Ch1Settings> webcamService;

        public Form1()
        {
            InitializeComponent();

            logger = (ILogger)Program.ServiceProvider.GetService(typeof(ILogger));
            webcamService = (IWebcamService<Ch1Settings>)Program.ServiceProvider.GetService(typeof(IWebcamService<Ch1Settings>));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            webcamService.OnWebcamStatus += OnWebcamStatus;

            var tokenSource = new CancellationTokenSource();

            if (webcamService.ChannelHasWebcamSet() && tokenSource != null)
            {
                webcamService.OnFrame += OnWebcamFrame;
                var idx = 0;
                var mainPictureBoxSize = new Size(pictureBoxWebcam.Width, pictureBoxWebcam.Height);
                webcamService.IsRunning = false;
                webcamService.WebcamDoWork(mainPictureBoxSize, idx, tokenSource.Token, isWorkingArea: true);
            }

        }

        private void OnWebcamFrame(Mat frame)
        {
            OnFrame(frame);
        }

        private void OnFrame(Mat frame)
        {
            var oldFrame = pictureBoxWebcam.Image;

            pictureBoxWebcam.Image = BitmapConverter.ToBitmap(frame);
            if (oldFrame != null)
            {
                oldFrame.Dispose();
            }
        }

        // <inheritdoc />
        public void OnFrameError()
        {
            BeginInvoke(new Action(() =>
            {
                pictureBoxWebcam.Image = pictureBoxWebcam.ErrorImage;
            }));
        }

        // <inheritdoc />
        public void OnWebcamStatus(WebcamStatus status, string cameraName)
        {
            if (status.IsError())
            {
                OnFrameError();
            }
        }
    }
}
