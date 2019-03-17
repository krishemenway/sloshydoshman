import {PlayedGame} from "Server";
import {GoToView} from "App";
import {Observable, Computed} from "knockout";
import * as Pagination from "Pagination/PaginationComponent";
import * as PlayedGameComponent from "GameView/PlayedGameComponent";
import * as MomentFormatDate from "KnockoutHelpers/MomentFormatDateBindingHandler";
import * as ko from "knockout";

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
		GoToView(PlayedGameComponent.Name, {PlayedGameId: game.PlayedGameId});
	}

	private GetGamesPage = () => {
		let selectedIndex = this.PageNumber() - 1;
		let startIndex = selectedIndex * this.GamesPageSize;

		return this.Games.slice(startIndex, startIndex + this.GamesPageSize);
	}

	public Games: PlayedGame[];

	public GamesPageSize: number = 8;
	public PageNumber: Observable<number>;
	public GamesPage: Computed<PlayedGame[]>;
	public TotalGamesCount: Computed<number>;
}

export var ComponentName : string = "PlayerGameList";
ko.components.register(ComponentName, {
	viewModel: PlayedGameListModel,
	template: `
		<div>
			<div class="margin-bottom-half">
				<div class="inline-block" style="width: 70%"></div>
				<div class="inline-block" style="width: 15%"></div>
				<div class="inline-block text-center" style="width: 15%">Waves</div>
			</div>

			<ul class="bg-alternating-colors">
				<!-- ko foreach: $component.GamesPage() -->
				<li class="padding-vertical clickable" data-bind="click: $component.OnGameSelect">
					<div class="inline-block font-12 text-left" style="width: 70%">
						<!-- ko text: Map --><!-- /ko -->&nbsp;<span class="text-gray margin-left" data-bind="${MomentFormatDate.DataBind("TimeStarted", "MMMM Do, YYYY")}"></span>
					</div>
					<div class="inline-block font-10 text-center" style="width: 15%" data-bind="text: Difficulty"></div>
					<div class="inline-block font-14 text-center" style="width: 15%"><!-- ko text: ReachedWave --><!-- /ko -->&nbsp;/&nbsp;<!-- ko text: TotalWaves --><!-- /ko --></div>
				</li>
				<!-- /ko -->

				<!-- ko foreach: new Array($component.GamesPageSize - $component.GamesPage().length) -->
				<li class="padding-vertical">
					<div style="width: 70%" class="inline-block font-12 text-left">&nbsp;</div>
					<div style="width: 15%" class="inline-block font-10">&nbsp;</div>
					<div style="width: 15%" class="inline-block font-14">&nbsp;</div>
				</li>
				<!-- /ko -->
			</ul>

			<div data-bind="component: {name: '${Pagination.ComponentName}', params: {SelectedPage: $component.PageNumber, PageSize: $component.GamesPageSize, TotalItemCount: $component.TotalGamesCount}}" />
		</div>`,
});
