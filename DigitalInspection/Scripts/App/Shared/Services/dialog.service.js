const DialogService = function () { };

DialogService.confirmDelete = function (formName) {

	var deferred = $.Deferred();

	$("#confirmDelete").modal();

	$("#confirmDelete_success").click(function () {
		$("#" + formName).submit();
		deferred.resolve(true);
	});
	$("#confirmDelete_cancel").click(function(){
		deferred.resolve(false);
	});

	return deferred.promise();
};