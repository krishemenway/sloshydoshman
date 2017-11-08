import {PieChartData,PieChartComponentOptions, ChartJSOptions} from 'ChartJS/Chart';
import * as $ from "jquery";

export var Name : string = "ChartJSPieChart";

export declare class Chart {
	constructor(canvas: CanvasRenderingContext2D, options: ChartJSOptions);
	destroy() : void;
	update(duration: number, lazy: boolean) : void;
}

export interface ColorPair {
	DefaultColor: string;
	HoverColor: string;
}

export let DefaultColorSet: ColorPair[] = [
	{DefaultColor: '#4D4D4D', HoverColor: '#676767'},
	{DefaultColor: '#5DA5DA', HoverColor: '#77BFF4'},
	{DefaultColor: '#FAA43A', HoverColor: '#FFBE54'},
	{DefaultColor: '#60BD68', HoverColor: '#7AD782'},
	{DefaultColor: '#F17CB0', HoverColor: '#FF96CA'},
	{DefaultColor: '#B2912F', HoverColor: '#CCAB49'},
	{DefaultColor: '#B276B2', HoverColor: '#CC90CC'},
	{DefaultColor: '#DECF3F', HoverColor: '#F8E959'},
	{DefaultColor: '#F15854', HoverColor: '#FF726E'},
	{DefaultColor: '#b29595', HoverColor: '#CCAFAF'},
];

let createCanvas = (parentElement: Element) : HTMLCanvasElement => {
	let canvas = $("canvas").get(0);
	return <HTMLCanvasElement>canvas;
};

function createChartData(data: PieChartData[], colorSet: ColorPair[]) {
	return {
		labels: data.map((point) => point.Label),
		datasets: [
			{
				data: data.map((point) => point.Value),
				backgroundColor: data.map((point: PieChartData, index: number) => colorSet[index].DefaultColor),
				hoverBackgroundColor: data.map((point: PieChartData, index: number) => colorSet[index].HoverColor)
			}
		]
	};
}

function init(element: Element, valueAccessor: () => KnockoutComputed<PieChartData[]>, allBindingsAccessor?: KnockoutAllBindingsAccessor, viewModel?: any, bindingContext?: KnockoutBindingContext) : void {
	let elementIsCanvas = element.nodeName.toLowerCase() === "canvas";
	let canvasElement: HTMLCanvasElement = elementIsCanvas ? <HTMLCanvasElement>element : <HTMLCanvasElement>createCanvas(element);
	let context = <CanvasRenderingContext2D>canvasElement.getContext("2d");

	let options : PieChartComponentOptions = {
		responsive: true,
		cutoutPercentage: 25,
		legend: {
			position: 'bottom',
			labels: {
				fontColor: '#CFCFCF'
			}
		}
	};

	let elementAsAny : any = element;
	elementAsAny.Chart = new Chart(context, { type: 'pie', data: createChartData(ko.unwrap(valueAccessor()), DefaultColorSet), options: options });

	ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
		elementAsAny.Chart.destroy();
	});
}

function update(element: Element, valueAccessor: () => KnockoutComputed<PieChartData[]>, allBindingsAccessor?: KnockoutAllBindingsAccessor, viewModel?: any, bindingContext?: KnockoutBindingContext) : void {
	let elementAsAny : any = element;
	$.extend(elementAsAny.Chart.data.datasets[0], createChartData(ko.unwrap(valueAccessor()), DefaultColorSet));
	elementAsAny.Chart.update();
}

ko.bindingHandlers[Name] = { init: init, update: update };
ko.virtualElements.allowedBindings[Name] = true;
