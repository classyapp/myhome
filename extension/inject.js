var squareWidth = 200;
var thumbWidth = 200;
var thumbHeight = 200;
var popupWidth = 900;
var popupHeight = 480;

var frameId = 'homelab-frame';
var bgId = 'homelab-bg';
var imageBoxId = 'homelab-imagebox';

function domCreate(tag, className, id) {
    var elem = document.createElement(tag);
    if(id) { elem.id = id; }
    if(className) { elem.className = className; }
    return elem;
}

function domRemove(id) {
	var elem = document.getElementById(id);
	if (elem && elem.parentNode) {
		elem.parentNode.removeChild(elem);
	}
}

function domSetEvent(elem, name, callback) {
    var wrapper = function(event) {
            event = event || window.event;
            callback.call(elem, event);
    }
    if(elem.addEventListener) {
            elem.addEventListener(name, wrapper, false);
    } else if(elem.attachEvent) {
            elem.attachEvent('on' + name, wrapper);
    }
}

function Clean() {
	domRemove(bgId);
	domRemove(frameId);
	domRemove(imageBoxId);
	window.scroll(0, 0);
}

// if frame is there, remove it
Clean();

// add the main frame
var iframe = domCreate('iframe', 'full-bg', frameId);
iframe.width = '100%';
iframe.height = '100%';
iframe.allowTransparency = 'true';
iframe.style.height = Math.max(document.body.scrollHeight, document.body.offsetHeight) + 'px';
document.body.appendChild(iframe);

var bg = domCreate('div', 'full-bg', bgId);
bg.style.height = Math.max(document.body.scrollHeight, document.body.offsetHeight) + 'px';;
document.body.appendChild(bg);

var imageBox = domCreate('div', 'full-bg', imageBoxId);
imageBox.innerHTML = '<div id="header"><div id="dismiss"></div></div>';
document.body.appendChild(imageBox);

var imageContainer = domCreate('div', null, 'imageContainer');
imageBox.appendChild(imageContainer);
domSetEvent(document, 'keydown', function(event) {
        if(event.keyCode == 27) { Clean(); }
});

function addImage(imageSrc) {
	var imageSquare = domCreate('div', 'imageWrapper');
	var img = document.createElement('img');
	img.style.opacity = '0';
	img.onload = function() {
        var width = this.width;
        var height = this.height;
        if(width < height) {
                var ratio = thumbWidth / height;
                var longEdge = height;
        } else {
                var ratio = thumbHeight / width;
                var longEdge = width;
        }
        if(width < 200 && height < 200) {
                var ratio = 1;
        }

        var size = 'small';
        if(longEdge > 1000) {
                size = 'x-large';
        } else if(longEdge > 600) {
                size = 'large';
        } else if (longEdge > 100) {
                size = 'medium';
        }

        var newWidth = Math.floor(width * ratio);
        var newHeight = Math.floor(height * ratio);
        this.width = newWidth;
        this.height = newHeight;
        this.style.left = Math.floor((squareWidth - newWidth)/2) + 'px';
        this.style.top = Math.floor((squareWidth - newHeight)/2) + 'px';
        this.style.opacity = '1';
    }

    var thumbWrapper = domCreate('div', 'thumb');
    thumbWrapper.appendChild(img);

    imageSquare.appendChild(thumbWrapper);
    imageContainer.appendChild(imageSquare);

    domSetEvent(imageSquare, 'click', function() {
            var popout = 'https://www.homelab.com/photo/new/fromurl?';
            popout += 'externalMediaUrl=' + encodeURIComponent(imageSrc);
            popout += '&title=' + encodeURIComponent(document.title);
            popout += '&originUrl=' + encodeURIComponent(document.location.href);
            var left = Math.floor((screen.width - popupWidth)/2);
            var top = Math.floor((screen.height - popupHeight)/2);
            window.open(popout, 'HOMELAB' + new Date().getTime(), 'status=no,resizable=yes,scrollbars=yes,personalbar=no,directories=no,location=no,toolbar=no,menubar=no,width=' + popupWidth + ',height=' + popupHeight + ',left=' + left + ',top=' + top);
            Clean();
    });

	img.src = imageSrc;
    if(img.width > 0 || img.height > 0) {
            img.onload();
            img.onload = null;
    }
};

images = [];
for (i = 0; i < document.images.length; i++) {
	var img = document.images[i];

	if (IsValidImageElement(img)) {
        images.push(img);
	}
}
if (images.length > 0) images.sort(function(a,b) { return (b.width * b.height) - (a.width * a.height); });
for (var i=0; i < images.length; i++) { addImage(images[i].src); }
