const FormService = function () { };

FormService.triggerExternalSubmit = function(formId, resourceId, showProgress) {
	var formElement = $('#' + formId);
	var routingAction = formElement.attr('action');
	formElement.attr('action', routingAction + '/' + resourceId);
	formElement.submit();

	if (showProgress) {
		FormService.showProgress();
	}
};

FormService.submit = function (formId, showProgress) {
	var formElement = $('#' + formId);
	formElement.submit();

	if (showProgress) {
		FormService.showProgress();
	}
};

FormService.showProgress = function () {
	$(document).ready(function () {
		const OUTER_SELECTOR = ".ma-progress-bar";
		$(OUTER_SELECTOR).css({ visibility: "visible" });

		const INNER_SELECTOR = ".progress-bar[role='progressbar']";
		const PROGRESS_BAR_SELECTOR = OUTER_SELECTOR + " " + INNER_SELECTOR;
		function animateloop() {
			$(PROGRESS_BAR_SELECTOR).css({ marginLeft: "-45%" });
			$(PROGRESS_BAR_SELECTOR).animate(
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
