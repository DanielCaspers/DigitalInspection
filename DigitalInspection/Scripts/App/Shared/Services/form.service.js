const FormService = function () { };

FormService.triggerExternalSubmit = function(formId, resourceId) {
	var formElement = $('#' + formId);
	var routingAction = formElement.attr('action');
	formElement.attr('action', routingAction + '/' + resourceId);
	formElement.submit();
};
