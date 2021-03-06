﻿OnFacebookLogin = function (profile) { location.reload(false); }

function HookFbLogin() {
    $('.btn-fb-login').click(function () {
        FB.login(function (response) {
            if (response.authResponse) {
                var access_token = FB.getAuthResponse()['accessToken'];
                $.ajax('/login/fb', {
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({ 'token': access_token }),
                    success: function (data) {
                        if (data.IsValid) OnFacebookLogin(data.Profile);
                    }
                });
            } else {
                console.log('User cancelled login or did not fully authorize.');
            }
        }, { scope: 'basic_info,email,user_friends,user_photos,user_website,publish_actions' });
    });
};

$(document).on("classy.ajax.reconnect", function (e) {
    HookFbLogin();
});

HookFbLogin();