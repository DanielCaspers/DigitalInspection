class TableService {
	public static BASE_TABLE_CONFIG: any = {
		dom: 't<"container-flex between"lip>',
		lengthMenu: [10, 20, 50, 100],
		columnDefs: [],
		language: {
			info: '_TOTAL_ results',
			infoFiltered: '(filtered from _MAX_)',
			infoEmpty: 'No results',
			lengthMenu: 'Page size _MENU_',
			zeroRecords: ''
		}
	};

	public static showTable(
		elementId: string,
		config: any,
		onSelect?: (e: Event, dataTableInstance: any, type: string, indexes: any) => void,
		onUserSelect?: (e: Event, dataTableInstance: any, type: string, cell: any, originalEvent: Event) => void): void {
		$(document).ready(() => {
			const tableSelector = `#${elementId}`;
			const searchInputSelector = `${tableSelector}_searchInput`;
			const clearSearchSelector = `${tableSelector}_clearSearch`;
			if (!config) {
				config = TableService.BASE_TABLE_CONFIG;
			}

			let table = $(tableSelector).DataTable(config);

			// TODO: Remove these assertions after migrating all CSHTML scripts to TypeScript
			if (typeof onSelect === 'function') {
				table.on('select', onSelect);
			}

			if (typeof onUserSelect === 'function') {
				table.on('user-select', onUserSelect);
			}

			$(searchInputSelector).on('keyup', function(): void {
				table.search(this.value).draw();
			});

			$(clearSearchSelector).click( function(): void {
				$(searchInputSelector).val('');
				table.search(this.value).draw();
			});
		});
	}

	public static toggleCheckboxesForColumn(index: number, checkAllCheckbox: any): void {
		let jqSelector = `table tbody tr td:nth-child(${index}) input[type=checkbox]`;

		// For strict toggling behavior without respect to the check all box
		// var isChecked = !$(jqSelector).prop('checked');

		// Toggles checkbox DIRECTLY related to parent state, and has better handling of indeterminate state
		$(jqSelector).prop('checked', checkAllCheckbox.checked);
	}
}
