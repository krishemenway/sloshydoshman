import {Dictionary} from "CommonDataStructures/Dictionary";
import {ResultOf} from "CommonDataStructures/ResultOf";
import {GameViewModel,PlayedGame,RecentGameResponse,ScoreboardPlayer} from "Server";
import {GoToView} from "App";
import * as PlayerView from "PlayerView/PlayerProfileComponent";
import * as GameView from "GameView/PlayedGameComponent";
import * as ko from "knockout";
import * as $ from "jquery";

export var Name : string = "ServerRecentWins";

export class ServerRecentWinsViewModel {
	constructor(params?: any) {
		this.RecentWins = ko.observableArray([]);
		this.SelectedRecentWin = ko.observable(null);
		this.GamesByPlayedGameId = {};
		this.RecentWinPlayers = ko.pureComputed(this.FindRecentWinPlayers, this);
		this.RotateSelectedGameInterval = window.setInterval(this.SelectNextGame, 10000);

		this.LoadServerStats(150);
	}

	public dipose() {
		this.ClearGameRotation();
	}

	public GoToGame = (game: PlayedGame) => {
		GoToView(GameView.ComponentName, {"PlayedGameId": game.PlayedGameId});
	}

	public FindOrCreatePlayerGameData = (game: PlayedGame) : KnockoutObservable<GameViewModel|null> => {
		if(!this.GamesByPlayedGameId[game.PlayedGameId]) {
			this.GamesByPlayedGameId[game.PlayedGameId] = ko.observable(null);
		}

		return this.GamesByPlayedGameId[game.PlayedGameId];
	}

	public OnHover = (game: PlayedGame) : void => {
		this.SelectedRecentWin(game);
		this.ClearGameRotation();
	}

	public FindPerkForRecentWin = (player: ScoreboardPlayer) : string|null => {
		let recentWin = this.SelectedRecentWin();

		if (recentWin === null) {
			return null;
		}

		let lastWaveForPlayer = player.PlayerWaveInfo[recentWin.TotalWaves+1];

		if(!lastWaveForPlayer) {
			return null;
		}

		return lastWaveForPlayer.Perk;
	}

	public GoToPlayer = (player: ScoreboardPlayer) : void => {
		GoToView(PlayerView.Name, {SteamId: player.SteamId});
	}

	private SelectNextGame = () : void => {
		let recentWin = this.SelectedRecentWin();
		if (recentWin === null) {
			return;
		}

		let currentIndex = this.RecentWins().indexOf(recentWin);

		if(currentIndex < 0) {
			return;
		}
		
		let nextGame = currentIndex + 1 >= this.RecentWins().length ? this.RecentWins()[0] : this.RecentWins()[currentIndex+1];
		this.SelectedRecentWin(nextGame);
	}

	private ClearGameRotation = () : void => {
		if(!!this.RotateSelectedGameInterval) {
			window.clearInterval(this.RotateSelectedGameInterval);
			this.RotateSelectedGameInterval = 0;
		}
	}

	private LoadServerStats = (gamesToSearch: number) : void => {
		$.get(`/webapi/games/recent?count=${gamesToSearch}&startingAt=0`).done((response: ResultOf<RecentGameResponse>) => {
			this.RecentWins(response.Data.RecentGames.filter((playedGame) => playedGame.PlayersWon).slice(0, 4));

			if(this.RecentWins().length < 4) {
				this.LoadServerStats(gamesToSearch+100);
			} else {
				this.SelectedRecentWin(this.RecentWins()[0]);
				this.LoadPlayerDataForRecentWins();
			}
		});
	}

	private LoadPlayerDataForRecentWins = () : void => {
		this.RecentWins().forEach(playedGame => {
			$.get(`/webapi/games/profile?playedGameId=${playedGame.PlayedGameId}`)
			 .done((response: ResultOf<GameViewModel>) => this.FindOrCreatePlayerGameData(playedGame)(response.Data));
		});
	}

	private FindRecentWinPlayers = () : ScoreboardPlayer[] => {
		let recentWin = this.SelectedRecentWin();
		if (recentWin === null) {
			return [];
		}

		let selectedWinGame = this.FindOrCreatePlayerGameData(recentWin)();
		if (selectedWinGame === null) {
			return [];
		}

		return selectedWinGame.Scoreboard.Players.filter((player) => {
			if (recentWin === null || !player.PlayerWaveInfo[recentWin.TotalWaves+1]) {
				return false;
			}

			return !!player.PlayerWaveInfo[recentWin.TotalWaves+1].Perk;
		});
	}

	public RecentWins: KnockoutObservableArray<PlayedGame>;
	public SelectedRecentWin: KnockoutObservable<PlayedGame|null>;
	public RecentWinPlayers: KnockoutComputed<ScoreboardPlayer[]>;

	private RotateSelectedGameInterval: number;
	private GamesByPlayedGameId: Dictionary<KnockoutObservable<GameViewModel|null>>;
}

ko.components.register(Name, {
	viewModel: ServerRecentWinsViewModel,
	template: `
		<div class="red-handle-container">
			<div class="font-24 text-white small-caps">Recent Wins</div>
			<div class="flex-row padding-bottom" data-bind="foreach: RecentWins">
				<div class="flex-even-distribution recent-win-game" data-bind="click: $component.GoToGame, event: {mouseover: $component.OnHover}, css: {selected: $component.SelectedRecentWin() === $data}">
					<div class="map-cover margin-bottom" data-bind="css: Map" style="width: 238px; height: 119px;" />
					<div class="text-white font-14 text-center margin-bottom-half" data-bind="text: Map" />
					<div class="text-gray font-14 text-center margin-bottom" data-bind="momentFormatDate: {Format: 'MMM Do YYYY', Date: TimeFinished}" />
				</div>
			</div>

			<div class="flex-row">
				<!-- ko foreach: RecentWinPlayers -->
				<div class="flex-even-distribution text-white text-center recent-win-player padding clickable" data-bind="click: $component.GoToPlayer" style="height: 90px; overflow: hidden;">
					<div class="perk-icon width-32" data-bind="css: $component.FindPerkForRecentWin($data)" />
					<div class="font-12" data-bind="html: UserName" />
				</div>
				<!-- /ko -->
			</div>
		</div>`,
});