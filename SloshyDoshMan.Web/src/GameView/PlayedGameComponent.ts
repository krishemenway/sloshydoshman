import {ResultOf} from "CommonDataStructures/ResultOf";
import {ScoreboardPlayer, GameViewModel} from "Server";
import {GoToView} from "App";
import {Observable} from "knockout";
import * as HashChange from "KnockoutHelpers/HashchangeExtender";
import * as MomentFormatDate from "KnockoutHelpers/MomentFormatDateBindingHandler";
import * as PlayerView from "PlayerView/PlayerProfileComponent";
import * as ko from "knockout";
import * as $ from "jquery";
import { layout, margin, textColor, text, padding, events, createStyles, redHandleContainer } from "AppStyles";
import { perk } from "Perks/PerkStyles";
import { map } from "Maps/MapStyles";

class PlayedGameViewModel {
	constructor() {
		this.PlayedGameId = HashChange.CreateObservable("PlayedGameId", "");
		this.Game = ko.observable(null);
		this.InitializeGame();
	}

	public SelectPlayer(player: ScoreboardPlayer) {
		GoToView(PlayerView.Name, {SteamId: player.SteamId});
	}

	private InitializeGame = () => {
		$.get(`/webapi/games/profile?playedGameId=${this.PlayedGameId()}`).done((response: ResultOf<GameViewModel>) => this.Game(response.Data));
	}

	public Game: Observable<GameViewModel|null>;
	public PlayedGameId: Observable<string>;
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

	gameScoreboard: {
		"& table": {
			borderCollapse: "collapse",
			width: "initial",
		},
	
		"& th, & td": {
			width: "69px",
		},
	
		"& th": {
			"& > div": {
				transform: "translate(50px, -3px) rotate(315deg)",
			},
	
			"& > div > span": {
				borderBottom: "1px solid #383838",
			},
		},
	
		"& td": {
			border: "1px solid #383838",
	
			"&:first-child": {
				width: "150px",
			},
		},
	},
}).attach().classes;

export var Name : string = "PlayedGame";
ko.components.register(Name, {
	viewModel: PlayedGameViewModel,
	template: `
		<div class="${layout.centerLayout1000}" data-bind="with: Game">
			<div class="${layout.flexRow} ${margin.bottom}">
				<div class="${redHandleContainer.container} ${layout.flexFillRemaining} ${margin.rightHalf} ${textColor.white}">
					<div class="${textColor.gray9f} ${text.font30} ${text.bold} ${margin.bottom}" data-bind="text: PlayedGame.Map" />

					<div class="${margin.bottomHalf}">
						<!-- ko if: PlayedGame.PlayersWon -->Boss Defeated!<!-- /ko -->
						<!-- ko ifnot: PlayedGame.PlayersWon -->Wave <!-- ko text: PlayedGame.ReachedWave --><!-- /ko --><!-- /ko -->
					</div>

					<div class="${margin.bottom}"><!-- ko text: Scoreboard.TotalKills --><!-- /ko --> Total Kills</div>

					<div class="${margin.bottom}">Difficulty: <!-- ko text: PlayedGame.Difficulty --><!-- /ko --></div>

					<div class="${margin.bottomHalf}" data-bind="${MomentFormatDate.DataBind("PlayedGame.TimeStarted", "dddd MMMM Do, YYYY")}" />

					<div class="${margin.bottom}">
						<!-- ko ${MomentFormatDate.DataBind("PlayedGame.TimeStarted", "hh:mm:ss A")} --><!-- /ko -->
						&nbsp;&ndash;&nbsp;
						<!-- ko if: PlayedGame.TimeFinished --><!-- ko ${MomentFormatDate.DataBind("PlayedGame.TimeFinished", "hh:mm:ss A")} --><!-- /ko --><!-- /ko -->
						<!-- ko ifnot: PlayedGame.TimeFinished -->Maybe soon?<!-- /ko -->
					</div>
				</div>

				<div class="${redHandleContainer.container} ${layout.flexWrapContent} ${margin.leftHalf}">
					<div class="${map.mapCover}" style="width: 480px; height: 240px" data-bind="css: PlayedGame.Map">
						<!-- ko if: PlayedGame.PlayersWon --><div class="${styles.bossDefeatedBanner}">Boss Defeated!</div><!-- /ko -->
					</div>
				</div>
			</div>

			<div class="${styles.gameScoreboard} ${redHandleContainer.container} ${margin.bottom} ${textColor.white}">
				<table class="${text.font12} ${layout.blockCenter}">
					<thead>
						<tr>
							<th class="${text.inset} ${textColor.gray9f} ${text.toLower} ${text.font48} ${text.bold}">Kills</th>
							<!-- ko foreach: new Array(PlayedGame.TotalWaves+1) -->
								<!-- ko if: $index()+1 <= $parent.PlayedGame.TotalWaves -->
									<th><div><span class="${padding.verticalHalf} ${padding.horizontal}">Wave <!-- ko text: $index()+1 --><!-- /ko --></span></div></th>
								<!-- /ko -->
								<!-- ko if: $index()+1 > $parent.PlayedGame.TotalWaves -->
									<th><div><span class="${padding.verticalHalf} ${padding.horizontal}">Boss</span></div></th>
								<!-- /ko -->
							<!-- /ko -->
						</tr>
					</thead>
					<tbody data-bind="foreach: {data: Scoreboard.Players, as: 'player'}">
						<tr>
							<td class="${events.clickable} ${padding.half} ${text.left}" data-bind="click: $component.SelectPlayer, html: UserName"></td>

							<!-- ko foreach: new Array($component.Game().PlayedGame.TotalWaves+1) -->
							<td class="${padding.half} ${text.center}" data-bind="with: player.PlayerWaveInfo[$index()+1]">
								<div class="${perk.perkIcon} ${perk.width32} ${layout.blockCenter}" data-bind="css: Perk, attr: {title: Perk}" />
								<div class="${text.center}" data-bind="text: Kills" />
							</td>
							<!-- /ko -->
						</tr>
					</tbody>
				</table>
			</div>
		</div>`,
});