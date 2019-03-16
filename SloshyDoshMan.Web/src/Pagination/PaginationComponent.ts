import * as ClickableNumberRange from "Pagination/ClickableNumberRangeComponent";
import { Observable, Computed } from "knockout";
import * as ko from "knockout";

export var ComponentName : string = "Pagination";

interface PaginationParams {
	SelectedPage: Observable<number>;
	TotalItemCount: Computed<number>;
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

	public TotalPages: Computed<number>;
	public SelectedPage: Observable<number>;
	public PageSize: number = 10;
}

ko.components.register(ComponentName, {
	viewModel: PaginationViewModel,
	template: `
		<div class="pages-control no-select" data-bind="if: TotalPages() > 1">
			<button class="previous text-center" data-bind="click: PreviousPage, enable: CanGoBack, visible: CanGoBack">&lt;</button>

			<div class="text-center" data-bind="component: {name: '${ClickableNumberRange.Name}', params: {From: 1, To: TotalPages(), SelectedIndex: SelectedPage, OnNumberClicked: GoToPage}}" />

			<button class="next text-center" data-bind="click: NextPage, enable: CanGoForward, visible: CanGoForward">&gt;</button>
		</div>`,
});