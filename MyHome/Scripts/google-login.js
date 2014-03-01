OnGoogleLogin = function (profile) { location.reload(false); }

function GoogleLogin(token) {
    $.ajax('/login/google', {
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify({ 'token': token }),
        success: function (data) {
            if (data.IsValid) OnGoogleLogin(data.Profile);
        }
    });
}