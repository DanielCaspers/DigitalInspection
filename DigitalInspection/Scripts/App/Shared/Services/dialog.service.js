const DialogService = function () { };

DialogService.confirmDelete = function (resource, resourceId, appendBaseUrl, cb) {
	$(document).ready(function () {

		$("#confirmDelete").modal();

		const API_BASE_URL = 'http://localhost:54343/';

		$("#confirmDelete_success").on("click", function () {

			var url = '';

			if (appendBaseUrl) {
				url = API_BASE_URL + url;
			}

			url += resource + '/Delete/' + resourceId;

			console.log("delete was called with ", url);

			$.post(url, {}, cb);
		});
	});
};