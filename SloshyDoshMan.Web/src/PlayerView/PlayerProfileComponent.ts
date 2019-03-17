import { ResultOf } from "CommonDataStructures/ResultOf";
import { PlayerViewModel } from "Server";
import { Observable, Subscription } from "knockout";
import * as HashChange from "../KnockoutHelpers/HashchangeExtender";
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import * as PlayerPerkStatistics from "PlayerView/PlayerPerkStatisticsComponent";
import * as PlayerMapStatistics from "PlayerView/PlayerMapStatisticsComponent";
import * as PlayedGameList from "PlayerView/PlayedGameListComponent";
import * as ko from "knockout";

class PlayerProfileViewModel {
	constructor() {
		this.PlayerViewModel = ko.observable(null);
		this.SteamId = HashChange.CreateObservable("SteamId", "");
		this.SteamIdSubscription = this.SteamId.subscribe(() => this.InitializePlayer());
		this.InitializePlayer();
	}

	public dispose() {
		this.SteamIdSubscription.dispose();
	}

	public OnMapSelected = (map: string) => {
		console.log(map);
	}

	private InitializePlayer = () => {
		$.get(`/webapi/players/profile?steamId=${this.SteamId()}`).done((response: ResultOf<PlayerViewModel>) => this.PlayerViewModel(response.Data));
	}

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

					<div data-bind="component: {name: '${PlayedGameList.ComponentName}', params: {Games: AllGames}}"></div>
				</div>

				<div class="text-white text-center margin-bottom">
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