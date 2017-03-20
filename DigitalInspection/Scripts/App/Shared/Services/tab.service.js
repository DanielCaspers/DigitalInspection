const TabService = function () { };
const tabClasses = "btn-info btn-raised";

TabService.changeTab = function (event) {
	//Remove selected state from the child that has it
	$("#tabContainer a.btn").removeClass(tabClasses);

	var sourceId = event.srcElement.id;

	//Add selected state to clicked tab
	$("#" + sourceId).addClass(tabClasses);
};

TabService.selectTab = function (tabId) {
	$("#" + tabId).addClass(tabClasses);
}