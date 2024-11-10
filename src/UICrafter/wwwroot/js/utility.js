window.blazorUtility = window.blazorUtility || {};

window.blazorUtility.setThemeCookie = function (base64Value) {
	const cookieName = "ThemeInfo";
	const expirationDays = 365; // 1 year
	const date = new Date();
	date.setTime(date.getTime() + (expirationDays * 24 * 60 * 60 * 1000));
	const expires = "expires=" + date.toUTCString();
	document.cookie = `${cookieName}=${base64Value};${expires};path=/;Secure;SameSite=Lax`;
}

window.blazorUtility.isSystemDarkMode = function () {
	return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
};
