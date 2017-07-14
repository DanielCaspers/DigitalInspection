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
		}
	});

	dialogElement.on('hidden.bs.modal', function () {
		// Reset values in the form for next open
		formElement[0].reset();
		validator.resetForm();

		var selectInputs = $('#' + dialogId).find('select');
		for(var selectInput of selectInputs) {
			$(selectInput).multiselect('deselectAll', false);
			$(selectInput).multiselect('updateButtonText');
		}
	});

}

DialogService.confirmLeavingUnsavedChanges = function () {
	$(document).ready(function () {

		// This event binding must happen prior to initializing dirtyForms plugin for disabling save buttons
		$('form').on('dirty.dirtyforms clean.dirtyforms scan.dirtyforms', function (ev) {
			var $saveButton = $('#toolbarContainer')
				.find('i.material-icons')
				.filter(function (index) {
					return this.innerHTML === 'save';
				})
				.parent();

			if (ev.type === 'dirty') {
				// Enable save button
				$saveButton.removeAttr('disabled');
			} else {
				// Disable save button on initialization and on subsequent cleans
				$saveButton.attr('disabled', 'disabled');
			}
		});

		$('form').dirtyForms({
			dialog: {
				title: 'Discard changes and leave page?',
				proceedButtonText: 'Discard',
				stayButtonText: 'Cancel'
			},
			message: ''
		});

		$(document).bind('defer.dirtyforms', function () {
			// Need to wrap in a timeout to allow click event of navigating away in app to process
			setTimeout(FormService.hideProgress, 10);
		});

		$(document).bind('proceed.dirtyforms', function () {
			FormService.showProgress();
		});
	});
};

// Auto execute to provide this behavior to all forms on load
DialogService.confirmLeavingUnsavedChanges();
