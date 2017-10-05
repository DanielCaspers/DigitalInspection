class DialogService {

	public static confirmLeavingUnsavedChanges_onProceed: () => void;

	public static confirmDelete(formName: string): void {

		$('#confirmDelete').modal();

		$('#confirmDelete_success').click(() => {
			$('#' + formName).submit();
		});

		// $('#confirmDelete_cancel').click(() => {
		// });
	}

	public static show(dialogId: string, formName: string): void {
		let dialogElement: JQuery = $('#' + dialogId);
		let formElement: JQuery = $('#' + formName);
		dialogElement.modal();
		let validator: any = formElement.validate();

		$(`#${dialogId}_success`).click((e: Event) => {
			// Prevent stacking instances of submission if one after another occur without navigating away.
			// EG, submitting '1' returns 1, submitting '2' returns two '2's
			e.preventDefault();
			e.stopImmediatePropagation();

			if (formElement.valid()) {
				formElement.submit();

				dialogElement.modal('hide');
			}
		});

		dialogElement.on('hidden.bs.modal', () => {
			// Reset values in the form for next open
			if (formElement[0]) {
				(formElement[0] as any).reset();
				validator.resetForm();
			}

			let selectInputs: any = $('#' + dialogId).find('select');
			for (let selectInput of selectInputs) {
				$(selectInput).multiselect('deselectAll', false);
				$(selectInput).multiselect('updateButtonText');
			}
		});
	}

	public static confirmLeavingUnsavedChanges(): void {
		$(document).ready(() => {

			// This event binding must happen prior to initializing dirtyForms plugin for disabling save buttons
			$('form').on('dirty.dirtyforms clean.dirtyforms scan.dirtyforms', (ev: Event) => {
				let $saveButton: JQuery = $('#toolbarContainer')
					.find('i.material-icons')
					.filter(function ( /* index */): boolean {
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

			$(document).bind('defer.dirtyforms', () => {
				// Need to wrap in a timeout to allow click event of navigating away in app to process
				setTimeout(FormService.hideProgress, 10);
			});

			$(document).bind('proceed.dirtyforms', () => {
				if (typeof DialogService.confirmLeavingUnsavedChanges_onProceed === 'function') {
					DialogService.confirmLeavingUnsavedChanges_onProceed();
				} else {
					FormService.showProgress();
				}
			});
		});
	}
}

// Auto execute to provide this behavior to all forms on load
DialogService.confirmLeavingUnsavedChanges();
