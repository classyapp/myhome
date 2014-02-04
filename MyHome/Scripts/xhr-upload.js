function XHRFileUpload(path, file, extraData) {
    this._path = path;
    this._file = file;
    this._extra = extraData;
    this._xhr = null;
}

XHRFileUpload.prototype = (function () {

    var _onProgress = function (e) {
        if (e.lengthComputable) {
            var percentComplete = (e.loaded / e.total) * 100;
            console.log(percentComplete + '% uploaded');
        }
    };

    var _onError = function (evt) {
        alert('error');
    };

    var _onSuccess = function (evt) {
        alert('done');
    };

    var uploadFile = function (onProgress, onSuccess, onError) {
        onProgress = onProgress || _onProgress;
        onSuccess = onSuccess || _onSuccess;
        onError = onError || _onError;

        if (this._path != null && this._path != "" && this._file != null) {
            this._xhr = (window.XMLHttpRequest) ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
            this._xhr.upload.onerror = onError;
            this._xhr.upload.onprogress = onProgress;
            this._xhr.onreadystatechange = $.proxy(function () {
                if (this._xhr.readyState == 4 && this._xhr.status == 200) { onSuccess($.parseJSON(this._xhr.response)) }
            }, this);

            var data = new FormData();
            data.append("file", this._file);
            if (this._extra != null) {
                for (item in this._extra) {
                    data.append(item, this._extra[item]);
                }
            }

            this._xhr.open('POST', this._path, true);
            this._xhr.setRequestHeader("Accept", "application/json");
            this._xhr.send(data);
        }
    };

    return {
        constructor: XHRFileUpload,
        upload: uploadFile
    }
})();
