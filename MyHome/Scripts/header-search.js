$(function () {
    var listingsSuggestions = new Bloodhound({
        name: 'listings-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        remote: 'search/listings/suggest?q=%QUERY'
    });
    var profilesSuggestions = new Bloodhound({
        name: 'profiles-suggestions',
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        prefetch: '',
        remote: 'search/profiles/suggest?q=%QUERY'
    });

    listingsSuggestions.initialize();
    profilesSuggestions.initialize();

    $('#q.typeahead').typeahead({
        minLength: 2,
        hightlight: true
    }, {
        name: 'listing-suggestions',
        displayKey: 'Value',
        source: listingsSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">'+ searchListingsSuggestionsHeader +'</span>'
        }
    }, {
        name: 'profile-suggestions',
        displayKey: 'Value',
        source: profilesSuggestions.ttAdapter(),
        templates: {
            header: '<span class=\"tt-suggestion-header\">' + searchProfilesSuggestionsHeader+'</span>'
        }
    });

    $(document).bind('typeahead:selected', function (event, suggestion, dataset) {
        if (dataset == 'profile-suggestions')
            window.location.href = Classy.UrlBuilder.ProfilePage(suggestion.Key, suggestion.Value);
        else
            $('#navbar-search').submit();
    });
});