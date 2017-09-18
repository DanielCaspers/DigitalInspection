if (typeof UrlRewriterService === 'undefined') {
	var UrlRewriterService = function () { };
}

UrlRewriterService.addUrlResourceId = function (formId, resourceId) {
	var formElement = $('#' + formId);
	var routingAction = formElement.attr('href');
	if (formElement.attr('data-url-modified')) {
		var tokenizedURL = routingAction.split('/');
		tokenizedURL.pop(); // Get rid of previous resource
		var baseResource = tokenizedURL.join('/'); // Rebuild without the old ID
		formElement.attr('href', baseResource + '/' + resourceId);
	}
	else {
		formElement.attr('href', routingAction + '/' + resourceId);
		formElement.attr('data-url-modified', true);
	}
};

//UrlRewriterService.addUrlParameter = function (formId, urlParam) {
//	var formElement = $('#' + formId);
//	var routingAction = formElement.attr('action');
//	var queryParameterString = "?" + urlParam.name + "=" + urlParam.value;
//	formElement.attr('action', routingAction + queryParameterString);
//};

