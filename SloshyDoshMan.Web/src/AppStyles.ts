import * as jss from "jss";

export const layout = jss.default.createStyleSheet({
	redHandleContainer: {
		"background-color": "rgba(32, 32, 32, .4)",
		"border-style": "solid",
		"border-width": "10px",
		"border-image": "url('/CommonImages/handle-corners-red.png') 10 10 10 10 repeat",
		"padding": "10px",
	},
	header: {
		"font-size": "48px",
		"line-height": "40px",
		"margin-left": "5px",
		"font-weight": "bold",
		"color": "#9f9f9f",
		"font-variant": "small-caps",
		"text-transform": "lowercase",
		"letter-spacing": "5px",
		"-webkit-text-stroke-width": "1px",
		"-webkit-text-stroke-color": "#151515",
	},
	flexRow: {
		"display": "flex",
		"flex-direction": "row",
		"flex-wrap": "nowrap",
	},
	flexEvenDistribution: {
		"flex-grow": 1,
		"flex-basis": 0,
	},
}).attach().classes;

export const textColor = jss.default.createStyleSheet({
	white: { color: "#E8E8E8" },
	gray: { color: "#555555" },
}).attach().classes;

export const margin = jss.default.createStyleSheet({
	all: { "margin": "10px", },
	half: { "margin": "5px", },

	vertical: { "margin-top": "10px", "margin-bottom": "10px", },
	verticalHalf: { "margin-top": "5px", "margin-bottom": "5px", },

	horizontal: { "margin-left": "10px", "margin-right": "10px", },
	horizontalHalf: { "margin-left": "5px", "margin-right": "5px", },

	right: { "margin-right": "10px", },
	rightHalf: { "margin-right": "5px", },
	rightDouble: { "margin-right": "20px", },

	left : { "margin-left": "10px", },
	leftHalf: { "margin-left": "5px", },
	leftDouble: { "margin-left": "20px", },

	top: { "margin-top": "10px", },
	topHalf: { "margin-top": "5px", },
	topDouble: { "margin-top": "20px", },

	bottom : { "margin-bottom": "10px", },
	bottomHalf: { "margin-bottom": "5px", },
	bottomDouble: { "margin-bottom": "20px", },
}).attach().classes;

export const padding = jss.default.createStyleSheet({
	all: { "padding": "10px", },
	half: { "padding": "5px", },

	vertical: { "padding-top": "10px", "padding-bottom": "10px", },
	verticalHalf: { "padding-top": "5px", "padding-bottom": "5px", },

	horizontal: { "padding-left": "10px", "padding-right": "10px", },
	horizontalHalf: { "padding-left": "5px", "padding-right": "5px", },

	right: { "padding-right": "10px", },
	rightHalf: { "padding-right": "5px", },
	rightDouble: { "padding-right": "20px", },

	left : { "padding-left": "10px", },
	leftHalf: { "padding-left": "5px", },
	leftDouble: { "padding-left": "20px", },

	top: { "padding-top": "10px", },
	topHalf: { "padding-top": "5px", },
	topDouble: { "padding-top": "20px", },

	bottom : { "padding-bottom": "10px", },
	bottomHalf: { "padding-bottom": "5px", },
	bottomDouble: { "padding-bottom": "20px", },
}).attach().classes;

export const text = jss.default.createStyleSheet({
	light: { "font-weight": 100 },
	bold: { "font-weight": "bold" },

	font10: { "font-size": "10px" },
	font12: { "font-size": "12px" },
	font14: { "font-size": "14px" },
	font16: { "font-size": "16px" },
	font20: { "font-size": "20px" },
	font22: { "font-size": "22px" },
	font24: { "font-size": "24px" },
	font26: { "font-size": "26px" },
	font28: { "font-size": "28px" },
	font30: { "font-size": "30px" },
	font32: { "font-size": "32px" },
	font34: { "font-size": "34px" },
	font36: { "font-size": "36px" },
	font48: { "font-size": "48px" },
	font56: { "font-size": "56px" },

	left: { "text-align": "left" },
	center: { "text-align": "center" },
	right: { "text-align": "right" },

	smallCaps: { "font-variant": "small-caps" },
}).attach().classes;
