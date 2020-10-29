const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const path = require("path");

module.exports = {
	entry: {
		app: "./src/App.ts",
	},

	output: {
		filename: "[name].js",
		path: __dirname + "/dist"
	},

	resolve: {
		extensions: [".ts", ".tsx", ".js", ".json"],
		modules: [
			path.resolve(__dirname, "src"),
			"node_modules"
		]
	},

	module: {
		rules: [
			{ test: /\.(png|jpg|gif)$/, use: [{ loader: "file-loader", options: {} }], },
			{ test: /\.ts?$/, loader: "ts-loader" },
		]
	},

	plugins: [
		new CleanWebpackPlugin({ cleanStaleWebpackAssets: false }),
		new CopyPlugin({
			patterns: [
				{ from: "./src/favicon.ico", to: ".", flatten: true },
				{ from: "./src/**/*.html", to: ".", flatten: true },
			]
		}),
	],

	externals: {
		"jquery": "jQuery",
		"moment": "moment",
		"knockout": "ko",
		"jss": "jss",
	},
};