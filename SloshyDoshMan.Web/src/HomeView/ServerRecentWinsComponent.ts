import * as ko from "knockout";
import * as $ from "jquery";
import { layout, text, textColor, margin, padding, events, createStyles, redHandleContainer, marginMobile, background } from "AppStyles";
import { GameViewModel, RecentWinsResponse, ScoreboardPlayer } from "Server";
import { GoToPlayedGame } from "GameView/PlayedGameComponent";
import { MomentFormat } from "KnockoutHelpers/MomentFormatDateBindingHandler";
import { GoToPlayerProfile } from "PlayerView/PlayerProfileComponent";
import { PerkIconComponent } from "Perks/PerkIconComponent";

var Name : string = "ServerRecentWins";
export function ServerRecentWinsComponent() {
	return `component: {name: '${Name}'}`;
}

export class ServerRecentWinsViewModel {
	constructor(params?: any) {
		this.RecentWins = ko.observableArray();
		this.SelectedRecentWin = ko.observable(null);
		this.RecentWinPlayers = ko.pureComputed(this.FindRecentWinPlayers, this);
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
		this.SelectedRecentWin(game);
		this.ClearGameRotation();
	}

	public FindPerkForRecentWin = (player: ScoreboardPlayer) : string|null => {
		let recentWin = this.SelectedRecentWin();

		if (recentWin === null) {
			return null;
		}

		let lastWaveForPlayer = player.PlayerWaveInfo[recentWin.PlayedGame.TotalWaves+1];

		if(!lastWaveForPlayer) {
			return null;
		}

		return lastWaveForPlayer.Perk;
	}

	public GoToPlayer = (player: ScoreboardPlayer) : void => {
		GoToPlayerProfile(player.SteamId);
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

	private LoadServerStats = () : void => {
		$.get(`/webapi/games/recentWins`).done((response: RecentWinsResponse) => {
			this.RecentWins(response.RecentWins.slice(0, 4));
			this.SelectedRecentWin(this.RecentWins()[0]);
		});
	}

	private FindRecentWinPlayers = () : ScoreboardPlayer[] => {
		let recentWin = this.SelectedRecentWin();
		if (recentWin === null) {
			return [];
		}

		return recentWin.Scoreboard.Players.filter((player) => {
			if (recentWin === null || !player.PlayerWaveInfo[recentWin.PlayedGame.TotalWaves+1]) {
				return false;
			}

			return !!player.PlayerWaveInfo[recentWin.PlayedGame.TotalWaves+1].Perk;
		});
	}

	public RecentWins: ko.ObservableArray<GameViewModel>;
	public SelectedRecentWin: ko.Observable<GameViewModel|null>;
	public RecentWinPlayers: ko.Computed<ScoreboardPlayer[]>;

	private RotateSelectedGameInterval: number;
}

const styles = createStyles({
	recentWinPlayer: {
		border: "1px solid #282828",
		borderTopWidth: "5px",
		backgroundColor: "rgba(0,0,0,.5)",
	},

	recentWinGame: {
		border: "1px solid transparent",
		cursor: "pointer",

		"&.selected": {
			borderColor: "#282828",
			backgroundColor: "rgba(0,0,0,.5)",
		},
	},
}).attach().classes;

ko.components.register(Name, {
	viewModel: ServerRecentWinsViewModel,
	template: `
		<div class="${marginMobile.horizontal}">
			<div class="${text.font24} ${textColor.white} ${text.smallCaps} ${text.center} ${margin.topHalf} ${margin.bottomHalf}">Recent Wins</div>

			<div class="${redHandleContainer.container}">
				<div data-bind="foreach: RecentWins">
					<div class="${layout.width25} ${layout.inlineBlock} ${styles.recentWinGame}" data-bind="click: $component.GoToGame, event: {mouseover: $component.OnHover}, css: {selected: $component.SelectedRecentWin() === $data}">
						<div class="${background.cover} ${margin.bottom}" style="min-height: 119px" data-bind="style: { backgroundImage: 'url(\\'/Maps/'+PlayedGame.Map+'-480x240\\')' }" />
						<div class="${textColor.white} ${text.font14} ${text.center} ${margin.bottomHalf}" data-bind="text: PlayedGame.Map" />
						<div class="${textColor.gray} ${text.font14} ${text.center} ${margin.bottom}" data-bind="${MomentFormat("PlayedGame.TimeFinished", "MMM Do YYYY")}" />
					</div>
				</div>

				<div class="${layout.flexRow}">
					<!-- ko foreach: RecentWinPlayers -->
					<div class="${layout.flexEvenDistribution} ${textColor.white} ${text.center} ${styles.recentWinPlayer} ${padding.all} ${events.clickable}" data-bind="click: $component.GoToPlayer">
						<!-- ko ${PerkIconComponent("$component.FindPerkForRecentWin($data)")} --><!-- /ko -->
						<div class="${text.font12} ${text.center}" data-bind="html: UserName" />
					</div>
					<!-- /ko -->
				</div>
			</div>
		</div>`,
});