if (typeof TabService === 'undefined') {
	var TabService = function () { };
}

TabService.tabClasses = "btn-info btn-raised";

TabService.changeTab = function (event) {
	//Remove selected state from the child that has it
	$("#tabContainer a.btn").removeClass(TabService.tabClasses);

	var sourceId = event.srcElement.id;

	//Add selected state to clicked tab
	$("#" + sourceId).addClass(TabService.tabClasses);
};

TabService.selectTab = function (tabId) {
	$("#" + tabId).addClass(TabService.tabClasses);
};
