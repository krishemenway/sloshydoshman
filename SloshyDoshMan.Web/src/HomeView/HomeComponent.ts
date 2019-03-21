import * as ko from "knockout";
import { margin, text, textColor } from "../AppStyles";
import * as ServerRecentWins from "HomeView/ServerRecentWinsComponent";
import * as ServerRecentGames from "HomeView/ServerRecentGamesComponent";
import * as ServerStatistics from "HomeView/ServerStatisticsComponent";
import * as PlayerSearch from "HomeView/PlayerSearchComponent";

class HomeViewModel {
	constructor(params?: any) { }
}

export var Name : string = "Home";
ko.components.register(Name, {
	viewModel: HomeViewModel,
	template: `
		<div>
			<div class="${margin.bottom}" data-bind="component: {name: '${ServerRecentWins.Name}'}" />
			<div class="${margin.bottom}" data-bind="component: {name: '${PlayerSearch.Name}'}" />
			<div class="${margin.bottom}" data-bind="component: {name: '${ServerRecentGames.Name}'}" />
			<div class="${margin.bottom}" data-bind="component: {name: '${ServerStatistics.Name}'}" />
			<div class="${margin.bottom} ${text.center} ${textColor.white} ${text.font14} ${text.smallCaps}">
				<a href="https://github.com/krishemenway/sloshydoshman">source code</>
			</div>
		</div>`,
});
