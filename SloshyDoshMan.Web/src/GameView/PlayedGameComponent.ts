import * as ko from "knockout";
import * as $ from "jquery";
import { layout, margin, textColor, text, createStyles, redHandleContainer } from "AppStyles";
import { map } from "Maps/MapStyles";
import { ResultOf } from "CommonDataStructures/ResultOf";
import { GameViewModel } from "Server";
import { GoToView } from "App";
import { CreateHashChangeObservable } from "KnockoutHelpers/HashchangeExtender";
import { MomentFormat } from "KnockoutHelpers/MomentFormatDateBindingHandler";
import { GameScoreboardComponent } from "./GameScoreboardComponent";

var PlayedGameName : string = "PlayedGame";
export function PlayedGameComponent() {
	return `component: {name: '${PlayedGameName}'}`;
}

export function GoToPlayedGame(playedGameId: string) {
	GoToView(PlayedGameName, { "PlayedGameId": playedGameId });
}

class PlayedGameViewModel {
	constructor() {
		this.PlayedGameId = CreateHashChangeObservable("PlayedGameId", "");
		this.Game = ko.observable(null);
		this.InitializeGame();
	}

	private InitializeGame = () => {
		$.get(`/webapi/games/profile?playedGameId=${this.PlayedGameId()}`).done((response: ResultOf<GameViewModel>) => this.Game(response.Data));
	}

	public Game: ko.Observable<GameViewModel|null>;
	public PlayedGameId: ko.Observable<string>;
}

const styles = createStyles({
	bossDefeatedBanner: {
		position: "absolute",
		top: "42px",
		right: "-78px",
		transform: "rotate(35deg)",
		backgroundColor: "rgb(125,89,0)",
		width: "320px",
		height: "40px",
		lineHeight: "40px",
		textAlign: "center",

		boxShadow: "0 7px 6px -6px #000",

		fontWeight: "bold",
		fontSize: "20px",

		textShadow: "1px 1px 2px black",
	},
}).attach().classes;

ko.components.register(PlayedGameName, {
	viewModel: PlayedGameViewModel,
	template: `
		<div class="${layout.centerLayout1000}" data-bind="with: Game">
			<div class="${layout.flexRow} ${margin.bottom}">
				<div class="${redHandleContainer.container} ${layout.flexFillRemaining} ${margin.rightHalf} ${textColor.white}">
					<div class="${textColor.gray9f} ${text.font30} ${text.bold} ${margin.bottom}" data-bind="text: PlayedGame.Map" />

					<div class="${margin.bottomHalf}">
						<!-- ko text: PlayedGame.Difficulty --><!-- /ko -->
						&nbsp;&mdash;&nbsp;
						<!-- ko if: PlayedGame.PlayersWon -->Boss Defeated!<!-- /ko -->
						<!-- ko ifnot: PlayedGame.PlayersWon -->Reached Wave <!-- ko text: PlayedGame.ReachedWave --><!-- /ko --><!-- /ko -->
					</div>

					<div class="${margin.bottom}"><!-- ko text: Scoreboard.TotalKills --><!-- /ko --> Total Kills</div>

					<div class="${margin.bottom}"></div>

					<div class="${margin.bottomHalf}" data-bind="${MomentFormat("PlayedGame.TimeStarted", "dddd MMMM Do, YYYY")}" />

					<div class="${margin.bottom}">
						<!-- ko ${MomentFormat("PlayedGame.TimeStarted", "hh:mm:ss A")} --><!-- /ko -->
						&nbsp;&ndash;&nbsp;
						<!-- ko if: PlayedGame.TimeFinished --><!-- ko ${MomentFormat("PlayedGame.TimeFinished", "hh:mm:ss A")} --><!-- /ko --><!-- /ko -->
						<!-- ko ifnot: PlayedGame.TimeFinished -->Maybe soon?<!-- /ko -->
					</div>
				</div>

				<div class="${redHandleContainer.container} ${layout.flexWrapContent} ${margin.leftHalf}">
					<div class="${map.mapCover}" style="width: 480px; height: 240px" data-bind="css: PlayedGame.Map">
						<!-- ko if: PlayedGame.PlayersWon --><div class="${styles.bossDefeatedBanner}">Boss Defeated!</div><!-- /ko -->
					</div>
				</div>
			</div>

			<div data-bind="${GameScoreboardComponent("Scoreboard", "PlayedGame.TotalWaves")}" />
		</div>`,
});