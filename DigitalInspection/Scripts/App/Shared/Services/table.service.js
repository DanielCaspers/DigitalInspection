const TableService = function () { };

TableService.toggleCheckboxesForColumn = function(index) {
	var jqSelector = "table tbody tr td:nth-child(" + index + ") input[type=checkbox]";

	//Handles toggle behavior
	var isChecked = !$(jqSelector).prop("checked");

	$(jqSelector).prop("checked", isChecked);
}