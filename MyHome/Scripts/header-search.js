$(function () {

    var rooms = [];
    $.each(Classy.SiteMetadata.Rooms, function() {
        rooms.push({ Value: this });
    });
    var styles = [];
    $.each(Classy.SiteMetadata.Styles, function() {
        styles.push({ Value: this });
    });

    var roomsSuggestions = new Bloodhound({
        name: 'rooms-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        local: rooms
    });
    var stylesSuggestions = new Bloodhound({
        name: 'styles-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        local: styles
    });
    var profilesSuggestions = new Bloodhound({
        name: 'profiles-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        remote: '//' + window.location.host + '/search/profiles/suggest?q=%QUERY'
    });
    var keywordsSuggestions = new Bloodhound({
        name: 'keywords-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        remote: '//' + window.location.host + '/search/keywords/suggest?q=%QUERY'
    });

    roomsSuggestions.initialize();
    stylesSuggestions.initialize();
    profilesSuggestions.initialize();
    keywordsSuggestions.initialize();

    $('#q.typeahead').typeahead({
        minLength: 2,
        hightlight: true
    }, {
        name: 'rooms-suggestions',
        displayKey: 'Value',
        source: roomsSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchSuggestionsRoomsHeader + '</span>'
        }
    }, {
        name: 'styles-suggestions',
        displayKey: 'Value',
        source: stylesSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchSuggestionsStylesHeader + '</span>'
        }
    }, {
        name: 'profile-suggestions',
        displayKey: 'Value',
        source: profilesSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchSuggestionsProfilesHeader + '</span>'
        }
    },
    {
        name: 'keywords-suggestion',
        displayKey: 'Value',
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchSuggestionsOtherHeader + '</span>'
        },
        source: keywordsSuggestions.ttAdapter()
    },
    {
        name: 'free-search-suggestion',
        displayKey: 'Value',
        source: function (query, callback) {
            callback([{ Value: 'Search for \'<strong>' + query + '</strong>\'' }]);
        }
    });

    $(document).bind('typeahead:selected', function (event, suggestion, dataset) {
        if (dataset == 'profile-suggestions')
            window.location.href = '//' + window.location.host + Classy.UrlBuilder.ProfilePage(suggestion.Key, suggestion.Value);
        else if (dataset == 'rooms-suggestions' || dataset == 'styles-suggestions')
            window.location.href = '//' + window.location.host + '/photo/' + encodeURIComponent(suggestion.Value);
        else if (dataset == 'keywords-suggestion')
            window.location.href = '//' + window.location.host + '/search/' + encodeURIComponent(suggestion.Value);
        else
            window.location.href = '//' + window.location.host + '/search/' + encodeURIComponent($($('#q').val()).html());
    });

    $("#q").keyup(function (e) {
        var queryValue = $('#q').val().trim();
        if (e.keyCode == 13) {
            if (queryValue == '') return false;
            if ($('.navbar .tt-suggestion.tt-cursor').length == 0)
                window.location.href = '//' + window.location.host + '/search/' + encodeURIComponent(queryValue);
        }
    });
});