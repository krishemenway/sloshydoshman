import { Observable, Computed } from "knockout";
import * as ko from "knockout";

interface ClickableNumberRangeParams {
	From: number;
	To: number;
	SelectedIndex: Observable<number>;
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

	public SelectedIndex: Observable<number>;

	public BeginRange: Computed<number[]>;
	public MiddleRange: Computed<number[]>;
	public EndRange: Computed<number[]>;

	public OnNumberClicked: (numberClicked: number) => void;

	private From: number;
	private To: number;
	private TotalRange: number;
}

export var Name : string = "ClickableNumberRange";
ko.components.register(Name, {
	viewModel: ClickableNumberRangeViewModel,
	template: `
		<div class="clickable-number-range text-center">
			<!-- ko foreach: BeginRange() -->
			<button class="select-number-button text-center" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
			<!-- /ko -->

			<!-- ko if: MiddleRange().length -->
				&hellip;&nbsp;

				<!-- ko foreach: MiddleRange() -->
				<button class="select-number-button text-center" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
				<!-- /ko -->

				&hellip;&nbsp;
			<!-- /ko -->

			<!-- ko if: MiddleRange().length === 0 && EndRange().length > 0 -->
				&hellip;&nbsp;
			<!-- /ko -->

			<!-- ko foreach: EndRange() -->
			<button class="select-number-button text-center" data-bind="click: $component.OnNumberClicked, text: $data, css: {selected: $data===$component.SelectedIndex()}"></button>&nbsp;
			<!-- /ko -->
		</div>`,
});