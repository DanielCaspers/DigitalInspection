class UrlRewriterService {
	public static addUrlResourceId(formId: string, resourceId: string): void {
		let formElement = $('#' + formId);
		const routingAction = formElement.attr('href');

		if (formElement.attr('data-url-modified')) {
			let tokenizedURL: string[] = routingAction.split('/');
			tokenizedURL.pop(); // Get rid of previous resource
			const baseResource = tokenizedURL.join('/'); // Rebuild without the old ID
			formElement.attr('href', baseResource + '/' + resourceId);
		}
		else {
			formElement.attr('href', routingAction + '/' + resourceId);
			formElement.attr('data-url-modified', 'true');
		}
	}
}
