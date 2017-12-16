interface ScrollableTab {
	paneId: string;
	title: string;
	active: boolean;
	disabled: boolean;
};

class ScrollableTabService {

	private static readonly CONTAINER_SELECTOR = '#ma-scrollable-tab-container';

	private static readonly CONFIG = {
		tabs: [],
		enableSwiping: true,
		ignoreTabPanes: true,
		scrollToTabEdge: true,
		disableScrollArrowsOnFullyScrolled: true,
		tabClickHandler: function (e) {
			let query = LocationService.parseQuery();
			// The tag ID is after the relative target anchor (#) in the full URL
			query.tagId = this.href.split('#')[1];
			LocationService.search(query);
		},
		leftArrowContent: ScrollableTabService.buildScrollTabArrow('left'),
		rightArrowContent: ScrollableTabService.buildScrollTabArrow('right')
	};

	public static initialize(config): void {
		$(ScrollableTabService.CONTAINER_SELECTOR)
			.scrollingTabs(config);
	}

	public static getConfig(tabs) {
		return {
			...ScrollableTabService.CONFIG,
			tabs
		};
	}

	private static buildScrollTabArrow(direction: 'right' | 'left'): string {
		const template = `
			<div class="scrtabs-tab-scroll-arrow scrtabs-tab-scroll-arrow-${direction}">
				<a class="btn btn-primary no-margin no-padding-horizontal">
					<i class="material-icons">chevron_${direction}</i>
				</a>
			</div>
		`;

		return template;
	}
}
