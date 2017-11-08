
export interface PieChartData {
	Value: number;
	Label: string;
}

export interface PieChartComponentOptions extends BaseChartJSComponentOptions {
	cutoutPercentage?: number; // The percentage of the chart that is cut out of the middle.
	rotation?: number; // Starting angle to draw arcs from
	circumference?: number; // Sweep to allow arcs to cover
}

export interface ChartJSLegendOptions {
	display?: boolean;
	position?: string;
	fullWidth?: boolean;
	onClick?: Function;
	onHover?: Function;
	labels?: any;
	reverse?: boolean;
}

export interface BaseChartJSComponentOptions {
	defaultFontColor?: string; // Default font color for all text
	defaultFontFamily?: string; // Default font family for all text
	defaultFontSize?: number; // Default font size (in px) for text. Does not apply to radialLinear scale point labels
	defaultFontStyle?: string; // Default font style. Does not apply to tooltip title or footer. Does not apply to chart title

	responsive?: boolean; // Resizes the chart canvas when its container does.
	responsiveAnimationDuration?: number; // Duration in milliseconds it takes to animate to new size after a resize event.
	maintainAspectRatio?: boolean; // Maintain the original canvas aspect ratio (width / height) when resizing
	events?: Array<["mousemove","mouseout","click","touchstart","touchmove","touchend"]>; // Events that the chart should listen to for tooltips and hovering
	onClick?: Function; // Called if the event is of type 'mouseup' or 'click'. Called in the context of the chart and passed the event and an array of active elements
	legendCallback?: Function; // Function to generate a legend. Receives the chart object to generate a legend from. Default implementation returns an HTML string.
	onResize?: Function; // Called when a resize occurs. Gets passed two arguments: the chart instance and the new size.

	legend?: ChartJSLegendOptions;
}

export interface ChartJSOptions {
	type: string;
	data: any;
	options: BaseChartJSComponentOptions;
}
