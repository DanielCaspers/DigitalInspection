const TableService = function () { };

TableService.toggleCheckboxesForColumn = function(index, checkAllCheckbox) {
	var jqSelector = "table tbody tr td:nth-child(" + index + ") input[type=checkbox]";

	// For strict toggling behavior without respect to the check all box
	//var isChecked = !$(jqSelector).prop("checked");

	// Toggles checkbox DIRECTLY related to parent state, and has better handling of indeterminate state
	$(jqSelector).prop("checked", checkAllCheckbox.checked);
}