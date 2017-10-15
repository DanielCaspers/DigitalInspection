class UploadInspectionPhotosDialog {

	public static initialize(): void {
		UploadInspectionPhotosDialog.launchFilePicker();

		$("#fileInput").change(function () {
			UploadInspectionPhotosDialog.previewPhotoUpload(this);
			UploadInspectionPhotosDialog.updateActionButtonIcon();
		});
	}

	private static previewPhotoUpload(input): void {
		if (input.files && input.files[0]) {
			var reader = new FileReader();

			reader.onload = function (e: any) {
				$('#camera-viewfinder').attr('src', e.target.result);
			}

			reader.readAsDataURL(input.files[0]);
		}
	}

	private static updateActionButtonIcon(): void {
		document.getElementById('camera-button').innerText = "redo";
	}

	private static launchFilePicker(): void {
		$('#fileInput').click();
	}
}


