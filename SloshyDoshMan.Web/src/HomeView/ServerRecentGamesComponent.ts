import {ResultOf} from "CommonDataStructures/ResultOf";
import {PlayedGame, RecentGameResponse} from "Server";
import {GoToView} from "App";
import * as HashChange from "KnockoutHelpers/HashchangeExtender";
import * as MomentFormatDate from "KnockoutHelpers/MomentFormatDateBindingHandler";
import * as Pagination from "Pagination/PaginationComponent";
import * as GameView from "GameView/PlayedGameComponent";
import * as ko from "knockout";
import * as $ from "jquery";

export var Name : string = "ServerRecentGames"; 

class RecentGamesViewModel {
	constructor(params?: any) {
		this.HasLoadedGames = ko.observable(false);
		this.LoadedGames = ko.observableArray([]);
		this.PageNumber = HashChange.CreateObservable<number>("p", 1);
		this.TotalNumberOfGames = ko.observable(0);

		if(this.PageNumber() === undefined || this.PageNumber() === null) {
			this.PageNumber(1);
		}

		this.LoadPage(this.PageNumber());

		this.PageNumberSubscription = this.PageNumber.subscribe((newPage: number) => this.LoadPage(newPage));
	}

	public dispose() {
		this.PageNumberSubscription.dispose();
	}

	public SelectGame = (game: PlayedGame) : void => {
		GoToView(GameView.ComponentName, {"PlayedGameId": game.PlayedGameId});
	}

	private LoadPage(pageNumber: number) {
		let startingAt = this.PageSize*(this.PageNumber()-1);
		$.get(`/webapi/home/recentgames?count=${this.PageSize}&startingAt=${startingAt}`).done(this.HandleGamesResponse);
	}

	private HandleGamesResponse = (response: ResultOf<RecentGameResponse>) => {
		this.TotalNumberOfGames(response.Data.TotalGames); 
		this.LoadedGames(response.Data.RecentGames);
	}

	public TotalNumberOfGames: KnockoutObservable<number>;
	public PageNumber: KnockoutObservable<number>;
	public PageSize: number = 10;

	public HasLoadedGames: KnockoutObservable<boolean>;
	public LoadedGames: KnockoutObservableArray<PlayedGame>;

	private PageNumberSubscription: KnockoutSubscription;
}

ko.components.register(Name, {
	viewModel: RecentGamesViewModel,
	template: `
		<div>
			<div class="font-36 text-white small-caps text-center margin-top-half">all games</div>
			<div class="red-handle-container text-white">
				<table class="bg-alternating-colors margin-bottom font-14 text-center width-100">
					<thead><tr><td></td><td></td><td>Length</td><td>Waves</td></tr></thead>
					<tbody>
						<!-- ko foreach: LoadedGames -->
						<tr class="clickable" style="line-height: 26px" data-bind="click: $component.SelectGame">
							<td class="padding-half text-left" style="width: 69%"><!-- ko text: Map --><!-- /ko -->&nbsp;<span class="text-gray margin-left" data-bind="${MomentFormatDate.Name}: {Format: 'MMMM Do, YYYY', Date: TimeStarted}"></span></td>
							<td class="padding-half text-center" style="width: 11%" data-bind="text: Difficulty"></td>
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