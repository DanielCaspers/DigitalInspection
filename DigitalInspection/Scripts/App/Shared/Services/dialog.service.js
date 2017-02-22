const DialogService = function () { };

DialogService.confirmDelete = function (formName) {

	var deferred = $.Deferred();

	$("#confirmDelete").modal();

	$("#confirmDelete_success").click(function () {
		$("#" + formName).submit();
		deferred.resolve(true);
	});
	$("#confirmDelete_cancel").click(function(){
		deferred.resolve(false);
	});

	return deferred.promise();
};

DialogService.show = function (dialogId, formName) {

	var dialogElement = $("#" + dialogId);
	var formElement = $("#" + formName);
	dialogElement.modal();
	var validator = formElement.validate();

	$("#" + dialogId + "_success").click(function (e) {
		// Prevent stacking instances of submission if one after another occur without navigating away.
		// EG, submitting "1" returns 1, submitting "2" returns two "2"s
		e.preventDefault();
		e.stopImmediatePropagation();

		if (formElement.valid()) {
			formElement.submit();

			dialogElement.modal('hide');

			// Reset values in the form for next open
			formElement[0].reset();
			validator.resetForm();
		}
	});

	$("#" + dialogId + "_cancel").click(function (e) {
		// Reset values in the form for next open
		formElement[0].reset();
		validator.resetForm();
	});

}