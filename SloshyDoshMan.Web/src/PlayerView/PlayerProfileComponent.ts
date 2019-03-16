import {ResultOf} from "CommonDataStructures/ResultOf";
import {PlayedGame,PlayerViewModel} from "Server";
import {GoToView} from "App";
import {Observable, Computed, Subscription} from "knockout";
import * as HashChange from "../KnockoutHelpers/HashchangeExtender";
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import * as PlayerPerkStatistics from "PlayerView/PlayerPerkStatisticsComponent";
import * as PlayerMapStatistics from "PlayerView/PlayerMapStatisticsComponent";
import * as Pagination from "Pagination/PaginationComponent";
import * as GameView from "GameView/PlayedGameComponent";
import * as ko from "knockout";

class PlayerProfileViewModel {
	constructor() {
		this.PlayerViewModel = ko.observable(null);
		this.SteamId = HashChange.CreateObservable("SteamId", "");
		this.PageNumber = ko.observable(1);
		this.TotalGamesCount = ko.pureComputed(this.CalculateTotalGamesCount, this);
		this.RecentGamesPage = ko.pureComputed(this.GetRecentGamesPage, this);
		this.SteamIdSubscription = this.SteamId.subscribe(() => this.InitializePlayer());
		this.InitializePlayer();
	}

	public dispose() {
		this.SteamIdSubscription.dispose();
	}

	public SelectGame = (game: PlayedGame) => {
		GoToView(GameView.Name, {PlayedGameId: game.PlayedGameId});
	}

	public OnMapSelected = (map: string) => {
		console.log(map);
	}

	private GetRecentGamesPage = () => {
		let player = this.PlayerViewModel();
		if (player === null) {
			return [];
		}

		let selectedIndex = this.PageNumber()-1;
		let startIndex = selectedIndex*this.RecentGamesPageSize;
		return player.AllGames.slice(startIndex, startIndex + this.RecentGamesPageSize);
	}

	private CalculateTotalGamesCount = () => {
		let playerModel = this.PlayerViewModel();
		return playerModel !== null ? playerModel.AllGames.length : 0;
	}

	private InitializePlayer = () => {
		$.get(`/webapi/players/profile?steamId=${this.SteamId()}`).done((response: ResultOf<PlayerViewModel>) => this.PlayerViewModel(response.Data));
	}

	public RecentGamesPageSize: number = 8;
	public PageNumber: Observable<number>;
	public RecentGamesPage: Computed<PlayedGame[]>;
	public TotalGamesCount: Computed<number>;
	public PlayerViewModel: Observable<PlayerViewModel|null>;
	public SteamId: Observable<string>;
	private SteamIdSubscription: Subscription;
}

export var Name : string = "Player";
ko.components.register(Name, {
	viewModel: PlayerProfileViewModel,
	template: `
		<div class="stats-row flex-row" data-bind="with: PlayerViewModel">
			<div class="flex-even-distribution profile margin-right-half" style="max-width: 55%">
				<div class="red-handle-container margin-bottom">
					<div class="font-48 text-white bold" data-bind="html: UserName" />
					<div class="text-white">Steam ID: <span class="bold text-gray" data-bind="text: SteamId" /></div>
				</div>

				<div class="red-handle-container margin-bottom text-white">
					<div class="header text-center margin-bottom">recent games</div>

					<div class="recent-games-view" data-bind="foreach: ">
						<div class="recent-game"><!-- ko text: Map --><!-- /ko --></div>
					</div>

					<div class="margin-bottom-half">
						<div class="inline-block" style="width: 70%"></div>
						<div class="inline-block" style="width: 15%"></div>
						<div class="inline-block text-center" style="width: 15%">Waves</div>
					</div>

					<ul class="bg-alternating-colors">
						<!-- ko foreach: $component.RecentGamesPage() -->
						<li class="padding-vertical clickable" data-bind="click: $component.SelectGame">
							<div class="inline-block font-12 text-left" style="width: 70%"><!-- ko text: Map --><!-- /ko -->&nbsp;<span class="text-gray margin-left" data-bind="momentFormatDate: {Format: 'MMMM Do, YYYY', Date: TimeStarted}"></span></div>
							<div class="inline-block font-10 text-center" style="width: 15%" data-bind="text: Difficulty"></div>
							<div class="inline-block font-14 text-center" style="width: 15%"><!-- ko text: ReachedWave --><!-- /ko -->&nbsp;/&nbsp;<!-- ko text: TotalWaves --><!-- /ko --></div>
						</li>
						<!-- /ko -->

						<!-- ko foreach: new Array($component.RecentGamesPageSize - $component.RecentGamesPage().length) -->
						<li class="padding-vertical">
							<div style="width: 70%" class="inline-block font-12 text-left">&nbsp;</div>
							<div style="width: 15%" class="inline-block font-10">&nbsp;</div>
							<div style="width: 15%" class="inline-block font-14">&nbsp;</div>
						</li>
						<!-- /ko -->
					</ul>

					<div data-bind="component: {name: '${Pagination.ComponentName}', params: {SelectedPage: $component.PageNumber, PageSize: $component.RecentGamesPageSize, TotalItemCount: $component.TotalGamesCount}}" />
				</div>

				<div class="text-white text-center  margin-bottom">
					<div class="text-center width-50 inline-block padding-right-half">
						<div class="red-handle-container">
							<div class="font-48 font-light" data-bind="${NumberWithCommas.Name}: TotalKills" />
							<div class="font-14 text-gray">Total Kills</div>
						</div>
					</div>

					<div class="text-center width-50 inline-block padding-left-half">
						<div class="red-handle-container">
							<div class="font-48 font-light" data-bind="${NumberWithCommas.Name}: TotalGames" />
							<div class="font-14 text-gray">Total Games</div>
						</div>
					</div>
				</div>

				<div class="red-handle-container" data-bind="component: {name: '${PlayerPerkStatistics.ComponentName}', params: $component.PlayerViewModel}" />
			</div>

			<div class="flex-even-distribution red-handle-container map-stats margin-left-half text-white">
				<div class="header text-center margin-bottom-double">maps</div>
				<div data-bind="component: {name: '${PlayerMapStatistics.ComponentName}', params: {PlayerViewModel: $component.PlayerViewModel}}" />
			</div>
		</div>`,
});