import * as ClickableNumberRange from "Pagination/ClickableNumberRangeComponent";
import { Observable, Computed, Subscribable } from "knockout";
import * as ko from "knockout";
import { text, createStyles } from "AppStyles";

export var ComponentName : string = "Pagination";
export function PaginationComponent(selectedPageObservable: string, totalItemCountSubscribable: string, pageSize: number|string) {
	return `component: {name: '${ComponentName}', params: {SelectedPage: ${selectedPageObservable}, PageSize: ${pageSize}, TotalItemCount: ${totalItemCountSubscribable}}}`;
}

interface PaginationParams {
	SelectedPage: Observable<number>;
	TotalItemCount: Subscribable<number>;
	PageSize: number;
}

const styles = createStyles({
	next: {
		right: 0,
	},
	previous: {
		left: 0,
	},
	pagesControl: {
		position: "relative",

		"& $previous, & $next": {
			width: "50px",
			height: "40px",

			backgroundColor: "#181818",
			borderRadius: "3px",
			border: "1px solid #080808",

			position: "absolute",
			top: "0",
		},
		"& $previous:disabled, & $next:disabled": {
			backgroundColor: "transparent",
			borderColor: "#080808",
		},
	},
}).attach().classes;

export class PaginationViewModel {
	constructor(params?: PaginationParams) {
		if (!params || !params.TotalItemCount) {
			throw "Missing required params";
		}

		this.SelectedPage = params.SelectedPage;
		this.PageSize = !!params && params.PageSize || 10;
		this.TotalPages = ko.pureComputed(() => Math.ceil(params.TotalItemCount() / this.PageSize));
		this.CanGoForward = ko.pureComputed(() => this.SelectedPage() < this.TotalPages());
		this.CanGoBack = ko.pureComputed(() => this.SelectedPage() > 1);
	}

	public NextPage = () => {
		this.SelectedPage(this.SelectedPage()+1);
	}

	public PreviousPage = () => {
		this.SelectedPage(this.SelectedPage()-1);
	}

	public GoToPage = (pageNumber: number) => {
		this.SelectedPage(pageNumber);
	}

	public CanGoForward: Computed<boolean>;
	public CanGoBack: Computed<boolean>;

	public TotalPages: Subscribable<number>;
	public SelectedPage: Observable<number>;
	public PageSize: number = 10;
}

ko.components.register(ComponentName, {
	viewModel: PaginationViewModel,
	template: `
		<div class="${styles.pagesControl} ${text.noSelect}" data-bind="if: TotalPages() > 1">
			<button class="${styles.previous} ${text.center}" data-bind="click: PreviousPage, enable: CanGoBack, visible: CanGoBack">&lt;</button>

			<div class="${text.center}" data-bind="component: {name: '${ClickableNumberRange.Name}', params: {From: 1, To: TotalPages(), SelectedIndex: SelectedPage, OnNumberClicked: GoToPage}}" />

			<button class="${styles.next} ${text.center}" data-bind="click: NextPage, enable: CanGoForward, visible: CanGoForward">&gt;</button>
		</div>`,
});