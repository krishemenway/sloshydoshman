import * as ko from "knockout";
import { text, createStyles } from "AppStyles";
import { ClickableNumberRangeComponent } from "Pagination/ClickableNumberRangeComponent";

export var ComponentName : string = "Pagination";
export function PaginationComponent(selectedPageObservable: string, totalItemCountSubscribable: string, pageSize: number|string) {
	return `component: {name: '${ComponentName}', params: {SelectedPage: ${selectedPageObservable}, PageSize: ${pageSize}, TotalItemCount: ${totalItemCountSubscribable}}}`;
}

interface PaginationParams {
	SelectedPage: ko.Observable<number>;
	TotalItemCount: ko.Subscribable<number>;
	PageSize: number;
}

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
		this.SelectedPage(this.SelectedPage() + 1);
	}

	public PreviousPage = () => {
		this.SelectedPage(this.SelectedPage() - 1);
	}

	public GoToPage = (pageNumber: number) => {
		this.SelectedPage(pageNumber);
	}

	public CanGoForward: ko.Computed<boolean>;
	public CanGoBack: ko.Computed<boolean>;

	public TotalPages: ko.Subscribable<number>;
	public SelectedPage: ko.Observable<number>;
	public PageSize: number = 10;
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

ko.components.register(ComponentName, {
	viewModel: PaginationViewModel,
	template: `
		<div class="${styles.pagesControl} ${text.noSelect}" data-bind="if: TotalPages() > 1">
			<button class="${styles.previous} ${text.center}" data-bind="click: PreviousPage, enable: CanGoBack, visible: CanGoBack">&lt;</button>

			<div class="${text.center}" data-bind="${ClickableNumberRangeComponent(1, "TotalPages()", "SelectedPage", "GoToPage")}" />

			<button class="${styles.next} ${text.center}" data-bind="click: NextPage, enable: CanGoForward, visible: CanGoForward">&gt;</button>
		</div>`,
});