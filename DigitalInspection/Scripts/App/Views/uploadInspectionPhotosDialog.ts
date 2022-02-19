enum MediaType {
	Image = 'IMAGE',
	Video = 'VIDEO'
}

class UploadInspectionPhotosDialog {

	private static UPLOAD_CONTENT_CONTAINER_SELECTOR = '#uploadInspectionPhotosForm .upload-content-container';
	private static FILE_INPUT_SELECTOR = '#fileInput';
	private static FILE_SIZE_DISPLAY_SELECTOR = '#fileInputSizeDisplay';

	public static initialize(): void {
		// Auto-launch file picker
		$(UploadInspectionPhotosDialog.FILE_INPUT_SELECTOR)
			.click()
			.change(function () { // Cannot be ES6 arrow due to "this" scope
				UploadInspectionPhotosDialog.previewUpload(this);
				$('#uploadInspectionPhotosDialog_success').removeAttr('disabled');
			});
	}

	public static onHide(): void {
		const uploadContentContainer = $(UploadInspectionPhotosDialog.UPLOAD_CONTENT_CONTAINER_SELECTOR);
		if (uploadContentContainer.has('video')) {
			// Remove the video to mute, reduce processing resources, etc.
			uploadContentContainer.remove();
		}
	}

	private static previewUpload(input: any): void {
		if (!input.files || !input.files[0]) {
			console.error('No files provided to preview!');
		}

		const fileToUpload = input.files[0];
		const reader = new FileReader();

		if (fileToUpload.type.startsWith('image/')) {
			reader.onload = (e: any) => UploadInspectionPhotosDialog.createPreview(e.target.result as string, MediaType.Image);
		}
		else if (fileToUpload.type.startsWith('video/')) {
			reader.onload = (e: any) => UploadInspectionPhotosDialog.createPreview(e.target.result as string, MediaType.Video);
		}
		else {
			console.warn('Unsupported media type', fileToUpload.type);
		}

		reader.readAsDataURL(fileToUpload);

		UploadInspectionPhotosDialog.setFileUploadSizeIndicator(fileToUpload.size);
	}

	private static createPreview(source: string, type: MediaType): void {
		const uploadContentContainer = $(UploadInspectionPhotosDialog.UPLOAD_CONTENT_CONTAINER_SELECTOR);

		// Clear child elements if previously selected a file, but chose to upload a different file. 
		uploadContentContainer.empty();

		const previewElement = UploadInspectionPhotosDialog.createPreviewElement(source, type);

		uploadContentContainer.append(previewElement);
	}

	private static createPreviewElement(source: string, type: MediaType): JQuery {
		switch (type) {
			case MediaType.Image:
				return $('<img>')
					.attr('src', source)
					.addClass('img-responsive flex-media-viewer');

			case MediaType.Video: {
				const videoSource =
					$('<source>')
						.attr('src', source);

				return $('<video>')
					.attr({
						controls: '',
						loop: ''
					})
					.addClass('flex-media-viewer')
					.append(videoSource);
			}
			default:
				throw new Error(`Media type not yet implemented: ${type}`);
		}
	}

	private static setFileUploadSizeIndicator(fileSizeInBytes: number) {
		const bytesPerMB = 1048576
		const fileSizeInMB = fileSizeInBytes / bytesPerMB;

		$(UploadInspectionPhotosDialog.FILE_SIZE_DISPLAY_SELECTOR)
			.addClass('container-flex center text-info')
			.text(`Your file size is ${fileSizeInMB.toFixed(1)} MB`);
	}
}


