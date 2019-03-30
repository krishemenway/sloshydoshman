import * as ko from "knockout";
import { text, createStyles } from "AppStyles";

var Name : string = "ClickableNumberRange";
export function ClickableNumberRangeComponent(from: string|number, to: string|number, selectedIndex: string|number, onNumberClicked: string) {
	return `component: {name: '${Name}', params: {From: ${from}, To: ${to}, SelectedIndex: ${selectedIndex}, OnNumberClicked: ${onNumberClicked}}}`;
}

interface ClickableNumberRangeParams {
	From: number;
	To: number;
	SelectedIndex: ko.Observable<number>;
	OnNumberClicked: (numberClicked: number) => void;
}

class ClickableNumberRangeViewModel {
	constructor(params: ClickableNumberRangeParams) {
		this.From = params.From;
		this.To = params.To;
		this.TotalRange = this.To - this.From;
		this.SelectedIndex = params.SelectedIndex || ko.observable(0);
		this.OnNumberClicked = params.OnNumberClicked || ((clickedNumber: number) => { console.log(`Clicked ${clickedNumber}`)});

		this.BeginRange = ko.pureComputed(this.DetermineBeginRange);
		this.MiddleRange = ko.pureComputed(this.DetermineMiddleRange);
		this.EndRange = ko.pureComputed(this.DetermineEndRange);
	}

	private DetermineBeginRange = () : number[] => {
		if(this.TotalRange <= 6) {
			return this.CreateRange(this.From, this.To);
		}

		if(this.SelectedIndex() <= 5) {
			return this.CreateRange(this.From, Math.max(this.From + 2, this.SelectedIndex()+2));
		} else {
			return this.CreateRange(this.From, this.From+2);
		}
	}

	private DetermineMiddleRange = () : number[] => {
		if(this.TotalRange <= 6) {
			return [];
		}

		if(this.SelectedIndex() > 5 && this.SelectedIndex() < this.To - 5) {
			return this.CreateRange(this.SelectedIndex() - 2, this.SelectedIndex() + 2);
		} else {
			return [];
		}
	}

	private DetermineEndRange = () : number[] => {
		if(this.TotalRange <= 6) {
			return [];
		}

		if(this.SelectedIndex() >= this.To - 5) {
			return this.CreateRange(Math.min(this.To - 2, this.SelectedIndex()-2), this.To);
		} else {
			return this.CreateRange(this.To - 2, this.To);
		}
	}

	private CreateRange(from: number, to: number) {
		let array : number[] = [];

		for(let i = from; i <= to; i++) {
			array.push(i);
		}

		return array;
	}

	public SelectedIndex: ko.Observable<number>;

	public BeginRange: ko.Computed<number[]>;
	public MiddleRange: ko.Computed<number[]>;
	public EndRange: ko.Computed<number[]>;

	public OnNumberClicked: (numberClicked: number) => void;

	private From: number;
	private To: number;
	private TotalRange: number;
}

const styles = createStyles({
	selectNumberButton: {
		width: "30px",
		height: "40px",

		border: "1px solid transparent",

		lineHeight: "40px",

		"&:hover": {
			borderStyle: "dashed",
			borderColor: "rgba(255,255,255,.25)",
		},

		"&.selected": {
			borderColor: "rgba(255,255,255,.25)",
			borderStyle: "solid",
			cursor: "default",
		},
	},
}).attach().classes;

ko.components.register(Name, {
	viewModel: ClickableNumberRangeViewModel,
	template: `
		<div class="${text.center}">
			<!-- ko foreach: BeginRange() -->
			<button class="${styles.selectNumberButton} ${text.center}" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
			<!-- /ko -->

			<!-- ko if: MiddleRange().length -->
				&hellip;&nbsp;

				<!-- ko foreach: MiddleRange() -->
				<button class="${styles.selectNumberButton} ${text.center}" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
				<!-- /ko -->

				&hellip;&nbsp;
			<!-- /ko -->

			<!-- ko if: MiddleRange().length === 0 && EndRange().length > 0 -->
				&hellip;&nbsp;
			<!-- /ko -->

			<!-- ko foreach: EndRange() -->
			<button class="${styles.selectNumberButton} ${text.center}" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
			<!-- /ko -->
		</div>`,
});