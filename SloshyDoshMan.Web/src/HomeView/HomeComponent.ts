import * as ServerRecentWins from "HomeView/ServerRecentWinsComponent";
import * as ServerRecentGames from "HomeView/ServerRecentGamesComponent";
import * as ServerStatistics from "HomeView/ServerStatisticsComponent";
import * as PlayerSearch from "HomeView/PlayerSearchComponent";
import * as ko from "knockout";

class HomeViewModel {
	constructor(params?: any) {
	}
}

export var Name : string = "Home";
ko.components.register(Name, {
	viewModel: HomeViewModel,
	template: `
		<div>
			<div class="margin-bottom" data-bind="component: {name: '${ServerRecentWins.Name}'}" />
			<div class="margin-bottom" data-bind="component: {name: '${PlayerSearch.Name}'}" />
			<div class="margin-bottom" data-bind="component: {name: '${ServerRecentGames.Name}'}" />
			<div class="margin-bottom" data-bind="component: {name: '${ServerStatistics.Name}'}" />
			<div class="margin-bottom text-center text-white font-14 small-caps">
				<a href="https://github.com/krishemenway/killing-floor-2-stat-scraper">source code</>
			</div>
		</div>`,
});