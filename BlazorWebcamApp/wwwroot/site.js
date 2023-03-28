window.drawImage = (canvasId, imageDataUrl) => {
    const canvas = document.getElementById(canvasId);
    const context = canvas.getContext('2d');
    const image = new Image();
    image.onload = () => {
        context.drawImage(image, 0, 0, canvas.width, canvas.height);
    };
    image.src = imageDataUrl;
};