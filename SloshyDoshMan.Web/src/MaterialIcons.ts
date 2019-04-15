import { createStyles } from "AppStyles";

export const icons = createStyles({
	icon: {
		fontFamily: 'Material Icons',
		fontWeight: "normal",
		fontStyle: "normal",
		fontSize: "24px",
		lineHeight: "1",
		letterSpacing: "normal",
		textTransform: "none",
		display: "inline-block",
		whiteSpace: "nowrap",
		wordWrap: "normal",
		direction: "ltr",
		"-moz-font-feature-settings": "liga",
		"-moz-osx-font-smoothing": "grayscale",
	},
	flipHorizontal: {
		"-moz-transform": "scale(-1, 1)",
		"-webkit-transform": "scale(-1, 1)",
		"-o-transform": "scale(-1, 1)",
		"-ms-transform": "scale(-1, 1)",
		transform: "scale(-1, 1)",
	},
	flipVertical: {
		"-moz-transform": "scale(1, -1)",
		"-webkit-transform": "scale(1, -1)",
		"-o-transform": "scale(1, -1)",
		"-ms-transform": "scale(1, -1)",
		transform: "scale(1, -1)",
	},
}).attach().classes;
