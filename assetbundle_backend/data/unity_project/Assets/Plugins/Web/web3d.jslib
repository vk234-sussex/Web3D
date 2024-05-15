mergeInto(LibraryManager.library, {

  MarkAsReadyToLoad: function () {
    console.log("[UNITY CONTAINER] Got ready to load from WebAssembly!");
    window.markAsReadyToLoad();
  },

  MarkAsLoading: function () {
    console.log("[UNITY CONTAINER] WebAssembly loading in model");
    window.markAsLoading();
  },

  MarkLoaded: function () {
    console.log("[UNITY CONTAINER] WebAssembly loaded in model");
    window.markLoaded();
  }
})