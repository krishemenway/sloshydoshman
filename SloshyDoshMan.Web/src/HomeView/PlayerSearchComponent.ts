import {ResultOf} from "CommonDataStructures/ResultOf";
import {GoToView} from "App";
import * as Pagination from "Pagination/PaginationComponent";
import * as PlayerView from "PlayerView/PlayerProfileComponent";
import * as ko from "knockout";
import * as $ from "jquery";

export var Name : string = "PlayerSearch";

interface PlayerSearchResult {
	UserName: string;
	SteamId: string;
}

class PlayerSearchViewModel {
	constructor(params?: any) {
		this.SelectedPage = ko.observable(1);
		this.SearchResults = ko.observableArray([]);
		this.TotalItemCount = ko.pureComputed(() => this.SearchResults().length, this);
		this.Query = ko.observable("").extend({throttle: 500});
		this.Query.subscribe(this.OnQueryChanged);
		this.SearchResultsPage = ko.pureComputed(this.GetSearchResultsPage, this);
	}

	public GoToPlayer = (result: PlayerSearchResult) => {
		GoToView(PlayerView.Name, {SteamId: result.SteamId});
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

	public SelectedPage: KnockoutObservable<number>;
	public TotalItemCount: KnockoutComputed<number>;

	public SearchResults: KnockoutObservableArray<PlayerSearchResult>;
	public SearchResultsPage: KnockoutComputed<PlayerSearchResult[]>;
	public SearchResultsPageSize: number = 10;

	public Query: KnockoutObservable<string>;
}

ko.components.register(Name, {
	viewModel: PlayerSearchViewModel,
	template: `
		<div class="player-search red-handle-container">
			<div class="font-24 text-white small-caps">Search for player</div>

			<div class="search-text-wrapper">
				<input class="search font-24 width-100" type="search" ref="searchBox" data-bind="textInput: Query" />
			</div>

			<!-- ko if: SearchResultsPage().length > 0 -->
			<ul class="search-results text-white">
				<!-- ko foreach: SearchResultsPage() -->
				<li class="padding-horizontal padding-vertical width-100 search-result clickable" data-bind="click: $component.GoToPlayer, html: UserName"></li>
				<!-- /ko -->
				<!-- ko foreach: new Array(SearchResultsPageSize - SearchResultsPage().length) -->
				<li class="padding-horizontal padding-vertical width-100 search-result">&nbsp;</li>
				<!-- /ko -->
			</ul>
			<!-- /ko -->

			<!-- ko if: SearchResults().length > 0 -->
			<div class="text-white" data-bind="component: {name: '${Pagination.ComponentName}', params: {SelectedPage: SelectedPage, PageSize: SearchResultsPageSize, TotalItemCount: TotalItemCount}}" />
			<!-- /ko -->
		</div>`,
});