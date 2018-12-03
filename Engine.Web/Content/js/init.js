function timestamp() {
	var e = new Date;
	return e.getFullYear() + "" + e.getMonth() + 1 + e.getDate() + e.getHours() + e.getMinutes()
}
seajs.config({
	base: "/Content/js/",
	alias: {
		jquery: "/Scripts/jquery/jquery-1.7.1.min.js"
		// jquery: "/Scripts/jquery/jquery-1.7.1.min.js",
		// lib51: "lib51.js",
		// global: "global.js"
	},
	preload: ["jquery"],
	map: [
		[/^(.*\.(?:css|js))(.*)$/i, "$1?v=" + timestamp()]
	]
});

seajs.use("main");