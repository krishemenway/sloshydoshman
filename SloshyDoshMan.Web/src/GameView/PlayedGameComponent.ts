import {ResultOf} from "CommonDataStructures/ResultOf";
import {ScoreboardPlayer, GameViewModel} from "Server";
import {GoToView} from "App";
import {Observable} from "knockout";
import * as HashChange from "KnockoutHelpers/HashchangeExtender";
import * as MomentFormatDate from "KnockoutHelpers/MomentFormatDateBindingHandler";
import * as PlayerView from "PlayerView/PlayerProfileComponent";
import * as ko from "knockout";
import * as $ from "jquery";

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

export var Name : string = "PlayedGame";
ko.components.register(Name, {
	viewModel: PlayedGameViewModel,
	template: `
		<div class="center-layout-1000" data-bind="with: Game">
			<div class="flex-row margin-bottom">
				<div class="red-handle-container flex-fill-remaining margin-right-half text-white">
					<div class="gray-9f font-30 bold margin-bottom" data-bind="text: PlayedGame.Map" />

					<div class="margin-bottom-half">
						<!-- ko if: PlayedGame.PlayersWon -->Boss Defeated!<!-- /ko -->
						<!-- ko ifnot: PlayedGame.PlayersWon -->Wave <!-- ko text: PlayedGame.ReachedWave --><!-- /ko --><!-- /ko -->
					</div>

					<div class="margin-bottom"><!-- ko text: Scoreboard.TotalKills --><!-- /ko --> Total Kills</div>

					<div class="margin-bottom">Difficulty: <!-- ko text: PlayedGame.Difficulty --><!-- /ko --></div>

					<div class="margin-bottom-half" data-bind="${MomentFormatDate.DataBind("PlayedGame.TimeStarted", "dddd MMMM Do, YYYY")}" />

					<div class="margin-bottom">
						<!-- ko ${MomentFormatDate.DataBind("PlayedGame.TimeStarted", "hh:mm:ss A")} --><!-- /ko -->
						&nbsp;&ndash;&nbsp;
						<!-- ko if: PlayedGame.TimeFinished --><!-- ko ${MomentFormatDate.DataBind("PlayedGame.TimeFinished", "hh:mm:ss A")} --><!-- /ko --><!-- /ko -->
						<!-- ko ifnot: PlayedGame.TimeFinished -->Maybe soon?<!-- /ko -->
					</div>
				</div>

				<div class="red-handle-container flex-wrap-content margin-left-half">
					<div class="map-cover" data-bind="css: PlayedGame.Map">
						<!-- ko if: PlayedGame.PlayersWon --><div class="boss-defeated-banner">Boss Defeated!</div><!-- /ko -->
					</div>
				</div>
			</div>

			<div class="game-scoreboard red-handle-container margin-bottom text-white">
				<table class="font-12 block-center">
					<thead>
						<tr>
							<th class="inset-text gray-9f to-lower font-48 bold">Kills</th>
							<!-- ko foreach: new Array(PlayedGame.TotalWaves+1) -->
								<!-- ko if: $index()+1 <= $parent.PlayedGame.TotalWaves -->
									<th class="scoreboard-wave-header"><div><span class="padding-vertical-half padding-horizontal">Wave <!-- ko text: $index()+1 --><!-- /ko --></span></div></th>
								<!-- /ko -->
								<!-- ko if: $index()+1 > $parent.PlayedGame.TotalWaves -->
									<th class="scoreboard-wave-header"><div><span class="padding-vertical-half padding-horizontal">Boss</span></div></th>
								<!-- /ko -->
							<!-- /ko -->
						</tr>
					</thead>
					<tbody data-bind="foreach: {data: Scoreboard.Players, as: 'player'}">
						<tr>
							<td class="clickable padding-half text-left" data-bind="click: $component.SelectPlayer, html: UserName"></td>

							<!-- ko foreach: new Array($component.Game().PlayedGame.TotalWaves+1) -->
							<td class="padding-half text-center" data-bind="with: player.PlayerWaveInfo[$index()+1]">
								<div class="perk-icon width-32 block-center" data-bind="css: Perk, attr: {title: Perk}" />
								<div class="wave-kill-count text-center" data-bind="text: Kills" />
							</td>
							<!-- /ko -->
						</tr>
					</tbody>
				</table>
			</div>
		</div>`,
});