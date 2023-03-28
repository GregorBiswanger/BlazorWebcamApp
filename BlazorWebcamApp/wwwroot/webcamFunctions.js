window.webcamFunctions = {
    video: null,
    canvas: null,
    context: null,
    intervalId: null,

    startWebcam: async function (canvasId, dotnetReference) {
        if (this.intervalId) return;

        this.canvas = document.getElementById(canvasId);
        this.context = this.canvas.getContext('2d');

        this.video = document.createElement('video');
        this.video.width = this.canvas.width;
        this.video.height = this.canvas.height;

        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            this.video.srcObject = stream;
            await this.video.play();

            this.intervalId = setInterval(() => {
                this.context.drawImage(this.video, 0, 0, this.canvas.width, this.canvas.height);
                const imageDataUrl = this.canvas.toDataURL('image/jpeg', 0.5);
                dotnetReference.invokeMethodAsync('ProcessFrame', imageDataUrl);
            }, 1000 / 30); // 30 FPS
        } catch (err) {
            console.error('Error starting webcam:', err);
        }
    },

    stopWebcam: function () {
        if (!this.intervalId) return;

        clearInterval(this.intervalId);
        this.intervalId = null;

        if (this.video && this.video.srcObject) {
            const tracks = this.video.srcObject.getTracks();
            tracks.forEach(track => track.stop());
            this.video.srcObject = null;
        }
    }
};