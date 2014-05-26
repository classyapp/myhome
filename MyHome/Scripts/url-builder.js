var Classy = Classy || {};

Classy.UrlBuilder = {
    ProfilePage: function(id, name) {
        return '/profile/' + id + '/' + encodeURIComponent(name.replace(' ', '-').replace('&','-'));
    }
};