import * as ko from "knockout";
import { layout, text, padding, background, events, margin, textColor } from "AppStyles";
import { PlayedGame } from "Server";
import { GoToPlayedGame } from "GameView/PlayedGameComponent";
import { MomentFormat } from "KnockoutHelpers/MomentFormatDateBindingHandler";
import { PaginationComponent } from "Pagination/PaginationComponent";

var ComponentName : string = "PlayerGameList";
export function PlayedGameListComponent(gamesParameter: string, pageSizeParameter?: string|number) {
	return `component: {name: '${ComponentName}', params: {Games: ${gamesParameter}, PageSize: ${pageSizeParameter}}}`;
}

interface PlayedGameListModel {
	Games: PlayedGame[];
	PageSize: number;
}

class PlayedGameListModel {
	constructor(params: PlayedGameListModel) {
		this.GamesPageSize = params.PageSize || 8;
		this.Games = params.Games;
		this.PageNumber = ko.observable(1);
		this.TotalGamesCount = ko.pureComputed(() => this.Games.length, this);
		this.GamesPage = ko.pureComputed(this.GetGamesPage, this);
	}

	public OnGameSelect = (game: PlayedGame) => {
		GoToPlayedGame(game.PlayedGameId);
	}

	private GetGamesPage = () => {
		let selectedIndex = this.PageNumber() - 1;
		let startIndex = selectedIndex * this.GamesPageSize;

		return this.Games.slice(startIndex, startIndex + this.GamesPageSize);
	}

	public Games: PlayedGame[];

	public GamesPageSize: number = 8;
	public PageNumber: ko.Observable<number>;
	public GamesPage: ko.Computed<PlayedGame[]>;
	public TotalGamesCount: ko.Computed<number>;
}

ko.components.register(ComponentName, {
	viewModel: PlayedGameListModel,
	template: `
		<div>
			<div class="${margin.bottomHalf}">
				<div class="${layout.inlineBlock} ${layout.width70}"></div>
				<div class="${layout.inlineBlock} ${layout.width15}"></div>
				<div class="${layout.inlineBlock} ${text.center} ${layout.width15}">Waves</div>
			</div>

			<ul class="${background.bgAlternateDarken}">
				<!-- ko foreach: $component.GamesPage() -->
				<li class="${padding.vertical} ${events.clickable}" data-bind="click: $component.OnGameSelect">
					<div class="${layout.inlineBlock} ${text.font12} ${text.left} ${layout.width70}">
						<!-- ko text: Map --><!-- /ko -->
						&nbsp;
						<span class="${textColor.gray} ${margin.left}" data-bind="${MomentFormat("TimeStarted", "MMMM Do, YYYY")}" />
					</div>

					<div class="${layout.inlineBlock} ${text.font10} ${text.center} ${layout.width15}" data-bind="text: Difficulty" />

					<div class="${layout.inlineBlock} ${text.font14} ${text.center} ${layout.width15}">
						<!-- ko text: ReachedWave --><!-- /ko -->&nbsp;/&nbsp;<!-- ko text: TotalWaves --><!-- /ko -->
					</div>
				</li>
				<!-- /ko -->

				<!-- ko foreach: new Array($component.GamesPageSize - $component.GamesPage().length) -->
				<li class="${padding.vertical}">
					<div class="${layout.inlineBlock} ${text.font12} ${text.left} ${layout.width70}">&nbsp;</div>
					<div class="${layout.inlineBlock} ${text.font10} ${layout.width15}">&nbsp;</div>
					<div class="${layout.inlineBlock} ${text.font14} ${layout.width15}">&nbsp;</div>
				</li>
				<!-- /ko -->
			</ul>

			<div data-bind="${PaginationComponent("$component.PageNumber", "$component.TotalGamesCount", "$component.GamesPageSize")}" />
		</div>`,
});
