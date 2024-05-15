function bootUnity(unityId) {
    var canvas = document.getElementById(unityId);
    var loader = document.getElementById("unity-load-percent");
    canvas.style.display = 'none';
    var config = {
        dataUrl: "Build/Component.data",
        frameworkUrl: "Build/Component.framework.js",
        codeUrl: "Build/Component.wasm",
        streamingAssetsUrl: "StreamingAssets",
        companyName: "Val Knight",
        productName: "Web3D Test",
        productVersion: "0.1.0",
    };

    function progressHandler(progress) {
        if (progress >= 0.85) {
            canvas.style.display = '';
        }
        var percent = progress * 100 + '%';
        canvas.style.background = 'linear-gradient(to right, white, white ' + percent + ', transparent ' + percent + ', transparent) no-repeat center';
        canvas.style.backgroundSize = '100% 1rem';
        loader.innerHTML = `${Math.round((progress/0.9) * 100)}% loaded`;
    }

    function onResize() {
        // Do Resize Handling Here!
    }

    createUnityInstance(canvas, config, progressHandler).then(function (instance) {
        canvas = instance.Module.canvas;
        onResize();
        // Export the Unity instance, such that it can be used externally
        window.unityInstance = instance;
    });
    window.addEventListener('resize', onResize);
    onResize();

    if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
        // Mobile device style: fill the whole browser client area with the game canvas:
        const meta = document.createElement('meta');
        meta.name = 'viewport';
        meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
        document.getElementsByTagName('head')[0].appendChild(meta);
    }
}

function markAsReadyToLoad() {
    enableUnityBindingButtons();
    document.getElementById("unity-load-percent").style.display = 'none';
    eval(document.getElementById("unity-load-event").innerHTML);
}

function markAsLoading() {
    disableUnityBindingButtons();
}

function markLoaded() {
    enableUnityBindingButtons();
}

function enableUnityBindingButtons() {
    var unityButtons = document.getElementsByClassName('unity-binding-button');
    for (var i = 0; i < unityButtons.length; i++) {
        unityButtons[i].removeAttribute('disabled');
    }
}

function disableUnityBindingButtons() {
    var unityButtons = document.getElementsByClassName('unity-binding-button');
    for (var i = 0; i < unityButtons.length; i++) {
        unityButtons[i].setAttribute('disabled', '');
    }

}

function sendMessageToInstantiatedObject(message) {
    window.unityInstance.SendMessage('InstantiatedObject', message);
}

function setLit() {
    sendMessageToInstantiatedObject('SetLit');
}

function setWireframe() {
    sendMessageToInstantiatedObject('SetWireframe');
}

function toggleAutoRotation() {
    sendMessageToInstantiatedObject('ToggleAutoRotate');
}

function toggleReflectionProbes() {
    window.unityInstance.SendMessage('ReflectionProbeToggler', 'Toggle');
}

function toggleStudio() {
    window.unityInstance.SendMessage('StudioToggler', 'Toggle');
}

function toggleLights() {
    window.unityInstance.SendMessage('LightsToggler', 'Toggle');
}

function loadModel(modelId) {
    window.unityInstance.SendMessage('LoadFromNetwork', 'LoadModelBundleWithId', modelId);
}

function resetScene() {
    window.unityInstance.SendMessage('LoadFromNetwork', 'ResetScene');
}

function setCameraMode(mode) {
    window.unityInstance.SendMessage('MainCam', 'ApplyMode', mode);
}

function fullscreen() {
    window.unityInstance.SetFullscreen(1);
}

// Export functions for Unity
window.markAsReadyToLoad = markAsReadyToLoad;
window.bootUnity = bootUnity;