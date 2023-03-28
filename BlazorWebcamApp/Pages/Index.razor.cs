using ElectronNET.API;
using Microsoft.JSInterop;
using OpenCvSharp;

namespace BlazorWebcamApp.Pages
{
    public partial class Index
    {
        private VideoCapture? _capture;
        private CancellationTokenSource? _cancellationTokenSource;

        private async Task StartWebcam()
        {
            _capture = new VideoCapture(0);
            _capture.Set(VideoCaptureProperties.FrameWidth, 640);
            _capture.Set(VideoCaptureProperties.FrameHeight, 480);

            if (!_capture.IsOpened())
            {
                // Error handling
                Electron.Dialog.ShowErrorBox("Error", "No webcam found");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            await Task.Run(async () =>
            {
                using var mat = new Mat();
                while (!token.IsCancellationRequested)
                {
                    _capture.Read(mat);
                    if (mat.Empty()) continue;

                    using var memoryStream = new MemoryStream();
                    var encodeParams = new ImageEncodingParam[] { new(ImwriteFlags.JpegQuality, 70) };
                    mat.WriteToStream(memoryStream, ".jpg", encodeParams);

                    var imageDataUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}";

                    await JSRuntime.InvokeVoidAsync("drawImage", token, "webcamCanvas", imageDataUrl);
                    await Task.Delay(33, token); // 30 FPS
                }
            }, token);
        }

        private void StopWebcam()
        {
            _cancellationTokenSource?.Cancel();
            _capture?.Dispose();
        }
    }
}
