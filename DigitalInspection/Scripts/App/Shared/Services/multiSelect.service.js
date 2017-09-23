if (typeof MultiSelectService === 'undefined') {
	var MultiSelectService = function () { };
}

MultiSelectService.BASE_MULTISELECT_CONFIG = {
	buttonWidth: '100%',
	enableFiltering: true,
	enableCaseInsensitiveFiltering: true,
	onDropdownHide: function () {
		$('button.multiselect-clear-filter').click();
	}
};

MultiSelectService.show = function (elementId, config) {
	$(document).ready(function () {
		var element = $('#' + elementId);

		if (!config) {
			config = MultiSelectService.BASE_MULTISELECT_CONFIG;
		}

		element.multiselect(config);
	});
};

MultiSelectService.autoSelectOptions = function (elementId, optionIds) {
	$(document).ready(function () {
		var element = $('#' + elementId);

		element.multiselect('select', optionIds);
	});
}
