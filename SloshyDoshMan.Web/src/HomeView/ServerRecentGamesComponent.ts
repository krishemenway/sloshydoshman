import {ResultOf} from "CommonDataStructures/ResultOf";
import {PlayedGame, RecentGameResponse} from "Server";
import {GoToView} from "App";
import {Observable, ObservableArray, Subscription} from "knockout";
import * as MomentFormatDate from "KnockoutHelpers/MomentFormatDateBindingHandler";
import * as Pagination from "Pagination/PaginationComponent";
import * as GameView from "GameView/PlayedGameComponent";
import * as ko from "knockout";
import * as $ from "jquery";

class RecentGamesViewModel {
	constructor(params?: any) {
		this.HasLoadedGames = ko.observable(false);
		this.LoadedGames = ko.observableArray();
		this.PageNumber = ko.observable(1);
		this.TotalNumberOfGames = ko.observable(0);

		this.LoadPage(1);
		this.PageNumberSubscription = this.PageNumber.subscribe((newPage: number) => this.LoadPage(newPage));
	}

	public dispose() {
		this.PageNumberSubscription.dispose();
	}

	public SelectGame = (game: PlayedGame) : void => {
		GoToView(GameView.Name, {"PlayedGameId": game.PlayedGameId});
	}

	private LoadPage(page: number) {
		let startingAt = this.PageSize*(page-1);
		$.get(`/webapi/games/recent?count=${this.PageSize}&startingAt=${startingAt}`).done(this.HandleGamesResponse);
	}

	private HandleGamesResponse = (response: ResultOf<RecentGameResponse>) => {
		this.TotalNumberOfGames(response.Data.TotalGames); 
		this.LoadedGames(response.Data.RecentGames);
	}

	public TotalNumberOfGames: Observable<number>;
	public PageNumber: Observable<number>;
	public PageSize: number = 10;

	public HasLoadedGames: Observable<boolean>;
	public LoadedGames: ObservableArray<PlayedGame>;

	private PageNumberSubscription: Subscription;
}

export var Name : string = "ServerRecentGames"; 
ko.components.register(Name, {
	viewModel: RecentGamesViewModel,
	template: `
		<div>
			<div class="font-36 text-white small-caps text-center margin-top-half">all games</div>
			<div class="red-handle-container text-white">
				<table class="bg-alternating-colors margin-bottom font-14 text-center width-100">
					<thead><tr><td></td><td></td><td class="text-center">Length</td><td class="text-center">Waves</td></tr></thead>
					<tbody>
						<!-- ko foreach: LoadedGames -->
						<tr class="clickable" style="line-height: 26px" data-bind="click: $component.SelectGame">
							<td class="padding-half text-left" style="width: 69%"><!-- ko text: Map --><!-- /ko -->&nbsp;<span class="text-gray margin-left" data-bind="${MomentFormatDate.DataBind("TimeStarted", "MMMM Do, YYYY")}"></span></td>
							<td class="padding-half text-center" style="width: 11%; min-width: 106px" data-bind="text: Difficulty"></td>
							<td class="padding-half text-center" style="width: 10%" data-bind="text: Length"></td>
							<td class="padding-half text-center" style="width: 10%"><!-- ko text: ReachedWave --><!-- /ko -->&nbsp;/&nbsp;<!-- ko text: TotalWaves --><!-- /ko --></td>
						</tr>
						<!-- /ko -->

						<!-- ko foreach: new Array(PageSize - LoadedGames().length) -->
						<tr><td class="padding-vertical" colspan="4">&nbsp;</td></tr>
						<!-- /ko -->
					</tbody>
				</table>

				<div data-bind="component: {name: '${Pagination.ComponentName}', params: {SelectedPage: PageNumber, PageSize: PageSize, TotalItemCount: TotalNumberOfGames}}" />
			</div>
		</div>`,
});