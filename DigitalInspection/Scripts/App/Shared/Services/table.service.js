if (typeof TableService === 'undefined') {
	var TableService = function () { };
}

TableService.BASE_TABLE_CONFIG = {
	dom: 't<"container-flex between"lip>',
	lengthMenu: [10, 20, 50, 100],
	columnDefs: [],
	language: {
		info: "_TOTAL_ results",
		infoFiltered: "(filtered from _MAX_)",
		infoEmpty: "No results",
		lengthMenu: "Page size _MENU_",
		zeroRecords: ""
	}
};

TableService.showTable = function (elementId, config, onSelect) {
	$(document).ready(function () {
		var tableSelector = '#' + elementId;

		if (!config) {
			config = TableService.BASE_TABLE_CONFIG;
		}

		var table = $(tableSelector).DataTable(config);

		if (typeof onSelect === 'function') {
			table.on('select', onSelect);
		}

		$(tableSelector + '_searchInput').on('keyup', function () {
			table.search(this.value).draw();
		});

		$(tableSelector + '_clearSearch').click(function () {
			$(tableSelector + '_searchInput').val('');
			table.search(this.value).draw();
		});
	});
}

TableService.toggleCheckboxesForColumn = function (index, checkAllCheckbox) {
	var jqSelector = "table tbody tr td:nth-child(" + index + ") input[type=checkbox]";

	// For strict toggling behavior without respect to the check all box
	//var isChecked = !$(jqSelector).prop("checked");

	// Toggles checkbox DIRECTLY related to parent state, and has better handling of indeterminate state
	$(jqSelector).prop("checked", checkAllCheckbox.checked);
}
