window.addEventListener("load", Initialize, true);

var homeLabButton = document.createElement('a');
homeLabButton.innerHTML = 'Add to HomeLab';
homeLabButton.className = 'addToHomeLabLink';
homeLabButton.addEventListener('click', AddToHomeLab, true);
homeLabButton.href = 'javascript:void(0)';

document.body.appendChild(homeLabButton);

function Initialize() {
	var imgElements = document.getElementsByTagName('img');
	for (i = 0; i < imgElements.length; i++) {
		var img = imgElements[i];
		if (IsValidImageElement(img)) {
			img.addEventListener('mouseover', ShowHomeLabButton, true);
			img.addEventListener('mouseout', HideHomeLabButton, true);
		}
	}
}

function ShowHomeLabButton(e)
{
	var img = e.srcElement;
	homeLabButton.style.display = 'block';
	homeLabButton.style.top = (RealOffset(img).top + 15) + 'px';
	homeLabButton.style.left = (RealOffset(img).left + 15) + 'px';
	homeLabButton.setAttribute('data-url', img.src);
}

function HideHomeLabButton(e)
{
	var el = e.toElement || e.relatedTarget;
	if (el == homeLabButton) return;

	homeLabButton.style.display = 'none';
}

function IsValidImageElement(img)
{
	var isValid = img.src != null && 
		img.src != "" && 
		img.src.indexOf('base64') == -1  && 
		img.width > 300;
	return isValid;
}

function AddToHomeLab(e)
{
	var url = e.srcElement.getAttribute('data-url');
	var win = window.open('http://www.myhome.co.il/web/photo/from?url=' + encodeURIComponent(url), 'HomeLabWindow', 'width=550,height=600,scrollbars=no,location=no,resizable=no,status=no,toolbar=no');
}

function RealOffset(element) {
    var top = 0, left = 0;
    do {
        top += element.offsetTop  || 0;
        left += element.offsetLeft || 0;
        element = element.offsetParent;
    } while(element);

    return {
        top: top,
        left: left
    };
};