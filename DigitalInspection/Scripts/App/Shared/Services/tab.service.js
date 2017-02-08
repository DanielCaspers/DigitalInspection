const TabService = function () { };

TabService.changeTab = function (event) {
	//Remove selected state from the child that has it
	$("#tabContainer a.btn").removeClass("btn-info");

	var sourceId = event.srcElement.id;

	//Add selected state to clicked tab
	$("#" + sourceId).addClass("btn-info");
};