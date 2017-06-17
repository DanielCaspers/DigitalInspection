const TableService = function () { };

TableService.showTable = function (elementId, pageSizeOptions, columnRules) {
	$(document).ready(function () {
		var tableSelector = '#' + elementId;
		
		if (!pageSizeOptions) {
			pageSizeOptions = [10, 20, 50, 100];
		}

		if (!columnRules) {
			columnRules = [];
		}

		var table = $(tableSelector).DataTable({
			dom: 't<"container-flex between"lip>',
			lengthMenu: pageSizeOptions,
			columnDefs: columnRules,
			language: {
				info: "_TOTAL_ results",
				infoFiltered: "(filtered from _MAX_)",
				infoEmpty: "No results",
				lengthMenu: "Page size _MENU_",
				zeroRecords: ""
			}
		});

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

TableService.navigateOnRowClick = function () {
	$(document).ready(function () {
		// TODO: Rebind this event on table pagination, or page size change
		// Integrate select plugin for just navigating on the row click event instead 
		// https://datatables.net/extensions/select/examples/api/events.html
		$("table tbody tr").click(function (event) {
			window.location.href = $(this).attr('data-url');
		});
	});
}
