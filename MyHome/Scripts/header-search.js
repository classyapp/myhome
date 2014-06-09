$(function () {

    var rooms = [];
    $.each(Classy.SiteMetadata.Rooms, function() {
        rooms.push({ Value: this.Value, Key: this.Key });
    });
    var styles = [];
    $.each(Classy.SiteMetadata.Styles, function() {
        styles.push({ Value: this.Value, Key: this.Key });
    });

    var roomsSuggestions = new Bloodhound({
        name: 'rooms-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Key'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        local: rooms
    });
    var stylesSuggestions = new Bloodhound({
        name: 'styles-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Key'),
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
        hightlight: true,
        hint: false
    }, {
        name: 'rooms-suggestions',
        displayKey: 'Key',
        source: roomsSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchSuggestionsRoomsHeader + '</span>'
        }
    }, {
        name: 'styles-suggestions',
        displayKey: 'Key',
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
        source: function(query, callback) {
            callback([{ Value: query }]);
        },
        templates: {
            suggestion: function(query) {
                return '<p>Search for \'<strong>' + query.Value + '</strong>\'</p>';
            }
        }
    });

    var typeaheadSelectedFlag = false;

    $('#navbar-search').bind('typeahead:selected', function (event, suggestion, dataset) {
        typeaheadSelectedFlag = true;
        if (dataset == 'profile-suggestions')
            window.location.href = '//' + window.location.host + Classy.UrlBuilder.ProfilePage(suggestion.Key, suggestion.Value.toSlug());
        else if (dataset == 'rooms-suggestions' || dataset == 'styles-suggestions')
            window.location.href = '//' + window.location.host + '/photo/' + suggestion.Value.toSlug();
        else if (dataset == 'keywords-suggestion')
            window.location.href = '//' + window.location.host + '/search/' + suggestion.Value.toSlug();
        else
            window.location.href = '//' + window.location.host + '/search/' + suggestion.Value.toSlug();
    });

    $("#q").keyup(function (e) {
        var queryValue = $('#q').val().trim();
        if (e.keyCode == 13) {
            if (typeaheadSelectedFlag) {
                typeaheadSelectedFlag = false;
                return false;
            }
            if (queryValue == '') return false;
            if ($('#navbar-search .tt-suggestion.tt-cursor').length == 0)
                window.location.href = '//' + window.location.host + '/search/' + queryValue.toSlug();
            return false;
        }
    });
    $('#search-submit').click(function () {
        var queryValue = $('#q').val().trim();
        window.location.href = '//' + window.location.host + '/search/' + queryValue.toSlug();
    });
});