import { PlayerViewModel,PlayerMapStatistic, PlayedGame } from 'Server';
import { Observable, Computed } from 'knockout';
import * as PlayedGameList from "PlayerView/PlayedGameListComponent";
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import * as ko from 'knockout';
import { padding, text, layout, events, margin } from 'AppStyles';

export var ComponentName = "PlayerMapStatistics";

interface PlayerMapStatisticsParams
{
	PlayerViewModel : Observable<PlayerViewModel>;
}

class PlayerMapStatisticsViewModel {
	constructor(params: PlayerMapStatisticsParams) {
		this.PlayerViewModel = params.PlayerViewModel;
		this.Difficulty = ko.observable(this.Difficulties[this.Difficulties.length-1]);
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

	public Difficulty: Observable<string>;
	public Map: Observable<string>;

	public Difficulties: string[] = [ 'Hard', 'Suicidal', 'Hell on Earth' ];
	public MapStatisticsForDifficulty: Computed<PlayerMapStatistic[]>;
	public GamesForMapDifficulty: Computed<PlayedGame[]>;
	public PlayerViewModel: Observable<PlayerViewModel>;
}

ko.components.register(ComponentName, {
	viewModel: PlayerMapStatisticsViewModel,
	template: `
		<div>
			<div class="${layout.flexRow} tabs ${margin.bottom}" data-bind="foreach: Difficulties">
				<div class="${layout.flexEvenDistribution} ${events.clickable} ${text.center} tab" data-bind="css: {selected: $data === $component.Difficulty()}, text: $data, click: $component.SelectDifficulty" />
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
						<td class="${layout.width15} ${padding.half} ${text.right}" data-bind="${NumberWithCommas.Name}: TotalKills" />
					</tr>
				</tbody>
			</table>

			<div data-bind="visible: !!Map()">
				<div data-bind="component: {name: '${PlayedGameList.ComponentName}', params: {Games: GamesForMapDifficulty(), PageSize: 20}}"></div>
			</div>
		</div>`,
});
