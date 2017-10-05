﻿class InspectionTable {
	private static TABLE_CONFIG = TableService.BASE_TABLE_CONFIG;

	public static initialize(): void {
		InspectionTable.initializeTable();
		InspectionTable.initializeSelect();
		InspectionTable.initializeConditionGroupControls();
	}

	private static onSelect(e: Event, dataTableInstance: any, type: string, indexes: any): void {
		if (type === 'row') {
			var row = dataTableInstance.row(indexes).node();
			console.log('Selected ' + row);
			// Removed because not using buttons in toolbar - See #62
			//$('#inspectionToolbar').addClass('row-selected')
			//UrlRewriterService.addUrlResourceId('AddMeasurementDialogButton', row.attributes['data-checklistitem-id'].value);
		}
	}

	private static onPreSelect(e: Event, dataTableInstance: any, type: string, cell: any, originalEvent: Event): void {
		if ($(cell.node()).parent().hasClass('selected')) {
			e.preventDefault();
		}
	}

	private static initializeTable(): void {
		InspectionTable.TABLE_CONFIG.select = true,
		InspectionTable.TABLE_CONFIG.paging = false,
		InspectionTable.TABLE_CONFIG.ordering = false;
		InspectionTable.TABLE_CONFIG.info = false;
		InspectionTable.TABLE_CONFIG.lengthMenu = [20];
		InspectionTable.TABLE_CONFIG.columnDefs = [];

		TableService.showTable(
			'inspectionTable',
			InspectionTable.TABLE_CONFIG,
			InspectionTable.onSelect,
			InspectionTable.onPreSelect
		);
	}

	private static initializeSelect(): void {
		$('.selectpicker').selectpicker({
			width: '100px'
		});
	}

	private static initializeConditionGroupControls(): void {
		// TODO - WARNING - This might not work with pagination
		$(function () {
			$('.condition-group button.group-left, .condition-group button.group-right').click(function (e) {
				let sideButton = $(this);
				// Add selected style to side button
				sideButton.addClass('active').siblings().removeClass('active');

				let imposterSelect = sideButton.closest('.condition-group').find('.recommended-service-picker');
				// Remove selected style from button wrapping select
				imposterSelect.removeClass('active');

				// Reset value of select
				imposterSelect.siblings('.selectpicker').selectpicker('val', 0);
			});

			$('.selectpicker').on('changed.bs.select', function (e) {
				let imposterSelect = $(this);
				// Add selected style to button wrapping select
				imposterSelect.siblings('.recommended-service-picker').addClass('active');

				// Remove selected style from side buttons
				imposterSelect.closest('.condition-group').children('button.group-left, button.group-right').removeClass('active');
			});
		});
	}
}
