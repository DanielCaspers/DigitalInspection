class FormService {
	private static OUTER_PROGRESSBAR_SELECTOR: string = '.ma-progress-bar';
	private static INNER_PROGRESSBAR_SELECTOR: string = '.ma-progress-bar .progress-bar[role="progressbar"]';
	public static triggerExternalSubmit(
		formId: string,
		resourceId: string,
		showProgress: boolean,
		urlParam?: any): void {
		let formElement = $('#' + formId);

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
	}

	public static addUrlResourceId(formId: string, resourceId: string): void {
		let formElement = $('#' + formId);
		let routingAction = formElement.attr('action');
		formElement.attr('action', routingAction + '/' + resourceId);
	}

	public static submit(formId: string, showProgress: boolean): void {
		let formElement = $('#' + formId);
		formElement.submit();

		if (showProgress) {
			FormService.showProgress();
		}
	}

	public static showProgress(): void {
		$(document).ready(() => {
			$(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: 'visible' });

			const animateloop = () => {
				$(FormService.INNER_PROGRESSBAR_SELECTOR).css({ marginLeft: '-45%' });
				$(FormService.INNER_PROGRESSBAR_SELECTOR).animate(
					{
						marginLeft: '145%'
					},
					1200,
					function (): void { animateloop(); }
				);
			};
			animateloop();
		});
	}

	public static hideProgress(): void {
		$(FormService.OUTER_PROGRESSBAR_SELECTOR).css({ visibility: 'hidden' });
		$(FormService.INNER_PROGRESSBAR_SELECTOR).stop();
	}

	// TODO Move to the urlRewriterService
	private static addUrlParameter(
		formId: string,
		urlParam: { name: string, value: string }): void {
		let formElement = $(`#${formId}`);
		const routingAction = formElement.attr('action');

		let query = "";
		// If query parameter is missing, begin the query parameter list
		if (routingAction.indexOf('?') === -1) {
			query += '?';
		}
		// Else, tack on one add'l parameter to existing query string
		else {
			query += '&';
		}
		query += `${urlParam.name}=${urlParam.value}`;
		formElement.attr('action', routingAction + query);
	}
}
