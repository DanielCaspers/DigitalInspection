class ViewInspectionPhotosDialog {
	private static CAROUSEL_CONTENT_CONTAINER_SELECTOR = '#viewInspectionPhotosDialog .carousel-inner';
	private static CAROUSEL_CONTENT_SELECTOR = ViewInspectionPhotosDialog.CAROUSEL_CONTENT_CONTAINER_SELECTOR + ' .item.active .carousel-content';
	private static CAROUSEL_SELECTOR = '#inspectionPhotosCarousel';

	public static onImageVisibilityToggle(element): void {
		const formElement = $(element).closest('form');
		FormService.addUrlParameter(formElement, { name: 'isVisibleToCustomer', value: element.checked });
		formElement.submit();
	}

	public static deletePhoto(): void {
		const imageId = $(ViewInspectionPhotosDialog.CAROUSEL_CONTENT_SELECTOR).attr('data-image-id');
		FormService.triggerExternalSubmit('viewInspectionPhotosForm', null, false, { name: 'imageId', value: imageId });
	}

	public static onHide(e: any): void {
		// On hiding the dialog, remove all videos or images (so they are not consuming CPU, playing audio, reducing battery life)
		$(ViewInspectionPhotosDialog.CAROUSEL_CONTENT_CONTAINER_SELECTOR).empty();
	};

	public static initialize(e: any): void {
		// On carousel slide, pause the active video (since the video element will still be in the DOM)
		$(ViewInspectionPhotosDialog.CAROUSEL_SELECTOR).on('slide.bs.carousel', () => {
			$(ViewInspectionPhotosDialog.CAROUSEL_CONTENT_CONTAINER_SELECTOR + ' video').each(function() { // Needed for "this" scope to work correctly
				$(this).trigger('pause');
			});
		})
	}
}
