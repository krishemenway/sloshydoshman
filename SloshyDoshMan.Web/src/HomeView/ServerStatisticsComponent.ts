import * as ko from "knockout";
import * as $ from "jquery";
import { text, textColor, margin, layout, redHandleContainer } from "AppStyles";
import {ResultOf} from "CommonDataStructures/ResultOf";
import {PieChartData} from "ChartJS/Chart";
import {Computed, Observable} from "knockout";
import * as ChartJsPieChart from "ChartJS/ChartJSPieChartBindingHandler";

var Name : string = "ServerStatistics";
export function ServerStatisticsComponent() {
	return `component: {name: '${Name}'}`;
}

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
		$.ajax({url: "/webapi/games/statistics"}).done((response : ResultOf<ServerStatistics>) => { this.Stats(response.Data); });
	}

	private TotalPerkWavesChartData = () : PieChartData[] => {
		let stats = this.Stats();

		if(stats === null) {
			return [];
		}

		return stats.Perks.map((perk: PerkStatistics) => <PieChartData>{
			Label: perk.PerkName,
			Value: perk.TotalWavesPlayed
		});
	}

	private TotalPerkKillsChartData = () : PieChartData[] => {
		let stats = this.Stats();

		if(stats === null) {
			return [];
		}

		return stats.Perks.map((perk: PerkStatistics) => <PieChartData>{
			Label: perk.PerkName,
			Value: perk.TotalKills
		});
	}

	public PerkTotalKillsData: Computed<PieChartData[]>;
	public PerkTotalWavesData: Computed<PieChartData[]>;
	public Stats: Observable<ServerStatistics|null>;
}

ko.components.register(Name, {
	viewModel: ServerStatisticsViewModel,
	template: `
		<div class="${text.font36} ${textColor.white} ${text.smallCaps} ${text.center} ${margin.topHalf}">server statistics</div>
		<div class="${layout.flexRow}" data-bind="with: Stats">
			<div class="${redHandleContainer.container} ${layout.width50} ${margin.rightHalf}">
				<div class="${text.font28} ${text.inset} ${textColor.gray9f} ${text.bold} ${text.toUpper} ${text.center} ${margin.bottom}">Perks Played</div>
				<canvas data-bind="${ChartJsPieChart.Name}: $component.PerkTotalWavesData" />
			</div>

			<div class="${redHandleContainer.container} ${layout.width50} ${margin.leftHalf}">
				<div class="${text.font28} ${text.inset} ${textColor.gray9f} ${text.bold} ${text.toUpper} ${text.center} ${margin.bottom}">Kills Per Perk</div>
				<canvas data-bind="${ChartJsPieChart.Name}: $component.PerkTotalKillsData" />
			</div>
		</div>`,
});
