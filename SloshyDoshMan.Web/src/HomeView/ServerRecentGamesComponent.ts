import * as ko from "knockout";
import * as $ from "jquery";
import { padding, text, textColor, margin, layout, events, background, redHandleContainer } from "AppStyles";
import {ResultOf} from "CommonDataStructures/ResultOf";
import {PlayedGame, RecentGameResponse} from "Server";
import {GoToView} from "App";
import { MomentFormat } from "KnockoutHelpers/MomentFormatDateBindingHandler";
import { PaginationComponent } from "Pagination/PaginationComponent";
import { PlayedGameName } from "GameView/PlayedGameComponent";

export var Name : string = "ServerRecentGames"; 
export function ServerRecentGamesComponent() {
	return `component: {name: '${Name}'}`;
}
class ServerRecentGamesModel {
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
		GoToView(PlayedGameName, {"PlayedGameId": game.PlayedGameId});
	}

	private LoadPage(page: number) {
		let startingAt = this.PageSize*(page-1);
		$.get(`/webapi/games/recent?count=${this.PageSize}&startingAt=${startingAt}`).done(this.HandleGamesResponse);
	}

	private HandleGamesResponse = (response: ResultOf<RecentGameResponse>) => {
		this.TotalNumberOfGames(response.Data.TotalGames); 
		this.LoadedGames(response.Data.RecentGames);
	}

	public TotalNumberOfGames: ko.Observable<number>;
	public PageNumber: ko.Observable<number>;
	public PageSize: number = 10;

	public HasLoadedGames: ko.Observable<boolean>;
	public LoadedGames: ko.ObservableArray<PlayedGame>;

	private PageNumberSubscription: ko.Subscription;
}

ko.components.register(Name, {
	viewModel: ServerRecentGamesModel,
	template: `
		<div>
			<div class="${text.font36} ${textColor.white} ${text.smallCaps} ${text.center} ${margin.topHalf}">all games</div>
			<div class="${redHandleContainer.container} ${textColor.white}">
				<table class="${background.bgAlternateDarken} ${margin.bottom} ${text.font14} ${text.center} ${layout.width100}">
					<thead><tr><td></td><td></td><td class="${text.center}">Length</td><td class="${text.center}">Waves</td></tr></thead>
					<tbody>
						<!-- ko foreach: LoadedGames -->
						<tr class="${events.clickable}" style="line-height: 26px" data-bind="click: $component.SelectGame">
							<td class="${padding.half} ${text.left}" style="width: 69%">
								<!-- ko text: Map --><!-- /ko -->&nbsp;
								<span class="${textColor.gray} ${margin.left}" data-bind="${MomentFormat("TimeStarted", "MMMM Do, YYYY")}" />
							</td>
							<td class="${padding.half} ${text.center}" style="width: 11%; min-width: 106px" data-bind="text: Difficulty"></td>
							<td class="${padding.half} ${text.center} ${layout.width10}" data-bind="text: Length"></td>
							<td class="${padding.half} ${text.center} ${layout.width10}"><!-- ko text: ReachedWave --><!-- /ko -->&nbsp;/&nbsp;<!-- ko text: TotalWaves --><!-- /ko --></td>
						</tr>
						<!-- /ko -->

						<!-- ko foreach: new Array(PageSize - LoadedGames().length) -->
						<tr style="line-height: 26px"><td class="${padding.half}" colspan="4">&nbsp;</td></tr>
						<!-- /ko -->
					</tbody>
				</table>

				<div data-bind="${PaginationComponent("PageNumber", "TotalNumberOfGames", "PageSize")}" />
			</div>
		</div>`,
});