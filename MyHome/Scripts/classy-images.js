Classy.Images = {

    Thumbnail: function(imageId, width, height) {
        return '//' + Classy.SiteMetadata.CdnUrl + '/thumbnail/' + imageId + '?Width=' + width + '&Height=' + height + '&format=json';
    }

};