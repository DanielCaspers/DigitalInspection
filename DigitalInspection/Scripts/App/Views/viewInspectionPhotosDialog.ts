class ViewInspectionPhotosDialog {

	public static onImageVisibilityToggle(element): void {
		const formElement = $(element).closest('form');
		FormService.addUrlParameter(formElement, { name: 'isVisibleToCustomer', value: element.checked });
		formElement.submit();
	}

	public static deletePhoto(): void {
		const imageId = $('.carousel-inner .item.active img').attr('data-image-id');
		FormService.triggerExternalSubmit('viewInspectionPhotosForm', null, false, { name: 'imageId', value: imageId });
	}
}
