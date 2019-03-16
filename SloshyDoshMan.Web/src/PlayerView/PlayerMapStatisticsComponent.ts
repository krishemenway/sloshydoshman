import {PlayerViewModel,PlayerMapStatistic} from 'Server';
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import { Observable, Computed } from 'knockout';
import * as ko from 'knockout';

export var ComponentName = "PlayerMapStatistics";

interface PlayerMapStatisticsParams
{
	PlayerViewModel : Observable<PlayerViewModel>;
}

class PlayerMapStatisticsViewModel {
	constructor(params: PlayerMapStatisticsParams) {
		this.PlayerViewModel = params.PlayerViewModel;
		this.Difficulty = ko.observable(this.Difficulties[this.Difficulties.length-1]);
		this.MapStatisticsForDifficulty = ko.pureComputed(this.FindMapStatisticsForDifficulty, this);
	}

	public FindMapStatisticsForDifficulty = () : PlayerMapStatistic[] => {
		let difficulty = this.Difficulty();
		return this.PlayerViewModel().MapStatistics.filter(stats => stats.Difficulty === difficulty);
	}

	public SelectDifficulty = (difficulty: string) => {
		this.Difficulty(difficulty);
	}

	public OnMapSelected = (map: string) => {
		console.log(map);
	}

	public Difficulties: string[] = [ 'Hard', 'Suicidal', 'Hell on Earth' ];
	public Difficulty: Observable<string>;
	public MapStatisticsForDifficulty: Computed<PlayerMapStatistic[]>;
	public PlayerViewModel: Observable<PlayerViewModel>;
}

ko.components.register(ComponentName, {
	viewModel: PlayerMapStatisticsViewModel,
	template: `
		<div class="player-map-statistics">
			<div class="flex-row tabs margin-bottom" data-bind="foreach: Difficulties">
				<div class="flex-even-distribution clickable text-center tab" data-bind="css: {selected: $data === $component.Difficulty()}, text: $data, click: $component.SelectDifficulty" />
			</div>

			<table class="width-100">
				<thead>
					<tr>
						<td></td>
						<td class="padding-half text-center font-12 bold small-caps"><span class="margin-left inline-block">Games<br/>Won</span></td>
						<td class="padding-half text-right font-12 bold small-caps">Total<br/>Kills</td>
					</tr>
				</thead>
				<tbody data-bind="foreach: MapStatisticsForDifficulty">
					<tr class="clickable" data-bind="click: $component.OnMapSelected">
						<td style="width: 50%" class="padding-half text-left small-caps" data-bind="text: Map" />
						<td style="width: 15%" class="padding-half text-right">
							<div class="width-40 inline-block text-right padding-right-half" data-bind="text: GamesWon" />
							<div class="width-10 inline-block text-center">/</div>
							<div class="width-40 inline-block text-left padding-left-half" data-bind="text: GamesPlayed" />
						</td>
						<td style="width: 15%" class="padding-half text-right" data-bind="${NumberWithCommas.Name}: TotalKills" />
					</tr>
				</tbody>
			</table>
		</div>`,
});
