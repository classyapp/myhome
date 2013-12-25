OnFacebookLogin = function (profile) { location.reload(false); }

$('#sign-in-facebook').click(function () {
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
    });
});