import * as ko from "knockout";
import { text, createStyles, layout, margin } from "AppStyles";
import { icons } from "MaterialIcons";

export var ComponentName : string = "Pagination";
export function PaginationComponent(selectedPageSubscribable: string, totalItemCountSubscribable: string, pageSize: number|string) {
	return `component: {name: '${ComponentName}', params: {SelectedPage: ${selectedPageSubscribable}, PageSize: ${pageSize}, TotalItemCount: ${totalItemCountSubscribable}}}`;
}

interface PaginationParams {
	SelectedPage: ko.Subscribable<number>;
	TotalItemCount: ko.Subscribable<number>;
	PageSize: number;
}

export class PaginationViewModel {
	constructor(params?: PaginationParams) {
		if (!params || !params.TotalItemCount) {
			throw "Missing required params";
		}

		this.TotalItemCount = params.TotalItemCount;
		this.SelectedPage = params.SelectedPage;

		this.PageSize = !!params && params.PageSize || 10;
	}

	public TotalPages: ko.Computed<number> = ko.pureComputed(() => {
		return Math.ceil(this.TotalItemCount() / this.PageSize);
	}, this);

	public CanGoFirst: ko.Computed<boolean> = ko.pureComputed(() => {
		return this.SelectedPage() > 1;
	}, this);

	public CanGoLast: ko.Computed<boolean> = ko.pureComputed(() => {
		return this.SelectedPage() < this.TotalPages();
	}, this);

	public GoToFirstPage = () => {
		this.GoToPage(1);
	}

	public GoToLastPage = () => {
		this.GoToPage(this.TotalPages());
	}

	public GoToPage = (pageNumber: number) => {
		if (pageNumber < 1 || pageNumber > this.TotalPages()) {
			return;
		}

		this.SelectedPage(pageNumber);
	}

	public PageRange: ko.Computed<number[]> = ko.pureComputed(() => {
		let lowerbound = Math.max(0, this.SelectedPage() - 1);
		let upperbound = Math.min(this.TotalPages() + 1, this.SelectedPage() + 1);

		return this.CreateRange(lowerbound, upperbound);
	});

	private CreateRange(from: number, to: number) {
		let array : number[] = [];

		for(let i = from; i <= to; i++) {
			array.push(i);
		}

		return array;
	}

	public SelectedPage: ko.Subscribable<number>;
	public TotalItemCount: ko.Subscribable<number>;

	public PageSize: number = 10;
}

const styles = createStyles({
	selectNumberButton: {
		width: "30px",
		height: "40px",

		border: "1px solid transparent",

		lineHeight: "40px",

		"&:focus": {
			outline: 0,
		},

		"&:hover": {
			borderStyle: "dashed",
			borderColor: "rgba(100,100,100,.25)",
		},

		"&:disabled": {
			color: "#303030",
		},

		"&:disabled:hover": {
			borderColor: "transparent",
		},

		"&.selected": {
			borderColor: "rgba(100,100,100,.25)",
			borderStyle: "solid",
			cursor: "default",
		},
	},
}).attach().classes;

ko.components.register(ComponentName, {
	viewModel: PaginationViewModel,
	template: `
		<div class="${text.noSelect}" data-bind="if: TotalPages() > 1">

			<!-- ko if: TotalPages() > 1 -->
			<div class="${text.center}">
				<button class="${styles.selectNumberButton} ${text.center} ${margin.rightHalf}" data-bind="click: $component.GoToFirstPage, enable: CanGoFirst"><i class="${icons.icon} ${layout.vertMiddle}">skip_previous</i></button>

				<!-- ko foreach: PageRange() -->
				<button class="${styles.selectNumberButton} ${text.center}" data-bind="click: $component.GoToPage, text: $data, css: {selected: $data === $component.SelectedPage(), ${layout.invisible}: $data === 0 || $data === $component.TotalPages() + 1}" />
				<!-- /ko -->

				<button class="${styles.selectNumberButton} ${text.center} ${margin.leftHalf}" data-bind="click: $component.GoToLastPage, enable: CanGoLast"><i class="${icons.icon} ${layout.vertMiddle}">skip_next</i></button>
			</div>
			<!-- /ko -->

		</div>`,
});