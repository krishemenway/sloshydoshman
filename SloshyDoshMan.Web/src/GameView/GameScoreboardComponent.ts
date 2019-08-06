import * as ko from "knockout";
import { layout, margin, textColor, text, padding, events, createStyles, redHandleContainer } from "AppStyles";
import { perk } from "Perks/PerkStyles";
import { Scoreboard, ScoreboardPlayer } from "Server";
import { GoToPlayerProfile } from "PlayerView/PlayerProfileComponent";

var GameScoreboard : string = "GameScoreboard";
export function GameScoreboardComponent(scoreboard: string, totalWaves: string|number) {
	return `component: { name: '${GameScoreboard}', params: { Scoreboard: ${scoreboard}, TotalWaves: ${totalWaves} } }`;
}

interface GameScoreboardParams {
	Scoreboard: Scoreboard;
	TotalWaves: number;
}

class GameScoreboardViewModel {
	constructor(params: GameScoreboardParams) {
		this.Scoreboard = params.Scoreboard;
		this.TotalWaves = params.TotalWaves;
	}

	public SelectPlayer(player: ScoreboardPlayer) {
		GoToPlayerProfile(player.SteamId);
	}

	public Scoreboard: Scoreboard;
	public TotalWaves: number;
}

const styles = createStyles({
	gameScoreboard: {
		overflow: "hidden",

		"& table": {
			borderCollapse: "collapse",
			width: "initial",
		},
	
		"& th, & td": {
			width: "69px",
		},
	
		"& th > div": {
			transform: "translate(50px, -3px) rotate(315deg)",
			borderBottom: "1px solid #383838",
		},
	
		"& td": {
			border: "1px solid #383838",
	
			"&:first-child": {
				width: "150px",
			},
		},
	},
}).attach().classes;

ko.components.register(GameScoreboard, {
	viewModel: GameScoreboardViewModel,
	template: `
		<div class="${styles.gameScoreboard} ${redHandleContainer.container} ${margin.bottom} ${textColor.white}">
			<table class="${text.font12} ${layout.blockCenter}">
				<thead>
					<tr>
						<th class="${text.inset} ${textColor.gray9f} ${text.toLower} ${text.font48} ${text.bold}">Kills</th>

						<!-- ko foreach: new Array($component.TotalWaves) -->
						<th><div class="${padding.verticalHalf} ${padding.horizontalHalf}">Wave <!-- ko text: $index()+1 --><!-- /ko --></div></th>
						<!-- /ko -->

						<th><div class="${padding.verticalHalf} ${padding.horizontalHalf}">Boss</div></th>
					</tr>
				</thead>
				<tbody data-bind="foreach: {data: $component.Scoreboard.Players, as: 'player'}">
					<tr>
						<td class="${events.clickable} ${padding.half} ${text.left}" data-bind="click: $component.SelectPlayer, html: UserName"></td>

						<!-- ko foreach: new Array($component.TotalWaves+1) -->
						<td class="${padding.half} ${text.center}" data-bind="with: player.PlayerWaveInfo[$index()+1]">
							<div class="${perk.perkIcon} ${perk.width32} ${layout.blockCenter}" data-bind="css: Perk, attr: {title: Perk}" />
							<div class="${text.center}" data-bind="text: Kills" />
						</td>
						<!-- /ko -->
					</tr>
				</tbody>
			</table>
		</div>`,
});
