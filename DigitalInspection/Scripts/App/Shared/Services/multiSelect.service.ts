﻿class MultiSelectService {
	public static BASE_MULTISELECT_CONFIG: any = {
		buttonWidth: '100%',
		enableFiltering: true,
		enableCaseInsensitiveFiltering: true,
		onDropdownHide: () => {
			$('button.multiselect-clear-filter').click();
		}
	};

	public static show(elementId: string, config: any): void {
		$(document).ready(() => {
			let element = $('#' + elementId);

			if (!config) {
				config = MultiSelectService.BASE_MULTISELECT_CONFIG;
			}

			element.multiselect(config);
		});
	}

	public static autoSelectOptions(elementId: string, optionIds: string[]): void {
		$(document).ready(() => {
			let element = $('#' + elementId);

			element.multiselect('select', optionIds);
		});
	}
}