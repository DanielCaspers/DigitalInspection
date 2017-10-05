type ControlVisibilityMode = 'capture' | 'acceptance';

class UploadInspectionPhotosDialog {

	private static readonly CAPTURE_PHOTO_TEXT = 'Take a photo';
	private static readonly ACCEPTANCE_PHOTO_TEXT = 'Retake or keep going';
	private static carouselInsertionIndex = 1;

	public static initialize(): void {
		Webcam.set({
			width: 480,
			height: 360,
			dest_width: 480,
			dest_height: 360,
			image_format: 'jpeg',
			jpeg_quality: 90,
			//force_flash: false
		});
		Webcam.attach('#camera-viewfinder');
	}

	public static freezePhoto(): void {
		Webcam.freeze();
		UploadInspectionPhotosDialog.setCameraViewfinderCaption(UploadInspectionPhotosDialog.ACCEPTANCE_PHOTO_TEXT);
		UploadInspectionPhotosDialog.setControlVisibility('acceptance');
	}

	public static retryPhoto(): void {
		Webcam.unfreeze();
		UploadInspectionPhotosDialog.setCameraViewfinderCaption(UploadInspectionPhotosDialog.CAPTURE_PHOTO_TEXT);
		UploadInspectionPhotosDialog.setControlVisibility('capture');
	}

	public static acceptPhoto(): void {
		// take snapshot and get image data
		Webcam.snap((data_uri: string) => {
			// Push into next carousel index

			// Dynamically add <li>
			$('.carousel-indicators')
				.append(`<li data-target='#myCarousel' data-slide-to='${UploadInspectionPhotosDialog.carouselInsertionIndex}'></li>`);

			// Enable left and right chevron on the first time
			if (UploadInspectionPhotosDialog.carouselInsertionIndex === 1) {
				$('.carousel-control').addClass('visible');
			}
			UploadInspectionPhotosDialog.carouselInsertionIndex++;

			UploadInspectionPhotosDialog.setCameraViewfinderCaption(UploadInspectionPhotosDialog.CAPTURE_PHOTO_TEXT);
			UploadInspectionPhotosDialog.setControlVisibility('capture');
			UploadInspectionPhotosDialog.savePhotoToDOM(data_uri);
		});
	}

	private static setCameraViewfinderCaption(message: string): void {
		$('#camera-viewfinder-caption').text(message);
	}

	private static setControlVisibility(mode: ControlVisibilityMode): void {
		if (mode === 'capture') {
			$('.photo-control.capture-control').addClass('visible');
			$('.photo-control.acceptance-control').removeClass('visible');
		}
		else if (mode === 'acceptance') {
			$('.photo-control.acceptance-control').addClass('visible');
			$('.photo-control.capture-control').removeClass('visible');
		}
	}

	private static savePhotoToDOM(data_uri: string): void {
		$('.carousel-inner').append(
			`<div class="item">
				<div class="snapshot-display">
					<img src="${data_uri}"/>
				</div>
				<div class="carousel-caption">
					<span>Pretty neat, huh?</span>
				</div>
			</div>`
		);
	}
}


