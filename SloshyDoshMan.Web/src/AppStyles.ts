import * as jss from "jss";
import * as CSS from 'csstype';

export interface CSSProperties extends CSS.Properties<number | string> {
	// Allow pseudo selectors and media queries
	[k: string]: CSS.Properties<number | string>[keyof CSS.Properties] | CSSProperties;
}

export type StyleRules<ClassKey extends string = string> = Record<ClassKey, CSSProperties>;

export function createStyles<Name extends string>(styles: StyleRules<Name>): jss.StyleSheet<Name> {
	return jss.default.createStyleSheet<Name>(styles as any);
}

export const background = createStyles({
	bgAlternateDarken: {
		"& li:nth-child(even), & tr:nth-child(even)": {
			backgroundColor: "rgba(50,50,50,0.15)",
		},
	},
	cover: {
		background: "no-repeat transparent",
		backgroundSize: "cover",
		position: "relative",
		overflow: "hidden",
	},
}).attach().classes;

export const events = createStyles({
	clickable: {
		cursor: "pointer",

		"&:hover": {
			background: "rgba(50,50,50,.3) !important",
		},
	},
}).attach().classes;

export const redHandleContainer = createStyles({
	container: {
		backgroundColor: "rgba(32, 32, 32, .4)",
		borderStyle: "solid",
		borderWidth: "10px",
		borderImage: "url('/corners.png') 10 10 10 10 repeat",
		padding: "10px",
	},
	header: {
		fontSize: "48px",
		lineHeight: "40px",
		marginLeft: "5px",
		fontWeight: "bold",
		color: "#9f9f9f",
		fontVariant: "small-caps",
		textTransform: "lowercase",
		letterSpacing: "5px",
		"-webkit-text-stroke-width": "1px",
		"-webkit-text-stroke-color": "#151515",
	},
}).attach().classes;

export const layout = createStyles({
	invisible: {
		opacity: 0,
		cursor: "default",
	},
	centerLayout1000: {
		maxWidth: "1000px",
		margin: "0 auto"
	},
	vertMiddle: {
		verticalAlign: "middle",
	},
	horzRule: {
		height: "1px",
		width: "90%",
		borderBottom: "1px solid rgba(255,255,255,.2)",
		borderTop: "1px solid rgba(255,255,255,.4)",
	},
	flexRow: {
		display: "flex",
		flexDirection: "row",
		flexWrap: "nowrap",
	},
	flexEvenDistribution: {
		flexGrow: 1,
		flexBasis: 0,
	},
	flexFillRemaining: {
		flexGrow: 1
	},
	flexWrapContent: {
		flexGrow: 0,
		flexBasis: 0,
	},
	flexCenter: {
		alignItems: "center",
	},
	blockCenter: { 
		marginLeft: "auto",
		marginRight: "auto",
	},
	inlineBlock: {
		display: "inline-block",
	},
	width100: { width: "100%" },
	width70: { width: "70%" },
	width50: { width: "50%" },
	width40: { width: "40%" },
	width25: { width: "25%" },
	width15: { width: "15%" },
	width10: { width: "10%" },
}).attach().classes;

export const layoutMobile = createStyles({
	block: {"@media only screen and (max-width: 600px)": { display: "block" } },
	flexColumn: {
		"@media only screen and (max-width: 600px)": {
			display: "flex",
			flexDirection: "column",
		},
	},
	width50: { "@media only screen and (max-width: 600px)": { width: "50%" } },
	width100: { "@media only screen and (max-width: 600px)": { width: "100%" } },
}).attach().classes;

export const textColor = createStyles({
	white: { color: "#E8E8E8" },
	gray: { color: "#555555" },
	gray9f: { color: "#9F9F9F" },
	graye8: { color: "#E8E8E8" },
}).attach().classes;

export const margin = createStyles({
	all: { margin: "10px", },
	half: { margin: "5px", },

	vertical: { marginTop: "10px", marginBottom: "10px", },
	verticalHalf: { marginTop: "5px", marginBottom: "5px", },

	horizontal: { marginLeft: "10px", marginRight: "10px", },
	horizontalHalf: { marginLeft: "5px", marginRight: "5px", },

	right: { marginRight: "10px", },
	rightHalf: { marginRight: "5px", },
	rightDouble: { marginRight: "20px", },

	left : { marginLeft: "10px", },
	leftHalf: { marginLeft: "5px", },
	leftDouble: { marginLeft: "20px", },

	top: { marginTop: "10px", },
	topHalf: { marginTop: "5px", },
	topDouble: { marginTop: "20px", },

	bottom : { marginBottom: "10px", },
	bottomHalf: { marginBottom: "5px", },
	bottomDouble: { marginBottom: "20px", },
}).attach().classes;

