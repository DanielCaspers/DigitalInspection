if (typeof MultiSelectService === 'undefined') {
	var MultiSelectService = function () { };
}

MultiSelectService.show = function (elementId, config) {
	
	$(document).ready(function () {
		var element = $('#' + elementId);

		if (!config) {
			config = {
				buttonWidth: '100%',
				enableFiltering: true,
				enableCaseInsensitiveFiltering: true,
				onDropdownHide: function () {
					$('button.multiselect-clear-filter').click();
				}
			};
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
