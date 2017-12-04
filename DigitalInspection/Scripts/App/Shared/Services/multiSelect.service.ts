class MultiSelectService {
	public static BASE_MULTISELECT_CONFIG: any = {
		buttonWidth: '100%',
		enableFiltering: true,
		enableCaseInsensitiveFiltering: true,
		onDropdownHide: () => {
			$('button.multiselect-clear-filter').click();
		}
	};

	public static show(selector: string, config: any): void {
		$(document).ready(() => {
			let selectedElements = $(selector);

			if (!config) {
				config = MultiSelectService.BASE_MULTISELECT_CONFIG;
			}

			selectedElements.multiselect(config);
		});
	}
}
