if (typeof HttpClientService === 'undefined') {
	var HttpClientService = function () { };
}

HttpClientService.get = function (url, data, cb) {
	$.get(url, data, cb);
};

//this.post = function (url, data, cb, appendBaseUrl) {
//	if (appendBaseUrl) {
//		url = API_BASE_URL + url;
//	}
//	$.post(url, data, cb);
//};