export const marginMobile = createStyles({
	clear: { "@media only screen and (max-width: 600px)": { margin: "0" } },

	all: { "@media only screen and (max-width: 600px)": { margin: "10px" } },
	half: { "@media only screen and (max-width: 600px)": { margin: "5px" } },

	vertical: { "@media only screen and (max-width: 600px)": { marginTop: "10px", marginBottom: "10px" } },
	verticalHalf: { "@media only screen and (max-width: 600px)": { marginTop: "5px", marginBottom: "5px" } },

	horizontal: { "@media only screen and (max-width: 600px)": { marginLeft: "10px", marginRight: "10px" } },
	horizontalHalf: { "@media only screen and (max-width: 600px)": { marginLeft: "5px", marginRight: "5px" } },

	right: { "@media only screen and (max-width: 600px)": { marginRight: "10px" } },
	rightHalf: { "@media only screen and (max-width: 600px)": { marginRight: "5px" } },
	rightDouble: { "@media only screen and (max-width: 600px)": { marginRight: "20px" } },

	left : { "@media only screen and (max-width: 600px)": { marginLeft: "10px" } },
	leftHalf: { "@media only screen and (max-width: 600px)": { marginLeft: "5px" } },
	leftDouble: { "@media only screen and (max-width: 600px)": { marginLeft: "20px" } },

	top: { "@media only screen and (max-width: 600px)": { marginTop: "10px" } },
	topHalf: { "@media only screen and (max-width: 600px)": { marginTop: "5px" } },
	topDouble: { "@media only screen and (max-width: 600px)": { marginTop: "20px" } },

	bottom : { "@media only screen and (max-width: 600px)": { marginBottom: "10px" } },
	bottomHalf: { "@media only screen and (max-width: 600px)": { marginBottom: "5px" } },
	bottomDouble: { "@media only screen and (max-width: 600px)": { marginBottom: "20px" } },
}).attach().classes;

export const padding = createStyles({
	all: { padding: "10px", },
	half: { padding: "5px", },

	vertical: { paddingTop: "10px", paddingBottom: "10px", },
	verticalHalf: { paddingTop: "5px", paddingBottom: "5px", },

	horizontal: { paddingLeft: "10px", paddingRight: "10px", },
	horizontalHalf: { paddingLeft: "5px", paddingRight: "5px", },

	right: { paddingRight: "10px", },
	rightHalf: { paddingRight: "5px", },
	rightDouble: { paddingRight: "20px", },

	left : { paddingLeft: "10px", },
	leftHalf: { paddingLeft: "5px", },
	leftDouble: { paddingLeft: "20px", },

	top: { paddingTop: "10px", },
	topHalf: { paddingTop: "5px", },
	topDouble: { paddingTop: "20px", },

	bottom : { paddingBottom: "10px", },
	bottomHalf: { paddingBottom: "5px", },
	bottomDouble: { paddingBottom: "20px", },
}).attach().classes;

export const text = createStyles({
	light: { fontWeight: 100 },
	bold: { fontWeight: "bold" },

	font10: { fontSize: "10px" },
	font12: { fontSize: "12px" },
	font14: { fontSize: "14px" },
	font16: { fontSize: "16px" },
	font20: { fontSize: "20px" },
	font22: { fontSize: "22px" },
	font24: { fontSize: "24px" },
	font26: { fontSize: "26px" },
	font28: { fontSize: "28px" },
	font30: { fontSize: "30px" },
	font32: { fontSize: "32px" },
	font34: { fontSize: "34px" },
	font36: { fontSize: "36px" },
	font48: { fontSize: "48px" },
	font56: { fontSize: "56px" },

	left: { textAlign: "left" },
	center: { textAlign: "center" },
	right: { textAlign: "right" },

	smallCaps: { fontVariant: "small-caps" },
	inset: {
		letterSpacing: "5px",
		"-webkit-text-stroke-width": "1px",
		"-webkit-text-stroke-color": "#151515",
	},

	noSelect: {
		"-webkit-touch-callout": "none",
		"-webkit-user-select": "none",
		"-khtml-user-select": "none",
		"-moz-user-select": "none",
		"-ms-user-select": "none",
		userSelect: "none",
	},

	toLower: { 
		fontVariant: "small-caps",
		textTransform: "lowercase",
		letterSpacing: "5px",
	},
	toUpper: {
		textTransform: "uppercase",
		letterSpacing: "5px",
	},
}).attach().classes;
