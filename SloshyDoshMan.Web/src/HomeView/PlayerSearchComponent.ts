import * as ko from "knockout";
import * as $ from "jquery";
import { layout, text, textColor, padding, events, createStyles, redHandleContainer } from "AppStyles";
import { ResultOf } from "CommonDataStructures/ResultOf";
import { GoToPlayerProfile } from "PlayerView/PlayerProfileComponent";
import { PaginationComponent } from "Pagination/PaginationComponent";

var Name : string = "PlayerSearch";
export function PlayerSearchComponent() {
	return `component: {name: '${Name}'}`;
}

interface PlayerSearchResult {
	UserName: string;
	SteamId: string;
}

class PlayerSearchViewModel {
	constructor(params?: any) {
		this.SelectedPage = ko.observable(1);
		this.SearchResults = ko.observableArray();
		this.TotalItemCount = ko.pureComputed(() => this.SearchResults().length, this);
		this.Query = ko.observable("").extend({throttle: 500});
		this.Query.subscribe(this.OnQueryChanged);
		this.SearchResultsPage = ko.pureComputed(this.GetSearchResultsPage, this);
	}

	public GoToPlayer = (result: PlayerSearchResult) => {
		GoToPlayerProfile(result.SteamId);
	}

	private GetSearchResultsPage = () => {
		let selectedIndex = this.SelectedPage()-1;
		let startIndex = selectedIndex*this.SearchResultsPageSize;

		return this.SearchResults().slice(startIndex, startIndex + this.SearchResultsPageSize);
	}

	private OnQueryChanged = (newQuery: string) => {
		if(newQuery.length <= 2) {
			this.SearchResults([]);
			this.SelectedPage(1);
		} else {
			$.get(`/webapi/players/search?query=${newQuery}`).done((response: ResultOf<PlayerSearchResult[]>) => { this.SearchResults(response.Data); this.SelectedPage(1); });
		}
	}

	public SelectedPage: ko.Observable<number>;
	public TotalItemCount: ko.Computed<number>;

	public SearchResults: ko.ObservableArray<PlayerSearchResult>;
	public SearchResultsPage: ko.Computed<PlayerSearchResult[]>;
	public SearchResultsPageSize: number = 10;

	public Query: ko.Observable<string>;
}

const styles = createStyles({
	playerSearch: {
		transition: "all 400ms cubic-bezier(0.120, 0.460, 0.215, 0.750)",
	},
	search: {
		padding: "10px",
		background: "#181818",
		border: "0",
		color: "#e8e8e8",
	},
	searchTextWrapper: {
		borderBottom: "1px solid #e8e8e8",
		paddingBottom: "10px",
		marginBottom: "10px",
	},
	searchResult: { 
		display: "block",
		"&:nth-child(odd)": {
			backgroundColor: "transparent",
		},
		"&:nth-child(even)": {
			backgroundColor: "rgba(50,50,50,0.15)",
		},
	},
}).attach().classes;

ko.components.register(Name, {
	viewModel: PlayerSearchViewModel,
	template: `
		<div class="${styles.playerSearch} ${redHandleContainer.container}">
			<div class="${text.font24} ${textColor.white} ${text.smallCaps}">Search for player</div>

			<div class="${styles.searchTextWrapper}">
				<input class="${styles.search} ${text.font24} ${layout.width100}" type="search" ref="searchBox" data-bind="textInput: Query" />
			</div>

			<!-- ko if: SearchResultsPage().length > 0 -->
			<ul class="${textColor.white}">
				<!-- ko foreach: SearchResultsPage() -->
				<li class="${padding.all} ${layout.width100} ${styles.searchResult} ${events.clickable}" data-bind="click: $component.GoToPlayer, html: UserName"></li>
				<!-- /ko -->
				<!-- ko foreach: new Array(SearchResultsPageSize - SearchResultsPage().length) -->
				<li class="${padding.all} ${layout.width100} ${styles.searchResult}">&nbsp;</li>
				<!-- /ko -->
			</ul>
			<!-- /ko -->

			<!-- ko if: SearchResults().length > 0 -->
			<div class="${textColor.white}" data-bind="${PaginationComponent("SelectedPage", "TotalItemCount", "SearchResultsPageSize")}" />
			<!-- /ko -->
		</div>`,
});