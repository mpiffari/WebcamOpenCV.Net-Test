using OpenCvSharp;
using System.ComponentModel;
using System.Drawing;

namespace AnchorTest
{
    public static class Enums
    {
        public enum LogScope
        {
            [Description("Work")]
            WORK,

            [Description("Webcam")]
            WEBCAM,

            [Description("Serial")]
            SERIAL,

            [Description("Serial")]
            NORMATIVE
        }

        public enum ChannelTest : int
        {
            Ch1 = 1,
            Ch2 = 2,
            Ch3 = 3,
            Ch4 = 4,
            Ch5 = 5
        }

        public static string ChannelTestName(this ChannelTest channel)
        {
            return "-";
        }

        public enum WebcamStatus
        {
            None,
            CameraNotAvailable,
            CameraAvailable,
            Fail,
            Connecting,
            Connected
        }

        public static string Description(this WebcamStatus channel, string webcamName)
        {
            return "-";
        }


        public static bool IsError(this WebcamStatus channel)
        {
            switch (channel)
            {
                case WebcamStatus.None:
                    return true;
                case WebcamStatus.CameraNotAvailable:
                    return true;
                case WebcamStatus.Fail:
                    return true;
                case WebcamStatus.Connecting:
                    return false;
                case WebcamStatus.Connected:
                    return false;
                case WebcamStatus.CameraAvailable:
                    return false;
            }
            return false;
        }

        public enum ReportFileType
        {
            TXT,
            CSV,
            EXCEL,
            PDF
        }

        public enum ImageFileType
        {
            [Description("png")]
            PNG,

            [Description("jpeg")]
            JPEG
        }

        public enum PointType
        {
            OriginalFrameCenter,
            CustomFrameCenter,
            UserPutted
        }

        public enum PointAppearanceType
        {
            Cross,
            X,
            Circle,
            CircleWithCross,
            Star
        }

        public enum Eye
        {
            Left,
            Right,
            Monocular
        }

        public static Scalar Color(this Eye channel)
        {
            switch (channel)
            {
                case Eye.Left:
                    return Scalar.Red;
                case Eye.Right:
                    return Scalar.Blue;
                case Eye.Monocular:
                    return Scalar.Black;
            }
            return Scalar.White;
        }

        public enum NotificationAction
        {
            Wait,
            Start,
            Close
        }

        public enum NotificationType
        {
            Success,
            Warning,
            Error,
            Info
        }
    }
}
