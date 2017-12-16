class LocationService {
	public static parseQuery(): any {
		// Search query w/o leading '?'
		let query = window.location.search.substring(1);

		let params = {};

		let queries = query.split("&");
		let i;
		for (i = 0; i < queries.length; i++) {
			let temp = queries[i].split('=');
			params[temp[0]] = temp[1];
		}

		return params;
	}

	public static search(queryObj: any): void {
		let queryFragments: string[] = [];
		for (var prop in queryObj) {
			if (queryObj.hasOwnProperty(prop)) {
				queryFragments.push(encodeURIComponent(prop) + "=" + encodeURIComponent(queryObj[prop]));
			}
		}

		let query = queryFragments.join('&');

		window.location.search = query;
	}
}
