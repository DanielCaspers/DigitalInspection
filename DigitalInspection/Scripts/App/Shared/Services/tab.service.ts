class TabService {
	private static tabClasses: string = 'btn-info btn-raised';

	public static changeTab(event: Event): void {
		// Remove selected state from the child that has it
		$('#tabContainer a.btn').removeClass(TabService.tabClasses);

		let sourceId = event.srcElement.id;

		// Add selected state to clicked tab
		$(`#${sourceId}`).addClass(TabService.tabClasses);
	}

	public static selectTab(tabId: string): void {
		$(`#${tabId}`).addClass(TabService.tabClasses);
	}

}
