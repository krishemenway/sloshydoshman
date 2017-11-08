import {ResultOf} from "CommonDataStructures/ResultOf";
import {PieChartData} from "ChartJS/Chart";
import * as ChartJsPieChart from "ChartJS/ChartJSPieChartBindingHandler";
import * as ko from "knockout";
import * as $ from "jquery";

export var Name : string = "ServerStatistics";

interface PerkStatistics
{
	PerkName: string;
	TotalWavesPlayed: number;
	TotalKills: number;
}

interface ServerStatistics {
	Perks: PerkStatistics[];
}

class ServerStatisticsViewModel {
	constructor(params?: any) {
		this.Stats = ko.observable(null);
		this.PerkTotalWavesData = ko.pureComputed(this.TotalPerkWavesChartData, this);
		this.PerkTotalKillsData = ko.pureComputed(this.TotalPerkKillsChartData, this);
		this.LoadServerStats();
	}

	public LoadServerStats = () => {
		$.ajax({url: "/webapi/home/serverstats"}).done((response : ResultOf<ServerStatistics>) => { this.Stats(response.Data); });
	}

	private TotalPerkWavesChartData = () : PieChartData[] => {
		let stats = this.Stats();

		if(stats === null) {
			return [];
		}

		return this.Stats().Perks.map((perk: PerkStatistics) => <PieChartData>{
			Label: perk.PerkName,
			Value: perk.TotalWavesPlayed
		});
	}

	private TotalPerkKillsChartData = () : PieChartData[] => {
		let stats = this.Stats();

		if(stats === null) {
			return [];
		}

		return this.Stats().Perks.map((perk: PerkStatistics) => <PieChartData>{
			Label: perk.PerkName,
			Value: perk.TotalKills
		});
	}

	public PerkTotalKillsData: KnockoutComputed<PieChartData[]>;
	public PerkTotalWavesData: KnockoutComputed<PieChartData[]>;
	public Stats: KnockoutObservable<ServerStatistics|null>;
}

ko.components.register(Name, {
	viewModel: ServerStatisticsViewModel,
	template: `
		<div class="font-36 text-white small-caps text-center margin-top-half">server statistics</div>
		<div class="flex-row" data-bind="with: Stats">
			<div class="red-handle-container flex-distribution width-50 margin-right-half">
				<div class="font-28 inset-text gray-9f bold to-upper text-center margin-bottom">Perks Played</div>
				<canvas data-bind="${ChartJsPieChart.Name}: $component.PerkTotalWavesData" />
			</div>

			<div class="red-handle-container flex-distribution width-50 margin-left-half">
				<div class="font-28 inset-text gray-9f bold to-upper text-center margin-bottom">Kills Per Perk</div>
				<canvas data-bind="${ChartJsPieChart.Name}: $component.PerkTotalKillsData" />
			</div>
		</div>`,
});
