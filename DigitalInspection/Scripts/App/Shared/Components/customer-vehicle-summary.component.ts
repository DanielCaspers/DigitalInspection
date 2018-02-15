class CustomerVehicleSummaryComponent {
	public static setContrast(): void {

		var rgb = $('.car-color-badge').css('background-color');
		var colors = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);

		var r = colors[1];
		var g = colors[2];
		var b = colors[3];

		// http://www.w3.org/TR/AERT#color-contrast
		var perceivedBrightness = Math.round(((parseInt(r) * 299) +
			(parseInt(g) * 587) +
			(parseInt(b) * 114)) / 1000);

		var foregroundColor = (perceivedBrightness > 125) ? 'black' : 'white';

		$('.car-color-badge').css('color', foregroundColor);
	}
}

