function markAsReadyToLoad() {
    console.warn("// TODO: received mark event in template. bind this properly to enable button!");
}

function markAsLoading() {
    console.warn("// TODO: received mark loading in template. bind this properly to disable button");
}

function markLoaded() {
    console.warn("// TODO: received mark loaded in template. bind this properly to enable button");
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

function loadModel(modelId) {
    window.unityInstance.SendMessage('LoadFromNetwork', 'LoadModelBundleWithId', modelId);
}
        
function resetScene() {
    window.unityInstance.SendMessage('LoadFromNetwork', 'ResetScene');
}
        
function loadCokeCan() {
    loadModel("model_coke");
}
