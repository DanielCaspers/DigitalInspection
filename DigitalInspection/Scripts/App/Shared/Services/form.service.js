if (typeof FormService === 'undefined') {
	var FormService = function () { };
}

FormService.triggerExternalSubmit = function(formId, resourceId, showProgress, urlParam) {
	var formElement = $('#' + formId);

	if (resourceId) {
		FormService.addUrlResourceId(formId, resourceId);
	}

	if (urlParam) {
		FormService.addUrlParameter(formId, urlParam);
	}

	formElement.submit();

	if (showProgress) {
		FormService.showProgress();
	}
};

FormService.addUrlResourceId = function (formId, resourceId) {
	var formElement = $('#' + formId);
	var routingAction = formElement.attr('action');
	formElement.attr('action', routingAction + '/' + resourceId);
};

FormService.addUrlParameter = function (formId, urlParam) {
	var formElement = $('#' + formId);
	var routingAction = formElement.attr('action');
	var queryParameterString = "?" + urlParam.name + "=" + urlParam.value;
	formElement.attr('action', routingAction + queryParameterString);
};

FormService.submit = function (formId, showProgress) {
	var formElement = $('#' + formId);
	formElement.submit();

	if (showProgress) {
		FormService.showProgress();
	}
};

FormService.OUTER_PROGRESSBAR_SELECTOR = ".ma-progress-bar";
FormService.INNER_PROGRESSBAR_SELECTOR = ".ma-progress-bar .progress-bar[role='progressbar']";

FormService.showProgress = function () {
	$(document).ready(function () {
		$(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: "visible" });

		function animateloop() {
			$(FormService.INNER_PROGRESSBAR_SELECTOR).css({ marginLeft: "-45%" });
			$(FormService.INNER_PROGRESSBAR_SELECTOR).animate(
				{
					marginLeft: "145%"
				},
				1200,
				function () { animateloop(); }
			);
		}
		animateloop();
	});
};

FormService.hideProgress = function () {
	$(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: "hidden" });
	$(FormService.INNER_PROGRESSBAR_SELECTOR).stop();
};
