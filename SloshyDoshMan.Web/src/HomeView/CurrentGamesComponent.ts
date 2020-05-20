import * as ko from "knockout";
import * as $ from "jquery";
import { layout, text, textColor, margin, padding, events, createStyles, redHandleContainer, marginMobile, background } from "AppStyles";
import { GameViewModel, CurrentGamesResponse, ScoreboardPlayer } from "Server";
import { GoToPlayedGame } from "GameView/PlayedGameComponent";
import { MomentFormat } from "KnockoutHelpers/MomentFormatDateBindingHandler";
import { GoToPlayerProfile } from "PlayerView/PlayerProfileComponent";
import { PerkIconComponent } from "Perks/PerkIconComponent";

var Name : string = "CurrentGames";
export function CurrentGamesComponent() {
	return `component: {name: '${Name}'}`;
}

export class CurrentGamesViewModel {
	constructor(params?: any) {
		this.CurrentGames = ko.observableArray();
		this.SelectedGame = ko.observable(null);
		this.SelectedGamePlayers = ko.pureComputed(this.FindCurrentGamePlayers, this);
		this.RotateSelectedGameInterval = window.setInterval(this.SelectNextGame, 10000);

		this.LoadServerStats();
	}

	public dipose() {
		this.ClearGameRotation();
	}

	public GoToGame = (game: GameViewModel) => {
		GoToPlayedGame(game.PlayedGame.PlayedGameId);
	}

	public OnHover = (game: GameViewModel) : void => {
		this.SelectedGame(game);
		this.ClearGameRotation();
	}

	public FindPerkForCurrentGame = (player: ScoreboardPlayer) : string|null => {
		let selectedGame = this.SelectedGame();

		if (selectedGame === null) {
			return null;
		}

		let mostRecentWaveForPlayer = player.PlayerWaveInfo[selectedGame.PlayedGame.ReachedWave];

		if(!mostRecentWaveForPlayer) {
			return null;
		}

		return mostRecentWaveForPlayer.Perk;
	}

	public GoToPlayer = (player: ScoreboardPlayer) : void => {
		GoToPlayerProfile(player.SteamId);
	}

	private SelectNextGame = () : void => {
		let selectedGame = this.SelectedGame();
		if (selectedGame === null) {
			return;
		}

		let currentIndex = this.CurrentGames().indexOf(selectedGame);

		if(currentIndex < 0) {
			return;
		}
		
		let nextGame = currentIndex + 1 >= this.CurrentGames().length ? this.CurrentGames()[0] : this.CurrentGames()[currentIndex+1];
		this.SelectedGame(nextGame);
	}

	private ClearGameRotation = () : void => {
		if(!!this.RotateSelectedGameInterval) {
			window.clearInterval(this.RotateSelectedGameInterval);
			this.RotateSelectedGameInterval = 0;
		}
	}

	private LoadServerStats = () : void => {
		$.get(`/webapi/games/currentGames`).done((response: CurrentGamesResponse) => {
			this.CurrentGames(response.CurrentGames.slice(0, 4));
			this.SelectedGame(this.CurrentGames()[0]);
		});
	}

	private FindCurrentGamePlayers = () : ScoreboardPlayer[] => {
		let selectedGame = this.SelectedGame();
		if (selectedGame === null) {
			return [];
		}

		return selectedGame.Scoreboard.Players.filter((player) => {
			if (selectedGame === null || !player.PlayerWaveInfo[selectedGame.PlayedGame.ReachedWave]) {
				return false;
			}

			return !!player.PlayerWaveInfo[selectedGame.PlayedGame.ReachedWave].Perk;
		});
	}

	public CurrentGames: ko.ObservableArray<GameViewModel>;
	public SelectedGame: ko.Observable<GameViewModel|null>;
	public SelectedGamePlayers: ko.Computed<ScoreboardPlayer[]>;

	private RotateSelectedGameInterval: number;
}

const styles = createStyles({
	player: {
		border: "1px solid #282828",
		borderTopWidth: "5px",
		backgroundColor: "rgba(0,0,0,.5)",
	},

	playedGame: {
		border: "1px solid transparent",
		cursor: "pointer",

		"&.selected": {
			borderColor: "#282828",
			backgroundColor: "rgba(0,0,0,.5)",
		},
	},
}).attach().classes;

ko.components.register(Name, {
	viewModel: CurrentGamesViewModel,
	template: `
		<!-- ko if: CurrentGames().length > 0 -->
		<div class="${marginMobile.horizontal}">
			<div class="${text.font24} ${textColor.white} ${text.smallCaps} ${text.center} ${margin.topHalf} ${margin.bottomHalf}">Current Games</div>

			<div class="${redHandleContainer.container}">
				<div data-bind="foreach: CurrentGames">
					<div class="${layout.width25} ${layout.inlineBlock} ${styles.playedGame}" data-bind="click: $component.GoToGame, event: {mouseover: $component.OnHover}, css: {selected: $component.SelectedGame() === $data}">
						<div class="${background.cover} ${margin.bottom}" style="min-height: 119px" data-bind="style: { backgroundImage: 'url(\\'/Maps/'+PlayedGame.Map+'-480x240\\')' }" />
						<div class="${textColor.white} ${text.font14} ${text.center} ${margin.bottomHalf}" data-bind="text: PlayedGame.Map" />
						<div class="${textColor.gray} ${text.font14} ${text.center} ${margin.bottom}" data-bind="${MomentFormat("PlayedGame.TimeStarted", "MMM Do YYYY")}" />
					</div>
				</div>

				<div class="${layout.flexRow}">
					<!-- ko foreach: SelectedGamePlayers -->
					<div class="${layout.flexEvenDistribution} ${textColor.white} ${text.center} ${styles.player} ${padding.all} ${events.clickable}" data-bind="click: $component.GoToPlayer">
						<!-- ko ${PerkIconComponent("$component.FindPerkForCurrentGame($data)")} --><!-- /ko -->
						<div class="${text.font12} ${text.center}" data-bind="html: UserName" />
					</div>
					<!-- /ko -->
				</div>
			</div>
		</div>
		<!-- /ko -->`,
});