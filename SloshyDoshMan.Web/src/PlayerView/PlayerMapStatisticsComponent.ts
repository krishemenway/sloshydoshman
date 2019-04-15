import * as ko from 'knockout';
import { padding, text, layout, events, margin, createStyles } from 'AppStyles';
import { PlayerViewModel,PlayerMapStatistic, PlayedGame } from 'Server';
import { NumberWithCommas } from "KnockoutHelpers/NumberWithCommasBindingHandler";
import { PlayedGameListComponent } from 'PlayerView/PlayedGameListComponent';

var ComponentName = "PlayerMapStatistics";
export function PlayerMapStatisticsComponent(playerViewModelParameter: string) {
	return `component: {name: '${ComponentName}', params: {PlayerViewModel: ${playerViewModelParameter}}}`;
}

interface PlayerMapStatisticsParams
{
	PlayerViewModel : ko.Observable<PlayerViewModel>;
}

class PlayerMapStatisticsViewModel {
	constructor(params: PlayerMapStatisticsParams) {
		this.PlayerViewModel = params.PlayerViewModel;

		this.Difficulty = ko.observable(this.HighestDifficultyPlayerHasWon());
		this.Map = ko.observable("");

		this.MapStatisticsForDifficulty = ko.pureComputed(this.FindMapStatisticsForDifficulty, this);
		this.GamesForMapDifficulty = ko.pureComputed(this.FindGamesForMapDifficulty, this);
	}

	public SelectDifficulty = (difficulty: string) => {
		this.Difficulty(difficulty);
		this.Map("");
	}

	public OnMapSelected = (mapStatistic: PlayerMapStatistic) => {
		if (mapStatistic.GamesPlayed === 0) {
			return;
		}

		this.Map(mapStatistic.Map);
	}

	private FindGamesForMapDifficulty = () : PlayedGame[] => {
		return this.PlayerViewModel().AllGames.filter((game) => game.Difficulty === this.Difficulty() && game.Map === this.Map());
	}

	private FindMapStatisticsForDifficulty = () : PlayerMapStatistic[] => {
		let difficulty = this.Difficulty();
		return this.PlayerViewModel().MapStatistics.filter(stats => stats.Difficulty === difficulty);
	}

	private HighestDifficultyPlayerHasWon() {
		let highestDifficulty = 0;

		for(let i = 0; i < this.PlayerViewModel().AllGames.length; i++) {
			let game = this.PlayerViewModel().AllGames[i];

			if (game.PlayersWon && this.DifficultiesInOrder.indexOf(game.Difficulty) > highestDifficulty) {
				highestDifficulty = this.DifficultiesInOrder.indexOf(game.Difficulty);
			}

			if (highestDifficulty === this.DifficultiesInOrder.length - 1) {
				return this.DifficultiesInOrder[highestDifficulty];
			}
		}

		return this.DifficultiesInOrder[highestDifficulty];
	}

	private DifficultiesInOrder = [
		'Hard','Suicidal','Hell on Earth'
	]

	public Difficulty: ko.Observable<string>;
	public Map: ko.Observable<string>;

	public Difficulties: string[] = [ 'Hard', 'Suicidal', 'Hell on Earth' ];
	public MapStatisticsForDifficulty: ko.Computed<PlayerMapStatistic[]>;
	public GamesForMapDifficulty: ko.Computed<PlayedGame[]>;
	public PlayerViewModel: ko.Observable<PlayerViewModel>;
}

const styles = createStyles({
	tab: {
		border: "1px solid #383838",
		borderTopRightRadius: "2px",
		borderTopLeftRadius: "2px",
		padding: "10px 2px 10px 2px",

		"&.selected": {
			borderBottomColor: "transparent",
		},
	},
}).attach().classes;

ko.components.register(ComponentName, {
	viewModel: PlayerMapStatisticsViewModel,
	template: `
		<div>
			<div class="${layout.flexRow} ${margin.bottom}" data-bind="foreach: Difficulties">
				<div class="${layout.flexEvenDistribution} ${events.clickable} ${text.center} ${styles.tab}" data-bind="css: {selected: $data === $component.Difficulty()}, text: $data, click: $component.SelectDifficulty" />
			</div>

			<table class="${layout.width100}" data-bind="visible: !Map()">
				<thead>
					<tr>
						<td />
						<td class="${padding.half} ${text.center} ${text.font12} ${text.bold} ${text.smallCaps}">
							<span class="${margin.left} ${layout.inlineBlock}">Games<br/>Won</span>
						</td>
						<td class="${padding.half} ${text.right} ${text.font12} ${text.bold} ${text.smallCaps}">Total<br/>Kills</td>
					</tr>
				</thead>
				<tbody data-bind="foreach: MapStatisticsForDifficulty">
					<tr data-bind="click: $component.OnMapSelected, css: { ${events.clickable}: GamesPlayed > 0 }">
						<td class="${layout.width50} ${padding.half} ${text.left} ${text.smallCaps}" data-bind="text: Map" />
						<td class="${layout.width15} ${padding.half} ${text.right}">
							<div class="${layout.width40} ${layout.inlineBlock} ${text.right} ${padding.rightHalf}" data-bind="text: GamesWon" />
							<div class="${layout.width10} ${layout.inlineBlock} ${text.center}">/</div>
							<div class="${layout.width40} ${layout.inlineBlock} ${text.left} ${padding.leftHalf}" data-bind="text: GamesPlayed" />
						</td>
						<td class="${layout.width15} ${padding.half} ${text.right}" data-bind="${NumberWithCommas("TotalKills")}" />
					</tr>
				</tbody>
			</table>

			<div data-bind="visible: !!Map()">
				<div data-bind="${PlayedGameListComponent("GamesForMapDifficulty()", 20)}" />
			</div>
		</div>`,
});
