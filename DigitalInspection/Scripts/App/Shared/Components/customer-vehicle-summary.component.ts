class CustomerVehicleSummaryComponent {
	public static setContrast(): void {

		const rgb = $('.car-color-badge').css('background-color');
		const colors = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);

		if (colors === null || colors.length !== 4) {
			return;
		}

		const r = colors[1];
		const g = colors[2];
		const b = colors[3];

		// http://www.w3.org/TR/AERT#color-contrast
		const perceivedBrightness = Math.round(((parseInt(r) * 299) +
			(parseInt(g) * 587) +
			(parseInt(b) * 114)) / 1000);

		const foregroundColor = (perceivedBrightness > 125) ? 'black' : 'white';

		$('.car-color-badge').css('color', foregroundColor);
	}
}

